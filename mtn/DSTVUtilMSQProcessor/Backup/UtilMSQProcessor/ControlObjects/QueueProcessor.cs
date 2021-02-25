using System;
using System.Collections.Generic;
using System.Text;
using System.Messaging;
using System.Collections;
using UtilMSQProcessor.EntityObjects;
using UtilMSQProcessor.PegPayLevel1Api;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Data;
using log4net;
using System.Threading;

namespace UtilMSQProcessor.ControlObjects
{
    public class QueueProcessor
    {
        public string mtnDSTVQueue = "";
        public string mtnSTKQueue = "";
        public string smsQueuepath = "";
        private MessageQueue queue;
        private Message msg;
        private DatabaseHandler dh;
        private MessageQueue smsqueue;
        private Message smsmsg;
        // Define a static logger variable so that it references the name of your class 
        private static readonly ILog log = LogManager.GetLogger(typeof(QueueProcessor));
        public int MaxThreads = 5;

        public QueueProcessor()
        {
            dh = new DatabaseHandler();
            mtnDSTVQueue = dh.queue;
            mtnSTKQueue = dh.stkQueue;
            smsQueuepath = dh.SmsQueue;
            if (MessageQueue.Exists(mtnDSTVQueue))
            {
                queue = new MessageQueue(mtnDSTVQueue);
            }
            else
            {
                queue = MessageQueue.Create(mtnDSTVQueue);
            }
        }

        public void ProcessDstvQueue()
        {
            Transaction trans = PickTransactionCopyFromQueue();
            try
            {
                //use PickTransactionFromQueue
                //ThreadPool.SetMaxThreads(MaxThreads, MaxThreads);
                //ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessADstvTransaction), trans);
                ProcessDstvTransaction(trans);

            }
            catch (Exception ex)
            {
                dh.LogError("ProcessDstvQueue: " + ex.Message);
                Console.WriteLine("Exception :" + ex.Message);
            }

        }

        private void ProcessADstvTransaction(Object a)
        {
            Transaction trans = a as Transaction;
            string tranId = trans.VendorTranId;
            try
            {
                
                SaveInPostLogs(trans);
                try
                {
                    //its in deleted
                    //i.e has ever been failed
                    if (dh.ExistsInDeletedTable(trans.VendorTranId, "DSTV", trans.VendorCode))
                    {
                        Console.WriteLine("ECW Retry: Was Already Deleted");
                        if (HasEverBeenCompletedAtMTN(trans))
                        {
                            RemoveFromQueue(trans.TranId);
                        }
                        else
                        {
                            String FailureReason = "ECW Retry: Was Already Deleted";
                            FailTransactionAtMtnAndDelete(trans, FailureReason, false, "DSTV");
                        }
                        dh.LogError("ProcessDstvQueue: " + tranId + " " + "ECW Retry: Was Already Deleted");
                    }
                    //Exists in View of Failed,Recieved,Reconciled with sent to vendor 1
                    else if (dh.ExistsInPegPay(trans.VendorTranId, "DSTV", trans.VendorCode))
                    {
                        Console.WriteLine("ECW Retry: In Recieved Or Failed");
                        //if (HasEverBeenCompletedAtMTN(trans))
                        //{
                        //    dh.UpdateSentToVendor(trans.VendorTranId, 1);
                        RemoveFromQueue(trans.TranId);
                        //Log tran id,custref,amount,recorddate
                        string line = "VendorTranId=" + trans.VendorTranId + " CustRef=" + trans.CustomerRef +
                                    " Amount=" + trans.TranAmount + " RecordDate=" + trans.PaymentDate;
                        LogToFile(line);
                        dh.LogError("ProcessDstvQueue: " + tranId + " " + "ECW Retry: In Recieved Or Failed");
                        //}
                        //else
                        //{
                        //    CompleteAtMtnAndUpdateFlag(trans);
                        //}
                    }
                    //this is a new request
                    //send to utilities API for logging
                    else
                    {

                        PegPayLevel1Api.PegPay pegpay = new UtilMSQProcessor.PegPayLevel1Api.PegPay();

                        if (trans.IsSTKPayment())
                        {
                            SendToSTKQueue(trans);
                            RemoveFromQueue(trans.TranId);
                        }
                        else
                        {
                            ProcessDstvTransaction(trans);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception :" + ex.Message);
                    WriteToErrorFile(ex.Message, tranId);
                    Console.WriteLine(ex.Message);
                    SkipTransactionInQueue(trans);
                    dh.LogError("ProcessDstvQueue: " + ex.Message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                WriteToErrorFile(ex.Message, tranId);
                SkipTransactionInQueue(trans);
                dh.LogError("ProcessDstvQueue: " + ex.Message);
            }
        }

        private void ProcessDstvTransaction2(Object tr)
        {
            Transaction trans = (Transaction)tr;
            ProcessDstvTransaction(trans);
        }

        private void ProcessDstvTransaction(Transaction trans)
        {
            if (trans.IsValidTransaction())
            {

                //send to utilities API for Validation and Logging in DB
                PegPayLevel1Api.Response levelResp = CallLevel1(trans);

                //is success response
                if (levelResp.ResponseField6.Equals("0"))
                {
                    Console.WriteLine("Success on Level 1");
                    CompleteAtMtnAndUpdateFlag(trans);
                }
                else
                {
                    Console.WriteLine("Failed on Level 1:" + levelResp.ResponseField7);
                    dh.LogError(levelResp.ResponseField6+": "+levelResp.ResponseField7);
                    if (IsDuplicateTranId(levelResp))
                    {
                        if (HasEverBeenCompletedAtMTN(trans))
                        {
                            dh.UpdateSentToVendor(trans.VendorTranId, 1);
                            RemoveFromQueue(trans.TranId);
                        }
                        else
                        {
                            CompleteAtMtnAndUpdateFlag(trans);
                        }
                    }
                    //timeout
                    else if (levelResp.ResponseField6.Equals("000"))
                    {
                        //RemoveFromQueue(trans.TranId);
                        SkipTransactionInQueue(trans);
                    }
                    //general error
                    else if (levelResp.ResponseField7.ToUpper().Contains("GENERAL"))
                    {
                        RemoveFromQueue(trans.TranId);
                        //SkipTransactionInQueue(trans);
                    }
                    //pegpay db unavalable
                    else if (levelResp.ResponseField7.ToUpper().Contains("PEGPAYDB"))
                    {
                        RemoveFromQueue(trans.TranId);
                        //SkipTransactionInQueue(trans);
                    }
                    //suspected double posting
                    else if (levelResp.ResponseField7.ToUpper().Contains("SUSPECTED DOUBLE POSTING"))
                    {
                        RemoveFromQueue(trans.TranId);
                        //SkipTransactionInQueue(trans);
                    }
                    //other failure reason
                    else
                    {
                        string failureReason = levelResp.ResponseField7;
                        FailTransactionAtMtnAndDelete(trans, failureReason, false, "DSTV");
                    }
                }
            }
            else
            {
                //put trans in deleted with status description
                //fail it at mtn
                string failureReason = trans.StatusDescription;
                FailTransactionAtMtnAndDelete(trans, failureReason, false, "DSTV");
            }

        }

        private void LogToFile(string line)
        {
            try
            {
                string LogFilePath = @"E:\Logs\DstvLogs\SuspiciousFiles.txt";
                string[] Lines = { };
                if (File.Exists(LogFilePath))
                {
                    Lines = File.ReadAllLines(LogFilePath);
                }
                List<string> contentToWrite = new List<string>();
                contentToWrite.AddRange(Lines);
                contentToWrite.Add(line);
                File.WriteAllLines(LogFilePath, contentToWrite.ToArray());
            }
            catch (Exception e)
            {

            }
        }


        private bool IsRepeatingMTNId(Transaction trans)
        {
            try
            {
                DataTable results = dh.CrossCheckVendorRef(trans.VendorTranId);
                if (results.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal string GetNewBoquetCode(string oldCode)
        {
            DataTable dt = dh.GetBouquetByBouquetCode(oldCode);
            return dt.Rows[0]["BouquetCodeNew"].ToString();
        }

        private Response CallLevel1(Transaction tr)
        {
            Response resp = new Response();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

                PegPay pegpay = new PegPay();
                TransactionRequest trans = new TransactionRequest();
                trans.PostField1 = tr.CustomerRef;
                trans.PostField2 = tr.CustomerName;
                trans.PostField3 = GetNewBoquetCode(tr.Area);//Area
                trans.PostField4 = "DSTV";//UtilityCode
                trans.PostField21 = tr.CustomerType;
                trans.PostField5 = tr.PaymentDate;
                trans.PostField6 = tr.PaymentType;
                trans.PostField7 = tr.TranAmount;
                trans.PostField8 = tr.TranType;
                trans.PostField9 = tr.VendorCode;
                trans.PostField10 = tr.Password;//"";//Password
                trans.PostField11 = tr.CustomerTel;
                trans.PostField12 = tr.Reversal;
                trans.PostField13 = tr.TranIdToReverse;
                trans.PostField14 = tr.Teller;
                trans.PostField15 = tr.Offline;
                trans.PostField17 = "";//chequeNumber
                trans.PostField18 = tr.TranNarration;
                trans.PostField19 = "";//email
                trans.PostField20 = tr.VendorTranId;
                string dataToSign = tr.CustomerRef +
                                    tr.CustomerName +
                                    tr.CustomerTel +
                                    tr.VendorTranId +
                                    tr.VendorCode +
                                    tr.Password +
                                    tr.PaymentDate +
                                    tr.Teller +
                                    tr.TranAmount +
                                    tr.TranNarration +
                                    tr.TranType;
                trans.PostField16 = GetDigitalSignature(dataToSign);// "1234" for MTN
                Console.WriteLine("Level 1 on " + DateTime.Now);

                resp = pegpay.PostTransaction(trans);

                Console.WriteLine("Response from Level 1 on " + DateTime.Now);

                string whatToLog = "Request Object Sent :" + Environment.NewLine + tr.ToString() +
                                   Environment.NewLine + Environment.NewLine;
                whatToLog = whatToLog + "Response Object Recieved :" + resp.ToString() +
                            Environment.NewLine + Environment.NewLine;
                whatToLog = whatToLog + "---------------------------------------";
                log.Info(whatToLog);
                return resp;
            }
            catch (WebException ex1)
            {
                resp.ResponseField6 = "000";
                resp.ResponseField7 = "TIMEOUT";
                WriteToErrorFile(ex1.Message, tr.VendorTranId);
                dh.LogError(tr.VendorTranId + " - " + ex1.Message);
            }
            catch (Exception e)
            {
                resp.ResponseField6 = "2000";
                resp.ResponseField7 = e.Message;
                dh.LogError(tr.VendorTranId+" - "+e.Message);
                WriteToErrorFile(e.Message, tr.VendorTranId);
            }
            return resp;
        }

        private string GetDigitalSignature(string dataToSign)
        {
            return "1234";//for MTN
        }

        private void WriteToErrorFile(string Message, string tranId)
        {
            string whatToLog = "Exception In Dstv Msmq Proc:Tran Id = " + tranId + " Message = " + Message + Environment.NewLine;
            log.Info(whatToLog);

        }

        private bool HasEverBeenCompletedAtMTN(Transaction trans)
        {
            if (dh.CheckIfItWasSuccessfullAtECW(trans))
            {
                return true;
            }
            return false;
        }

        private void RemoveFromQueue(string MsgId)
        {
            try
            {
                MessageQueue msMq = msMq = new MessageQueue(mtnDSTVQueue);

                msMq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });
                // to delete the transaction jst recieve it.
                // Recieve removes top most item and returns it
                Transaction trans = (Transaction)msMq.ReceiveById(MsgId).Body;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }

        private bool IsDuplicateTranId(Response levelResp)
        {
            //status code for duplicate vendor ref is 20
            if (levelResp.ResponseField6.Equals("20"))
            {
                return true;
            }

            return false;
        }

        private bool AmountIsGreaterThanBalance(string TranAmount, string outStandingBal, string customerType)
        {
            try
            {
                if (customerType.Trim().ToUpper().Equals("POSTPAID"))
                {
                    return true;
                }
                else
                {
                    if (string.IsNullOrEmpty(TranAmount))
                    {
                        TranAmount = "0";
                    }
                    if (string.IsNullOrEmpty(outStandingBal))
                    {
                        outStandingBal = "0";
                    }
                    double Amount = Convert.ToDouble(TranAmount);
                    double Bal = Convert.ToDouble(outStandingBal);
                    if (Amount > Bal)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SkipTransactionInQueue(Transaction trans)
        {
            MessageQueue msMq = msMq = new MessageQueue(mtnDSTVQueue);
            Message msg = new Message();
            msg.Body = trans;
            msg.Label = trans.VendorTranId;
            msg.Recoverable = true;
            //send to Bottom
            msMq.Send(msg);
            // to delete the transaction jst recieve it.
            // Recieve removes top most item and returns it
            msMq.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });
            trans = (Transaction)msMq.ReceiveById(trans.TranId).Body;
            return;
        }

        private void SendToSTKQueue(Transaction trans)
        {

            MessageQueue queue;
            if (MessageQueue.Exists(mtnSTKQueue))
            {
                queue = new MessageQueue(mtnSTKQueue);
            }
            else
            {
                queue = MessageQueue.Create(mtnSTKQueue);
            }
            Message msg = new Message();
            msg.Body = trans;
            msg.Label = trans.VendorTranId;
            msg.Recoverable = true;
            queue.Send(msg);
            return;
        }


        private bool IsTimeOut(UtilMSQProcessor.PegPayLevel1Api.Response levelResp)
        {
            if (levelResp.ResponseField6.Equals("000"))
            {
                return true;
            }
            return false;
        }

        private void FailTransactionAtMtnAndDelete(Transaction trans, string failureReason, bool InReceivedTble, string UtilityCode)
        {
            string status = "FAILED";

            //notify MTN using PaymentCompleted Mechanism.
            MTNPaymentCompletedResponse Response = NotifyMTNofStatusAtUtility(trans, status, failureReason);

            //log the MTN Response
            LogTestMTNResponse(Response, trans, status);

            //if MTN has been successfully notified
            if (Response.HasBeenSuccessfullAtMtn())
            {
                Console.WriteLine("Successfull Failed " + trans.VendorTranId + " :" + failureReason + " MSISDN:" + trans.CustomerTel +"Date: "+DateTime.Now.ToString());
                //send SMS
                //SMS sms = new SMS();
                //string Msg = "YAKA Purchase of UGX " + trans.TranAmount +
                //             " failed. Reason:" + failureReason + ". or contact MTN";
                //sms.Mask = "8888";
                //sms.Message = Msg;
                //sms.Phone = trans.CustomerTel;
                //sms.Sender = "MTNService";
                //sms.VendorTranId = trans.VendorTranId;
                //sms.Reference = "";
                //LogSMS(sms);
                Console.WriteLine("Tran Id:" + trans.VendorTranId + " Successfully Failed" + " MSISDN:" + trans.CustomerTel + "Date: " + DateTime.Now.ToString());
                //dh.LogSMS(trans.CustomerTel, trans.VendorTranId, Msg, Mask, Service);
            }
            else
            {
                Console.WriteLine("Failed to Fail " + trans.VendorTranId + " :" + Response.Reason + " MSISDN:" + trans.CustomerTel + "Date: " + DateTime.Now.ToString());
            }
            if (InReceivedTble)
            {
                dh.deleteTransaction2(trans.VendorTranId, Response.Reason);
            }
            else
            {
                dh.InsertDeleteTransation(trans, UtilityCode, failureReason);
            }


            //remove from MSMQ
            RemoveFromQueue(trans.TranId);
        }

        private void CompleteAtMtnAndUpdateFlag(Transaction trans)
        {
            string status = "COMPLETED";

            //notify MTN using PaymentCompleted Mechanism.
            MTNPaymentCompletedResponse Response = NotifyMTNofStatusAtUtility(trans, status, "");

            //if deadlock at MTN
            if (Response.Reason.Contains("RESOURCE_TEMPORARY_LOCKED"))
            {
                Response = NotifyMTNofStatusAtUtility(trans, status, "");
            }

            //if MTN has been successfully notified
            if (Response.HasBeenSuccessfullAtMtn())
            {
                Console.WriteLine(trans.VendorTranId + " Successfully Completed" + " MSISDN:" + trans.CustomerTel + "Date: " + DateTime.Now.ToString());
                bool FlagHasBeenUpdatedOk = false;
                try
                {
                    //set sentToVendor=1
                    //dh.UpdateSentToVendor(trans.VendorTranId, 1);
                    dh.UpdateSentToVendor1(trans.VendorTranId, 1, DateTime.Parse(trans.QueueTime));
                    FlagHasBeenUpdatedOk = true;
                }
                catch (Exception ex1)
                {
                    FlagHasBeenUpdatedOk = false;
                    Console.WriteLine(ex1.Message);
                }
                if (FlagHasBeenUpdatedOk)
                {
                    //remove from MSMQ
                    RemoveFromQueue(trans.TranId);
                }
                else
                {
                    //skip transaction in Queue
                    SkipTransactionInQueue(trans);
                }
                Console.WriteLine("Successfull AT MTN");
            }
            else
            {

                //get Failure Reason
                string Reason = Response.Reason;
                Console.WriteLine("Failed to Complete " + trans.VendorTranId + " :" + Reason + " MSISDN:" + trans.CustomerTel + "Date: " + DateTime.Now.ToString());
                //delete transaction
                dh.deleteTransaction2(trans.VendorTranId, Reason);
                //remove transaction from Queue
                RemoveFromQueue(trans.TranId);
                //send SMS
                //SMS sms = new SMS();
                //string Msg = "YAKA Purchase of UGX " + trans.TranAmount +
                //             " failed. Reason: Transaction not completed. or contact MTN";
                //sms.Mask = "8888";
                //sms.Message = Msg;
                //sms.Phone = trans.CustomerTel;
                //sms.Sender = "MTNService";
                //sms.VendorTranId = trans.VendorTranId;
                //sms.Reference = "";
                //LogSMS(sms);

                Console.WriteLine("Tran Id:" + trans.VendorTranId + " Failed:" + Reason + " MSISDN:" + trans.CustomerTel + "Date: " + DateTime.Now.ToString());
            }
            //log the MTN Response
            LogTestMTNResponse(Response, trans, status);
        }

        private void LogSMS(SMS sms)
        {
            try
            {
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

        private void LogTestMTNResponse(MTNPaymentCompletedResponse Response, Transaction trans, string status)
        {
            dh.InsertIntoVendorResponseLogs(trans.VendorTranId, Response.Reason, Response.xmlResponse, status);
        }

        private void SaveInPostLogs(Transaction trans)
        {
            dh = new DatabaseHandler();
            dh.SavePostLog(trans, "DSTV", "", "");
        }

        public Transaction PickTransactionFromQueue()
        {
            msg = new Message();
            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });
            msg = queue.Receive();
            string msgId = msg.Id;
            Transaction trans = (Transaction)msg.Body;
            trans.TranId = msgId;
            return trans;
        }

        public Transaction PickTransactionCopyFromQueue()
        {
            msg = new Message();
            queue.Formatter = new XmlMessageFormatter(new Type[] { typeof(Transaction) });
            msg = queue.Peek();
            string msgId = msg.Id;
            Transaction trans = (Transaction)msg.Body;
            trans.TranId = msgId;
            return trans;
        }

        private Customer GetCustDetails(Transaction trans, string UtilityCode)
        {
            Customer cust = new Customer();
            try
            {
                DatabaseHandler dh = new DatabaseHandler();
                cust = dh.GetCustDetails(trans.CustomerRef, UtilityCode,trans.Area);
                if (!cust.StatusCode.Equals("0"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    PegPayLevel1Api.PegPay pegpay = new UtilMSQProcessor.PegPayLevel1Api.PegPay();
                    PegPayLevel1Api.Response resp = new UtilMSQProcessor.PegPayLevel1Api.Response();
                    PegPayLevel1Api.QueryRequest queryRequest = new UtilMSQProcessor.PegPayLevel1Api.QueryRequest();
                    queryRequest.QueryField1 = trans.CustomerRef;
                    queryRequest.QueryField4 = UtilityCode;
                    queryRequest.QueryField5 = trans.VendorCode;
                    queryRequest.QueryField6 = trans.Password;
                    resp = pegpay.QueryCustomerDetails(queryRequest);
                    if (resp.ResponseField6.Equals("0"))
                    {
                        cust.CustomerType = resp.ResponseField5;
                        cust.CustomerName = resp.ResponseField2;
                        cust.Balance = resp.ResponseField4;
                        cust.StatusCode = "0";
                        cust.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        cust.CustomerType = "";
                        cust.CustomerName = "";
                        cust.StatusCode = "1000";
                        cust.StatusDescription = "INVALID CUSTOMER REF.";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cust;
        }


        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private MTNPaymentCompletedResponse NotifyMTNofStatusAtUtility1(Transaction trans, string Status, string failureReason)
        {
            MTNPaymentCompletedResponse mtnResp = new MTNPaymentCompletedResponse();
            try
            {
                //string mtnNotifyUrl = "http://172.25.48.36:8323/mom/mt/paymentcompleted";//live url
                string mtnNotifyUrl = dh.GetSystemParameter(8, 3);
                string request = GetMTNPaymentCompletedRequest(trans, Status, failureReason);
                WebRequest r = WebRequest.Create(mtnNotifyUrl);
                r.Method = "POST";
                r.ContentType = "text/xml";
                byte[] byteArray = Encoding.UTF8.GetBytes(request);
                r.ContentLength = byteArray.Length;
                string nonce = "WScqanjCEAC4mQoBE07sAQ==";//should be any random string
                string created = "" + DateTime.Now;//the time we created the string
                string SP_ID = dh.GetSystemParameter(8, 1);//"2560110001302";
                string password = dh.GetSystemParameter(8, 2);///"Huawei2014";
                string MSISDN = trans.CustomerTel;
                string Signature = "43AD232FD45FF";
                string Cookies = "sessionid=default8fcee064690b45faa9f8f6c7e21c5e5a";
                string toBeHashed = nonce + created + password;
                string passwordDigest = GetSHA1Hash(toBeHashed);
                r.Timeout = 190000;
                r.Headers["Authorization"] = "WSSE realm=\"SDP\"," +
                                               "profile=\"UsernameToken\"";
                r.Headers["X-WSSE"] = "UsernameToken Username=\"" + SP_ID + "\"," +
                                               "PasswordDigest=\"" + passwordDigest + "\"," +
                                               "Nonce=\"" + nonce + "\"," +
                                               "Created=\"" + created + "\"";
                r.Headers["X-RequestHeader"] = "request ServiceId=\"\"," +
                                               "TransId=\"\"," +
                                               "LinkId=\"\"," +
                                               "FA=\"\"";
                r.Headers["Signature"] = Signature;
                r.Headers["Cookie"] = Cookies;
                r.Headers["Msisdn"] = MSISDN;
                r.Headers["X-HW-Extension"] = "k1=v1;k2=v2";

                Stream dataStream = r.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                WebResponse response = r.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader rdr = new StreamReader(dataStream);
                string feedback = rdr.ReadToEnd();
                mtnResp = GetMTNResponse(feedback);
                return mtnResp;
            }
            catch (WebException ex)
            {
                if (ex.Message.ToUpper().Contains("TIMED OUT") || ex.Message.ToUpper().Contains("UNABLE TO CONNECT TO THE REMOTE SERVER"))
                {
                    //Do nothing
                    mtnResp.successfullyNotified = "00";
                    mtnResp.isFailureResponse = true;
                    mtnResp.Reason = ex.Message;
                    mtnResp.xmlResponse = "";
                }
                // we reached MTN but there is a problem with the transaction i.e mayb its no longer active etc
                else
                {
                    using (Stream stream = ex.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            string feedback = reader.ReadToEnd();
                            //DataFile df = new DataFile();
                            //ArrayList a = new ArrayList();
                            //a.Add("Vendor Ref:-" + trans.VendorTransactionRef);
                            //df.writeToFile("C://DSTVFailed.txt", a);
                            mtnResp = GetMTNResponse(feedback);
                            return mtnResp;
                        }
                        catch (Exception)
                        {
                            mtnResp.successfullyNotified = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToUpper().Contains("TIMED OUT") || ex.Message.Contains("UNABLE TO CONNECT TO REMOTE SERVER"))
                {
                    //Do nothing
                }
            }
            return mtnResp;

        }

        private MTNPaymentCompletedResponse NotifyMTNofStatusAtUtility(Transaction trans, string Status, string failureReason)
        {
            MTNPaymentCompletedResponse mtnResp = new MTNPaymentCompletedResponse();
            string timeIn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string request = "";
            try
            {
                //string mtnNotifyUrl = "http://172.25.48.36:8323/mom/mt/paymentcompleted";//live url
                
                string mtnNotifyUrl = dh.GetSystemParameter(8, 3);
                request = GetMTNPaymentCompletedRequest(trans, Status, failureReason);
                WebRequest r = WebRequest.Create(mtnNotifyUrl);
                r.Method = "POST";
                r.ContentType = "text/xml";
                byte[] byteArray = Encoding.UTF8.GetBytes(request);
                r.ContentLength = byteArray.Length;
                string nonce = "WScqanjCEAC4mQoBE07sAQ==";//should be any random string
                string created = "" + DateTime.Now;//the time we created the string
                string SP_ID = dh.GetSystemParameter(8, 1);//"2560110001302";//get spid and password for DSTV
                string password = dh.GetSystemParameter(8, 2);///"Huawei2014";
                string MSISDN = trans.CustomerTel;
                string Signature = "43AD232FD45FF";
                string Cookies = "sessionid=default8fcee064690b45faa9f8f6c7e21c5e5a";
                string toBeHashed = nonce + created + password;
                string passwordDigest = GetSHA1Hash(toBeHashed);
                r.Timeout = 190000;
                r.Headers["Authorization"] = "WSSE realm=\"SDP\"," +
                                               "profile=\"UsernameToken\"";
                r.Headers["X-WSSE"] = "UsernameToken Username=\"" + SP_ID + "\"," +
                                               "PasswordDigest=\"" + passwordDigest + "\"," +
                                               "Nonce=\"" + nonce + "\"," +
                                               "Created=\"" + created + "\"";
                r.Headers["X-RequestHeader"] = "request ServiceId=\"\"," +
                                               "TransId=\"\"," +
                                               "LinkId=\"\"," +
                                               "FA=\"\"";
                r.Headers["Signature"] = Signature;
                r.Headers["Cookie"] = Cookies;
                r.Headers["Msisdn"] = MSISDN;
                r.Headers["X-HW-Extension"] = "k1=v1;k2=v2";

                Stream dataStream = r.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                WebResponse response = r.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader rdr = new StreamReader(dataStream);
                string feedback = rdr.ReadToEnd();
                string whatToLog = "PaymentCompletedRequestSent :" +
                                   Environment.NewLine + request +
                                   Environment.NewLine + Environment.NewLine;
                whatToLog = whatToLog + "Vendor Xml Response :" + feedback +
                                   Environment.NewLine + Environment.NewLine;
                whatToLog = whatToLog + "---------------------------------------";
                log.Info(whatToLog);
                string timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
               // dh.LogRequest("DSTV", trans.VendorTranId, request, feedback);

                dh.LogRequest("DSTV", trans.VendorTranId, "[Request Time: " + timeIn + "] " + request, "[Response Out: " + timeOut + "] " + feedback);

                mtnResp = GetMTNResponse(feedback);
                return mtnResp;
            }
            catch (WebException ex)
            {
                dh.LogError(ex.Message);
                if (ex.Message.ToUpper().Contains("TIMED OUT") || ex.Message.ToUpper().Contains("UNABLE TO CONNECT TO THE REMOTE SERVER"))
                {
                    //Do nothing
                    mtnResp.successfullyNotified = "00";
                    mtnResp.isFailureResponse = true;
                    mtnResp.Reason = ex.Message;
                    mtnResp.xmlResponse = "";
                }
                // we reached MTN but there is a problem with the transaction i.e mayb its no longer active etc
                else
                {
                    using (Stream stream = ex.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        try
                        {
                            string feedback = reader.ReadToEnd();
                            //DataFile df = new DataFile();
                            //ArrayList a = new ArrayList();
                            //a.Add("Vendor Ref:-" + trans.VendorTransactionRef);
                            //df.writeToFile("C://DSTVFailed.txt", a);
                            mtnResp = GetMTNResponse(feedback);
                            string timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            dh.LogRequest("DSTV", trans.VendorTranId, "[Request Time: " + timeIn + "] " + request, "[Response Out: " + timeOut + "] " + feedback);
                            return mtnResp;
                        }
                        catch (Exception ee)
                        {
                            mtnResp.successfullyNotified = "0";
                            dh.LogError("GetMTNResponse: " + ee.Message + " = " + ee.InnerException);
                        }
                    }
                }
                dh.LogError("WebException: " + ex.Message + " = " + ex.InnerException);
            }
            catch (Exception ex)
            {
                if (ex.Message.ToUpper().Contains("TIMED OUT") || ex.Message.Contains("UNABLE TO CONNECT TO REMOTE SERVER"))
                {
                    //Do nothing
                }
                dh.LogError("NotifyMTNofStatusAtUtility: " + ex.Message + " = " + ex.InnerException);
            }
            return mtnResp;

        }

        private string GetSHA1Hash(string toBeHashed)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(toBeHashed);
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(bytes);
            string authkey = Convert.ToBase64String(hash);
            return authkey;
        }

        public MTNPaymentCompletedResponse GetMTNResponse(string feedback)
        {
            MTNPaymentCompletedResponse resp = new MTNPaymentCompletedResponse();
            resp.xmlResponse = feedback;
            //PARSE XML
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(feedback);
            XmlNodeList elemList = xmlDoc.GetElementsByTagName("*");
            bool success = GetTranStatus(feedback);


            if (success)
            {
                resp.Reason = "";
                resp.successfullyNotified = "1";
                resp.isFailureResponse = false;
            }
            else
            {
                //if it is an error at DSTV they return the falure reason in the error code
                for (int i = 0; i < elemList.Count; i++)
                {
                    try
                    {
                        resp.Reason = elemList[i].Attributes["errorcode"].Value;
                        resp.successfullyNotified = "1";//true
                        resp.isFailureResponse = true;
                        return resp;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return resp;
        }

        private bool GetTranStatus(string feedback)
        {
            bool Successful;
            try
            {
                XmlDocument XmlRequest = new XmlDocument();
                XmlRequest.LoadXml(feedback);
                XmlNodeList successlist = XmlRequest.GetElementsByTagName("ns2:paymentcompletedresponse");
                XmlNodeList failurelist = XmlRequest.GetElementsByTagName("ns2:errorResponse");
                if (successlist.Count > 0)
                {
                    Successful = true;
                }
                else if (failurelist.Count > 0)
                {
                    Successful = false;
                }
                else
                {
                    Successful = false;
                }
                return Successful;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private string GetMTNPaymentCompletedRequest(Transaction trans, string status, string FailureReason)
        {
            string requestBody = "";
            //    if (status.Equals("FAILED"))
            //    {
            //        requestBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            //"<ns2:paymentcompletedrequest  xmlns:ns2=\"http://www.ericsson.com/em/emm/sp/backend\" >" +
            //"<transactionid>" + trans.VendorTransactionRef + "</transactionid>" +
            //"<providertransactionid>" + trans.TransNo + "</providertransactionid>" +
            //"<status>" + status + "</status>" +
            //"<message>" + "TEST" + "</message>" +
            //"</ns2:paymentcompletedrequest>";
            //    }
            //    else
            //    {
            requestBody = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
    "<ns2:paymentcompletedrequest  xmlns:ns2=\"http://www.ericsson.com/em/emm/sp/backend\" >" +
    "<transactionid>" + trans.VendorTranId + "</transactionid>" +
    "<providertransactionid>" + /*trans.TranId*/"123" + "</providertransactionid>" +
    "<status>" + status + "</status>" +
    "</ns2:paymentcompletedrequest>";
            //}
            return requestBody;
        }

    }
}