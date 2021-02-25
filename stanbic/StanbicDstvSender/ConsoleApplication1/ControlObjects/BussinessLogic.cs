using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApplication1.EntityObjects;
using System.Net;
using System.IO;
using System.Messaging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ConsoleApplication1.DSTVApi;
using ConsoleApplication1.LevelOnePegPay;
using System.Net.Security;
using System.Web.Services.Protocols;
using System.Globalization;
using ConsoleApplication1.StanbicApi;
using System.Text.RegularExpressions;

namespace ConsoleApplication1.ControlObjects
{
    public class BussinessLogic
    {
        DatabaseHandler dh = new DatabaseHandler();
        public const string smsQueuepath = @".\private$\smsQueue";


        internal PostResponse SendToKYU(SchoolsTransaction trans)
        {
            PostResponse resp = new PostResponse();
            string custref = trans.CustRef;
            string vendorCode = trans.VendorCode;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dh.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);

                //if credentails are found
                if (!creds.UtilityCode.Equals(""))
                {
                    //code to post transaction 
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    KYUBillingInterface.ValidateResponse response = new KYUBillingInterface.ValidateResponse();
                    KYUBillingInterface.EPayment epayment = new KYUBillingInterface.EPayment();
                    string studentNo = trans.CustRef;
                    string ReferenceNo = studentNo;

                    string RequestState = "1";
                    string password = creds.UtilityPassword;
                    string dataToSign = "ref=" + studentNo;
                    string Signature = GetKyuDigitalSignature(password, dataToSign); ;

                    string ResourceId = creds.UtilityCode;
                    string VendorTransactionRef = trans.VendorTransactionRef;
                    string BillAmount = trans.TransactionAmount;
                    string CollectionArea = "Kampala";
                    string TransactionType = "CASH";
                    string ChequeNumber = "";
                    string ChequeMaturitydate = DateTime.Now.ToString("dd-MM-yyyy");
                    string date = DateTime.Now.ToString("dd-MM-yyyy");

                    //first validate reference number
                    response = epayment.ValidateRequest(ReferenceNo, RequestState, Signature, ResourceId);

                    //it passes validation at KYU
                    if (response.Status.Equals("200"))
                    {
                        RequestState = "2";
                        string authToken = "";

                        KYUBillingInterface.CommitResp commitResponse = epayment.CommitRequest(ReferenceNo,
                                                    RequestState,
                                                    VendorTransactionRef,
                                                    response.AuthToken,
                                                    BillAmount,
                                                    CollectionArea,
                                                    TransactionType,
                                                    ChequeNumber,
                                                    ChequeMaturitydate,
                                                    date);
                        //success on committing
                        if (commitResponse.Status.Equals("200"))
                        {
                            string utilityRef = commitResponse.Message;
                            //mark it as succeeded
                            dh.UpdateSentTransactionById(trans.VendorTransactionRef, utilityRef, "SUCCESS");
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";
                            Msg = "Dear  " + trans.CustName +
                                  ", " + trans.UtilityCode + " has received a payment of  UGX  " + trans.TransactionAmount +
                                  " for a/c  " + trans.CustRef;
                        }
                        //reference number expired or already used
                        else if (commitResponse.Status.Equals("407"))
                        {
                            string utilityRef = commitResponse.Message;
                            //mark it as succeeded
                            dh.UpdateSentTransactionById(trans.VendorTransactionRef, utilityRef, "SUCCESS");
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";
                            Msg = "Dear  " + trans.CustName +
                                  ", " + trans.UtilityCode + " has received a payment of  UGX  " + trans.TransactionAmount +
                                  " for a/c  " + trans.CustRef;
                        }
                        //some other failure
                        else
                        {
                            //fail transaction here and at SBU
                            //transfer transaction to 
                            //deleted transactions table with Reason
                            string Reason = resp.StatusDescription;
                            dh.TransferDeletedTransaction(trans.TranId, Reason);
                            dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                            Msg = "Dear  " + trans.CustName +
                                  ", Your payment of  UGX  " + trans.TransactionAmount +
                                  " to " + trans.UtilityCode + " has Failed";
                        }
                    }
                    //reference no has failed validation
                    //at KYU
                    else
                    {
                        //fail transaction here and at SBU
                        //transfer transaction to 
                        //deleted transactions table with Reason
                        string Reason = resp.StatusDescription;
                        dh.TransferDeletedTransaction(trans.TranId, Reason);
                        dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                        Msg = "Dear  " + trans.CustName +
                              ", Your payment of  UGX  " + trans.TransactionAmount +
                              " to " + trans.UtilityCode + " has Failed";
                    }




                }
                //no credentails found
                else
                {
                    //delete transaction
                    string Reason = "UTILITY CREDENTAILS NOT SET";
                    dh.TransferDeletedTransaction(trans.TranId, Reason);
                    dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, "URA");
                    Msg = "Dear  " + trans.CustName +
                          ", Your payment of  UGX  " + trans.TransactionAmount +
                          " to " + trans.UtilityCode + " has Failed";
                }

                if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                {
                    string Service = "CellulantService";
                    SMS sms = new SMS();
                    sms.Mask = "STANBIC";
                    sms.Message = Msg;
                    sms.Phone = trans.CustomerTel;
                    sms.Reference = trans.VendorTransactionRef;
                    sms.Sender = Service;
                    sms.VendorTranId = trans.VendorTransactionRef;
                    LogSMS(sms);
                }
            }
            catch (Exception ex)
            {
                //do nothing
                dh.LogError(ex.Message + "PostUMEMEPayments SERVICE POSTING - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }


        public static string GetKyuDigitalSignature(string sign, string contentTosgin)
        {
            //current
            byte[] key = new Byte[64];
            byte[] content = new Byte[100];
            key = Encoding.ASCII.GetBytes(sign);
            content = Encoding.ASCII.GetBytes(contentTosgin);

            using (HMACSHA256 hmc = new HMACSHA256(key))
            {
                //byte[] hashvalue = hmc.ComputeHash(content);
                byte[] hashvalue = hmc.ComputeHash(content);
                //return hashvalue;
                StringBuilder hexValue = new StringBuilder(hashvalue.Length * 2);
                foreach (byte b in hashvalue)
                {
                    hexValue.Append(b.ToString("X02"));

                }
                return hexValue.ToString().ToLower();
            }

        }


        internal PostResponse SendToMAK(SchoolsTransaction trans)
        {
            PostResponse resp = new PostResponse();
            string custref = trans.CustRef;
            string vendorCode = trans.VendorCode;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dh.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);

                //if credentails are found
                if (!creds.UtilityCode.Equals(""))
                {
                    //code to post transaction 
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    MUKApi.Transaction tr = new MUKApi.Transaction();

                    tr.StudentNumber = trans.CustRef;
                    tr.StudentName = trans.CustName;
                    tr.Course = trans.Area;
                    tr.StatusCode = "0";
                    tr.StatusDescription = "SUCCESS";
                    tr.StudentTel = trans.CustomerTel;
                    tr.Offline = "0";
                    tr.VendorCode = creds.UtilityCode;
                    tr.Password = creds.UtilityPassword;
                    tr.PaymentDate = DateTime.Parse(trans.PaymentDate).ToString("dd/MM/yyyy");
                    tr.PaymentType = "01";
                    tr.Reversal = "0";
                    tr.Teller = trans.CustName;
                    tr.TranAmount = trans.TransactionAmount;
                    tr.TranNarration = trans.Narration;
                    tr.TranType = "CASH";
                    tr.VendorTranId = trans.VendorTransactionRef;
                    tr.TranIdToReverse = "";

                    string dataToSign = tr.StudentNumber + tr.StudentName + tr.StudentTel +
                                        tr.Course + tr.VendorTranId + tr.VendorCode +
                                        tr.Password + tr.PaymentDate + tr.Teller +
                                        tr.TranAmount + tr.TranNarration + tr.TranType;

                    tr.DigitalSignature = SignMUKData(dataToSign);
                    MUKApi.EPayment Epay = new MUKApi.EPayment();
                    MUKApi.Response mukResp = Epay.PostMukPayment(tr);


                    //success 
                    if (mukResp.StatusCode == "0")
                    {
                        string utilityRef = mukResp.ReceiptNumber;
                        //mark it as succeeded
                        dh.UpdateSentTransactionById(trans.VendorTransactionRef, utilityRef, "SUCCESS");
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                        Msg = "Dear  " + trans.CustName +
                              ", " + trans.UtilityCode + " has received a payment of  UGX  " + trans.TransactionAmount +
                              " for a/c  " + trans.CustRef;
                    }
                    //duplicate at makerere
                    else if (resp.StatusDescription.ToUpper().Contains("DUPLICATE"))
                    {
                        string utilityRef = mukResp.ReceiptNumber;
                        //mark it as succeeded
                        dh.UpdateSentTransactionById(trans.VendorTransactionRef, utilityRef, "SUCCESS");
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                        Msg = "Dear  " + trans.CustName +
                              ", " + trans.UtilityCode + " has received a payment of  UGX  " + trans.TransactionAmount +
                              " for a/c  " + trans.CustRef;
                    }
                    //failure at MAKERERE
                    else
                    {
                        //fail transaction here and at SBU

                        //transfer transaction to 
                        //deleted transactions table with Reason
                        string Reason = mukResp.StatusDescription;
                        dh.TransferDeletedTransaction(trans.TranId, Reason);
                        dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                        Msg = "Dear  " + trans.CustName +
                              ", Your payment of  UGX  " + trans.TransactionAmount +
                              " to " + trans.UtilityCode + " has Failed";
                    }

                }
                //no credentails found
                else
                {
                    //delete transaction
                    string Reason = "UTILITY CREDENTAILS NOT SET";
                    dh.TransferDeletedTransaction(trans.TranId, Reason);
                    dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, "URA");
                    Msg = "Dear  " + trans.CustName +
                          ", Your payment of  UGX  " + trans.TransactionAmount +
                          " to " + trans.UtilityCode + " has Failed";
                }

                if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                {
                    string Service = "CellulantService";
                    SMS sms = new SMS();
                    sms.Mask = "STANBIC";
                    sms.Message = Msg;
                    sms.Phone = trans.CustomerTel;
                    sms.Reference = trans.VendorTransactionRef;
                    sms.Sender = Service;
                    sms.VendorTranId = trans.VendorTransactionRef;
                    LogSMS(sms);
                }
            }
            catch (Exception ex)
            {
                //do nothing
                dh.LogError(ex.Message + "PostUMEMEPayments SERVICE POSTING - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }

        static X509Certificate2 GetCertificate(string serialNo)
        {

            try
            {

                var rgx = new Regex("[^a-fA-F0-9]");
                var serial = rgx.Replace(serialNo, string.Empty).ToUpper();
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySerialNumber, serial, false);
                store.Close();
                return certs[0];
            }
            catch (Exception ex)
            {
                X509Certificate2Collection certs1 = null;
                Console.WriteLine("Exception on getting cert " + ex.Message);
                return certs1[0];
            }

        }

        private string SignMUKData(string dataToSign)
        {
            string serialNo = "8e983e7edcf94f86499073b108620a2a";

            X509Certificate2 cert = GetCertificate(serialNo);

            //certificate = @"E:\Certificates\CELL\CELL.pfx";

            //X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(dataToSign);
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            string strDigCert = Convert.ToBase64String(digitalCert);
            return strDigCert;
        }
        public PostResponse SendToMUBS(SchoolsTransaction tran)
        {
            PostResponse resp = new PostResponse();
            try
            {
                UtilityCredentials creds = dh.GetUtilityCreds(tran.UtilityCode, tran.VendorCode);
                String Msg = "";

                //if credentails are found
                if (!creds.UtilityCode.Equals(""))
                {
                    //build transaction request
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    CEMAS.EPayment cemas = new CEMAS.EPayment();
                    CEMAS.Response cemasResp = new CEMAS.Response();
                    CEMAS.Transaction trans = new CEMAS.Transaction();

                    trans.StudentNumber = tran.CustRef;
                    trans.StudentName = tran.CustName;
                    trans.StudentTel = tran.CustomerTel;
                    trans.VendorTranId = tran.VendorTransactionRef;
                    trans.Course = "";
                    trans.TranAmount = tran.TransactionAmount;
                    trans.Currency = "UGX";
                    trans.TranType = "CASH";
                    trans.AccountNumber = "0140007129401";
                    trans.ChequeNumber = "";
                    trans.DigitalSignature = tran.DigitalSignature;
                    trans.Institution = "MUBS";
                    trans.Teller = tran.CustName;
                    trans.PaymentType = "TUIT";
                    trans.VendorCode = creds.UtilityCode;
                    trans.TranIdToReverse = "";
                    trans.Password = creds.UtilityPassword;
                    trans.Institution = "MUBS";
                    trans.UnivCode = "MUBS";
                    trans.Offline = "1";
                    trans.SentToTalisma = false;

                    trans.TranNarration = "MUBS Paymnet from " + tran.CustomerTel + " for " + tran.CustRef;
                    trans.Semester = "";
                    trans.YearOfStudy = "";
                    trans.PaymentDate = DateTime.Now.ToString();
                    string dataToSign = trans.StudentNumber + trans.StudentName + trans.StudentTel +
                    trans.Course + trans.VendorTranId + trans.VendorCode + trans.Password +
                    trans.PaymentDate + trans.Teller + trans.TranAmount + trans.TranNarration + trans.TranType;
                    trans.DigitalSignature = SignCertificate(dataToSign);

                    //send to CEMAS
                    cemasResp = cemas.PostTransaction(trans);

                    //success or duplicate
                    if (cemasResp.StatusCode == "0" || cemasResp.StatusCode == "20")
                    {
                        resp.StatusCode = "0";
                        string utilityRef = cemasResp.ReceiptNumber;
                        dh.UpdateSentTransactionById(tran.VendorTransactionRef, utilityRef, "SUCCESS");
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                        Msg = "Dear  " + tran.CustName +
                            ", " + tran.UtilityCode + " has received a payment of  UGX  " + tran.TransactionAmount +
                            " for a/c  " + tran.CustRef;
                    }
                    //general error
                    if (cemasResp.StatusDescription.Contains("GENERAL ERROR"))
                    {
                        //do nothing
                        resp.StatusCode = "100";
                        resp.StatusDescription = cemasResp.StatusDescription;
                    }
                    //it has failed for some other reason
                    else
                    {
                        string Reason = cemasResp.StatusDescription;
                        dh.TransferDeletedTransaction(tran.TranId, Reason);
                        dh.LogError(Reason + " - " + tran.CustRef, tran.VendorCode, DateTime.Now, tran.UtilityCode);
                        Msg = "Dear  " + tran.CustName +
                              ", Your payment of  UGX  " + tran.TransactionAmount +
                              " to " + tran.UtilityCode + " has Failed";
                    }
                }
                else
                {
                    string Reason = "UTILITY CREDENTIALS NOT SET";
                    dh.TransferDeletedTransaction(tran.TranId, Reason);
                }
                if (tran.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                {
                    string Service = "CellulantService";
                    SMS sms = new SMS();
                    sms.Mask = "STANBIC";
                    sms.Message = Msg;
                    sms.Phone = tran.CustomerTel;
                    sms.Reference = tran.VendorTransactionRef;
                    sms.Sender = Service;
                    sms.VendorTranId = tran.VendorTransactionRef;
                    LogSMS(sms);
                }
            }
            catch (TimeoutException ee)
            {
                resp.StatusCode = "101";
                resp.StatusDescription = "ERROR: TIMEOUT ON CALLING CEMAS";
            }
            catch (Exception ex)
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "ERROR: " + ex.Message;

            }
            return resp;


        }

        private string SignCertificate(string text)
        {
            string serialNo = "e79860a603f7cdab4bb2b7505391487a";
            //string certificate = @"E:\Certificates\StanbicCB.pfx";
            X509Certificate2 cert = GetCertificate(serialNo);
            //X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(text);
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            string s = hash.ToString();
            string strDigCert = Convert.ToBase64String(digitalCert);
            return strDigCert;

        }



        internal PostResponse SendToMUBS2(SchoolsTransaction trans)
        {
            PostResponse resp = new PostResponse();
            string custref = trans.CustRef;
            string vendorCode = trans.VendorCode;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dh.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);

                //if credentails are found
                if (!creds.UtilityCode.Equals(""))
                {
                    //code to post transaction 
                    System.Net.ServicePointManager.Expect100Continue = false;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    string myUrl2 = dh.GetSystemSetting(11, 1);

                    //build http get request
                    string urlParams2 = "act = POST & cur = UGX & acc = " + trans.Teller +
                                        "& dt =" + trans.PaymentDate +
                                        "& stno =" + trans.CustRef +
                                        "& chqno =" + trans.ChequeNumber +
                                        " &dr =" + trans.Reversal +
                                        " &cr =" + trans.PaymentType +
                                        " & bank =" + creds.BankCode +
                                        " & username =" + creds.Utility +
                                        "& password =" + creds.UtilityPassword +
                                        "& phone =" + trans.Telephone +
                                        " & yrofstudy =" + "" +
                                        "& sem =" + "" + "& bankRef =" + trans.VendorTransactionRef +
                                        " & naration =" + trans.Narration;

                    myUrl2 = myUrl2 + urlParams2;
                    HttpWebRequest r2 = (HttpWebRequest)System.Net.WebRequest.Create(myUrl2);
                    r2.Headers.Clear();
                    r2.AllowAutoRedirect = true;
                    r2.PreAuthenticate = true;
                    r2.ContentType = "application / x - www - form - urlencoded";
                    r2.Credentials = CredentialCache.DefaultCredentials;
                    r2.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                    r2.Timeout = 150000;
                    Encoding byteArray2 = Encoding.GetEncoding("utf-8");
                    Stream dataStream2;
                    WebResponse response2 = (HttpWebResponse)r2.GetResponse();
                    Console.WriteLine(((HttpWebResponse)response2).StatusDescription);
                    dataStream2 = response2.GetResponseStream();
                    StreamReader rdr2 = new StreamReader(dataStream2);
                    string feedback2 = rdr2.ReadToEnd();

                    string[] array = feedback2.Split(',');

                    //success
                    if (array.Length == 9)
                    {
                        string errorCode = array[0].ToString().Replace(",", "");
                        string Ref = GetDetail(array[1].ToString());
                        resp.StatusCode = errorCode.Replace("\n", "");
                        resp.PegPayPostId = Ref;

                        //mark it as succeeded
                        dh.UpdateSentTransactionById(trans.VendorTransactionRef, Ref, "SUCCESS");
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }

                    //failure at MUBS
                    else
                    {
                        //fail transaction here and at SBU
                        string errorCode = array[0].ToString().Replace(",", "");
                        string errorMessage = array[1].ToString().Replace(",", "");
                        resp.StatusDescription = errorMessage;
                        resp.StatusCode = errorCode;

                        //transfer transaction to 
                        //deleted transactions table with Reason
                        string Reason = errorMessage;
                        dh.TransferDeletedTransaction(trans.TranId, Reason);
                        dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                        Msg = "Dear  " + trans.CustName +
                              ", Your payment of  UGX  " + trans.TransactionAmount +
                              " to " + trans.UtilityCode + " has Failed";
                    }

                }
                //no credentails found
                else
                {
                    //delete transaction
                    string Reason = "UTILITY CREDENTAILS NOT SET";
                    dh.TransferDeletedTransaction(trans.TranId, Reason);
                    dh.LogError(Reason + " - " + custref, vendorCode, DateTime.Now, "URA");
                    Msg = "Dear  " + trans.CustName +
                          ", Your payment of  UGX  " + trans.TransactionAmount +
                          " to " + trans.UtilityCode + " has Failed";
                }

                if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                {
                    string Service = "CellulantService";
                    SMS sms = new SMS();
                    sms.Mask = "STANBIC";
                    sms.Message = Msg;
                    sms.Phone = trans.CustomerTel;
                    sms.Reference = trans.VendorTransactionRef;
                    sms.Sender = Service;
                    sms.VendorTranId = trans.VendorTransactionRef;
                    LogSMS(sms);
                }
            }
            catch (Exception ex)
            {
                //do nothing
                dh.LogError(ex.Message + "PostUMEMEPayments SERVICE POSTING - " + custref, vendorCode, DateTime.Now, trans.UtilityCode);
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }

        private static string GetDetail(string input)
        {
            string[] array = input.Split(':');
            string output = array[1].ToString().Trim();
            return output;
        }

        private void LogSMS(SMS sms)
        {
            try
            {
                MessageQueue smsqueue;
                if (MessageQueue.Exists(smsQueuepath))
                {
                    smsqueue = new MessageQueue(smsQueuepath);
                }
                else
                {
                    smsqueue = MessageQueue.Create(smsQueuepath);
                }
                Message smsmsg = new Message(sms);
                smsmsg.Label = sms.VendorTranId;
                smsmsg.Recoverable = true;
                smsqueue.Send(smsmsg);
            }
            catch (Exception ex)
            {
                //donothing
            }
        }


        public PostResponse MakeDstvPayment(ConsoleApplication1.EntityObjects.Transaction trans)
        {
            PostResponse postResp = new PostResponse();
            try
            {
                Console.WriteLine("Posting Transaction " + trans.VendorTransactionRef);
                //Get dstv credentials
                UtilityCredentials creds = dh.GetUtilityCreds("DSTV", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    SelfCareService dstv = new SelfCareService();
                    SubmitPaymentResponse resp = new SubmitPaymentResponse();

                    //TO BE DONE: we need to transfer these hard coded values to the db 
                    string datasource = dh.GetSystemSetting(9, 3);
                    string bussinesUnit = trans.CustomerType;
                    string vendorCode = creds.UtilityCode;
                    string language = dh.GetSystemSetting(9, 5);
                    string IpAddress = dh.GetSystemSetting(9, 6);
                    string paymentVendorCode = creds.BankCode;//password;
                    //string substring = trans.VendorTransactionRef.Substring(1);
                    string transactionNumber = trans.VendorTransactionRef;// int.Parse(substring);
                    bool transactionNumberSpecified = true;
                    bool customerNumberSpecified = true;

                    decimal tranAmount = 0;
                    decimal result = (Convert.ToDecimal(trans.TransactionAmount));
                    tranAmount = result;

                    bool amountSpecified = true;
                    int invoicePeriod = 1;
                    bool invoicePeriodSpecified = true;
                    string currency = dh.GetSystemSetting(9, 4);
                    uint customerNumber = GetCustomerNumber(trans.CustRef);
                    string paymentDescription = "SBU Payment From " + trans.CustomerTel;

                    int AmountPaid = (Convert.ToInt32(trans.TransactionAmount));
                    ConsoleApplication1.EntityObjects.BouquetDetails bouquetDetails = dh.GetBouquetClosestToAmountPaid(AmountPaid, "DSTV");
                    Console.WriteLine("Picked " + bouquetDetails.BouquetCode + " for tran:" + trans.VendorTransactionRef);
                    PaymentProduct productChoosen = new PaymentProduct();
                    productChoosen.ProductUserKey = bouquetDetails.BouquetCode;

                    //leave empty since its topUp
                    PaymentProduct[] productCollection ={ 
                        //productChoosen
                    };
                    string paymentMethod = trans.TransactionType;

                    //send to dstv
                    //resp = dstv.SubmitPayment(vendorCode,
                    //                    datasource,
                    //                    paymentVendorCode,
                    //                    transactionNumber,
                    //                    transactionNumberSpecified,
                    //                    customerNumber,
                    //                    customerNumberSpecified,
                    //                    tranAmount,
                    //                    amountSpecified,
                    //                    invoicePeriod,
                    //                    invoicePeriodSpecified,
                    //                    currency,
                    //                    paymentDescription,
                    //                    productCollection,
                    //                    paymentMethod,
                    //                    language,
                    //                    IpAddress,
                    //                    bussinesUnit);
                    dstv.Url = @"https://54.72.34.166//VendorSelfCare/SelfCareService.svc?singleWsdl";
                    resp = dstv.SubmitPaymentBySmartCard(vendorCode, datasource, paymentVendorCode, transactionNumber, trans.CustRef, tranAmount, amountSpecified, invoicePeriod,
                                  invoicePeriodSpecified, currency, paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit);
                    if (IsSuccessResponse(resp))
                    {
                        Console.WriteLine("Posting Successful for " + trans.VendorTransactionRef);

                        //Update tran in Recieved Table
                        dh.UpdateSentTransactionById(trans.VendorTransactionRef, "" + resp.SubmitPayment.receiptNumber, "1");
                        postResp.StatusCode = "0";
                        postResp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + resp.SubmitPayment.ErrorMessage);
                        //Transfer transaction to Deleted with Reason
                        dh.TransferDeletedTransaction(trans.TranId, resp.SubmitPayment.ErrorMessage);
                        postResp.StatusCode = "100";
                        postResp.StatusDescription = resp.SubmitPayment.ErrorMessage;
                    }
                }
            }
            //this is how dstv communicates validation errors
            catch (SoapException ex)
            {
                if (ex.Message.ToUpper().Contains("Duplicate".ToUpper()))
                {
                    Console.WriteLine("Posting Successful for " + trans.VendorTransactionRef);
                    //Update tran in Recieved Table
                    dh.UpdateSentTransactionById(trans.VendorTransactionRef, "", "1");
                    postResp.StatusCode = "0";
                    postResp.StatusDescription = "SUCCESS";
                }
                else
                {
                    Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);

                    //Transfer transaction to Failed with Reason
                    string failureReason = ex.Message;
                    dh.TransferDeletedTransaction(trans.TranId, failureReason);
                    postResp.StatusCode = "100";
                    postResp.StatusDescription = failureReason;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                dh.LogError(ex.Message + "PostDstvPayments SERVICE POSTING - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "DSTV");
                postResp.PegPayPostId = "";
                postResp.StatusCode = "000";
                postResp.StatusDescription = "Error AT PegPay";
            }
            return postResp;
        }


        public PostResponse MakeGoTvPayment(ConsoleApplication1.EntityObjects.Transaction trans)
        {
            PostResponse postResp = new PostResponse();
            try
            {
                Console.WriteLine("Posting Transaction " + trans.VendorTransactionRef);
                //get dstv credentials
                UtilityCredentials creds = dh.GetUtilityCreds("DSTV", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    SelfCareService dstv = new SelfCareService();
                    SubmitPaymentResponse resp = new SubmitPaymentResponse();

                    //we need to transfer all these to the db after tests
                    string datasource = dh.GetSystemSetting(9, 3);
                    string bussinesUnit = trans.CustomerType;
                    string vendorCode = creds.UtilityPassword;//PegasusGoTv
                    string language = dh.GetSystemSetting(9, 5);
                    string IpAddress = dh.GetSystemSetting(9, 6);
                    string paymentVendorCode = creds.BankCode;
                    //string vendortranid = trans.VendorTransactionRef.Substring(1);
                    string transactionNumber = trans.VendorTransactionRef;// int.Parse(vendortranid);
                    bool transactionNumberSpecified = true;
                    bool customerNumberSpecified = true;

                    decimal tranAmount = 0;
                    decimal result = (Convert.ToDecimal(trans.TransactionAmount));
                    tranAmount = result;

                    bool amountSpecified = true;
                    int invoicePeriod = 1;
                    bool invoicePeriodSpecified = true;
                    string currency = dh.GetSystemSetting(9, 4);
                    uint customerNumber = GetCustomerNumber(trans.CustRef);
                    string paymentDescription = "SBU Payment From " + trans.CustomerTel;

                    int AmountPaid = (Convert.ToInt32(trans.TransactionAmount));
                    ConsoleApplication1.EntityObjects.BouquetDetails bouquetDetails = dh.GetBouquetClosestToAmountPaid(AmountPaid, "GOTV");
                    Console.WriteLine("Picked " + bouquetDetails.BouquetCode + " for tran:" + trans.VendorTransactionRef);
                    PaymentProduct productChoosen = new PaymentProduct();
                    productChoosen.ProductUserKey = bouquetDetails.BouquetCode;

                    //leave empty since its topUp
                    PaymentProduct[] productCollection ={ 
                        //productChoosen
                    };

                    string paymentMethod = trans.TransactionType;
                    //resp = dstv.SubmitPayment(vendorCode,
                    //                          datasource,
                    //                          paymentVendorCode,
                    //                          transactionNumber,
                    //                          transactionNumberSpecified,
                    //                          customerNumber,
                    //                          customerNumberSpecified,
                    //                          tranAmount,
                    //                          amountSpecified,
                    //                          invoicePeriod,
                    //                          invoicePeriodSpecified,
                    //                          currency,
                    //                          paymentDescription,
                    //                          productCollection,
                    //                          paymentMethod,
                    //                          language,
                    //                          IpAddress,
                    //                          bussinesUnit);
                    resp = dstv.SubmitPaymentBySmartCard(vendorCode, datasource, paymentVendorCode, transactionNumber, trans.CustRef, tranAmount, amountSpecified, invoicePeriod,
                                  invoicePeriodSpecified, currency, paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit);

                    if (IsSuccessResponse(resp))
                    {
                        Console.WriteLine("Posting Successfull for " + trans.VendorTransactionRef);
                        //Update tran in Recieved Table
                        dh.UpdateSentTransactionById(trans.VendorTransactionRef, "" + resp.SubmitPayment.receiptNumber, "SUCCESS");
                        postResp.StatusCode = "0";
                        postResp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + resp.SubmitPayment.ErrorMessage);
                        //Transfer transaction to Failed with Reason
                        dh.TransferDeletedTransaction(trans.TranId, resp.SubmitPayment.ErrorMessage);
                        postResp.StatusCode = "100";
                        postResp.StatusDescription = resp.SubmitPayment.ErrorMessage;
                    }

                }
            }
            //this is how dstv communicate validation errors
            catch (SoapException ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);

                //Transfer transaction to Failed with Reason
                dh.TransferDeletedTransaction(trans.TranId, ex.Message);
                postResp.StatusCode = "100";
                postResp.StatusDescription = ex.Message;
            }
            //some other serious error has happend
            catch (Exception ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                dh.LogError(ex.Message + "PostDstvPayments SERVICE POSTING - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "DSTV");
                postResp.PegPayPostId = "";
                postResp.StatusCode = "000";
                postResp.StatusDescription = "Error AT PegPay";
            }
            return postResp;
        }

        private uint GetCustomerNumber(string smartCardNumber)
        {
            uint customerNo = 0;
            Customer cust = dh.GetCustDetails(smartCardNumber, "DSTV");
            if (cust.StatusCode.Equals("0"))
            {
                customerNo = uint.Parse(cust.CustomerRef);
            }
            return customerNo;
        }

        public void SendToDSTV(ConsoleApplication1.EntityObjects.Transaction trans)
        {
            //Customer cust = dh.GetCustDetails(trans.CustRef, "DSTV");
            //if (cust.CustomerType == "GOTV")
            //{
            //    MakeGoTvPayment(trans);
            //}
            //else if (cust.CustomerType == "DSTV")
            //{
            //    MakeDstvPayment(trans);
            //}
            //else
            //{
            //    //unknown dude
            //    //leave transaction as is
            //}
            DatabaseHandler dh = new DatabaseHandler();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            LevelOnePegPay.PegPay pegpay = new LevelOnePegPay.PegPay();
            LevelOnePegPay.Response response = new LevelOnePegPay.Response();
            LevelOnePegPay.QueryRequest request = new LevelOnePegPay.QueryRequest();
            //UtilityCredentials creds = dh.GetUtilityCreds("DSTV","MTN");
            request.QueryField1 = trans.CustRef;
            request.QueryField2 = "DSTV";
            request.QueryField4 = "DSTV";
            request.QueryField5 = "MTN";
            request.QueryField6 = "68R31WJ032";
            response = pegpay.QueryCustomerDetails(request);
            if (trans.CustomerType == "GOTV")
            {
                MakeGoTvPayment(trans);
            }
            //Dstv Transaction
            else if (trans.CustomerType == "2")
            {
                MakeDstvPayment(trans);
            }
            else
            {
                //unknown dude
                //do nothing
            }
        }

        public PostResponse SendToSchool(SchoolsTransaction trans)
        {
            PostResponse postResp = new PostResponse();
            if (trans.UtilityCode == "MUBS")
            {
                postResp = SendToMUBS(trans);
            }
            else if (trans.UtilityCode == "MAK")
            {
                postResp = SendToMAK(trans);
            }
            else if (trans.UtilityCode == "KYU")
            {
                postResp = SendToKYU(trans);
            }
            else
            {
                //unknown school
                //do nothing
            }
            return postResp;
        }

        private string GetDigitalSignature(string text, string vendorCode)
        {
            // retrieve private key||@"C:\PegPayCertificates1\Orange\41.202.229.3.cer"
            //string certificate = "";
            //if (vendorCode.ToUpper().Equals("STANBIC_VAS"))
            //{
            //    certificate = @"E:\\Certificates\\CELL\\CELL.pfx";
            //}
            //else
            //{
            //    certificate = @"E:\\Certificates\\" + vendorCode + "\\" + vendorCode + ".pfx";
            //}

            string serialNo = "8e983e7edcf94f86499073b108620a2a";
            X509Certificate2 cert = GetCertificate(serialNo);
            //string certificate = @"C:\PegPayCertificates1\Ezee-Money\ezeemoney-ug_com.crt";
            //X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

            // Hash the data
            SHA1Managed sha1 = new SHA1Managed();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(text);
            byte[] hash = sha1.ComputeHash(data);

            // Sign the hash
            byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
            string strDigCert = Convert.ToBase64String(digitalCert);
            return strDigCert;
        }

        private string formateDate(string date)
        {
            //string format = "dd/MM/yyyy";
            //string[]  newdate;
            //newdate=date.Substring(0,10);
            string[] newdate = date.Split(' ');
            string[] arr = newdate[0].Split('/');
            string day = arr[1];
            if (day.Length == 1)
            {
                day = "0" + day;
            }
            string month = arr[0];
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            string year = arr[2];
            string formatdate = day + "/" + month + "/" + year;
            return formatdate;
            //trim off time;


        }

        private UmemeApi.Transaction GetUmemeTrans(ConsoleApplication1.EntityObjects.UmemeTransaction trans, UtilityCredentials creds)
        {
            UmemeApi.Transaction umemeTrans = new UmemeApi.Transaction();
            umemeTrans.CustomerName = trans.CustName;
            umemeTrans.CustomerRef = trans.CustRef;
            umemeTrans.CustomerTel = trans.CustomerTel;
            umemeTrans.CustomerType = trans.CustomerType;
            umemeTrans.Offline = trans.Offline;
            umemeTrans.Password = creds.UtilityPassword;
            string format = "d/M/yyyy";
            string newdate = formateDate(trans.PaymentDate.Trim()).Trim();
            string payDate = newdate;//DateTime.ParseExact(newdate, format, CultureInfo.InvariantCulture).ToString();
            umemeTrans.PaymentDate = payDate;
            umemeTrans.PaymentType = trans.PaymentType;
            umemeTrans.Reversal = trans.Reversal;
            umemeTrans.StatusCode = "0";
            umemeTrans.StatusDescription = "SUCCESS";
            umemeTrans.Teller = trans.Teller;
            umemeTrans.TranAmount = trans.TransactionAmount;
            umemeTrans.TranIdToReverse = trans.TranIdToReverse;
            umemeTrans.TranNarration = trans.Narration;
            umemeTrans.TranType = trans.TransactionType;
            umemeTrans.VendorCode = creds.UtilityCode;
            umemeTrans.VendorTranId = trans.VendorTransactionRef;
            string dataToSign = umemeTrans.CustomerRef + umemeTrans.CustomerName + umemeTrans.CustomerTel + umemeTrans.CustomerType + umemeTrans.VendorTranId + umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.PaymentType + umemeTrans.Teller + umemeTrans.TranAmount + umemeTrans.TranNarration + umemeTrans.TranType;
            umemeTrans.DigitalSignature = GetDigitalSignature(dataToSign, trans.VendorCode);
            return umemeTrans;
        }




        public PostResponse SendToNWSC(ConsoleApplication1.EntityObjects.NWSCTransaction trans)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            string vendorCode = trans.VendorCode;
            string custref = trans.CustRef;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    NWSCApi.NWSCBillingInterface nwscapi = new NWSCApi.NWSCBillingInterface();
                    string format = "d/M/yyyy";
                    string newdate = formateDate(trans.PaymentDate).Trim();

                    DateTime payDate = DateTime.ParseExact(newdate, format, CultureInfo.InvariantCulture);

                    NWSCApi.Customer customer = nwscapi.verifyCustomerDetailsWithArea(trans.CustRef, trans.Area, creds.UtilityCode, creds.UtilityPassword);

                    if (customer.CustomerError.Equals("NONE"))
                    {

                        NWSCApi.PostResponse waterResp = nwscapi.postCustomerTransactionsWithArea(trans.CustRef, trans.CustName, customer.Area, trans.CustomerTel, payDate, int.Parse(trans.TransactionAmount), trans.VendorTransactionRef, trans.TransactionType, creds.UtilityCode, creds.UtilityPassword);

                        if (waterResp.PostError.Equals("NONE"))
                        {
                            resp.StatusCode = "0";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            dp.UpdateSentTransactionById(trans.VendorTransactionRef, "", "SUCCESS");
                            Msg = "Dear  " + trans.CustName + ", NWSC has received a payment of  UGX  " + trans.TransactionAmount + " for a/c  " + trans.CustRef;

                        }
                        else if (waterResp.PostError.Contains("Duplicate"))
                        {
                            resp.StatusCode = "0";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            dp.UpdateSentTransactionById(trans.VendorTransactionRef, "", "SUCCESS");
                            Msg = "Dear  " + trans.CustName + ", NWSC has received a payment of  UGX  " + trans.TransactionAmount + " for a/c  " + trans.CustRef;
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "100";
                            resp.StatusDescription = waterResp.PostError;
                            //fail transaction
                            dp.TransferDeletedTransaction(trans.TranId, waterResp.PostError);
                            dp.LogError(waterResp.PostError + " - " + custref, vendorCode, DateTime.Now, "NWSC");
                            Msg = "Dear  " + trans.CustName +
                             ", Your payment of  UGX  " + trans.TransactionAmount +
                             " to NWSC has Failed";
                        }
                    }
                    else
                    {
                        //do nothing
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = customer.CustomerError;
                        //fail transaction
                        dp.TransferDeletedTransaction(trans.TranId, customer.CustomerError);
                        dp.LogError(customer.CustomerError + " - " + custref, vendorCode, DateTime.Now, "NWSC");
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "29";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
                try
                {
                    if (trans.VendorCode.Equals("Orange") && !Msg.Equals(""))
                    {
                        string Service = "OrangeService";
                        SMS sms = new SMS();
                        sms.Mask = "AfriMoney";
                        sms.Message = Msg;
                        sms.Phone = trans.CustomerTel;
                        sms.Reference = trans.VendorTransactionRef;
                        sms.Sender = Service;
                        sms.VendorTranId = trans.VendorTransactionRef;
                        LogSMS(sms);
                    }
                    else if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                    {
                        string Service = "CellulantService";
                        SMS sms = new SMS();
                        sms.Mask = "STANBIC";
                        sms.Message = Msg;
                        sms.Phone = trans.CustomerTel;
                        sms.Reference = trans.VendorTransactionRef;
                        sms.Sender = Service;
                        sms.VendorTranId = trans.VendorTransactionRef;
                        LogSMS(sms);
                    }
                }
                catch (Exception e)
                {

                }
            }
            catch (Exception ex)
            {
                dp.LogError(ex.Message + " - " + custref, vendorCode, DateTime.Now, "NWSC");
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }


        public PostResponse SendToUmeme(ConsoleApplication1.EntityObjects.UmemeTransaction trans)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            string vendorCode = trans.VendorCode;
            string custref = trans.CustRef;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dp.GetUtilityCreds("UMEME", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {

                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    UmemeApi.EPayment umemeapi = new UmemeApi.EPayment();
                    UmemeApi.Transaction umemeTrans = GetUmemeTrans(trans, creds);
                    UmemeApi.Response umemeResp = umemeapi.PostBankUmemePayment(umemeTrans);

                    if (umemeResp.StatusCode.Equals("0"))
                    {
                        dp.UpdateSentTransactionById(trans.VendorTransactionRef, umemeResp.ReceiptNumber, "SUCCESS");
                        //resp.UmemeReceiptNumber = umemeResp.ReceiptNumber;
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                        if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                        {
                            Msg = "Dear " + trans.CustName + ", YAKA purchase of UGX " + trans.TransactionAmount + " for a/c " + trans.CustRef + " received by UMEME. Token: " + umemeResp.ReceiptNumber;
                        }
                        else
                        {
                            Msg = "Dear  " + trans.CustName + ", UMEME has received a payment of  UGX  " + trans.TransactionAmount + " for a/c  " + trans.CustRef + " . Your Ref. is  " + umemeResp.ReceiptNumber;
                        }
                    }
                    else if (umemeResp.StatusDescription.Trim().ToUpper().Contains("DUPLICATE VENDOR REFERENCE"))
                    {
                        //dp.UpdateSentTransaction(trans.TransNo, umemeResp.ReceiptNumber);
                        UmemeApi.Response umemeResp2 = umemeapi.GetTransactionDetails(trans.VendorTransactionRef, creds.UtilityCode, creds.UtilityPassword);
                        if (umemeResp2.StatusCode.Equals("0"))
                        {
                            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                            {
                                dp.UpdateSentTransactionById(trans.VendorTransactionRef, umemeResp2.Token, "SUCCESS");
                                Msg = "Dear " + trans.CustName +
                                      ", YAKA purchase of UGX " + trans.TransactionAmount +
                                      " for a/c " + trans.CustRef +
                                      " received by UMEME. Token: " + umemeResp2.Token;
                            }
                            else
                            {
                                Msg = "Dear  " + trans.CustName +
                                      ", UMEME has received a payment of  UGX  " + trans.TransactionAmount +
                                      " for a/c  " + trans.CustRef +
                                      " . Your Ref. is  " + umemeResp.ReceiptNumber;
                                dp.UpdateSentTransactionById(trans.VendorTransactionRef, umemeResp2.ReceiptNumber, "SUCCESS");
                            }
                        }
                        else
                        {
                            dp.UpdateSentTransactionById(trans.VendorTransactionRef, umemeResp.ReceiptNumber, "SUCCESS");
                        }
                        dp.LogError(umemeResp.StatusDescription + " - " + custref, vendorCode, DateTime.Now, "UMEME");
                        //resp.UmemeReceiptNumber = umemeResp.ReceiptNumber;
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }
                    else if (umemeResp.StatusDescription.Trim().ToUpper().Contains("SUSPECTED DOUBLE POSTING"))
                    {
                        dp.LogError(umemeResp.StatusDescription + " - " + custref, vendorCode, DateTime.Now, "UMEME");
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        dp.TransferDeletedTransaction(trans.TranId, umemeResp.StatusDescription + " AT UMEME");
                        //resp.UmemeReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = umemeResp.StatusDescription + " AT UMEME";
                        dp.LogError(umemeResp.StatusDescription + " - " + custref, vendorCode, DateTime.Now, "UMEME");
                        Msg = "Dear  " + trans.CustName +
                             ", Your payment of  UGX  " + trans.TransactionAmount +
                             " to UMEME has Failed";
                    }
                }
                else
                {
                    // resp.UmemeReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "29";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }

                //log SMS
                try
                {
                    if (trans.VendorCode.Equals("MTN") && trans.CustomerType.Equals("PREPAID") && !Msg.Equals(""))
                    {
                        string Service = "MTNService";
                        string Mask = "8888";
                        dp.LogSMS(trans.CustomerTel, trans.VendorTransactionRef, Msg, Mask, Service);
                    }
                    else if (trans.VendorCode.Equals("UTL") && !Msg.Equals(""))
                    {
                        string Service = "UTLService";
                        SMS sms = new SMS();
                        sms.Mask = "Msente";
                        sms.Message = Msg;
                        sms.Phone = trans.CustomerTel;
                        sms.Reference = trans.VendorTransactionRef;
                        sms.Sender = Service;
                        sms.VendorTranId = trans.VendorTransactionRef;
                        LogSMS(sms);
                    }
                    else if (trans.VendorCode.ToUpper().Equals("ORANGE") && !Msg.Equals(""))
                    {
                        string Service = "OrangeService";
                        SMS sms = new SMS();
                        sms.Mask = "AfriMoney";
                        sms.Message = Msg;
                        sms.Phone = trans.CustomerTel;
                        sms.Reference = trans.VendorTransactionRef;
                        sms.Sender = Service;
                        sms.VendorTranId = trans.VendorTransactionRef;
                        LogSMS(sms);
                    }
                    else if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                    {
                        string Service = "CellulantService";
                        SMS sms = new SMS();
                        sms.Mask = "STANBIC";
                        sms.Message = Msg;
                        sms.Phone = trans.CustomerTel;
                        sms.Reference = trans.VendorTransactionRef;
                        sms.Sender = Service;
                        sms.VendorTranId = trans.VendorTransactionRef;
                        LogSMS(sms);
                    }
                }
                catch (Exception ex)
                {
                    dp.LogError("Stanbic SendToUmeme - " + custref + " : " + ex.Message, vendorCode, DateTime.Now, "UMEME");
                }
            }
            catch (Exception ex)
            {
                dp.LogError("Stanbic SendToUmeme - " + custref + " : " + ex.Message, vendorCode, DateTime.Now, "UMEME");
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }

        public PostResponse SendToURA(URATransaction trans)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            string vendorCode = trans.VendorCode;
            string custref = trans.CustRef;
            string Msg = "";
            try
            {
                UtilityCredentials creds = dp.GetUtilityCreds("URA", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    ConsoleApplication1.StanbicApi.PegPay pegpay = new ConsoleApplication1.StanbicApi.PegPay();
                    ConsoleApplication1.StanbicApi.TransactionRequest tr = GetStanbicApiUraReqObj(trans);
                    ConsoleApplication1.StanbicApi.Response stanbicResp = pegpay.PostTransaction(tr);

                    //success
                    if (stanbicResp.ResponseField6 == "0")
                    {
                        resp.StatusCode = "0";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        dp.UpdateSentTransactionById(trans.VendorTransactionRef, stanbicResp.ResponseField8, "SUCCESS");
                        Msg = "Dear  " + trans.CustName +
                              ", URA has received a payment of  UGX  " + trans.TransactionAmount +
                              " for a/c  " + trans.CustRef;
                    }
                    //duplicate
                    else if (stanbicResp.ResponseField6.ToUpper().Contains("DUPLICATE"))
                    {
                        resp.StatusCode = "0";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        dp.UpdateSentTransactionById(trans.VendorTransactionRef, stanbicResp.ResponseField8, "SUCCESS");
                        Msg = "Dear  " + trans.CustName +
                              ", URA has received a payment of  UGX  " + trans.TransactionAmount +
                              " for a/c  " + trans.CustRef;
                    }
                    //failed
                    else
                    {

                        //do nothing
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = stanbicResp.ResponseField7;
                        //delete transaction
                        dp.TransferDeletedTransaction(trans.TranId, stanbicResp.ResponseField7);
                        dp.LogError(stanbicResp.ResponseField7 + " - " + custref, vendorCode, DateTime.Now, "URA");
                        Msg = "Dear  " + trans.CustName +
                              ", Your payment of  UGX  " + trans.TransactionAmount +
                              " to URA has Failed";
                    }
                }
                if (trans.VendorCode.ToUpper().Equals("STANBIC_VAS") && !Msg.Equals(""))
                {
                    string Service = "CellulantService";
                    SMS sms = new SMS();
                    sms.Mask = "STANBIC";
                    sms.Message = Msg;
                    sms.Phone = trans.CustomerTel;
                    sms.Reference = trans.VendorTransactionRef;
                    sms.Sender = Service;
                    sms.VendorTranId = trans.VendorTransactionRef;
                    LogSMS(sms);
                }
            }
            catch (Exception ex)
            {
                dp.LogError(ex.Message + "PostUMEMEPayments SERVICE POSTING - " + custref, vendorCode, DateTime.Now, "UMEME");
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }

        private ConsoleApplication1.StanbicApi.TransactionRequest GetStanbicApiUraReqObj(URATransaction tr)
        {
            ConsoleApplication1.StanbicApi.TransactionRequest trans = new ConsoleApplication1.StanbicApi.TransactionRequest();
            try
            {

                trans.PostField1 = tr.CustRef;//custRef
                trans.PostField2 = tr.CustName;//CustName
                trans.PostField3 = tr.TIN;
                trans.PostField21 = "Test";//CustomerType
                trans.PostField5 = "";//PaymentDate
                trans.PostField21 = "CASH";//PaymentType
                trans.PostField7 = tr.TransactionAmount;//TransactionAmount
                trans.PostField8 = "Tax";//TransactionType
                trans.PostField9 = "040047";//VendorCode
                trans.PostField10 = "C1bn@t5#739";//Password
                trans.PostField11 = tr.CustomerTel;//CustomerTel
                trans.PostField12 = "0";//Reversal
                trans.PostField13 = "";//TranIdToReverse
                trans.PostField14 = tr.CustomerTel;//Teller
                trans.PostField15 = "1";//Offline
                trans.PostField16 = "";//DigitalSignature
                trans.PostField17 = "";//ChequeNumber
                trans.PostField18 = "URAVAS";//Narration
                trans.PostField19 = "";//Email
                trans.PostField20 = tr.VendorTransactionRef;//VendorTransactionRef
                trans.PostField4 = "MBURA";//UtilityCode
            }
            catch (Exception ex)
            {
            }
            return trans;
        }





        private bool IsSuccessResponse(SubmitPaymentResponse resp)
        {
            if (resp.SubmitPayment.ErrorMessage == "" && resp.SubmitPayment.Status == true)
            {
                return true;
            }
            if (resp.SubmitPayment.ErrorMessage == null && resp.SubmitPayment.Status == true)
            {
                return true;
            }
            if (resp.SubmitPayment.ErrorMessage.ToUpper().Contains("ALREADY PROCESSED"))
            {
                return true;
            } 
            if (resp.SubmitPayment.ErrorMessage.ToUpper().Contains("DUPLICATE TRANSACTION"))
            {
                return true;
            }

            return false;
        }

        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }




    }
}
