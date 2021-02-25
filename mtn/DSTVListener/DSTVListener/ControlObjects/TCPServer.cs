using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;
using System.Threading;
using DSTVListener.EntityObjects;
using DSTVListener.ControlObjects;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Appender;


public class TCPServer
{
    private BussinessLogic bll = new BussinessLogic();
    DatabaseHandler dh = new DatabaseHandler();

    // Define a static logger variable so that it references the name of your class 
    private static readonly ILog log = LogManager.GetLogger(typeof(TCPServer));

    private Thread worker;

    private List<string> whiteListedNumbers = new List<string>();

    public TCPServer()
    {
        whiteListedNumbers.Add("256779999508");//Jackson MTN
        whiteListedNumbers.Add("256777853085");//deo
        whiteListedNumbers.Add("256773826678");//mr ronald
        whiteListedNumbers.Add("256771322191");//dennis
        whiteListedNumbers.Add("256772121644");//mtn team 
        whiteListedNumbers.Add("256772121919");//mtn team
        whiteListedNumbers.Add("256772121280");
        whiteListedNumbers.Add("256772121919");
    }

    public void ListenAndProcess()
    {
        try
        {


            //string filename = GetLogFileName();
            HttpListener listener = new HttpListener();
            //listener.Prefixes.Add("http://192.168.0.3:9011/pegasusaggregation/dstvpaymentsV1/");//Live URL
            listener.Prefixes.Add("https://192.168.16.34:9105/livepegasusaggregation/dstvpaymentsV1/");//Live URL

            listener.Start();

            while (true)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Console.WriteLine("************************************************");
                    Console.WriteLine("Listening For an HTTP Request...");
                    Console.WriteLine("************************************************");
                    HttpListenerContext context = listener.GetContext();
                    worker = new Thread(new ParameterizedThreadStart(HandleRequest));
                    worker.Start(context);
                   // HandleRequest(context);
                   // string request ="<ns0:getfinancialresourceinformationrequest xmlns:ns0=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\"><resource>FRI:1025630427@UDSTV.sp2/SP</resource><accountholderid>ID:256773159159/MSISDN</accountholderid><extension><option>COMPLW7</option></extension></ns0:getfinancialresourceinformationrequest>";
                    //ProcessRequest(request);
                }
                catch (Exception ex)
                {
                    //log Errors into a file;
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public void ListenAndProcessDebug()
    {
        try
        {


            //string filename = GetLogFileName();
            HttpListener listener = new HttpListener();
            //listener.Prefixes.Add("http://192.168.0.3:9011/pegasusaggregation/dstvpaymentsV1/");//Live URL
            //listener.Prefixes.Add("http://192.168.55.3:9011/pegasusaggregation/dstvpaymentsV1/");//Live URL
            listener.Prefixes.Add("https://192.168.55.3:9105/livepegasusaggregation/dstvpaymentsV1/");//Live URL
            //listener.Prefixes.Add("https://192.168.55.3:9105/livepegasusaggregation/dstvpaymentsV2/");//Live URL
            listener.Start();

            while (true)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Console.WriteLine("************************************************");
                    Console.WriteLine("Listening For an HTTP Request...");
                    Console.WriteLine("************************************************");
                    HttpListenerContext context = listener.GetContext();
                    //Thread thread = new Thread(new ThreadStart(HandleRequest
                   // HandleRequest(context);

                    worker = new Thread(new ParameterizedThreadStart(HandleRequest));
                    worker.Start(context);
                }
                catch (Exception ex)
                {
                    //log Errors into a file;
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public String GetLogFileName()
    {

        String filename = null;

        IAppender[] appenders = log.Logger.Repository.GetAppenders();
        // Check each appender this logger has
        foreach (IAppender appender in appenders)
        {
            Type t = appender.GetType();
            // Get the file name from the first FileAppender found and return
            if (t.Equals(typeof(FileAppender)) || t.Equals(typeof(RollingFileAppender)))
            {
                filename = ((FileAppender)appender).File;
                break;
            }
        }
        return filename;
    }

    private void HandleRequest(object httpContext)
    {
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            //pick up the request
            HttpListenerContext context = (HttpListenerContext)httpContext;
            string request = (new StreamReader(context.Request.InputStream).ReadToEnd());

            Console.WriteLine();
            Console.WriteLine("Time In:" + DateTime.Now);
            Console.WriteLine(".........................Request Made.........................");
            Console.WriteLine(request);

            //process the request
            string XmlResponse = ProcessRequest(request);

            //return the response
            byte[] buf = Encoding.ASCII.GetBytes(XmlResponse);
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf, 0, buf.Length);

            Console.WriteLine();
            Console.WriteLine(".........................Response to Request......................");
            Console.WriteLine(XmlResponse);
            

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            dh.LogError(ex.Message);
        }
    }

    public string ProcessRequest(string request)
    {
        //log request recieved
        string XmlResponse = "";
        string timeIn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string timeOut = "";
        ConfirmPaymentRequest paymentRequest = null;
        string requestType = null;
        try
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
            //get the type of request sent thru
            requestType = GetRequestType(request);
            
            //some one wants the customers account details and he has given us a phone number
            if (requestType.Equals("GET_FINANCIAL_INFO"))
            {
                //parse the xml request
                GetFinancialInformationRequest queryAccountDetailsRequest = GetQueryAccountDetailsRequest(request);

                if (queryAccountDetailsRequest.IsValidRequest())
                {
                    //query for the customer details using the customer phone number
                    XmlResponse = bll.QueryDetails(queryAccountDetailsRequest);
                }
                else
                {
                    //return error
                    XmlResponse = bll.ReturnGetFriErrorXml(queryAccountDetailsRequest, queryAccountDetailsRequest.StatusDescription);
                }
                timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dh.LogRequest("DSTV", queryAccountDetailsRequest.SmartCardNumber, "[Time In: " + timeIn + "] " + request, "[Time Out: " + timeOut + "] " + XmlResponse);
               
            }

            //mobile to bank
            else if (requestType.Equals("CONFRIM_PAYMENT"))
            {
                //query Account Details
                paymentRequest = GetConfirmPaymentRequest(request);
                ConfirmPaymentResponse paymentResponse = new ConfirmPaymentResponse();

             //   XmlResponse = IsValidTranAmount(paymentRequest);

                if (paymentRequest.IsValidRequest())
                {
                    paymentResponse = bll.ConfirmPayment(paymentRequest);
                    XmlResponse = GetConfirmPaymentXmlResponse(paymentResponse, paymentRequest);
                }
                else
                {
                    paymentRequest.StatusCode = "102";
                    paymentRequest.StatusDescription = "FAILED";
                    XmlResponse = GetConfirmPaymentXmlResponse(paymentResponse, paymentRequest);
                }
                timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dh.LogRequest("DSTV", paymentRequest.VendorTranId, "[Time In: " + timeIn + "] " + request, "[Time Out: " + timeOut + "] " + XmlResponse);
                //dh.LogRequestResponse(paymentRequest.TransactionId, request, XmlResponse, paymentResponse.StatusDescription);
            }

            //request is weird and has problems
            else
            {
                XmlDocument XmlRequest = new XmlDocument();
                XmlRequest.LoadXml(request);
                string tranid = XmlRequest.GetElementsByTagName("transactionid").Item(0).InnerText;
                XmlResponse = requestType.Equals("CONFRIM_PAYMENT") ? OperationNotSupportedYetResponse(tranid) : OperationNotSupportedYetResponse("");

                //XmlResponse = requestType.Equals("CONFRIM_PAYMENT") ? OperationNotSupportedYetResponse(paymentRequest.VendorTranId) : OperationNotSupportedYetResponse("");
                //XmlResponse = OperationNotSupportedYetResponse();
                timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dh.LogRequest("DSTV", "UNKWON REQUEST", "[Time In: " + timeIn + "] " + request, "[Time Out: " + timeOut + "] " + XmlResponse);
            }


        }
        catch (Exception ex)
        {
            dh.LogError("ProcessRequest: "+ex.Message);
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            string tranid = XmlRequest.GetElementsByTagName("transactionid").Item(0).InnerText;
            XmlResponse = requestType.Equals("CONFRIM_PAYMENT") ? OperationNotSupportedYetResponse(tranid) : OperationNotSupportedYetResponse("");
            //XmlResponse = requestType.Equals("CONFRIM_PAYMENT") ? OperationNotSupportedYetResponse(paymentRequest.VendorTranId) : OperationNotSupportedYetResponse("");
            //XmlResponse = OperationNotSupportedYetResponse();
        }
        string whatToLog = Environment.NewLine + "Request Recieved: " + request
            + Environment.NewLine
            + Environment.NewLine
            + "Response Sent at " + timeOut + ": " + XmlResponse
            + Environment.NewLine
            + "------------------------------------------------"
            + Environment.NewLine;
        //log response sent
        log.Info(whatToLog);
        return XmlResponse;

    }
    private ConfirmPaymentRequest GetConfirmPaymentRequest(string requestXml)
    {
        ConfirmPaymentRequest request = new ConfirmPaymentRequest();
        try
        {


            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(requestXml);
            // XmlNodeList PaymentRequestList = XmlRequest.GetElementsByTagName("ns0:paymentrequest");
            XmlNodeList elemList = XmlRequest.GetElementsByTagName("*");
            for (int i = 0; i < elemList.Count; i++)
            {
                string name = elemList[i].Name.ToUpper();
                string value = elemList[i].InnerText;
                if (name == "AMOUNT")
                {
                    request.TransactionAmount = value;
                }
                else if (name == "TRANSACTIONID")
                {
                    request.VendorTranId = value;
                }
                else if (name == "RECEIVINGFRI")
                {
                    request.CustomerRef = GetMeterNumber(value);
                    var Details = dh.GetCustomerDetails(request.CustomerRef, "", "DSTV");
                    request.CustName = Details.CustomerName;
                    request.ThirdPartyTransactionID = DateTime.Now.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + DateTime.Now.Ticks + "");
                }
                else if (name == "message".ToUpper())
                {
                    request.BouquetCode = value;
                }
                else if (name == "ACCOUNTHOLDERID")
                {
                    request.CustomerTel = GetCustomerTel(value);
                }
                else if (name == "SUBSCRIBERNUMBER")
                {
                    request.Subscriber = value;
                }

            }

            if (request.BouquetCode.ToUpper() == "BOX")
            {
                request.BouquetCode = "BO";
            }
            request.Utility = "DSTV";
        }
        catch (Exception ee)
        {
            throw ee;
        }

        return request;
    }
    private string GetMeterNumber(string xml)
    {
        char[] separaters = { '@', '/' };
        string[] parts = xml.Split(separaters);
        return parts[0].Replace("FRI:", "");
    }

    private string OperationNotSupportedYetResponse(string ProcessingNumber)
    {
        string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse " +
                                "xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message>OPERATION_NOT_SUPPORTED_YET</message>" +
                                "<extension/>" +
                                "</ns2:getfinancialresourceinformationresponse>";
        if (!string.IsNullOrEmpty(ProcessingNumber))
        {
            xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
               "<ns0:paymentresponse xmlns:ns0=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
               "   <providertransactionid>" + ProcessingNumber + "</providertransactionid>" +
               "   <message>bouquet code not supplied</message>" +
               "   <status>FAILED</status>" +
               "</ns0:paymentresponse>";
        }

        return xmlResponse;
    }

    private ConfirmPaymentRequest GetConfirmPaymentRequest2(string requestXml)
    {
        ConfirmPaymentRequest ConfirmationRequest = new ConfirmPaymentRequest();


        XmlDocument XmlRequest = new XmlDocument();
        XmlRequest.LoadXml(requestXml);
        XmlNodeList PaymentRequestList = XmlRequest.GetElementsByTagName("ns0:paymentrequest");
        XmlNodeList servicelist = XmlRequest.GetElementsByTagName("serviceId");
        XmlNodeList parameters = XmlRequest.GetElementsByTagName("parameter");
        XmlNodeList parametername = XmlRequest.GetElementsByTagName("name");
        XmlNodeList parametervalue = XmlRequest.GetElementsByTagName("value");
        //XmlNodeList traceUniqueIDlist = XmlRequest.GetElementsByTagName("ns1:traceUniqueID");
        //string traceUniqueID = traceUniqueIDlist[0].InnerXml;
        string serviceId = servicelist[0].InnerXml;
        ConfirmationRequest.ServiceId = serviceId;
        //ConfirmationRequest.TraceUniqueID = traceUniqueID;
        //Console.WriteLine(serviceId);
        int i = 0;
        foreach (XmlNode paramternode in parameters)
        {
            string parameterName = parametername[i].InnerText.Trim();
            string parameterValue = parametervalue[i].InnerText.Trim();
            switch (parameterName)
            {
                case "ProcessingNumber":
                    ConfirmationRequest.VendorTranId = parameterValue;
                    break;

                case "BouquetCode":
                    ConfirmationRequest.BouquetCode = parameterValue;
                    break;

                case "SenderID":
                    ConfirmationRequest.SenderID = parameterValue;
                    break;

                case "AcctRef":
                    ConfirmationRequest.CustomerRef = parameterValue;
                    break;

                case "RequestAmount":
                    ConfirmationRequest.TransactionAmount = parameterValue;
                    break;

                case "PaymentRef":
                    ConfirmationRequest.PaymentRef = parameterValue;
                    break;

                case "ThirdPartyTransactionID":
                    ConfirmationRequest.ThirdPartyTransactionID = parameterValue;
                    break;

                case "MOMAcctNum":
                    ConfirmationRequest.CustomerTel = parameterValue;
                    break;

                case "CustName":
                    ConfirmationRequest.CustName = parameterValue;
                    break;

                case "TXNType":
                    ConfirmationRequest.TXNType = parameterValue;
                    break;

                case "StatusCode":
                    ConfirmationRequest.StatusCode = parameterValue;
                    break;

                case "OpCoID":
                    ConfirmationRequest.OpCoID = parameterValue;
                    break;
            }
            i++;
        }
        ConfirmationRequest.BouquetCode = ConfirmationRequest.PaymentRef;
        if (ConfirmationRequest.BouquetCode.ToUpper() == "BOX")
        {
            ConfirmationRequest.BouquetCode = "BO";
        }
        ConfirmationRequest.Utility = "DSTV";

        //make sure we get a way of identifying the utility beyond the hard coding we are doing below.
        ConfirmationRequest.StatusCode = "0";
        ConfirmationRequest.StatusDescription = "SUCCESS";
        return ConfirmationRequest;
    }

    private string GetQueryAccountDetailsResponse(QueryDSTSVCustomerDetailsResponse queryAccountDetailsResponse, GetFinancialInformationRequest request)
    {
        if (request.StatusCode.Equals("0"))
        {

            string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse " +
                                "xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message></message>" +
                                "<extension>";
            string xmlFooter = " </extension>" +
                               "</ns2:getfinancialresourceinformationresponse>";
            xmlResponse = xmlResponse + xmlFooter;
            return xmlResponse;
        }
        else
        {
            return AccountNotFoundResponse();
        }

    }

    private string AccountNotFoundResponse()
    {
        string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse " +
                                "xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message>ACCOUNT_NOT_FOUND</message>" +
                                "<extension/>" +
                                "</ns2:getfinancialresourceinformationresponse>";
        return xmlResponse;
    }

    private string OperationNotSupportedYetResponse()
    {
        string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse " +
                                "xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message>OPERATION_NOT_SUPPORTED_YET</message>" +
                                "<extension/>" +
                                "</ns2:getfinancialresourceinformationresponse>";
        return xmlResponse;
    }

    private string GeneralErrorResponse()
    {
        string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                             "<message>GENERAL ERROR AT PEGASUS</message>";

        return xmlResponse;
    }

    private string GetRequestType(string requestXml)
    {
        string requestType = "";
        XmlDocument XmlRequest = new XmlDocument();
        XmlRequest.LoadXml(requestXml);
        XmlNodeList GetFinancialInfoList = XmlRequest.GetElementsByTagName("ns0:getfinancialresourceinformationrequest");
        XmlNodeList ConfirmPaymentList = XmlRequest.GetElementsByTagName("ns0:paymentrequest");

        if (GetFinancialInfoList.Count > 0)
        {
            requestType = "GET_FINANCIAL_INFO";
        }
        else if (ConfirmPaymentList.Count > 0)
        {
            requestType = "CONFRIM_PAYMENT";
        }
        else
        {
            requestType = "UNKNOWN";
        }
        return requestType;
    }

    private string GetConfirmPaymentXmlResponse(ConfirmPaymentResponse confirmPaymentRequestResponse, ConfirmPaymentRequest request)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string ProcessingNumber = request.VendorTranId;
        string StatusCode = confirmPaymentRequestResponse.StatusCode;
        string StatusDesc = confirmPaymentRequestResponse.StatusDescription;
        string ThirdPartyAcctRef = confirmPaymentRequestResponse.ThirdPartyAcctRef;
        string Token = confirmPaymentRequestResponse.Token;
        string Confirmrequest = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
           "<ns0:paymentresponse xmlns:ns0=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
           "   <providertransactionid>" + ProcessingNumber + "</providertransactionid>" +
           "   <message>Pending</message>" +
           "   <status>PENDING</status>" +
           "</ns0:paymentresponse>";
        
        //"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\""
       //+ " xmlns:b2b=\"http://b2b.mobilemoney.mtn.zm_v1.0/\">"
       //+ "<soapenv:Header/>"
       //+ "<soapenv:Body>"
       //+ "<b2b:processRequestResponse>"
       //+ "<return>"
       //+ "<name>ProcessingNumber</name>"
       //+ "<value>" + ProcessingNumber + "</value>"
       //+ "</return>"
       //+ "<return>"
       //+ "<name>StatusCode</name>"
       //+ "<value>" + StatusCode + "</value>"
       //+ "</return>"
       //+ "<return>"
       //+ "<name>StatusDesc</name>"
       //+ "<value>" + StatusDesc + "</value>"
       //+ "</return>"
       //+ "<return>"
       //+ "<name>ThirdPartyAcctRef</name>"
       //+ "<value>" + ThirdPartyAcctRef + "</value>"
       //+ "</return> "
       //+ "<return>"
       //+ "<name>Token</name>"
       //+ "<value>" + Token + "</value>"
       //+ "</return>"
       //+ "</b2b:processRequestResponse>"
       //+ "</soapenv:Body>"
       //+ "</soapenv:Envelope>";
        return Confirmrequest;
    }

    //private string IsValidTranAmount(ConfirmPaymentRequest request)
    //{

    //    string xml = "";
    //    if (queryResponse.ResponseField6.Equals("0"))
    //    {

    //        string amountDue = GetAmountDue(queryResponse.ResponseField10, queryResponse.ResponseField4);
    //        string price = (string.IsNullOrEmpty(queryResponse.ResponseField9)) ? "" : queryResponse.ResponseField9.Trim();

    //        if (GetFriRequest.FRIRequestType.ToUpper().Contains("BOX"))
    //        {
    //            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
    //              "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
    //              "<message>sucessfully received message</message>" +
    //              "<extension>" +
    //              "<responsecode>101</responsecode>" +
    //              "<name>" + queryResponse.ResponseField2.Trim().Replace("&", String.Empty).Replace(";", String.Empty).Replace(":", String.Empty) + "</name>" +
    //              "<AmountDue>0 UGX</AmountDue>" +
    //              "<CardNo>" + queryResponse.ResponseField1.Trim() + "</CardNo>" +
    //              "</extension>" +
    //              "</ns2:getfinancialresourceinformationresponse>";
    //        }
    //        else
    //        {
    //            xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
    //              "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
    //              "<message>sucessfully received message</message>" +
    //              "<extension>" +
    //              "<responsecode>101</responsecode>" +
    //              "<name>" + queryResponse.ResponseField2.Trim().Replace("&", String.Empty).Replace(";", String.Empty).Replace(":", String.Empty) + "</name>" +
    //              "<balance>" + queryResponse.ResponseField4.Split('.')[0].Trim() + " UGX</balance>" +
    //              "<CurrentP>" + price + "</CurrentP>" +
    //              "<NewP>" + price + "</NewP>" +
    //              "<AmountDue>" + amountDue.Trim() + " UGX</AmountDue>" +
    //              "<CardNo>" + queryResponse.ResponseField1.Trim() + "</CardNo>" +
    //              "</extension>" +
    //              "</ns2:getfinancialresourceinformationresponse>";
    //        }

    //    }
    //    else
    //    {
    //        xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
    //              "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
    //              "<message>" + queryResponse.ResponseField7 + "</message>" +
    //              "<extension>" +
    //              "<responsecode>102</responsecode>" +
    //              "</extension>" +
    //              "</ns2:getfinancialresourceinformationresponse>";
    //    }
    //    return xml;

    //    return Confirmrequest;
    //}

    private string GetQueryAccountDetailsXml(QueryDSTSVCustomerDetailsResponse Queryresponse)
    {
        string Name = Queryresponse.CustName;
        string BalanceAmount = Queryresponse.BalanceAmount;
        string StatusCode = Queryresponse.StatusCode;
        string ThirdPartyAcctRef = Queryresponse.ThirdPartyAcctRef;
        string StatusDesc = Queryresponse.StatusDescription;
        string response = "";

        response = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
        "<ns2:getfinancialresourceinformationresponse xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
        "<message> " + Name + " ,Account no." + ThirdPartyAcctRef + " ,Min Amount Due." + BalanceAmount + "</message>"
        + "</ns2:getfinancialresourceinformationresponse>";

        return response;
    }



    private GetFinancialInformationRequest GetQueryAccountDetailsRequest(string requestXml)
    {
        GetFinancialInformationRequest request = new GetFinancialInformationRequest();
        XmlDocument XmlRequest = new XmlDocument();
        XmlRequest.LoadXml(requestXml);
        XmlNodeList GetFinancialInfoList = XmlRequest.GetElementsByTagName("ns0:getfinancialresourceinformationrequest");

        //this should loop once
        foreach (XmlNode node in GetFinancialInfoList)
        {
            try
            {
                request.SmartCardNumber = GetSmartCard(node.SelectSingleNode("resource").InnerText);
                request.CustomerTel = GetCustomerTel(node.SelectSingleNode("accountholderid").InnerText);
                request.BouquetCode = GetBouquetCode(node.SelectSingleNode("extension").InnerText);
                request.BouquetCode = bll.GetNewBoquetCode(request.BouquetCode);
                request.FRIRequestType = GetFRIRequestType(node.SelectSingleNode("extension").InnerText);
                string BouquetCode = request.BouquetCode;
                request.UtilityCode = GetUtilityCode(BouquetCode);
                request.StatusCode = "0";
                request.StatusDescription = "SUCCESS";

            }
            catch (Exception e)
            {
                //request.resource = "";
                request.SmartCardNumber = "";
                request.StatusCode = "02";
                request.StatusDescription = "FAILED TO PARSE XML REQUEST";
            }
            break;
        }
        return request;

    }

    private string GetUtilityCode(string BouquetCode)
    {
        if (!string.IsNullOrEmpty(BouquetCode))
        {
            if (BouquetCode.StartsWith("GO"))
            {
                return "GOTV";
            }
            else
            {
                return "DSTV";
            }
        }
        else
        {
            return "DSTV";
        }
    }

    private string GetFRIRequestType(string xml)
    {
        if (xml == "ACT")
        {
            return "ReauthenticateSmartCard";
        }
        else if (xml == "BQT")
        {
            return "ViewBouquetDetails";
        }
        else if (xml == "BOX")
        {
            return "BoxOffice";
        }
        else return "QueryCustomerDetails"; ;
    }

    private string GetBouquetCode(string xml)
    {
        return xml;
    }

    private string GetSmartCard(string xml)
    {
        char[] separaters ={ '@', '/', ':' };
        string[] parts = xml.Split(separaters);
        return parts[1];
    }

    private string GetAccountID(string p)
    {
        return "";
    }

    private string GetCustomerTel(string accountHolderId)
    {
        char[] separaters ={ ':', '/' };
        string[] parts = accountHolderId.Split(separaters);
        return parts[1];
    }


}

