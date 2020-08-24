using LiveVasUtilityTranProcessorLibrary.EntityObjects;
using LiveVasUtilityTranProcessorLibrary.PegPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LiveVasUtilityTranProcessorLibrary.ControlObjects
{
    public class BussinessLogic
    {
        DatabaseHandler dh = new DatabaseHandler();

        private PostResponse GetExceptionResponse(Exception ex)
        {
            PostResponse resp = new PostResponse();
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
            return resp;
        }
        public PostResponse SendToUtilitiesApi(Transaction tran)
        {
           // Console.WriteLine("Processing Tran :" + tran.VendorTranId);
            PostResponse postResp = new PostResponse();
            if (tran.PassesValidation())
            {
                Response utilitiesApiResp = CallLevel1(tran);


                if (IsSuccessResp(utilitiesApiResp))
                {
                    string utilityRef = utilitiesApiResp.ResponseField8;
                    postResp.StatusCode = "0";
                    postResp.StatusDescription = "SUCCESS";
                    postResp.PegPayPostId = utilityRef;
                }
                //error like general error or pegpay db unavailable
                else if (IsSkippableErrorResponse(utilitiesApiResp))
                {
                    //do nothing levae transaction PENDING
                    Console.WriteLine("Skipping because UtilitiesApiResponse: " + utilitiesApiResp.ResponseField7);
                    //take transaction status back to PENDING
                    //so it can be retried
                    string Reason = utilitiesApiResp.ResponseField7;
                    postResp.StatusCode = "500";
                    postResp.StatusDescription = Reason;
                    postResp.PegPayPostId = "";
                }

                //some other error
                else
                {
                    string Reason = utilitiesApiResp.ResponseField7;
                    postResp.StatusCode = "100";
                    postResp.StatusDescription = Reason;
                    postResp.PegPayPostId = "";
                }
            }
            //it has failed validation
            else
            {
                string Reason = tran.StatusDescription;
                postResp.StatusCode = "100";
                postResp.StatusDescription = Reason;
                postResp.PegPayPostId = "";
            }
            return postResp;
        }


        public void MarkTransactionAsSUCCESSFULL(Transaction tran, string utilityTranRef, string status)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                Console.WriteLine("Tran :" + tran.VendorTranId + " Successfull: " + utilityTranRef);

                dh.UpdateTransactionStatus(tran.RecordId, status, utilityTranRef);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsSUCCESSFULL: " + ee.Message, "STANBIC_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }

        }

        private void MarkTransactionAsPending(Transaction tran, string utilityTranRef, string status)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                Console.WriteLine("Tran :" + tran.VendorTranId + " Pending: " + utilityTranRef);

                dh.UpdateTransactionStatus(tran.RecordId, status, utilityTranRef);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsPending: " + ee.Message, "STANBIC_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }
        }

        public void MarkTransactionAsFAILED(Transaction tran, string Reason)
        {
            
            try
            { //update tran status to indicate failure
                string status = "FAILED";
                Console.WriteLine("Tran :" + tran.VendorTranId + " Failed: " + Reason);
                //DatabaseHandler dh = new DatabaseHandler();
                dh.LogError(tran.VendorTranId + " " + Reason, "STANBIC_VAS", DateTime.Now, tran.UtilityCompany);
                dh.UpdateTransactionStatus(tran.RecordId, status, Reason);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsFAILED: " + ee.Message, "STANBIC_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }
        }


        public bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }



        internal void HandleUtilityResponse(PostResponse utilityResp, Transaction trans)
        {
            //This is how I mapped the response codes
            //0- Success, 100- Failed, 1000-Pending, 
            //500- Exceptional error/Undefined error do nothing e.g timeout,general error etc
            string StatusCode, StatusDesc;
            //log the response from the utility first
            LogUtilityResponse(utilityResp, trans);

            if (String.IsNullOrEmpty(utilityResp.PegPayPostId))
            {
                StatusCode = "100";
                StatusDesc = "DUPLICATE REFERENCE ID SUPPLIED";
            }
            else
            {
                StatusCode = utilityResp.StatusCode;
                StatusDesc = utilityResp.StatusDescription;
            }
            
           

            //success
            if (StatusCode == "0")
            {
                //mark transaction as successfull
                MarkTransactionAsSUCCESSFULL(trans, utilityResp.PegPayPostId, "SUCCESS");
                Console.WriteLine("Successfully processed"+trans.VendorTranId+ "with reference "+ utilityResp.PegPayPostId);
            }
            //exception or skippable error(general error) has occurec
            else if (StatusCode != "100")
            {
                string reason = utilityResp.StatusDescription;
                //mark transaction as pending
                MarkTransactionAsPending(trans, utilityResp.PegPayPostId, "PENDING");
                Console.WriteLine("Failed to process" + trans.VendorTranId + "with reference " + utilityResp.PegPayPostId);
            }
            //failure
            else
            {
                //mark transaction as failed
                string reason = StatusDesc;
                MarkTransactionAsFAILED(trans, reason);
            }

        }

        public bool IsSkippableErrorResponse(Response utilitiesApiResp)
        {
            if (utilitiesApiResp.ResponseField7.ToUpper().Contains("GENERAL ERROR"))
            {
                return true;
            }
            else if (utilitiesApiResp.ResponseField7.ToUpper().Contains("SUSPECTED DOUBLE"))
            {
                return true;
            }
            else if (utilitiesApiResp.ResponseField7.ToUpper().Contains("PEGPAYDB"))
            {
                return true;
            }
            //Timeout or other Exception
            else if (utilitiesApiResp.ResponseField6.Equals("000") || utilitiesApiResp.ResponseField6.Equals("2000"))
            {
                return true;
            }
            else if (utilitiesApiResp.ResponseField7.ToUpper().Contains("CONNECTIVITY ERROR"))
            {
                //CONNECTIVITY ERROR AT UTILITY
                return true;
            }
            else
            {
                return false;
            }
        }



        public bool IsSuccessResp(Response utilitiesApiResp)
        {
            if (utilitiesApiResp.ResponseField7.Contains("SUCCESS"))
            {
                return true;
            }
            else if (utilitiesApiResp.ResponseField7.Contains("DUPLICATE"))
            {
                return true;
            }
            return false;
        }

        public Response GetTransactionStatus(Transaction tr)
        {
            Response resp = new Response();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                QueryRequest request = new QueryRequest();
                request.QueryField5 = tr.VendorCode;
                request.QueryField6 = tr.Password;
                request.QueryField10 = tr.VendorTranId;


                PegPay.PegPay pegpay = new PegPay.PegPay();
                resp = pegpay.GetTransactionDetails(request);

                return resp;
            }
            catch (Exception e)
            {
                resp.ResponseField6 = "2000";
                resp.ResponseField7 = e.Message;
                
            }
            return resp;
        }

        public Response CallLevel1(Transaction tr)
        {
            Response resp = new Response();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

                PegPay.PegPay pegpay = new PegPay.PegPay();
                TransactionRequest trans = new TransactionRequest();
                trans.PostField1 = tr.CustRef;
                trans.PostField2 = tr.CustName;
                trans.PostField3 = tr.Area;//Area
                trans.PostField4 = tr.UtilityCompany;
                trans.PostField5 = tr.PaymentDate;
                trans.PostField6 = tr.PaymentType;
                trans.PostField7 = tr.TransactionAmount;
                trans.PostField8 = tr.TransactionType;
                trans.PostField9 = tr.VendorCode;
                trans.PostField10 = tr.Password;
                trans.PostField11 = tr.CustomerTel;
                trans.PostField12 = tr.Reversal;
                trans.PostField13 = tr.TranIdToReverse;
                trans.PostField14 = tr.Teller;
                trans.PostField15 = tr.Offline;
                trans.PostField17 = tr.ChequeNumber;//chequeNumber
                trans.PostField18 = tr.Narration;
                trans.PostField19 = tr.Email;//email
                trans.PostField20 = tr.VendorTranId;

                trans.PostField21 = "2";
                trans.PostField22 = tr.PaymentType;
                trans.PostField33 = tr.Area;//Area

                string dataToSign = tr.CustRef +
                                    tr.CustName +
                                    tr.CustomerTel +
                                    tr.VendorTranId +
                                    tr.VendorCode +
                                    tr.Password +
                                    tr.PaymentDate +
                                    tr.Teller +
                                    tr.TransactionAmount +
                                    tr.Narration +
                                    tr.TransactionType;
                trans.PostField16 = GetDigitalSignature(tr);// "1234" for MTN
                tr.digitalSignature = GetDigitalSignature(tr);

                resp = pegpay.PostTransaction(trans);

                return resp;
            }
            catch (Exception e)
            {
                resp.ResponseField6 = "2000";
                resp.ResponseField7 = e.Message;
                //dh.LogError("MarkTransactionAsPending: " + ee.Message, "STANBIC_VAS", DateTime.Now, tran.VendorTranId);
            }
            return resp;
        }


        public string GetDigitalSignature(Transaction umemeTrans)
        {
            // retrieve public key||@"C:\PegPayCertificates1\Orange\41.202.229.3.cer"
            string text = umemeTrans.CustRef + umemeTrans.CustName + umemeTrans.CustomerTel + umemeTrans.VendorTranId +
                          umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.Teller + umemeTrans.TransactionAmount +
                          umemeTrans.Narration + umemeTrans.TransactionType;
            string certificate = @"E:\Certs\pegasus.pfx";

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

        private void LogUtilityResponse(PostResponse response, Transaction tran)
        {
            DatabaseHandler dh = new DatabaseHandler();
            string OtherData = "UtilityCode = " + tran.UtilityCompany + ", UtilityRef = " + response.PegPayPostId + ", StatusCode = " + response.StatusCode + " StatusDesc = " + response.StatusDescription;
            dh.InsertIntoUtilityResponseLogs(tran.VendorCode, tran.VendorTranId, response.StatusCode, response.StatusDescription, OtherData);
        }



    }
}
