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
//usUmemeApiing UtilReqSender.;
using System.Messaging;
using System.Web.Services.Protocols;
using UtilReqSender.DstvApi;
using log4net;
using System.Threading;

namespace UtilReqSender.ControlObjects
{
    public class Procssor
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Procssor));
        private MessageQueue smsqueue;
        private Message smsmsg;
        public string smsQueuepath = "";
        public DatabaseHandler dh = new DatabaseHandler();

        public void ProcessMtnDstv()
        {
            try
            {
                //get pending transactions
                DataTable dt = dh.GetMtnDstvTransactionsToSend_1();
                Console.WriteLine("Total Pending {0}", dt.Rows.Count);

                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        //get trans object from data row
                        UtilReqSender.EntityObjects.Transaction trans = GetTransObject(row);
                        Console.WriteLine("Sending to Dstv");

                        DataTable table = dh.GetBouquetByBouquetCode(trans.Area);
                        trans.InvoicePeriod = 1;
                        if (table.Rows.Count > 0)
                        {
                            string invoiceP = table.Rows[0]["Period"].ToString();
                            int period = 0;
                            if (Int32.TryParse(invoiceP, out period))
                            {
                                trans.InvoicePeriod = period;
                            }
                        }

                        //change bouquet Code for Box Office Payment to BO
                        if (trans.Area == "BOX")
                        {
                            trans.Area = "BO";
                        }
                        else if (trans.Area.ToUpper() == "GOLITE" || trans.Area.ToUpper() == "GOLTANL" || trans.Area.ToUpper() == "GOHAN")
                        {
                            trans.Area = "GOLITE";
                        }
                        
                        //following mail from Ankit
                        //He confirmed that the bouquetCode For Compact is COMPW4(call GetAvailableProducts on API) not COMPW7
                        //change bouquet Code for Compact
                        //else if (trans.Area == "COMPW7")
                        //{

                        //    trans.Area = "COMPW4";
                        //}
                        Send2Dstv(trans);
                        //CreateWorkerThread(trans);

                    }
                    //exception when processing 1 transaction
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception in For Loop: " + ex.Message);
                        dh.LogError(ex.Message, "MTN", DateTime.Now, "DSTV");
                    }
                }
            }
            //General exception  
            catch (Exception ex)
            {
                Console.WriteLine("Exception Outside For Loop: " + ex.Message);
                dh.LogError(ex.Message, "MTN", DateTime.Now, "DSTV");
            }
        }

        private void CreateWorkerThread(UtilReqSender.EntityObjects.Transaction trans)
        {
            try
            {
                DatabaseHandler dh = new DatabaseHandler();

                //uncomment after
                //Send2Dstv(trans);


                Thread workerThread = new Thread(new ParameterizedThreadStart(Send2Dstv));
                ////Thread workerThread = new Thread((Send2Dstv));
                workerThread.Start(trans);

                dh.UpdateSentTransactionById3(trans.TranId, "", "PENDING");
                Console.WriteLine("Thread created");
            }
            catch (Exception e)
            {
                dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
            }
        }


        private void Send2Dstv(Object obj)
        {
            try
            {
                UtilReqSender.EntityObjects.Transaction trans = (UtilReqSender.EntityObjects.Transaction)obj;
                SendToMultichoice(trans);
            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "MTN", DateTime.Now, "DSTV");
            }
        }

        private UtilReqSender.EntityObjects.Transaction GetTransObject(DataRow dr)
        {
            UtilReqSender.EntityObjects.Transaction trans = new UtilReqSender.EntityObjects.Transaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());


            trans.Area = dr["Area"].ToString();


            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
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
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = Convert.ToInt32(dr["SentToVendor"].ToString().Trim());
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }


        public void UpdateCustomerDetails(UtilReqSender.EntityObjects.Transaction trans)
        {

            UtilReqSender.EntityObjects.Customer cust = dh.GetCustomerDetails(trans.CustRef, "DSTV");
            UpdateCustomerBalance(cust, trans);
            Console.WriteLine("FINISHED UPADTING BALANCE");

        }

        private void UpdateCustomerBalance(UtilReqSender.EntityObjects.Customer cust, UtilReqSender.EntityObjects.Transaction trans)
        {
            if (cust.StatusCode.Equals("0"))
            {
                UtilReqSender.EntityObjects.Customer dstvCustomer = QueryDstvForCustomerDetails(cust.CustomerRef);
                cust.Balance = dstvCustomer.Balance;
                cust.CustomerName = dstvCustomer.CustomerName;
                cust.Save();
                dh.UpdateCustomerName(cust.CustomerName, trans.TranId);
            }
        }


        public void ProcessReactivateRequest()
        {
            try
            {
                DataTable allRequests = dh.GetAllPendingReactivateRequests();
                foreach (DataRow row in allRequests.Rows)
                {
                    try
                    {
                        string smartCardNumber = row["SmartCardNumber"].ToString();
                        try
                        {
                            ReauthorizeDstvSmartCard(smartCardNumber);
                        }
                        catch (Exception e)
                        {
                            ReauthorizeGotvSmartCard(smartCardNumber);
                        }
                    }
                    //General Exception
                    catch (Exception e)
                    {
                        // do nothing
                    }
                }
            }
            //General Exception
            catch (Exception e)
            {
                //do nothing
            }
        }

        private void ReauthorizeDstvSmartCard(string smartCardNumber)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            GetBalanceByDeviceNumberResponse resp = new GetBalanceByDeviceNumberResponse();
            GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
            string datasource = dh.GetSystemSetting(9, 3);
            string newSmartCardNumber = smartCardNumber;
            DigitalReauthReason Reason = new DigitalReauthReason();
            Reason.Reason = DigitalReauthReasons.E16;
            string password = "PegasusDstv";
            string language = dh.GetSystemSetting(9, 5);
            string IpAddress = dh.GetSystemSetting(9, 6);
            string bussinesUnit = "Dstv";
            bool ReauthSucceeded = false;
            bool ReauthResultSpecified = true;
            dstv.ReAuthorize(datasource, smartCardNumber, Reason, password, language, IpAddress, bussinesUnit, out ReauthSucceeded, out ReauthResultSpecified);
            if (ReauthResultSpecified)
            {
                if (ReauthSucceeded)
                {
                    dh.UpdateReactivateRequestStatus(smartCardNumber, "SUCCESS", "1");
                }
                else
                {
                    dh.UpdateReactivateRequestStatus(smartCardNumber, "FAILED", "1");
                }
            }
            else
            {
                dh.UpdateReactivateRequestStatus(smartCardNumber, "REAUTH RESULT NOT SPECIFIED", "1");
            }
        }

        private void ReauthorizeGotvSmartCard(string smartCardNumber)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SelfCareService dstv = new SelfCareService();
                GetBalanceByDeviceNumberResponse resp = new GetBalanceByDeviceNumberResponse();
                GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
                string datasource = dh.GetSystemSetting(9, 3);
                string newSmartCardNumber = smartCardNumber;
                DigitalReauthReason Reason = new DigitalReauthReason();
                Reason.Reason = DigitalReauthReasons.GOTVE16;
                string password = "PegasusDstv";
                string language = dh.GetSystemSetting(9, 5);
                string IpAddress = dh.GetSystemSetting(9, 6);
                string bussinesUnit = "Gotv";
                bool ReauthSucceeded = false;
                bool ReauthResultSpecified = true;
                dstv.ReAuthorize(datasource, smartCardNumber, Reason, password, language, IpAddress, bussinesUnit, out ReauthSucceeded, out ReauthResultSpecified);
                if (ReauthResultSpecified)
                {
                    if (ReauthSucceeded)
                    {
                        dh.UpdateReactivateRequestStatus(smartCardNumber, "SUCCESS", "1");
                    }
                    else
                    {
                        dh.UpdateReactivateRequestStatus(smartCardNumber, "FAILED", "1");
                    }
                }
                else
                {
                    dh.UpdateReactivateRequestStatus(smartCardNumber, "REAUTH RESULT NOT SPECIFIED", "1");
                }
            }
            catch (Exception e)
            {
                dh.UpdateReactivateRequestStatus(smartCardNumber, e.Message, "1");
            }
        }




        private UtilReqSender.EntityObjects.Customer QueryDstvForCustomerDetails(string customerRef)
        {
            UtilReqSender.EntityObjects.Customer cust = new UtilReqSender.EntityObjects.Customer();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SelfCareService dstv = new SelfCareService();
                GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
                string datasource = dh.GetSystemSetting(9, 3);
                string currencycode = dh.GetSystemSetting(9, 4);
                string bussinesUnit = "dstv";
                string vendorCode = "PegasusDstv";
                string language = dh.GetSystemSetting(9, 5);
                string IpAddress = dh.GetSystemSetting(9, 6);
                uint customerNumber = uint.Parse(customerRef);
                //custRep = dstv.GetCustomerDetailsByCustomerNumber(datasource, customerNumber, true, currencycode, bussinesUnit, vendorCode, language, IpAddress);


                custRep = dstv.GetCustomerDetailsByCustomerNumber(datasource, customerNumber, true, currencycode, bussinesUnit, vendorCode, language, IpAddress, "0");


                cust.Balance = "" + custRep.accounts[0].totalBalance;
                cust.CustomerName = custRep.customerDetails.salutation + " " + custRep.customerDetails.surname + " " + custRep.customerDetails.initials;
                return cust;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void UpdateBouquetPrices()
        //{
        //    BouquetDetails[] allBouquets = QueryDstvForAllBouquets();
        //    foreach (BouquetDetails bouq in allBouquets)
        //    {
        //        bouq.UpdateBouquetPrice();
        //    }
        //}

        //private BouquetDetails[] QueryDstvForAllBouquets()
        //{
        //    List<BouquetDetails> allBouquets = new List<BouquetDetails>();
        //    try
        //    {
        //        BouquetDetails[] dstvBouquets = GetDstvBouquets();
        //        allBouquets.AddRange(dstvBouquets);
        //        BouquetDetails[] goTvBouquets = GetGoTvBouquets();
        //        allBouquets.AddRange(goTvBouquets);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return allBouquets.ToArray();
        //}

        //private BouquetDetails[] GetGoTvBouquets()
        //{
        //    List<BouquetDetails> all = new List<BouquetDetails>();
        //    try
        //    {
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        //        SelfCareService dstv = new SelfCareService();
        //        ProductwithoutChannel[] resp ={ };
        //        string[] Products ={ };
        //        string country = "Uganda";
        //        string vendorCode = "pegasusgotv";
        //        string IpAddress = "41.190.131.222";
        //        string bussinessUnit = "gotv";
        //        string interfaceType = "Gotv Facebook And Mobi";
        //        string language = "English";
        //        bool visibleForPayment = true;
        //        bool visibleForPaymentSpecified = true;
        //        bool visibleForView = true;
        //        bool visibleForViewSpecified = true;
        //        resp = dstv.GetAvailableProductsWithoutChannels(country, bussinessUnit, language, vendorCode, interfaceType, visibleForPayment, visibleForPaymentSpecified, visibleForView, visibleForViewSpecified, IpAddress);

        //        foreach (ProductwithoutChannel prdt in resp)
        //        {
        //            BouquetDetails bouq = new BouquetDetails();
        //            bouq.BouquetCode = prdt.Product_Key;
        //            bouq.BouquetDescription = prdt.Description;
        //            bouq.BouquetName = prdt.Product_Name;
        //            bouq.BouquetPrice = prdt.CustomPrice;
        //            bouq.PayTvCode = "DSTV";
        //            bouq.StatusCode = "0";
        //            bouq.StatusDescription = "SUCCESS";
        //            all.Add(bouq);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return all.ToArray();
        //}

        //private BouquetDetails[] GetDstvBouquets()
        //{
        //    List<BouquetDetails> all = new List<BouquetDetails>();
        //    try
        //    {
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        //        SelfCareService dstv = new SelfCareService();
        //        ProductwithoutChannel[] resp ={ };
        //        string[] Products ={ };
        //        string country = "Uganda";
        //        string vendorCode = "pegasusdstv";
        //        string IpAddress = "41.190.131.222";
        //        string bussinessUnit = "dstv";
        //        string interfaceType = "dstv WeChat";
        //        string language = "English";
        //        bool visibleForPayment = true;
        //        bool visibleForPaymentSpecified = true;
        //        bool visibleForView = true;
        //        bool visibleForViewSpecified = true;
        //        resp = dstv.GetAvailableProductsWithoutChannels(country, bussinessUnit, language, vendorCode, interfaceType, visibleForPayment, visibleForPaymentSpecified, visibleForView, visibleForViewSpecified, IpAddress);


        //        foreach (ProductwithoutChannel prdt in resp)
        //        {
        //            BouquetDetails bouq = new BouquetDetails();
        //            bouq.BouquetCode = prdt.Product_Key;
        //            bouq.BouquetDescription = prdt.Description;
        //            bouq.BouquetName = prdt.Product_Name;
        //            bouq.BouquetPrice = prdt.CustomPrice;
        //            bouq.PayTvCode = "DSTV";
        //            bouq.StatusCode = "0";
        //            bouq.StatusDescription = "SUCCESS";
        //            all.Add(bouq);
        //        }
        //    }
        //    catch (Exception)
        //    {


        //    }
        //    return all.ToArray();
        //}

        private PostResponse SendToMultichoice(UtilReqSender.EntityObjects.Transaction trans)
        {
            PostResponse resp = new PostResponse();
            try
            {
                UtilReqSender.EntityObjects.Customer cust = dh.GetCustomerDetails(trans.CustRef, "DSTV");
                if (cust.StatusCode.Equals("0"))
                {
                    trans.CustomerType = cust.CustomerType;

                    if (trans.CustomerType == "GOTV")
                    {
                        resp = MakeGoTvPayment(trans);
                    }
                    //Dstv Transaction
                    else if (trans.CustomerType == "DSTV")
                    {
                        resp = MakeDstvPayment(trans);
                    }
                    else
                    {
                        //unknown dude
                        //do nothing
                    }
                }
                else
                {
                    if (trans.CustomerType == "GOTV")
                    {
                        resp = MakeGoTvPayment(trans);
                    }
                    //Dstv Transaction
                    else if (trans.CustomerType == "DSTV")
                    {
                        resp = MakeDstvPayment(trans);
                    }
                    else
                    {
                        //unknown dude
                        //do nothing
                    }
                    //do nothing
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message + "PostDstvPayments SERVICE POSTING - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "DSTV");
                dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
                resp.PegPayPostId = "";
                resp.StatusCode = "000";
                resp.StatusDescription = "Error AT PegPay";
            }

            return resp;
        }

        private PostResponse TopUpAtMultichoice(UtilReqSender.EntityObjects.Transaction trans)
        {
            PostResponse postResp = new PostResponse();
            try
            {

                Console.WriteLine("Top Up Transaction found");
                trans.Area = GetCurrentBouquetCode(trans);
                Console.WriteLine("BouquetCode found is " + trans.Area);
                if (trans.CustomerType.ToUpper().Equals("GOTV"))
                {
                    dh.UpdateBouquetCode(trans.TranId, trans.VendorTransactionRef, trans.VendorCode, trans.Area);
                    MakeGoTvPayment(trans);
                }
                else if (trans.CustomerType.ToUpper().Equals("DSTV"))
                {
                    dh.UpdateBouquetCode(trans.TranId, trans.VendorTransactionRef, trans.VendorCode, trans.Area);
                    MakeDstvPayment(trans);
                }
                else
                {
                    //do nothing
                }
            }
            catch (Exception)
            {
            }
            return postResp;
        }

        private string GetCurrentBouquetCode(UtilReqSender.EntityObjects.Transaction trans)
        {
            uint customerNumber = GetCustomerNumber(trans.CustRef);
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SelfCareService dstv = new SelfCareService();
                Hardware[] resp = { };
                string datasource = "Uganda";//dh.GetSystemSetting(9, 3);
                string currencycode = "UGS"; //dh.GetSystemSetting(9, 4);
                string bussinesUnit = trans.CustomerType;
                string vendorCode = "Pegasus" + trans.CustomerType;
                string language = "English";//dh.GetSystemSetting(9, 5);
                string IpAddress = "41.190.131.222"; //dh.GetSystemSetting(9, 6);
                //resp = dstv.GetProducts(datasource, customerNumber, true, vendorCode, language, IpAddress, bussinesUnit);
                resp = dstv.GetProducts(datasource, customerNumber, true, vendorCode, language, IpAddress, bussinesUnit, "0");

                string key = "";
                bool found = false;
                DatabaseHandler dh = new DatabaseHandler();

                //get first valid bouquetcode
                foreach (Hardware res in resp)
                {
                    foreach (Service ser in res.Services)
                    {
                        key = ser.ProductUserKey;
                        DataTable tr = dh.GetBouquetByBouquetCode(key);
                        if (tr.Rows.Count > 0)
                        {
                            found = true;
                            break;
                        }

                    }
                    if (found) { break; } else { continue; }

                }

                //post using the first product key found
                if (!found)
                {
                    try
                    {
                        foreach (Hardware res in resp)
                        {
                            foreach (Service ser in res.Services)
                            {
                                key = ser.ProductUserKey;
                                break;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                    }
                }
                return key;

            }
            catch (Exception e)
            {
                return "";
            }
        }

        private PostResponse MakeDstvPayment(UtilReqSender.EntityObjects.Transaction trans)
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
                    string paymentVendorCode = dh.GetSystemSetting(9, 7);//password;
                    string substring = trans.VendorTransactionRef;//.Substring(1);
                    string transactionNumber = substring;// int.Parse(substring);
                    string transactionNumba = trans.VendorTransactionRef;
                    bool transactionNumberSpecified = true;
                    bool customerNumberSpecified = true;
                    decimal tranAmount = 0;

                    //int dollarRate = 2875;
                    decimal result = (Convert.ToDecimal(trans.TransactionAmount));
                    tranAmount = result;

                    bool amountSpecified = true;
                    int invoicePeriod = 1;
                    bool invoicePeriodSpecified = true;
                    string currency = dh.GetSystemSetting(9, 4);
                    uint customerNumber = GetCustomerNumber(trans.CustRef);
                    string paymentDescription = "Mtn Payment From " + trans.CustomerTel;
                    PaymentProduct product = new PaymentProduct();
                    product.ProductUserKey = trans.Area;
                    uint basketId = 0;
                    //specify product user is paying for
                    PaymentProduct[] productCollection ={ product };
                    string paymentMethod = "USSD";// trans.TransactionType;

                    //send to dstv
                    Console.WriteLine("Going to DSTV at:{0}",DateTime.Now);

                    //resp = dstv.SubmitPayment(vendorCode, datasource, paymentVendorCode, transactionNumber + "", customerNumber, customerNumberSpecified, tranAmount, amountSpecified, invoicePeriod, invoicePeriodSpecified, currency,
                    //    paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit, basketId, false);
                    resp = dstv.SubmitPaymentBySmartCard(vendorCode, datasource, paymentVendorCode, transactionNumber, trans.CustRef, tranAmount, amountSpecified, invoicePeriod,
                        invoicePeriodSpecified, currency, paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit);

                    Console.WriteLine("Respnse from DSTV at:{0}", DateTime.Now);
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
                    //Console.WriteLine("Response from DSTV at:{0}", DateTime.Now);
                    if (IsSuccessResponse(resp))
                    {
                        Console.WriteLine("Posting Successful for " + trans.VendorTransactionRef);
                        //Update tran in Recieved Table
                        dh.UpdateSentTransactionById(trans.TranId, "" + resp.SubmitPayment.receiptNumber, "1");
                        postResp.StatusCode = "0";
                        postResp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + resp.SubmitPayment.ErrorMessage);
                        //Transfer transaction to Failed with Reason
                        dh.LogError(trans.VendorTransactionRef + " " + resp.SubmitPayment.ErrorMessage, trans.VendorCode, DateTime.Now, "DSTV");
                        //int rows = dh.TransferFailedTransaction(trans.TranId, resp.SubmitPayment.ErrorMessage);
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
                    dh.UpdateSentTransactionById(trans.TranId, "", "1");
                    postResp.StatusCode = "0";
                    postResp.StatusDescription = "SUCCESS";
                }
                else
                {
                    Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                    //Transfer transaction to Failed with Reason
                    string failureReason = ex.Message;

                    //dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
                    int rows = dh.TransferFailedTransaction(trans.TranId, failureReason);
                    postResp.StatusCode = "100";
                    postResp.StatusDescription = failureReason;
                }
                dh.LogError(trans.VendorTransactionRef + " " + ex.Message, trans.VendorCode, DateTime.Now, "DSTV");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                dh.LogError(ex.Message + "PostDstvPayments SERVICE POSTING - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "DSTV");
                dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
                postResp.PegPayPostId = "";
                postResp.StatusCode = "000";
                postResp.StatusDescription = "Error AT PegPay";
            }
            return postResp;
        }

        private PostResponse MakeGoTvPayment(UtilReqSender.EntityObjects.Transaction trans)
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
                    string paymentVendorCode = dh.GetSystemSetting(9, 7);
                    string vendortranid = trans.VendorTransactionRef.Substring(1);
                    string transactionNumber = trans.VendorTransactionRef;// int.Parse(vendortranid);
                    bool transactionNumberSpecified = true;
                    bool customerNumberSpecified = true;
                    decimal tranAmount = 0;
                    //int dollarRate = 2875;
                    decimal result = (Convert.ToDecimal(trans.TransactionAmount));
                    tranAmount = result;
                    bool amountSpecified = true;
                    int invoicePeriod = trans.InvoicePeriod;
                    bool invoicePeriodSpecified = true;
                    string currency = dh.GetSystemSetting(9, 4);
                    uint customerNumber = GetCustomerNumber(trans.CustRef);
                    string paymentDescription = "Mtn Payment From " + trans.CustomerTel;
                    PaymentProduct product = new PaymentProduct();
                    product.ProductUserKey = trans.Area;//area has the bouquetcode
                    uint basketId = 0;
                    //product.
                    PaymentProduct[] productCollection ={ product };
                    string paymentMethod = "USSD";// trans.TransactionType;

                    //resp = dstv.SubmitPayment(vendorCode, datasource, paymentVendorCode, transactionNumber + "", customerNumber, customerNumberSpecified, tranAmount, amountSpecified, invoicePeriod, invoicePeriodSpecified, currency,
                    //    paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit, basketId, false);
           

                    resp = dstv.SubmitPaymentBySmartCard(vendorCode, datasource, paymentVendorCode, transactionNumber, trans.CustRef, tranAmount, amountSpecified, invoicePeriod,
                 invoicePeriodSpecified, currency, paymentDescription, productCollection, paymentMethod, language, IpAddress, bussinesUnit);
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
                    if (IsSuccessResponse(resp))
                    {
                        Console.WriteLine("Posting Successfull for " + trans.VendorTransactionRef);
                        //Update tran in Recieved Table
                        dh.UpdateSentTransactionById(trans.TranId, "" + resp.SubmitPayment.receiptNumber, "1");
                        postResp.StatusCode = "0";
                        postResp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + resp.SubmitPayment.ErrorMessage);
                        //Transfer transaction to Failed with Reason
                        dh.LogError(trans.VendorTransactionRef + " " + resp.SubmitPayment.ErrorMessage, trans.VendorCode, DateTime.Now, "DSTV");
                        //dh.TransferFailedTransaction(trans.TranId, resp.SubmitPayment.ErrorMessage);
                        postResp.StatusCode = "100";
                        postResp.StatusDescription = resp.SubmitPayment.ErrorMessage;
                    }

                }
            }
            //this is how dstv communicate validation errors
            catch (SoapException ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                string failureReason = ex.Message;
                dh.LogError(trans.VendorTransactionRef + " " + ex.Message, trans.VendorCode, DateTime.Now, "DSTV");
                //dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
                //Transfer transaction to Failed with Reason
                dh.TransferFailedTransaction(trans.TranId, ex.Message);
                postResp.StatusCode = "100";
                postResp.StatusDescription = ex.Message;
            }
            //some other serious error has happend
            catch (Exception ex)
            {
                Console.WriteLine("Posting Failed for " + trans.VendorTransactionRef + ":" + ex.Message);
                dh.LogError(ex.Message + "PostDstvPayments SERVICE POSTING - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "DSTV");
                dh.UpdateSentTransactionById3(trans.TranId, "", "INSERTED");
                postResp.PegPayPostId = "";
                postResp.StatusCode = "000";
                postResp.StatusDescription = "Error AT PegPay";
            }
            return postResp;
        }

        private uint GetCustomerNumber(string smartCardNumber)
        {
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            //SelfCareService dstv = new SelfCareService();
            //GetBalanceByDeviceNumberResponse resp = new GetBalanceByDeviceNumberResponse();
            //GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
            //string datasource = "Uganda_UAT";
            //string devicenumber = smartCardNumber;//"4261353579"; //"4261353579";//"2015897761";
            //string currencycode = "UGS";
            //string bussinesUnit = "dstv";
            //string vendorCode = "PegasusDstv";
            //string language = "English";
            //string IpAddress = "41.190.130.222";
            //resp = dstv.GetCustomerDetailsByDeviceNumber(datasource, devicenumber, currencycode, bussinesUnit, vendorCode, language, IpAddress);
            uint customerNo = 0;
            UtilReqSender.EntityObjects.Customer cust = dh.GetCustDetails(smartCardNumber, "DSTV");
            if (cust.StatusCode.Equals("0"))
            {
                customerNo = uint.Parse(cust.CustomerRef);
            }
            return customerNo;
        }

        private bool IsSuccessResponse(SubmitPaymentResponse resp)
        {
            if (resp.SubmitPayment.ErrorMessage == null && resp.SubmitPayment.Status == true)
            {
                return true;
            }
            if (resp.SubmitPayment.ErrorMessage == "" && resp.SubmitPayment.Status == true)
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

        //private UmemeApi.Transaction GetUmemeTrans(UmemeTransaction trans, UtilityCredentials creds)
        //{
        //    UmemeApi.Transaction umemeTrans = new UmemeApi.Transaction();
        //    umemeTrans.CustomerName = trans.CustName;
        //    umemeTrans.CustomerRef = trans.CustRef;
        //    umemeTrans.CustomerTel = trans.CustomerTel;
        //    umemeTrans.CustomerType = trans.CustomerType;
        //    umemeTrans.Offline = trans.Offline;
        //    umemeTrans.Password = creds.UtilityPassword;
        //    string format = "d/M/yyyy";
        //    string newdate = formateDate(trans.PaymentDate.Trim()).Trim();
        //    string payDate = newdate;//DateTime.ParseExact(newdate, format, CultureInfo.InvariantCulture).ToString();
        //    umemeTrans.PaymentDate = payDate;
        //    umemeTrans.PaymentType = trans.PaymentType;
        //    umemeTrans.Reversal = trans.Reversal;
        //    umemeTrans.StatusCode = "0";
        //    umemeTrans.StatusDescription = "SUCCESS";
        //    umemeTrans.Teller = trans.Teller;
        //    umemeTrans.TranAmount = trans.TransactionAmount;
        //    umemeTrans.TranIdToReverse = trans.TranIdToReverse;
        //    umemeTrans.TranNarration = trans.Narration;
        //    umemeTrans.TranType = trans.TransactionType;
        //    umemeTrans.VendorCode = creds.UtilityCode;
        //    umemeTrans.VendorTranId = trans.VendorTransactionRef;
        //    string dataToSign = umemeTrans.CustomerRef + umemeTrans.CustomerName + umemeTrans.CustomerTel + umemeTrans.CustomerType + umemeTrans.VendorTranId + umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.PaymentType + umemeTrans.Teller + umemeTrans.TranAmount + umemeTrans.TranNarration + umemeTrans.TranType;
        //    umemeTrans.DigitalSignature = GetDigitalSignature(dataToSign, trans.VendorCode);
        //    return umemeTrans;
        //}

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
