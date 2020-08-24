using LiveVasUtilitySenderLibrary.EntityObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LiveVasUtilitySenderLibrary.ControlObjects
{
    public class BusinessLogic
    {
        DatabaseHandler dh = new DatabaseHandler();

        public PostResponse SendToFlexipaySchools(Transaction tran)
        {
            PostResponse response = new PostResponse();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SchoolsApi.SchoolApi api = new SchoolsApi.SchoolApi();
                api.Timeout = 4500000;
                SchoolsApi.SchoolTransaction trans = new SchoolsApi.SchoolTransaction();
                trans.StudentId = tran.CustRef;
                trans.StudentName = tran.CustName;
                trans.Amount = tran.TransactionAmount;
                trans.PaymentDate = tran.PaymentDate;
                trans.Teller = tran.CustomerTel;
                trans.TransactionID = tran.VendorTranId;
                trans.TranCharge1 = "";
                trans.TranCharge2 = "";
                trans.VisaCardNumber = "";
                trans.VisaCvvNumber = "";

                trans.Narration = "TUITION";
                trans.Agent = "stan_counter";
                trans.SchoolCode = tran.Area;


                if (!string.IsNullOrEmpty(trans.SchoolCode))
                {
                    trans.Channel = "MBANK";
                    trans.Username = "Schoolsfees-Payment";
                    trans.Password = "czGlDWl8Y5xPwHWfwpZnOQ==";

                    string text = trans.SchoolCode + trans.StudentId + trans.StudentName + trans.Amount + trans.PaymentType + trans.PaymentDate + trans.Channel + trans.Username + trans.Password;


                    trans.DigitalSignature = GetSchoolTransactionSignature(text);
                    SchoolsApi.ApiResponse resp = api.MakePayment(trans);// ("209013245", "KCS02", "stan_counter", "czGlDWl8Y5xPwHWfwpZnOQ==");
                    //if (resp.ErrorCode.Equals("0") || resp.ErrorCode.Equals("200"))

                    if (resp.ErrorCode == "0" || resp.ErrorCode == "200")
                    {
                        response.StatusCode = "0";
                        response.StatusDescription = "SUCCESS";
                        response.PegPayPostId = resp.ReceiptNumber;
                    }
                    else
                    {
                        //Transaction (Process ID 152) was deadlocked on loc
                        if (resp.ErrorDescription.ToUpper().Contains("TIME") || resp.ErrorDescription.ToUpper().Contains("DEADLOCKED"))
                        {
                            response.StatusCode = "500";
                            response.StatusDescription = resp.ErrorDescription;
                        }
                        else
                        {
                            response.StatusCode = "100";
                            response.StatusDescription = resp.ErrorDescription;

                            // MarkTransactionAsFAILED(tran, resp.ErrorDescription);
                        }
                    }
                }
                else
                {
                    string narraiton = "SCHOOL CODE NOT SUPPLIED";
                    response.StatusCode = "100";
                    response.StatusDescription = narraiton;
                }
            }
            catch (Exception ee)
            {

            }

            return response;

        }

        internal void HandleUtilityResponse(PostResponse utilityResp, Transaction trans)
        {

            //This is how I mapped the response codes
            //0- Success, 100- Failed, 1000-Pending, 
            //500- Exceptional error/Undefined error do nothing e.g timeout,general error etc

            //log the response from the utility first
            LogUtilityResponse(utilityResp, trans);

            //depending on the response lets see how to handle
            DatabaseHandler dh = new DatabaseHandler();
            string StatusCode = utilityResp.StatusCode;
            string StatusDesc = utilityResp.StatusDescription;

            //success
            if (StatusCode == "0")
            {
                //mark transaction as successfull
                MarkTransactionAsSUCCESSFULL(trans, utilityResp.PegPayPostId, "SUCCESS");
            }
            //exception or skippable error(general error) has occurec
            else if (StatusCode == "500" || StatusCode == "1100")
            {
                string reason = utilityResp.StatusDescription;

            }
            //failure
            else
            {
                //mark transaction as failed
                string reason = utilityResp.StatusDescription;
                MarkTransactionAsFAILED(trans, reason);
            }

        }

        public void MarkTransactionAsSUCCESSFULL(Transaction tran, string utilityTranRef, string status)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                Console.WriteLine("Tran :" + tran.VendorTranId + " Successfull: " + utilityTranRef);

                dh.UpdateTransactionStatus(tran.VendorTranId, status, utilityTranRef);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsSUCCESSFULL: " + ee.Message, "KCB_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }
        }

        public void MarkTransactionAsFAILED(Transaction tran, string Reason)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            { //update tran status to indicate failure
                string status = "FAILED";
                Console.WriteLine("Tran :" + tran.VendorTranId + " Failed: " + Reason);
               
                dh.LogError(tran.VendorTranId + " " + Reason, "KCB_VAS", DateTime.Now, tran.UtilityCompany);
                dh.UpdateTransactionStatus(tran.VendorTranId, status, Reason);
            }
            catch (Exception ee)
            {

                dh.LogError("MarkTransactionAsFAILED: " + ee.Message, "KCB_VAS", DateTime.Now, tran.VendorTranId);
                throw ee;
            }
        }

        private void LogUtilityResponse(PostResponse response, Transaction tran)
        {
            DatabaseHandler dh = new DatabaseHandler();
            string OtherData = "UtilityCode = " + tran.UtilityCompany + ", UtilityRef = " + response.PegPayPostId + ", StatusCode = " + response.StatusCode + " StatusDesc = " + response.StatusDescription;
            dh.InsertIntoUtilityResponseLogs(tran.VendorCode, tran.VendorTranId, response.StatusCode, response.StatusDescription, OtherData);
        }

        private bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private string GetSchoolTransactionSignature(string Tosign)
        {
            string signature = "";
            try
            {
                string certificate = @"E:\Certificates\Certs\StanbicCert\StanbicSchools.pfx";
                X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);


                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

                // Hash the data
                SHA1Managed sha1 = new SHA1Managed();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(Tosign);
                 byte[] hash = sha1.ComputeHash(data);

                // Sign the hash
                byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
                string strDigCert = Convert.ToBase64String(digitalCert);
                return strDigCert;
            }
            catch (Exception ee)
            {
                dh.LogErrorKCB("", "", DateTime.Now, "PROCESSING", "", ee.Message, "");
            }
            return signature;
        }


    }
}
