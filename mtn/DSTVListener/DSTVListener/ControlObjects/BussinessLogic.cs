using System;
using System.Collections.Generic;
using System.Text;
using DSTVListener.EntityObjects;
using DSTVListener.UtilitiesApi;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Messaging;
using System.Data;

namespace DSTVListener.ControlObjects
{
    public class BussinessLogic
    {
        private DatabaseHandler dh = new DatabaseHandler();
        private String mtnDstvQueue = DatabaseHandler.QueueName;

        public string QueryDetails(GetFinancialInformationRequest GetFriRequest)
        {
            string queryRespXml = "";

            try
            {
                switch (GetFriRequest.FRIRequestType)
                {
                    case "BoxOffice":
                        queryRespXml = QueryForBoxOfficeCustomer(GetFriRequest);
                        break;

                    case "QueryCustomerDetails":
                        queryRespXml = QueryCustomerDetails(GetFriRequest);
                        break;

                    case "ViewBouquetDetails":
                        queryRespXml = QueryForAllBouquetDetails(GetFriRequest);
                        break;

                    case "ReauthenticateSmartCard":
                        queryRespXml = DoReauthenticateSmartOp(GetFriRequest);
                        break;

                    default:
                        queryRespXml = ReturnGetFriErrorXml(GetFriRequest, "SELECTED OPERATION IS NOT YET SUPPORTED.");
                        break;
                }
            }
            catch (Exception ex)
            {
                dh.LogError("QueryDetails: " + ex.Message);
                queryRespXml = ReturnGetFriErrorXml(GetFriRequest, "SORRY. UNABLE TO PROCESS YOUR REQUEST AT THE MOMENT");

            }
            return queryRespXml;
        }

        private string QueryForBoxOfficeCustomer(GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            try
            {
                //build dstv query request and send to utilities API
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

                //create utilities API object
                PegPay pegpay = new PegPay();
                pegpay.Url = "https://localhost:8896/LivePegPayApi_1/PegPay.asmx?WSDL";
                //create utilities API response object
                UtilitiesApi.Response queryResponse = new UtilitiesApi.Response();

                //create utilities API request object
                QueryRequest request = new QueryRequest();
                request.QueryField1 = GetFriRequest.SmartCardNumber;//cust ref
                request.QueryField2 = "";// GetFriRequest.BouquetCode;

                //box office is not enabled for Gotv dudes
                if (IsGoTvSmartCardNumber(GetFriRequest.SmartCardNumber))
                {
                    queryResponse.ResponseField6 = "100";
                    queryResponse.ResponseField7 = "Box Office Is Only Enabled For DSTV Subscribers";
                    xml = GetFRIResponeXmlForBoxOffice(queryResponse, GetFriRequest);
                    return xml;
                }
                else
                {
                    request.QueryField4 = "DSTV"; //GetFriRequest.UtilityCode;
                    request.QueryField2 = "BO";
                }

                request.QueryField5 = dh.GetSystemSetting(9, 1);//vendorcode
                request.QueryField6 = dh.GetSystemSetting(9, 2);//password

                //query for customer details from Utilities API
                queryResponse = pegpay.QueryCustomerDetails(request);

                //return xml response
                xml = GetFinancialInformationResponeXml(queryResponse, GetFriRequest);
            }

            catch (Exception e)
            {
                //better to fail a transaction here than to let it continue
                xml = ReturnGetFriErrorXml(GetFriRequest, "INVALID SMART CARD NUMBER.");
                dh.LogError("QueryDetails: "+ GetFriRequest.SmartCardNumber + " " + e.Message);
            }
            return xml;

        }

        private bool IsGoTvSmartCardNumber(string SmartCardNumber)
        {
            if (SmartCardNumber.StartsWith("2"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private string DoBoxOfficeOp(GetFinancialInformationRequest GetFriRequest)
        //{
        //    string xml = "";
        //    try
        //    {
        //        //System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        //        //PegPay pegpay = new PegPay();
        //        //UtilitiesApi.Response queryResponse = new UtilitiesApi.Response();
        //        //QueryRequest request = new QueryRequest();

        //        //request.QueryField1 = GetFriRequest.SmartCardNumber;//cust ref
        //        //request.QueryField2 = GetFriRequest.BouquetCode;
        //        //request.QueryField4 = GetFriRequest.UtilityCode;
        //        //request.QueryField5 = dh.GetSystemSetting(4, 6);
        //        //request.QueryField6 = dh.GetSystemSetting(4, 7);
        //        //queryResponse = pegpay.ReactivatePayTvCard(request);

        //        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        //        SelfCareService dstv = new SelfCareService();
        //        SubmitPaymentResponse resp = new SubmitPaymentResponse();
        //        string datasource = "Uganda_UAT";
        //        string bussinesUnit = "DSTV";
        //        string vendorCode = "pegasusdstv";
        //        string language = "English";
        //        string IpAddress = "45.56.107.51";
        //        string paymentVendorCode = "RTPP_Uganda_Pegasus";//password;
        //        int transactionNumber = 127;
        //        bool transactionNumberSpecified = true;
        //        bool customerNumberSpecified = true;
        //        decimal tranAmount = decimal.Parse(GetFriRequest.BoxOfficeAmount);
        //        bool amountSpecified = true;
        //        int invoicePeriod = 1;
        //        bool invoicePeriodSpecified = true;
        //        string currency = "UGS";
        //        uint customerNumber = GetCustomerNumber(GetFriRequest.SmartCardNumber);
        //        string paymentDescription = "Test";
        //        PaymentProduct product = new PaymentProduct();
        //        product.ProductUserKey = "BO";//"ACSSW4";
        //        //product.
        //        PaymentProduct[] productCollection ={ product };
        //        string paymentMethod = "CASH";
        //        resp = dstv.SubmitPayment(vendorCode,
        //                            datasource,
        //                            paymentVendorCode,
        //                            transactionNumber,
        //                            transactionNumberSpecified,
        //                            customerNumber,
        //                            customerNumberSpecified,
        //                            tranAmount,
        //                            amountSpecified,
        //                            invoicePeriod,
        //                            invoicePeriodSpecified,
        //                            currency,
        //                            paymentDescription,
        //                            productCollection,
        //                            paymentMethod,
        //                            language,
        //                            IpAddress,
        //                            bussinesUnit);
        //        DSTVListener.UtilitiesApi.Response queryResponse = new DSTVListener.UtilitiesApi.Response();
        //        queryResponse.ResponseField1 = "";
        //        queryResponse.ResponseField5 = "0";
        //        queryResponse.ResponseField6 = "SUCCESS";
        //        xml = GetFRIResponeXmlForBoxOffice(queryResponse, GetFriRequest);
        //    }
        //    catch (Exception ex)
        //    {
        //        xml = ReturnGetFriErrorXml(GetFriRequest, "Unable to Top Up your Box Office.Please Try again");
        //    }
        //    return xml;
        //}

        private uint GetCustomerNumber(string p)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private string GetFRIResponeXmlForBoxOffice(DSTVListener.UtilitiesApi.Response queryResponse, GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            if (queryResponse.ResponseField6.Equals("0"))
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>sucessfully received message</message>" +
                      "<extension>" +
                      "<responsecode>101</responsecode>" +
                      "<name>" + queryResponse.ResponseField2 + "</name>" +
                      "<balance>" + queryResponse.ResponseField4.Split('.')[0] + " UGX</balance>" +
                      "<CurrentP>" + queryResponse.ResponseField9 + "</CurrentP>" +
                      "<NewP>" + queryResponse.ResponseField9 + "</NewP>" +
                      "<AmountDue>" + queryResponse.ResponseField10 + " UGX</AmountDue>" +
                      "<CardNo>" + queryResponse.ResponseField1 + "</CardNo>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>" + queryResponse.ResponseField7 + "</message>" +
                      "<extension>" +
                      "<responsecode>102</responsecode>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
            }
            return xml;
        }

        private string QueryForAllBouquetDetails(GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPay pegpay = new PegPay();
                UtilitiesApi.BouquetDetails[] bouquetDetails = { };
                QueryRequest request = new QueryRequest();

                request.QueryField1 = "";//leave blank if you want all
                request.QueryField3 = ""; //GetFriRequest.BouquetCode;
                request.QueryField4 = "";
                request.QueryField5 = dh.GetSystemSetting(9, 1);
                request.QueryField6 = dh.GetSystemSetting(9, 2);

                bouquetDetails = pegpay.GetPayTVBouquetDetails(request);
                xml = GetFinancialInformationResponeXml(bouquetDetails, GetFriRequest);
            }
            catch (Exception ex)
            {
                dh.LogError("QueryForAllBouquetDetails: " + ex.Message);
                xml = ReturnGetFriErrorXml(GetFriRequest, "UNABLE TO GET BOUQUET DETAILS AT THIS TIME.PLEASE TRY AGAIN");
            }
            return xml;
        }

        private string DoReauthenticateSmartOp(GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPay api = new PegPay();
                QueryRequest query = new QueryRequest();
                query.QueryField1 = GetFriRequest.SmartCardNumber;
                query.QueryField5 = dh.GetSystemSetting(9, 1);
                query.QueryField6 = dh.GetSystemSetting(9, 2);
                DSTVListener.UtilitiesApi.Response resp = api.ReactivatePayTvCard(query);
                //int rows = dh.SaveReactivationRequest(GetFriRequest.SmartCardNumber);
                if (resp.ResponseField6.Equals("0"))//(rows > 0) 
                {
                    GetFriRequest.StatusCode = "0";
                    GetFriRequest.StatusDescription = "SUCCESS";
                }
                else
                {
                    GetFriRequest.StatusCode = "100";
                    GetFriRequest.StatusDescription = "FAILED TO PROCESS REQUEST";
                }
                xml = GetFRIResponeXmlForReactivateSmartCard(GetFriRequest);

            }
            catch (Exception ex)
            {
                dh.LogError("DoReauthenticateSmartOp: " + ex.Message);
                xml = ReturnGetFriErrorXml(GetFriRequest, "UNABLE TO REACTIVATE SMART CARD. PLEASE TRY AGAIN");
            }
            return xml;
        }

        private string GetFRIResponeXmlForReactivateSmartCard(GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            if (GetFriRequest.StatusCode.Equals("0"))
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>Succesfully Received and Processing Request</message>" +
                      "<extension>" +
                      "<responsecode>101</responsecode>" +
                      "<accountnumber>" + GetFriRequest.SmartCardNumber + "</accountnumber>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                        "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                        "<message>Smart Card Not Activated</message>" +
                        "<extension>" +
                        "<responsecode>102</responsecode>" +
                        "</extension>" +
                        "</ns2:getfinancialresourceinformationresponse>";
            }
            return xml;
        }

        public string ReturnGetFriErrorXml(GetFinancialInformationRequest GetFriRequest, string ErrorMessage)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                         "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                        "<message>" + ErrorMessage.Trim() + "</message>" +
                        "<extension>" +
                        "<responsecode>102</responsecode>" +
                        "</extension>" +
                        "</ns2:getfinancialresourceinformationresponse>";
            return xml;
        }



        private string QueryCustomerDetails(GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            try
            {
               
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPay pegpay = new PegPay();
                //pegpay.Url = "https://192.168.16.34:8896/LivePegPayApi_1/PegPay.asmx?WSDL";
                pegpay.Url = "https://localhost:8896/LivePegPayApi_1/PegPay.asmx?WSDL";
                UtilitiesApi.Response queryResponse = new UtilitiesApi.Response();
                QueryRequest request = new QueryRequest();

                request.QueryField1 = GetFriRequest.SmartCardNumber;//cust ref
                request.QueryField2 = GetFriRequest.BouquetCode;
                request.QueryField4 = GetFriRequest.UtilityCode;
                request.QueryField5 = dh.GetSystemSetting(9, 1);
                request.QueryField6 = dh.GetSystemSetting(9, 2);
                pegpay.Timeout = 200000;

                if (GetFriRequest.SmartCardNumber == "7016938096")
                {
                    //Console.WriteLine("Working");
                    Console.ReadLine();
                }
                queryResponse = pegpay.QueryCustomerDetails(request);
                
                xml = GetFinancialInformationResponeXml(queryResponse, GetFriRequest);
            }
            catch (Exception ex)
            {
                dh.LogError("QueryCustomerDetails: " + ex.Message + " " + GetFriRequest.SmartCardNumber);
                xml = ReturnGetFriErrorXml(GetFriRequest, "UNABLE TO VERIFY IUC AT THE MOMENT");
            }
            return xml;

        }

        private string GetFinancialInformationResponeXml(DSTVListener.UtilitiesApi.Response queryResponse, GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            if (queryResponse.ResponseField6.Equals("0"))
            {

                string amountDue = GetAmountDue(queryResponse.ResponseField10, queryResponse.ResponseField4);
                string price = (string.IsNullOrEmpty(queryResponse.ResponseField9)) ? "" : queryResponse.ResponseField9.Trim();

                if (GetFriRequest.FRIRequestType.ToUpper().Contains("BOX"))
                {
                    xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>sucessfully received message</message>" +
                      "<extension>" +
                      "<responsecode>101</responsecode>" +
                      "<name>" + queryResponse.ResponseField2.Trim().Replace("&", String.Empty).Replace(";", String.Empty).Replace(":", String.Empty) + "</name>" +
                      "<AmountDue>0 UGX</AmountDue>" +
                      "<CardNo>" + queryResponse.ResponseField1.Trim() + "</CardNo>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
                }
                else
                {
                    xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>sucessfully received message</message>" +
                      "<extension>" +
                      "<responsecode>101</responsecode>" +
                      "<name>" + queryResponse.ResponseField2.Trim().Replace("&", String.Empty).Replace(";", String.Empty).Replace(":", String.Empty) + "</name>" +
                      "<balance>" + queryResponse.ResponseField4.Split('.')[0].Trim() + " UGX</balance>" +
                      "<CurrentP>" + price + "</CurrentP>" +
                      "<NewP>" + price + "</NewP>" +
                      "<AmountDue>" + amountDue.Trim() + " UGX</AmountDue>" +
                      "<CardNo>" + queryResponse.ResponseField1.Trim() + "</CardNo>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
                }
                
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>" + queryResponse.ResponseField7 + "</message>" +
                      "<extension>" +
                      "<responsecode>102</responsecode>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
            }
            return xml;
        }

        private string GetAmountDue(string bouquetPrice, string customerBal)
        {
            string AmountDue = "0";
            try
            {
                //dstv returns 2 kinds of balance +ve and -ve balance
                //if -ve it means customer has a credit at DSTV
                //if +ve it means dstv owes customer that amount
                decimal bouqPrice = Convert.ToDecimal(bouquetPrice);
                decimal customerBalance = Convert.ToDecimal(customerBal);
                decimal result = bouqPrice + customerBalance;

                if (result < 0)
                {
                    AmountDue = ((result - result) - result).ToString();
                    //AmountDue = "Credit " + ((result - result) - result);
                    //AmountDue.Replace("-", string.Empty);
                }
                else
                {
                    AmountDue = "" + result;
                }
            }
            catch (Exception e)
            {
                //conversion has failed
            }
            return AmountDue;
        }

        private string GetFinancialInformationResponeXml(DSTVListener.UtilitiesApi.BouquetDetails[] bouquetDetails, GetFinancialInformationRequest GetFriRequest)
        {
            string xml = "";
            if (bouquetDetails[0].StatusCode.Equals("0"))
            {
                string header = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message>Bouquets Available</message>" +
                                "<extension>" +
                                "<responsecode>101</responsecode>";

                //int i = 1;
                //foreach (BouquetDetails bouq in bouquetDetails)
                //{
                //    string bouquetName = bouq.BouquetName.Trim().Replace(" ", string.Empty);
                //    header = header + "<" + bouquetName + ">" + i + ". " + bouq.BouquetName.Trim() + " UGX " + bouq.BouquetPrice + "</" + bouquetName + ">";
                //    i++;
                //}
                //removed numbering //nam migration
                foreach (BouquetDetails bouq in bouquetDetails)
                {
                    string bouquetName = bouq.BouquetName.Trim().Replace(" ", string.Empty);
                    header = header + "<" + bouquetName + ">" + bouq.BouquetName.Trim() + " UGX " + bouq.BouquetPrice + "</" + bouquetName + ">";
                }

                string footer = "</extension>" +
            "</ns2:getfinancialresourceinformationresponse>";

                xml = header + footer;
            }
            else
            {
                xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                      "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                      "<message>Bouquet Details not Available. Please Try again later.</message>" +
                      "<extension>" +
                      "<responsecode>102</responsecode>" +
                      "</extension>" +
                      "</ns2:getfinancialresourceinformationresponse>";
            }
            return xml;
        }


        public ConfirmPaymentResponse ConfirmPayment(ConfirmPaymentRequest paymentRequest)
        {
            ConfirmPaymentResponse confirmPaymentResp = new ConfirmPaymentResponse();
            try
            {
                DSTVListener.EntityObjects.Transaction trans = GetLevelOneObject(paymentRequest);
                if (trans.StatusCode.Equals("0"))
                {
                    SendTransactionToQueue(trans);
                }
                confirmPaymentResp = GetConfirmPaymentResponse(paymentRequest, trans);
                return confirmPaymentResp;
            }
            catch (Exception e)
            {

                confirmPaymentResp.StatusCode = "1000";
                confirmPaymentResp.StatusDescription = "PENDING";
                confirmPaymentResp.ThirdPartyAcctRef = "";
                confirmPaymentResp.Token = "";
                dh.LogError("ConfirmPayment: " + e.Message);
            }
            return confirmPaymentResp;
        }

        private DSTVListener.EntityObjects.Transaction GetLevelOneObject(ConfirmPaymentRequest ConfirmationRequest)
        {
            DSTVListener.EntityObjects.Transaction tranRequest = new DSTVListener.EntityObjects.Transaction();

            string Date = DateTime.Now.ToString("dd/MM/yyyy");
            string VendorCode = dh.GetSystemSetting(9, 1);//vendor code
            string password = dh.GetSystemSetting(9, 2);//password
            tranRequest.CustomerRef = ConfirmationRequest.CustomerRef;
            tranRequest.CustomerName = ConfirmationRequest.CustName;// CustomerName
            tranRequest.Area = ConfirmationRequest.BouquetCode; //ConfirmationRequest.BouquetCode;//area
            tranRequest.QueueTime = DateTime.Now.ToString();
            //if (BouquetCodeIsNumber(ConfirmationRequest.BouquetCode)) 
            //{

            //}
            //else
            tranRequest.PaymentDate = Date;
            tranRequest.CustomerType = "";//cust.CustomerType;
            tranRequest.TranAmount = ConfirmationRequest.TransactionAmount;
            tranRequest.TranType = "CASH";
            tranRequest.VendorCode = VendorCode;
            tranRequest.Password = password;
            tranRequest.CustomerTel = ConfirmationRequest.CustomerTel;
            tranRequest.Reversal = "0";//reversal
            tranRequest.TranIdToReverse = "";//tranid 2 reverse
            tranRequest.Teller = ConfirmationRequest.CustomerTel;//teller
            tranRequest.Offline = "0";//offline
            tranRequest.TranNarration = ConfirmationRequest.CustomerTel;//narration
            tranRequest.VendorTranId = ConfirmationRequest.VendorTranId;
            tranRequest.PaymentType = "2"; //Payment Type;
            tranRequest.DigitalSignature = "1234";//digital signature
            tranRequest.StatusCode = "0";
            tranRequest.StatusDescription = "SUCCESS";

            if (!String.IsNullOrEmpty(ConfirmationRequest.Subscriber))
            {
                tranRequest.CustomerTel = ConfirmationRequest.Subscriber;
                tranRequest.TranNarration = "ASSISTED";
            }
            else
            {
                tranRequest.CustomerTel = ConfirmationRequest.CustomerTel;
                //tranRequest.Tin = " ";
            }

            return tranRequest;
        }

        internal string GetNewBoquetCode(string oldCode)
        {
            //if (oldCode == "BQT") 
            //{
            //    return oldCode;
            //}

            DataTable dt = dh.ExecuteDataSet("GetBouquetByBouquetCode", oldCode).Tables[0];
            string bouquet = oldCode;

           
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["BouquetCodeNew"].ToString();
            }
            return bouquet;
        }

        private ConfirmPaymentResponse GetConfirmPaymentResponse(ConfirmPaymentRequest paymentRequest, DSTVListener.EntityObjects.Transaction trans)
        {
            ConfirmPaymentResponse conResp = new ConfirmPaymentResponse();
            if (trans.StatusCode.Equals("0"))
            {
                conResp.StatusCode = "1000";
                conResp.StatusDescription = "PENDING";
                conResp.ThirdPartyAcctRef = "";
                conResp.Token = "";
                return conResp;
            }
            else
            {
                conResp.StatusCode = "1000";
                conResp.StatusDescription = "PENDING";
                conResp.ThirdPartyAcctRef = "";
                conResp.Token = "";
                return conResp;
            }

        }

        private void SendTransactionToQueue(DSTVListener.EntityObjects.Transaction trans)
        {
            MessageQueue queue;
            if (MessageQueue.Exists(mtnDstvQueue))
            {
                queue = new MessageQueue(mtnDstvQueue);
            }
            else
            {
                queue = MessageQueue.Create(mtnDstvQueue);
            }
            //MessageQueue msMq = new MessageQueue(mtnDstvQueue);
            trans.QueueTime = DateTime.Now.ToString();
            Message msg = new Message();
            msg.Body = trans;
            msg.Label = trans.VendorTranId;
            msg.Recoverable = true;
            queue.Send(msg);


        }

        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private UtilitiesApi.Response SendToUtilitesApi(ConfirmPaymentRequest ConfirmationRequest)
        {
            UtilitiesApi.Response TranRes = new UtilitiesApi.Response();

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPay pegpay = new PegPay();
                TransactionRequest tranRequest = new TransactionRequest();
                QueryRequest request = new QueryRequest();
                UtilitiesApi.Response resp = new UtilitiesApi.Response();

                ConfirmationRequest.Utility = "DSTV";//change from Hardcode.
                string Date = DateTime.Now.ToString("dd/MM/yyyy");
                string VendorCode = dh.GetSystemSetting(4, 6);//vendor code
                string password = dh.GetSystemSetting(4, 7);//password
                tranRequest.PostField1 = ConfirmationRequest.CustomerRef;
                tranRequest.PostField2 = "";// CustomerName
                tranRequest.PostField3 = "";
                tranRequest.PostField4 = ConfirmationRequest.Utility;
                tranRequest.PostField5 = Date;
                tranRequest.PostField6 = "";//CustomerType;
                tranRequest.PostField7 = ConfirmationRequest.TransactionAmount;
                tranRequest.PostField8 = "CASH";
                tranRequest.PostField9 = VendorCode;
                tranRequest.PostField10 = password;
                tranRequest.PostField11 = ConfirmationRequest.CustomerTel;
                tranRequest.PostField12 = "0";
                tranRequest.PostField13 = "";
                tranRequest.PostField14 = ConfirmationRequest.CustomerTel;
                tranRequest.PostField15 = "0";
                tranRequest.PostField18 = "";
                tranRequest.PostField20 = ConfirmationRequest.VendorTranId;
                tranRequest.PostField21 = "2"; //Payment Type;
                string dataToSign = tranRequest.PostField1 + tranRequest.PostField2 + tranRequest.PostField11 + tranRequest.PostField20 + tranRequest.PostField9 + tranRequest.PostField10 + tranRequest.PostField5 + tranRequest.PostField14 + tranRequest.PostField7 + tranRequest.PostField18 + tranRequest.PostField8;
                tranRequest.PostField16 = "1234";//SignCertificate(dataToSign);
                TranRes = pegpay.PostTransaction(tranRequest);

            }
            catch (Exception ex)
            {
                TranRes.ResponseField6 = "100";
                TranRes.ResponseField6 = dh.GetStatusDescr(TranRes.ResponseField6);
                //ArrayList ERROR = new ArrayList();
                //string Confirmfilename = "Response_" + timestamp + ".xml";
                //ERROR.Add(ex.Message);
                //df.writeToFile(@"E:\ERROR.txt", ERROR);
            }
            return TranRes;
        }
    }
}
