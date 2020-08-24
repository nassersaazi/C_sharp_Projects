using System;
using System.Collections.Generic;
using System.Text;
using myReversalTester.EntityObject;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Globalization;
using myReversalTester.StanbicApi;

namespace myReversalTester.ControlObject
{
    class BusinessLogic
    {

        public PostResponse SendToUmeme(Transaction trans)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            string vendorCode = trans.VendorCode;
            string custref = trans.CustRef;

            try
            {
                UtilityCredentials creds = dp.GetUtilityCreds("UMEME", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    UmemeApi.EPayment umemeapi = new UmemeApi.EPayment();
                    creds.UtilityCode = "Stanbic-Bank";
                    creds.UtilityPassword = "86Y80RY700";

                    UmemeApi.Transaction umemeTrans = GetUmemeTrans(trans, creds);

                    //throw new Exception("unable to connect");
                    UmemeApi.Token umemeResp = umemeapi.PostYakaPayment(umemeTrans);

                    string UmemeStatusCode = umemeResp.StatusCode.ToUpper();
                    string UmemeStatusDesc = umemeResp.StatusDescription.Trim().ToUpper();

                    if (UmemeStatusCode == "0")
                    {
                        if (trans.CustomerType.ToUpper() == "PREPAID")
                        {
                            resp.NoOfUnits = umemeResp.Units;
                            resp.PegPayPostId = umemeResp.PrepaidToken;
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";
                        }
                        else
                        {
                            resp.NoOfUnits = umemeResp.Units;
                            resp.PegPayPostId = umemeResp.ReceiptNumber;
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";
                        }
                    }
                    else if (UmemeStatusDesc.Contains("DUPLICATE VENDOR REFERENCE"))
                    {
                        UmemeApi.Response umemeResp2 = umemeapi.GetTransactionDetails(trans.BankID, creds.UtilityCode, creds.UtilityPassword);
                        if (umemeResp2.StatusCode == "0")
                        {
                            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                            {
                                resp.PegPayPostId = umemeResp2.Token;
                                resp.StatusCode = "0";
                                resp.StatusDescription = "SUCCESS";
                            }
                            else
                            {
                                resp.PegPayPostId = umemeResp2.ReceiptNumber;
                                resp.StatusCode = "0";
                                resp.StatusDescription = "SUCCESS";
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "100";
                            resp.StatusDescription = umemeResp.StatusDescription + " AT UMEME";
                        }
                    }
                    else if (UmemeStatusDesc.Contains("SUSPECTED DOUBLE POSTING"))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = UmemeStatusDesc + " AT UMEME";
                    }
                    else if (UmemeStatusDesc.Contains("GENERAL ERROR"))
                    {
                        //signal to skip this transaction
                        //resp.PegPayPostId = "";
                        //resp.StatusCode = "500";
                        //resp.StatusDescription = UmemeStatusDesc;

                        UmemeApi.Response umemeResp2 = umemeapi.GetTransactionDetails(trans.BankID, creds.UtilityCode, creds.UtilityPassword);
                        if (umemeResp2.StatusCode.Equals("35") && umemeResp2.StatusDescription.Contains("DON'T EXIST")) // Doesnt exist
                        {
                            resp.StatusCode = "100";
                            resp.StatusDescription = UmemeStatusDesc;
                        }
                        else
                        {
                            resp.StatusCode = "500";
                            resp.StatusDescription = UmemeStatusDesc;
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = UmemeStatusDesc + " AT UMEME";
                    }
                }
                else
                {
                    //////////////////////////////// remove this part
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "UTILITY CREDENTIALS NOT SET AT PEGASUS FOR THIS REVERSAL";
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message.ToUpper().Trim();

                if (errorMsg.Contains("UNABLE TO CONNECT"))
                {
                    //signal to fail this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = ex.Message;
                }
                else
                {
                    //signal to skip this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "500";
                    resp.StatusDescription = ex.Message;
                }
            }
            return resp;
        }

        private UmemeApi.Transaction GetUmemeTrans(Transaction trans, UtilityCredentials creds)
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
            umemeTrans.VendorTranId = trans.BankID;
            string dataToSign = umemeTrans.CustomerRef + umemeTrans.CustomerName + umemeTrans.CustomerTel + umemeTrans.CustomerType + umemeTrans.VendorTranId + umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.PaymentType + umemeTrans.Teller + umemeTrans.TranAmount + umemeTrans.TranNarration + umemeTrans.TranType;
            umemeTrans.DigitalSignature = GetDigitalSignature(dataToSign, trans.VendorCode);
            return umemeTrans;
        }

        private string GetDigitalSignature(string text, string vendorCode)
        {
            // retrieve private key||@"C:\PegPayCertificates1\Orange\41.202.229.3.cer"
            string certificate = "";
            certificate = @"E:\\Certificates\\CELL\\CELL.pfx";
            //string certificate = @"C:\PegPayCertificates1\Ezee-Money\ezeemoney-ug_com.crt";
            X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
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


    /*    public PostResponse SendToNWSC(Transaction trans)
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
                    
                    string format = "dd/MM/yyyy";
                    string newdate = formateDate(trans.PaymentDate).Trim();

                    DateTime payDate = DateTime.ParseExact(newdate, format, CultureInfo.InvariantCulture);

                    //post using stanbic credentials
                    creds.UtilityCode = "Stanbic-Bank";
                    creds.UtilityPassword = "ZF0KTIFW";


                    NWSCApi.PostResponse waterResp = nwscapi.postCustomerTransactionsWithArea(trans.CustRef, trans.CustName, trans.Area, trans.CustomerTel, payDate, int.Parse(trans.TransactionAmount), trans.BankID, trans.TransactionType, creds.UtilityCode, creds.UtilityPassword);

                    if (waterResp.PostError.Equals("NONE"))
                    {
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }
                    else if (waterResp.PostError.ToUpper().Contains("DUPLICATE"))
                    {
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }
                    else if (waterResp.PostError.Contains("GENERAL"))
                    {
                        resp.StatusCode = "500";
                        resp.StatusDescription = waterResp.PostError;
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "100";
                        resp.StatusDescription = waterResp.PostError + " AT NWSC";
                    }
                }
                else
                {
                    //do nothing
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "UTILITY CREDENTIALS NOT SET AT PEGASUS";
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message.ToUpper().Trim();

                if (errorMsg.Contains("UNABLE TO CONNECT"))
                {
                    //signal to skip this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = ex.Message;
                }
                else if (errorMsg.Contains("SERVER WAS UNABLE TO PROCESS"))
                {
                    //signal to skip this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = ex.Message;
                }
                else if (errorMsg.Contains("TIME OUT") || errorMsg.Contains("TIMED OUT"))
                {
                    //signal to skip this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "500";
                    resp.StatusDescription = ex.Message;
                }
                else
                {
                    //signal to skip this transaction
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = ex.Message;
                }
            }
            return resp;
        }*/

       
        internal PostResponse GetUtilityNotSupportedResponse()
        {
            PostResponse utilityResp = new PostResponse();
            utilityResp.StatusCode = "100";
            utilityResp.StatusDescription = "UTILITY NOT YET SUPPORTED AT PEGASUS FOR THIS TYPE OF TRANSACTIONS";
            return utilityResp;
        }


        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        internal void HandleUtilityResponse(PostResponse utilityResp, Transaction trans)
        {
            //log the response from the utility first
            LogUtilityResponse(utilityResp, trans);

            //depending on the response lets see how to handle
            DatabaseHandler dh = new DatabaseHandler();
            string StatusCode = utilityResp.StatusCode;
            string StatusDesc = utilityResp.StatusDescription;

            //success
            if (StatusCode == "0")
            {
                //Transaction reached Utility successfull.Reverse as requested
                MarkTransactionAsSuccessfull(trans, utilityResp);
                
            }
            //exception or skippable error(general error) has occurec
            else if (StatusCode == "500")
            {
                //log error..then skip transaction i.e do nothing
                string errorMsg = StatusDesc;
                dh.LogError(errorMsg, trans.VendorCode, DateTime.Now, trans.UtilityCode);
                dh.UpdatePrepaidTransactionStatus(trans.VendorCode, trans.VendorTransactionRef, "PENDING");
            }
            //failure
            else if (StatusCode == "100")
            {
                //transaction dint reach Utility log
                TransactionNotAtUtilityAndReverseAtBank(trans, utilityResp);
            }
            else
            {
                //unkwon error
                //log error..then skip transaction i.e do nothing
                string errorMsg = StatusDesc;
                dh.LogError(errorMsg, trans.VendorCode, DateTime.Now, trans.UtilityCode);
                dh.UpdatePrepaidTransactionStatus(trans.VendorCode, trans.VendorTransactionRef, "PENDING");
            }
        }

        private void MarkTransactionAsSuccessfull(Transaction trans, PostResponse utilityResp)
        {
            string Reason = "";
            if (trans.UtilityCode == "UMEME")
            {
                Reason = utilityResp.NoOfUnits;
            }
            else if (trans.UtilityCode == "NWSC")
            {
                Reason = utilityResp.NoOfUnits;
            }
            else
            {
                Reason = utilityResp.PegPayPostId;
            }

            string UtilityRef = utilityResp.PegPayPostId;
            string Status = "SUCCESS";
            DatabaseHandler dh = new DatabaseHandler();
            dh.SuccessfulUtilityReversalRequests(trans.VendorTransactionRef, trans.VendorCode, Status, UtilityRef, Reason);
            TransactionReachedUtilityThenReverse(trans, utilityResp);
        }
        

        private void TransactionReachedUtilityThenReverse(Transaction trans, PostResponse utilityResp)
        {
            DatabaseHandler dh = new DatabaseHandler();
            BusinessLogic bll = new BusinessLogic();
            PostResponse bankResp = bll.ReverseTransactionAtStanbic(trans);

            //log bank response
            LogBankResponse(bankResp, trans);

            //reversed successfully at bank
            if (bankResp.StatusCode == "0")
            {
                string Reason = utilityResp.StatusDescription;
                string Status = "REVERSED";
                dh.ReverseTransactionAndCharges(trans.VendorTransactionRef, trans.VendorCode, Reason, bankResp.PegPayPostId);
            }
            else if (bankResp.StatusCode == "100")
            {
                //failed to reverse at Bank
                //lets leave this transaction pending
            }
            else
            {
                //unknown error
                //lets leave this transaction pending
            }
        }

        private void LogUtilityResponse(PostResponse response, Transaction tran)
        {
            DatabaseHandler dh = new DatabaseHandler();
            string OtherData = "UtilityCode = " + tran.UtilityCode + ", UtilityRef = " + response.PegPayPostId + ", StatusCode = " + response.StatusCode + " StatusDesc = " + response.StatusDescription;
            dh.InsertIntoUtilityResponseLogs(tran.VendorCode, tran.VendorTransactionRef, response.StatusCode, response.StatusDescription, OtherData);
        }


        private PostResponse ReverseTransactionAtStanbic(Transaction trans)
        {
            PostResponse postResp = new PostResponse();
            try
            {
                DatabaseHandler dh = new DatabaseHandler();
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                System.Net.ServicePointManager.Expect100Continue = false;

                PegPay pegpay = new PegPay();
                TransactionRequest req = new TransactionRequest();
                req.PostField8 = dh.GetSystemSetting(22, 1);
                req.PostField5 = trans.TransNo;
                req.PostField4 = trans.UtilityCode;

                string dataToSign = req.PostField5 + req.PostField4 + req.PostField8;
                req.PostField25 = GetDigitalSign(dataToSign);

                StanbicApi.Response resp = pegpay.ReverseTransaction(req);

                string statusCode = resp.ResponseField6.ToUpper();
                if (statusCode == "0" || statusCode == "21")
                {
                    postResp.StatusCode = "0";
                    postResp.StatusDescription = "SUCCESS";
                    postResp.PegPayPostId = resp.ResponseField8;
                    dh.UpdatePrepaidTransactionStatus(trans);

                }
                else
                {
                    postResp.StatusCode = "100";
                    postResp.StatusDescription = resp.ResponseField7;
                    postResp.PegPayPostId = "";
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message.ToUpper().Trim();

                if (errorMsg.Contains("UNABLE TO CONNECT"))
                {
                    //signal to skip this transaction
                    postResp.PegPayPostId = "";
                    postResp.StatusCode = "500";
                    postResp.StatusDescription = ex.Message;
                }
                else
                {
                    //signal to skip this transaction
                    postResp.PegPayPostId = "";
                    postResp.StatusCode = "500";
                    postResp.StatusDescription = ex.Message;
                }
            }
            return postResp;
        }

                
        private static string GetDigitalSign(string DataToSign)
        {
            try
            {
                string appPath, physicalPath;

                string certificate = @"E:\\PePay\\MicroPayTester\\StanbicCBprvt.pfx";
                X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.MachineKeySet);
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

                // Hash the data
                SHA1Managed sha1 = new SHA1Managed();

                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(DataToSign);
                byte[] hash = sha1.ComputeHash(data);

                // Sign the hash
                byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
                string strDigCert = Convert.ToBase64String(digitalCert);
                return strDigCert;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

        private void LogBankResponse(PostResponse response, Transaction tran)
        {
            DatabaseHandler dh = new DatabaseHandler();
            string OtherData = "PegPayId = " + response.PegPayPostId + ", TransactionId = " + response.StatusCode;
            dh.InsertIntoBankResponseLogs(tran.VendorCode, tran.VendorTransactionRef, response.StatusCode, response.StatusDescription, OtherData);
        }


        private void TransactionNotAtUtilityAndReverseAtBank(Transaction trans, PostResponse utilityResp)
        {
            DatabaseHandler dh = new DatabaseHandler();
            BusinessLogic bll = new BusinessLogic();
            PostResponse bankResp = bll.ReverseTransactionAtStanbic(trans);

            //log bank response
            LogBankResponse(bankResp, trans);

            //reversed successfully at bank
            if (bankResp.StatusCode == "0")
            {
                string Reason = utilityResp.StatusDescription;
                string Status = "FAILED";
                dh.FailTransactionAndReverseCharges(trans.VendorTransactionRef, trans.VendorCode, Reason, bankResp.PegPayPostId);
            }
            else if (bankResp.StatusCode == "100")
            {
                //failed to reverse at Bank
                //lets leave this transaction pending
            }
            else
            {
                //unknown error
                //lets leave this transaction pending
            }
        }
    }
}
