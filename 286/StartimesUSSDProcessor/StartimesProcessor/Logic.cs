using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net;
using System.Net.Security;
using System.Threading;
using StartimesProcessor.momo;
using System.Data;
using System.Text.RegularExpressions;

namespace StartimesProcessor
{
    public class Logic
    {
        DataLogic dh = new DataLogic();

        public void ProcessTransactions()
        {
            try
            {
                Request[] requests = dh.GetTransactionRequests();
                Console.WriteLine("Transactions found: " + requests.Length.ToString());
                foreach (Request request in requests)
                {
                    dh.UpdateTransactionStatus(request.TranId, "", "", "PROCESSING", "");

                    Thread worker = new Thread(new ParameterizedThreadStart(ProcessRequest));
                    worker.Start(request);
                    //ThreadPool.QueueUserWorkItem(ProcessRequest, request);
                    //ProcessRequest(request);
                }

            }
            catch (Exception ex)
            { 
                
            }
        }
        public void ProcessTransactions2()
        {
            try
            {
                Request[] requests = dh.GetTransactionRequests2();
                Console.WriteLine("Transactions found: " + requests.Length.ToString());
                foreach (Request request in requests)
                {
                    dh.UpdateTransactionStatus(request.TranId, "", "", "PROCESSING", "");

                    Thread worker = new Thread(new ParameterizedThreadStart(ProcessRequest));
                    worker.Start(request);
                    //ThreadPool.QueueUserWorkItem(ProcessRequest, request);
                     //ProcessRequest(request);
                }

            }
            catch (Exception ex)
            {

            }
        }
        public void ProcessTransactions3()
        {
            try
            {
                Request[] requests = dh.GetTransactionRequests3();
                Console.WriteLine("Transactions found: " + requests.Length.ToString());
                foreach (Request request in requests)
                {
                    dh.UpdateTransactionStatus(request.TranId, "", "", "PROCESSING", "");

                    Thread worker = new Thread(new ParameterizedThreadStart(ProcessRequest));
                    worker.Start(request);
                    //ThreadPool.QueueUserWorkItem(ProcessRequest, request);
                    ProcessRequest(request);
                }

            }
            catch (Exception ex)
            {

            }
        }
        public void ProcessPayments()
        {
            try
            {
                Request[] requests = dh.GetPayments();
                Console.WriteLine("Payments found: " + requests.Length.ToString());
                foreach (Request request in requests)
                {
                    //ThreadPool.QueueUserWorkItem(NotifyPayment, request);
                    NotifyPayment(request);
                }

            }
            catch (Exception ex)
            {
                
            }
        }

        private void NotifyPayment(Object requestObj)
        {
            Request request = (Request)requestObj;
            try
            {
                SendToPegPay(request);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ProcessRequest(Object requestObj)
        {
            Request request = (Request)requestObj;
            try
            {
                //delay sending of request till ussd session is closed
                //Thread.Sleep(new TimeSpan(0, 0, 7));//moved to procedure
                ApiResponse resp = MakePayment(request);
                dh.UpdateTransactionStatus(request.TranId, resp.PaymentRef, resp.StatusDesc, resp.State, "PAYMENT");
            }
            catch (Exception ex)
            {
                dh.UpdateTransactionStatus(request.TranId, "", ex.Message, "FAILED", "PAYMENT");
            }
        }

        private ApiResponse MakePayment(Request request)
        {
            ApiResponse result = new ApiResponse();
            if (request.Narration.Contains("CHANGE_BOUQUET") && request.Amount.Equals("0"))
            {
                
            }
            else if (request.TranType.Equals("BANK"))
            {
                result = PullFromBank(request);
            }
            else
            {
                result = ProcessMOMOPayIn(request);
            }
            return result;
        }

        private ApiResponse PullFromBank(Request request)
        {
            throw new Exception("This operation is not supported.");
        }

        private ApiResponse ProcessMOMOPayIn(Request request)
        {
            ApiResponse Resp = new ApiResponse();
            DataLogic dh = new DataLogic();
            Transaction momoTrans = GetMomoTrans(request);
            try
            {

                Response resp = new Response();
                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPayTelecomsApi pegpay = new PegPayTelecomsApi();
                pegpay.Url = "https://192.168.16.35:8002/LivePegPayTelecomsApi/";
                pegpay.Timeout = 480000;
                resp = pegpay.PostTransaction(momoTrans);
                if (resp.StatusCode.Equals(Globals.SUCCESS_STATUS_CODE))
                {
                    Resp.StatusCode = Globals.SUCCESS_STATUS_CODE;
                    Resp.StatusDesc = Globals.SUCCESS_TEXT;
                    Resp.PegPayId = resp.PegPayId;
                    Resp.PaymentRef = resp.TelecomId;
                    Resp.State = "SUCCESS";
                }
                else if ((resp.StatusDescription.Equals("PENDING") || resp.StatusDescription.Contains("DUPLICATE") || resp.StatusCode.Equals("122")))
                {
                    Resp = PollForFinalTransactionStatus(momoTrans, DateTime.Now, 60 * 8);
                }
                else
                {
                    Resp.StatusCode = resp.StatusCode;
                    Resp.StatusDesc = resp.StatusDescription;
                    Resp.PegPayId = "";
                    Resp.PaymentRef = "";
                    Resp.State = Resp.StatusDesc.Equals("PENDING") ? "PENDING" : "FAILED";
                }
            }
            catch (WebException ex1)
            {
                TranDetailResponse resp = new TranDetailResponse();
                PegPayTelecomsApi pegpay = new PegPayTelecomsApi();
                resp = pegpay.GetTransactionDetails(momoTrans.VendorCode, momoTrans.Password, momoTrans.VendorTranId);
                if (resp.StatusCode.Equals(Globals.SUCCESS_STATUS_CODE))
                {
                    Resp.StatusCode = Globals.SUCCESS_STATUS_CODE;
                    Resp.StatusDesc = Globals.SUCCESS_TEXT;
                    Resp.PegPayId = resp.PegpayId;
                    Resp.StatusDesc = resp.TelecomID;
                    Resp.State = Resp.StatusDesc;
                }
                else
                {
                    Resp.StatusCode = resp.StatusCode;
                    Resp.StatusDesc = resp.StatusDescription;
                    Resp.PegPayId = "";
                    Resp.StatusDesc = "";
                    Resp.State = Resp.StatusDesc.Equals("PENDING") ? "PENDING" : "FAILED";
                }
            }
            catch (Exception e)
            {
                Resp.StatusCode = "2000";
                Resp.StatusDesc = e.Message;
                Resp.PegPayId = "";
                Resp.StatusDesc = "";
                Resp.State = "PENDING";
            }
            return Resp;
        }


        private ApiResponse PullFromMobileMoney(Request request)
        {
            ApiResponse result = new ApiResponse();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                momo.PegPayTelecomsApi pegpay = new momo.PegPayTelecomsApi();
                momo.Transaction momoTrans = new momo.Transaction();
                momo.Response resp = new StartimesProcessor.momo.Response();
                momoTrans = GetMomoTrans(request);
                momo.TranDetailResponse status = GetTransactionStatus(momoTrans);
                if (status.StatusCode.Equals("16") || status.StatusCode.Equals("122"))
                {
                    resp = pegpay.PostTransaction(momoTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        result.StatusCode = resp.StatusCode;
                        result.StatusDesc = resp.StatusDescription;
                        result.PaymentRef = request.PostId = resp.TelecomId;
                        result.PegPayId = resp.PegPayId;
                        result.State = "SUCCESS";
                    }
                    else if ((resp.StatusDescription.Equals("PENDING") || resp.StatusCode.Equals("122")))
                    {
                        result = PollForFinalTransactionStatus(momoTrans, DateTime.Now, 60 * 8);
                    }
                    else
                    {
                        result.StatusCode = resp.StatusCode;
                        result.StatusDesc = resp.StatusDescription;
                        result.PaymentRef = request.PostId = resp.TelecomId;
                        result.PegPayId = resp.PegPayId;
                        result.State = status.StatusCode.Equals("22") ? "PENDING" : "FAILED";
                    }

                }
                else
                {
                    result.StatusCode = status.StatusCode;
                    result.StatusDesc = status.StatusDescription;
                    result.PaymentRef = request.PostId = status.TelecomID;
                    result.PegPayId = status.PegpayId;
                    result.State = status.StatusCode.Equals("0") ? "SUCCESS" : "FAILED";
                }
                return result;
            }
            catch (WebException ex)
            {
                momo.Transaction momoTrans = new momo.Transaction();
                momoTrans = GetMomoTrans(request);
                result = PollForFinalTransactionStatus(momoTrans, DateTime.Now, 60 * 8);
            }
            catch (Exception e)
            {
                momo.Transaction momoTrans = new momo.Transaction();
                momoTrans = GetMomoTrans(request);
                momo.TranDetailResponse status = GetTransactionStatus(momoTrans);
                if (status.Equals("16"))
                {
                    result.StatusCode = "000";
                    result.StatusDesc = "TIMEOUT";
                    result.State = "PENDING";
                }
                else
                {
                    result.StatusCode = status.StatusCode;
                    result.StatusDesc = status.StatusDescription;
                    result.PaymentRef = status.TelecomID;
                    result.PegPayId = status.PegpayId;
                    result.State = status.StatusCode.Equals("0") ? "SUCCESS" : "FAILED";
                }
            }
            return result;
        }

        private ApiResponse PollForFinalTransactionStatus(momo.Transaction tran, DateTime startTime, double maxSecondsTimeOut)
        {
            ApiResponse resp = new ApiResponse();

            try
            {
                //poll mtn for final transaction status
                momo.TranDetailResponse pollResp = GetTransactionStatus(tran);
                //dh.LogError(request, pollResp.StatusDescription, pollResp.StatusCode);

                //check for timeout
                DateTime maxWaitThreshold = startTime.AddSeconds(maxSecondsTimeOut);
                //its a timeout
                if (DateTime.Now > maxWaitThreshold)
                {
                    //resp.RequestId = tran.VendorTranId;
                    resp.PegPayId = pollResp.PegpayId;
                    resp.PaymentRef = pollResp.TelecomID;
                    resp.StatusCode = pollResp.StatusCode;
                    resp.StatusDesc = pollResp.StatusDescription;
                    resp.State = resp.StatusCode.Equals("0") ? "SUCCESS" : "FAILED";
                    return resp;
                }

                //its pending or we reorded a connectivity error
                if ((pollResp.StatusDescription == "PENDING") || (pollResp.StatusCode == Globals.RETRY_STATUS_CODE))
                {
                    Thread.Sleep(new TimeSpan(0, 0, Globals.POLL_SLEEP_TIME_IN_SECONDS));
                    return PollForFinalTransactionStatus(tran, startTime, maxSecondsTimeOut);
                }

                //it has either failed or succeeded
                else
                {
                    //resp.RequestId = request.PostId;
                    resp.PegPayId = pollResp.PegpayId;
                    resp.PaymentRef = pollResp.TelecomID;
                    resp.StatusCode = pollResp.StatusCode;
                    resp.StatusDesc = pollResp.StatusDescription;
                    resp.State = resp.StatusCode.Equals("0") ? "SUCCESS" : "FAILED";
                    return resp;
                }

            }
            catch (Exception ex)
            {
                //exception on polling, we return as is
                //dh.LogError(tran, "EXCEPTION: " + ex.Message, "100");
                Thread.Sleep(new TimeSpan(0, 0, Globals.POLL_SLEEP_TIME_IN_SECONDS));
                return PollForFinalTransactionStatus(tran, startTime, maxSecondsTimeOut);
            }
        }


        private StartimesProcessor.momo.Transaction GetMomoTrans(Request tran)
        {
            momo.Transaction trans = new momo.Transaction();
            trans.CustomerName = tran.CustName;
            trans.CustomerRef = tran.CustRef;
            trans.FromAccount = tran.AccountFrom;
            trans.ToAccount = tran.AccountFrom;
            trans.TranAmount = Convert.ToDouble(tran.Amount).ToString();//
            trans.TranCharge = "0";
            trans.VendorCode = "STARTIMES";
            trans.Password = "31J51FP483";
            trans.FromTelecom = tran.PayType;
            trans.ToTelecom = tran.PayType;
            trans.PaymentDate = DateTime.Now.ToString("MM/dd/yyyy");
            trans.Telecom = tran.PayType;
            trans.PaymentCode = "1";
            trans.VendorTranId = tran.SessionId;
            trans.TranType = "PULL";
            string dataToSign = trans.CustomerRef + trans.CustomerName + trans.FromTelecom + trans.ToTelecom + trans.VendorTranId + trans.VendorCode + trans.Password + trans.PaymentDate + trans.TranType + trans.PaymentCode + trans.TranAmount + trans.FromAccount + trans.ToAccount;
            trans.DigitalSignature = GetAirtelMoneySignature(dataToSign);
            return trans;
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


        public string GetAirtelMoneySignature(string Tosign)
        {
            //string certificate = @"E:\Certificates\Pegpay-AirtelMoney.pfx";
            ////X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
            //X509Certificate2 cert = new X509Certificate2(System.IO.File.ReadAllBytes(certificate)
            //                          , "Tingate710"
            //                          , X509KeyStorageFlags.MachineKeySet |
            //                            X509KeyStorageFlags.PersistKeySet |
            //                            X509KeyStorageFlags.Exportable);
            //RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

            string serial = "‎49 9e 39 6f 0b c5 93 92 49 2e 35 4f 19 64 1d 91";
            RSACryptoServiceProvider rsa = null;
            X509Certificate2 cert = GetCertificate(serial);//new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.MachineKeySet);
            rsa = (RSACryptoServiceProvider)cert.PrivateKey;

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

        private momo.TranDetailResponse GetTransactionStatus(momo.Transaction momoTrans)
        {
            ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            momo.PegPayTelecomsApi pegpay = new momo.PegPayTelecomsApi();
            pegpay.Url = "https://192.168.16.35:8002/LivePegPayTelecomsApi/";
            momo.TranDetailResponse tranresp = new momo.TranDetailResponse();
            //first try to get status
            tranresp = pegpay.GetTransactionDetails(momoTrans.VendorCode, momoTrans.Password, momoTrans.VendorTranId);
            return tranresp;
        }

        private void SendToPegPay(Request request)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
            DataLogic dl = new DataLogic();
            generic.PegPay api = new generic.PegPay();
            generic.TransactionRequest trans = new generic.TransactionRequest();

            string VendorCode = "PEGASUS";
            DataTable dt = dl.GetVendorCredentials(VendorCode);
            string Password = "";
            if( dt.Rows.Count >0)
            {
                DataRow row = dt.Rows[0];
                Password = DecryptString(row["Password"].ToString());

            }
            //string Password = "20G06AA534";

            string Custref = request.CustRef;
            string customerName = request.CustName;
            string customerType = request.Area;
            string paymentDate = DateTime.Now.ToString("dd/MM/yyyy");
            string paymentType = "2";
            string tranAmount = Convert.ToDouble(request.Amount).ToString();
            string tranType = "CASH";
            string customerTel = request.CustTel;
            string reversal = "0";
            string tranIdToReverse = "";
            string teller = request.PayType;
            string offline = "0";
            string chequeNumber = "";
            string narration = "Startimes 286 payment";
            string email = "techsupport@pegasustechnologies.co.ug";
            string vendorTransRef = request.PostId;
            string area = request.Bouquet;
            string utilityCode = "STARTIMES";


            //string VendorCode = "MTN";
            //string Password = "83Y84KW560";


            string dataToSign = Custref +
                                customerName +
                                customerTel +
                                vendorTransRef +
                                VendorCode +
                                Password +
                                paymentDate +
                                teller +
                                tranAmount +
                                narration +
                                tranType;


            trans.PostField1 = Custref;
            trans.PostField2 = customerName;
            trans.PostField3 = area;
            trans.PostField4 = utilityCode;
            trans.PostField21 = customerType;
            trans.PostField5 = paymentDate;
            trans.PostField6 = paymentType;
            trans.PostField7 = tranAmount;
            trans.PostField8 = tranType;
            trans.PostField9 = VendorCode;
            trans.PostField10 = Password;
            trans.PostField11 = customerTel;
            trans.PostField12 = reversal;
            trans.PostField13 = tranIdToReverse;
            trans.PostField14 = teller;
            trans.PostField15 = offline;
            trans.PostField16 = SignData(dataToSign);
            trans.PostField17 = chequeNumber;
            trans.PostField18 = narration;
            trans.PostField19 = email;
            trans.PostField20 = vendorTransRef;
            trans.PostField22 = request.Narration.Contains("CHANGE_BOUQUET") ? "RECHARGE" : "PAYMENT";
            
            generic.Response resp = api.PostTransaction(trans);

            if (resp.ResponseField6 == "1000" || resp.ResponseField6 == "0")
            {
                dh.UpdateTransactionStatus(request.TranId, resp.ResponseField8, resp.ResponseField9, resp.ResponseField7, "PEGPAYID");
            }
            else
            {
                if (resp.ResponseField7.Contains("DUPLICATE"))
                {
                    //System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    generic.QueryRequest req = new generic.QueryRequest();
                    generic.Response tranResp = new generic.Response();
                    req.QueryField10 = request.PostId;
                    req.QueryField5 = VendorCode;
                    req.QueryField6 = Password;

                    tranResp = api.GetTransactionDetails(req);

                    dh.UpdateTransactionStatus(request.TranId, tranResp.ResponseField8, tranResp.ResponseField9, tranResp.ResponseField7, "PEGPAYID");

                    if (resp.ResponseField6 == "1000")
                    {
                        NotifyCustomer(request, resp);
                    }
                }
            }
        }

        private void NotifyCustomer(Request request, StartimesProcessor.generic.Response resp)
        {
            DbApi.DataAccess access = new DbApi.DataAccess();
            DbApi.SMS sms = new DbApi.SMS();
            sms.Phone = request.CustTel;
            sms.Message = "Hello, your payment of " + Convert.ToDouble(request.Amount).ToString() + " has been forwarded for processing.";
            sms.Sender = "STARTIMES";
            DbApi.Result res = access.PostSMS(sms);
            if (res.StatusCode.Equals("0"))
            {
                Console.WriteLine("Sms sent to " + sms.Phone);
            }
            else
            {
                Console.WriteLine("Sms not sent to " + sms.Phone);
            }
        }

        private string SignData(string dataToSign)
        {
            //string certificate = @"C:\AirtelMoneyCerts\PegPay-AirtelMoney.pfx";
            //X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);//Tingate710
            //RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

            string serial = "‎49 9e 39 6f 0b c5 93 92 49 2e 35 4f 19 64 1d 91";
            RSACryptoServiceProvider rsa = null;
            X509Certificate2 cert = GetCertificate(serial);//new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.MachineKeySet);
            rsa = (RSACryptoServiceProvider)cert.PrivateKey;

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
        public string DecryptString(string Encrypted)
        {
            string ret = "";
            ret = Encryption.encrypt.DecryptString(Encrypted, "Umeme2501PegPay");
            return ret;
        }

        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Reset()
        {
            try
            {
                dh.ExecuteNonQuery("ResetStartimesTransactions");
            }
            catch (Exception ex)
            {

            }
        }
    }
}
