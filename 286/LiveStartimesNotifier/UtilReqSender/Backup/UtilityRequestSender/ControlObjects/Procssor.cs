using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;
using System.Net.Security;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using UtilReqSender.EntityObjects;
using UtilReqSender.StartimesConnect;
using System.Messaging;

namespace UtilReqSender.ControlObjects
{
    public class Procssor
    {

        private MessageQueue smsqueue;
        private Message smsmsg;
        public string smsQueuepath = "";
        public void ProcessMTNYaka()
        {
            try
            {
                DatabaseHandler dh = new DatabaseHandler();
                DataTable dt = dh.GetMTNYakaTransactionsToSend();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UtilReqSender.EntityObjects.UtilityTransaction trans = new UtilReqSender.EntityObjects.UtilityTransaction();
                        string utility = dr["UtilityCode"].ToString();
                        int TranId = Int32.Parse(dr["TranId"].ToString());
                        //trans.chequeNumber="";
                        trans.Bouquet = dr["Area"].ToString();
                        trans.VendorTransactionRef = dr["TranId"].ToString();
                        trans.TransactionId = dr["TranId"].ToString();
                        trans.UtilityRef = dr["UtilityTranRef"].ToString();
                        trans.Narration = dr["TranNarration"].ToString();
                        trans.CustName = dr["CustomerName"].ToString();
                        trans.CustRef = dr["CustomerRef"].ToString();
                        trans.CustomerTel = dr["CustomerTel"].ToString();
                        trans.CustomerType = dr["CustomerType"].ToString();
                        trans.VendorTransactionRef = dr["VendorTranId"].ToString();
                        trans.TransactionType = dr["TranType"].ToString();
                        trans.VendorCode = dr["VendorCode"].ToString();
                        trans.Password = "";
                        trans.Teller = dr["Teller"].ToString();
                        trans.Reversal = dr["Reversal"].ToString().ToUpper();
                        trans.PaymentDate = dr["PaymentDate"].ToString();
                        trans.PaymentType = dr["PaymentType"].ToString();
                        trans.TransactionAmount = dr["TranAmount"].ToString();
                        trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
                        trans.Status = dr["Status"].ToString();
                        trans.SentToVendor = GetInteger(dr["SentToVendor"].ToString().Trim());
                        trans.PaymentType = dr["Tin"].ToString(); //RECHARGE//PAYMENT
                        if (trans.Reversal.Equals("FALSE"))
                        {
                            trans.Reversal = "0";
                        }
                        else
                        {
                            trans.Reversal = "1";
                        }

                        SendToStartTimes(trans);

                        Console.WriteLine("Startimes Good");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int GetInteger(string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void ProcessFailedMTNYakaTransactions()
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                //System.Threading.Thread.Sleep(30000);
                dh.ProcessFailedTransactions();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                dh.LogError(ex.Message, "", DateTime.Now, "");
            }
        }

        private PostResponse SendToStartTimes(UtilReqSender.EntityObjects.UtilityTransaction trans)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            string vendorCode = trans.VendorCode;
            string custref = trans.CustRef;
            string Msg = "";
            try
            {
                Credentials creds = dp.GetUtilityCreds("STARTIMES", trans.VendorCode);
                if (!creds.UtilityCode.Equals(""))
                {                 
                    int tranId = 0;
                    if (!Int32.TryParse(trans.TransactionId, out tranId))
                    {
                        resp.StatusCode = "100";
                        resp.StatusDescription = "INVALID PEGPAY TRANSACTION ID " + trans.TransactionId + ", IT MUST BE NUMBERIC";
                        return resp;
                    }

                    

                    if (trans.PaymentType == "PAYMENT")
                    {
                        MakePayment(tranId ,trans, creds);
                    }
                    else
                    {
                        RechargeAndChangeBouquet(tranId, trans, creds);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "29";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            catch (Exception ex)
            {
                dp.LogError(ex.Message + "PostSTARTIMESPayments SERVICE POSTING - " + custref, vendorCode, DateTime.Now, "STARTIMES");
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }
            return resp;
        }

        public void RechargeAndChangeBouquet(int tranId, UtilReqSender.EntityObjects.UtilityTransaction trans, Credentials creds)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            StartimesConnect.StarTimesTransaction startTrans = GetStartimesTransaction(trans, creds);
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                StartimesConnect.StarTimesConnect connector = new StartimesConnect.StarTimesConnect();
                StartimesConnect.ThirdPartnerRes postResp = connector.ProcessRechargePayment(startTrans);
                if (postResp.returnCode.Equals("0"))
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Successful at Umeme");
                    dp.UpdateSentTransactionById(tranId, postResp.transactionlNo, "SUCCESS");
                    resp.StatusCode = "0";
                    resp.StatusDescription = postResp.returnMsg;
                    dp.LogError(postResp.returnMsg + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");

                }
                else if (postResp.returnMsg.Trim().ToUpper().Contains("DUPLICATE"))
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Successful at Umeme");

                    dp.UpdateSentTransactionById(tranId, postResp.transactionlNo, "SUCCESS");

                    dp.LogError(postResp.returnMsg + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                }

                else
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Failed at Umeme:" + postResp.returnMsg);
                    dp.TransferFailedTransaction(tranId, postResp.returnMsg + " AT STARTIMES");
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = postResp.returnMsg + " AT STARTIMES";
                    dp.LogError(postResp.returnMsg + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");

                }
            }
            catch (Exception ex)
            {
                //dp.TransferFailedTransaction(tranId, ex.Message + " AT STARTIMES");
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "Exception:" + ex.Message;
                dp.LogError(ex.Message + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");
            }
        }


        public void MakePayment(int tranId, UtilReqSender.EntityObjects.UtilityTransaction trans, Credentials creds)
        {
            PostResponse resp = new PostResponse();
            DatabaseHandler dp = new DatabaseHandler();
            StartimesConnect.StarTimesTransaction startTrans = GetStartimesTransaction(trans, creds);
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                StartimesConnect.StarTimesConnect connector = new StartimesConnect.StarTimesConnect();
                StartimesConnect.PaymentResponse postResp = connector.ProcessStarTimesPayment(startTrans);
                if (postResp.StatusCode.Equals("0"))
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Successful at Umeme");
                    dp.UpdateSentTransactionById(tranId, postResp.TransactionId, "SUCCESS");
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";

                }
                else if (postResp.StatusDescription.Trim().ToUpper().Contains("DUPLICATE"))
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Successful at Umeme");

                    dp.UpdateSentTransactionById(tranId, postResp.TransactionId, "SUCCESS");

                    dp.LogError(postResp.StatusDescription + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                }

                else
                {
                    Console.WriteLine(trans.VendorTransactionRef + ": Failed at Umeme:" + postResp.StatusDescription);
                    dp.TransferFailedTransaction(tranId, postResp.StatusDescription + " AT STARTIMES");
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = postResp.StatusDescription + " AT STARTIMES";
                    dp.LogError(postResp.StatusDescription + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");

                }
            }
            catch (Exception ex)
            {    
                 //dp.TransferFailedTransaction(tranId, ex.Message + " AT STARTIMES");
                 resp.PegPayPostId = "";
                 resp.StatusCode = "100";
                 resp.StatusDescription = "Exception:" + ex.Message;
                 dp.LogError(ex.Message + " - " + trans.CustRef, trans.VendorCode, DateTime.Now, "STARTIMES");
            }
        }

        private void LogSMS(SMS sms)
        {
            try
            {
                DatabaseHandler dh = new DatabaseHandler();
                smsQueuepath = dh.SmsQueuePath;
                if (MessageQueue.Exists(smsQueuepath))
                {
                    smsqueue = new MessageQueue(smsQueuepath);
                }
                else
                {
                    smsqueue = MessageQueue.Create(smsQueuepath);
                }
                smsmsg = new Message(sms);
                smsmsg.Label = sms.VendorTranId;
                smsmsg.Recoverable = true;
                smsqueue.Send(smsmsg);
            }
            catch (Exception ex)
            {
                //donothing
            }
        }

        private StartimesConnect.StarTimesTransaction GetStartimesTransaction(UtilityTransaction trans, Credentials creds)
        {
            StartimesConnect.StarTimesTransaction starTrans = new StartimesConnect.StarTimesTransaction();

            starTrans.VendorCode = creds.UtilityCode;
            starTrans.Password = creds.UtilityPassword;//"e10adc3949ba59abbe56e057f20f883e";
            starTrans.CustRef = trans.CustRef;// "02036708069";
            starTrans.CustomerTel = trans.CustomerTel;// "256787114014";
            starTrans.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            starTrans.CustName = trans.CustName;
            starTrans.VendorTransactionRef = trans.VendorTransactionRef;//DateTime.Now.ToString("yyyyMMddmmss");
            starTrans.TransactionAmount = trans.TransactionAmount;// "100000";
            starTrans.Bouquet = trans.PaymentType.Equals("PAYMENT") ? trans.Bouquet : trans.CustomerType;
            starTrans.FeeSpecified1 = "1";
            starTrans.UtilityCode = trans.CustomerType;
            starTrans.Narration = trans.PaymentType;
            
            //string dataToSign = umemeTrans.CustomerRef + umemeTrans.CustomerName + umemeTrans.CustomerTel + umemeTrans.CustomerType + umemeTrans.VendorTranId + umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.PaymentType + umemeTrans.Teller + umemeTrans.TranAmount + umemeTrans.TranNarration + umemeTrans.TranType;
            //starTrans.DigitalSignature = GetDigitalSignature(dataToSign, trans.VendorCode);
            return starTrans;
        }

        private string GetDigitalSignature(string text, string vendorCode)
        {
            // retrieve private key||@"C:\PegPayCertificates1\Orange\41.202.229.3.cer"
            string certificate = "";
            if (vendorCode.ToUpper().Equals("EZEEMONEY"))
            {
                certificate = @"E:\\Certificates\\" + vendorCode + "Certs\\" + vendorCode + ".pfx";
            }
            else
            {
                certificate = @"E:\\Certificates\\" + vendorCode + "\\" + vendorCode + ".pfx";
            }

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
        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
