using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data;
using System.Net;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Encryption;
using System.Net.Security;
using System.IO;
using System.Text;
using System.Globalization;
using PegPayUtilityApi.PegPayApi;
using log4net;
using System.Collections.Generic;

[WebService(Namespace = "http://PegPayPaymentsApi/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PegPay : System.Web.Services.WebService
{
    // Define a static logger variable so that it references the name of your class 
    private static readonly ILog log = LogManager.GetLogger(typeof(PegPay));
    DatabaseHandler handler = new DatabaseHandler();
    public PegPay()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string GetServerStatus()
    {
        string serverStatus = "";
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
            serverStatus = pegpay.GetServerStatus();
        }
        catch (Exception ex)
        {

        }
        return serverStatus;
    }

    [WebMethod]
    public Response GetTransactionDetails(QueryRequest query)
    {
        string ip = getRequestIp();
        handler.LogIp(ip, query.QueryField5);
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        Response resp = new Response();
        try
        {
            QueryResponse qResp = pegpay.GetTransactionDetails(query.QueryField10, query.QueryField5, query.QueryField6);
            if (qResp.StatusCode.Equals("0"))
            {

                resp.ResponseField8 = qResp.PegPayQueryId;
                resp.ResponseField6 = qResp.StatusCode;
                resp.ResponseField7 = qResp.StatusDescription;
                resp.ResponseField4 = qResp.CustomerReference;
                resp.ResponseField9 = qResp.NoOfUnits;
            }
            else
            {
                resp.ResponseField6 = qResp.StatusCode;
                resp.ResponseField7 = qResp.StatusDescription;
            }
        }
        catch (Exception ex)
        {
            string VendorCode = query.QueryField5.ToUpper();

            if (VendorIsPrepaidVendor(VendorCode))
            {
                //force them to re invoke get transaction status
                resp.ResponseField6 = "1000";
                resp.ResponseField7 = "PENDING";
                resp.ResponseField13 = "" + ex.Message;
            }
            else
            {
                //post paid vendor
                resp.ResponseField6 = "101";
                resp.ResponseField7 = "GENERAL ERROR AT PEGPAY";
                resp.ResponseField13 = "" + ex.Message;
            }
        }

        string whatToLog = "Request Recieved: " + Environment.NewLine + query.ToString() +
                          Environment.NewLine +
                          "Response Sent: " + resp.ToString() +
                          "------------------------------------------------" +
                          Environment.NewLine;
        //log.Info(whatToLog);
        return resp;
    }

    private bool VendorIsPrepaidVendor(string VendorCode)
    {
        if (VendorCode == "ISYS")
        {
            return true;
        }
        else if (VendorCode == "MICROPAY")
        {
            return true;
        }
        else if (VendorCode == "EGORA")
        {
            return true;
        }
        else if (VendorCode == "CHAPCHAP")
        {
            return true;
        }
        else if (VendorCode == "INTERSWITCH")
        {
            return true;
        }
        else if (VendorCode == "NOVOPLAT")
        {
            return true;
        }
        else if (VendorCode == "PEGASUS_TEST")
        {
            return true;
        }
        else if (VendorCode == "PEAKBW")
        {
            return true;
        }
        else if (VendorCode == "PESACHOICE")
        {
            return true;
        }
        else if (VendorCode == "REDCORE")
        {
            return true;
        }
        else if (VendorCode == "NCBANK_PRE")
        {
            return true;
        }
        return false;
    }


    [WebMethod]
    public Response GetPrepaidVendorDetails(QueryRequest query)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        Response resp = new Response();
        try
        {
            QueryResponse qResp = pegpay.GetPrepaidVendorDetails(query.QueryField5, query.QueryField6);

            if (qResp.StatusCode.Equals("0"))
            {
                resp.ResponseField8 = qResp.PegPayQueryId;
                resp.ResponseField6 = qResp.StatusCode;
                resp.ResponseField7 = qResp.StatusDescription;
                resp.ResponseField4 = qResp.CustomerReference;
            }
            else
            {
                resp.ResponseField6 = qResp.StatusCode;
                resp.ResponseField7 = qResp.StatusDescription;
            }
        }
        catch (Exception ex)
        {
            resp.ResponseField6 = "101";
            resp.ResponseField7 = "GENERAL ERROR AT PEGPAY";
            resp.ResponseField13 = "" + ex.Message;
        }

        string whatToLog = "Request Recieved: " + Environment.NewLine + query.ToString() +
                          Environment.NewLine +
                          "Response Sent: " + resp.ToString() +
                          "------------------------------------------------" +
                          Environment.NewLine;
        //log.Info(whatToLog);
        return resp;
    }
    private string getRequestIp()
    {
        string _custIP = null;
        try
        {

            _custIP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];

            if (String.IsNullOrEmpty(_custIP))
            {
                _custIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            }
            if (String.IsNullOrEmpty(_custIP))
            {
                _custIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }
        catch (Exception ex)
        {
            _custIP = "";
        }
        return _custIP;
    }
    [WebMethod]
    public Response QueryCustomerDetails(QueryRequest query)
    {
        string ip = getRequestIp();
        //handler.LogIp(ip, query.QueryField5);

        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        Response queryResponse = new Response();
        try
        {

            if (query.QueryField4 == null)
            {
                queryResponse.ResponseField1 = "";
                queryResponse.ResponseField2 = "";
                queryResponse.ResponseField3 = "";
                queryResponse.ResponseField4 = "";
                queryResponse.ResponseField6 = "103";
                queryResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else if (query.QueryField4.Trim().Equals(""))
            {
                queryResponse.ResponseField1 = "";
                queryResponse.ResponseField2 = "";
                queryResponse.ResponseField3 = "";
                queryResponse.ResponseField4 = "";
                queryResponse.ResponseField6 = "103";
                queryResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else
            {
                if (query.QueryField4.Equals("TUCKSEE"))
                {
                    //URAQueryResponse uraResp = pegpay.QueryURACustomerDetails(query.QueryField1, query.QueryField2, query.QueryField5, query.QueryField6);
                    KCCAResponse uraResp = pegpay.QueryTuckSeeCustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (uraResp.ErrorCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = uraResp.PaymentReference;
                        queryResponse.ResponseField2 = uraResp.CustomerName;
                        queryResponse.ResponseField3 = uraResp.CustomerPhone;
                        queryResponse.ResponseField4 = uraResp.PaymentAmount;
                        queryResponse.ResponseField6 = uraResp.ErrorCode;
                        queryResponse.ResponseField7 = uraResp.ErrorDescription;
                        queryResponse.ResponseField8 = uraResp.PaymentDate;
                        queryResponse.ResponseField9 = uraResp.ExpiryDate;
                        queryResponse.ResponseField10 = uraResp.AllowPartialPayment;
                        queryResponse.ResponseField11 = uraResp.TpgoReference;
                        queryResponse.ResponseField12 = uraResp.SystemsName;
                        queryResponse.ResponseField13 = uraResp.SystemCode;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = query.QueryField1;
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField3 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = uraResp.ErrorCode;
                        queryResponse.ResponseField7 = uraResp.ErrorDescription;
                        try
                        {
                            queryResponse.ResponseField1 = uraResp.PaymentReference;
                            queryResponse.ResponseField2 = uraResp.CustomerName;
                            queryResponse.ResponseField4 = uraResp.PaymentAmount;
                        }
                        catch (Exception ee)
                        {
                        }
                    }
                }
                else if (query.QueryField4.ToUpper().Equals("SOLAR"))
                {


                    string utilitycode = query.QueryField4;
                    string meternumber = query.QueryField1;
                    string vendorCode = query.QueryField5;
                    string password = query.QueryField6;

                    SolarCustomer customer = pegpay.ValidateSolarCustomer(utilitycode, meternumber, vendorCode, password);
                    if (customer.StatusCode == "0")
                    {
                        queryResponse.ResponseField1 = customer.CustomerRef;
                        queryResponse.ResponseField2 = customer.CustomerName;
                        queryResponse.ResponseField3 = customer.Usergroup;
                        queryResponse.ResponseField4 = customer.Balance;
                        queryResponse.ResponseField9 = customer.TIN;
                        queryResponse.ResponseField10 = customer.CustomerType;
                        queryResponse.ResponseField11 = "" + customer.IsRegistered;
                        queryResponse.ResponseField6 = customer.StatusCode;
                        queryResponse.ResponseField7 = customer.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField3 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = customer.StatusCode;
                        queryResponse.ResponseField7 = customer.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("NWSC"))
                {
                    NWSCQueryResponse nwscResp = pegpay.QueryNWSCCustomerDetails(query.QueryField1, query.QueryField2, query.QueryField5, query.QueryField6);
                    if (nwscResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = nwscResp.CustomerReference;
                        queryResponse.ResponseField2 = nwscResp.CustomerName;
                        queryResponse.ResponseField3 = nwscResp.Area;
                        queryResponse.ResponseField4 = nwscResp.OutstandingBalance;
                        queryResponse.ResponseField6 = nwscResp.StatusCode;
                        queryResponse.ResponseField7 = nwscResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField3 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = nwscResp.StatusCode;
                        queryResponse.ResponseField7 = nwscResp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("URA"))
                {
                    URAQueryResponse uraResp = pegpay.QueryURACustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (uraResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = uraResp.CustomerReference;
                        queryResponse.ResponseField2 = uraResp.CustomerName;
                        queryResponse.ResponseField3 = uraResp.TIN;
                        queryResponse.ResponseField4 = uraResp.OutstandingBalance;
                        queryResponse.ResponseField6 = uraResp.StatusCode;
                        queryResponse.ResponseField7 = uraResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField3 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = uraResp.StatusCode;
                        queryResponse.ResponseField7 = uraResp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("STARTIMES"))
                {
                    DSTVQueryResponse response = pegpay.QueryStartTimesCustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (response.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = response.CustomerReference;
                        queryResponse.ResponseField2 = response.CustomerName;
                        queryResponse.ResponseField4 = response.CustomerBalance;
                        queryResponse.ResponseField6 = response.StatusCode;
                        queryResponse.ResponseField5 = response.CustomerType;
                        queryResponse.ResponseField7 = response.StatusDescription;
                        queryResponse.ResponseField8 = response.CustType;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = query.QueryField1;
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = response.StatusCode;
                        queryResponse.ResponseField7 = response.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("UMEME"))
                {
                    UmemeQueryResponse umemeResp = pegpay.QueryUmemeCustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (umemeResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = umemeResp.CustomerReference;
                        queryResponse.ResponseField2 = umemeResp.CustomerName;
                        queryResponse.ResponseField4 = umemeResp.OutstandingBalance;
                        queryResponse.ResponseField5 = umemeResp.CustomerType;
                        queryResponse.ResponseField6 = umemeResp.StatusCode;
                        queryResponse.ResponseField7 = umemeResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = umemeResp.StatusCode;
                        queryResponse.ResponseField7 = umemeResp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("WENRECO"))
                {
                    QueryResponse resp = pegpay.VerifyWenrecoCustomer(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (resp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = resp.CustomerReference;
                        queryResponse.ResponseField2 = resp.CustomerName;
                        queryResponse.ResponseField4 = resp.CustomerBalance;
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField6 = resp.StatusCode;
                        queryResponse.ResponseField7 = resp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = resp.StatusCode;
                        queryResponse.ResponseField7 = resp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("NSSF"))
                {
                    NSSFQueryResponse NssfResp = pegpay.QueryNSSFCustomerDetails(query.QueryField4, query.QueryField1, query.QueryField5, query.QueryField6);
                    if (NssfResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = NssfResp.EmployerNo;
                        queryResponse.ResponseField2 = NssfResp.CustomerReference;
                        queryResponse.ResponseField4 = NssfResp.OutstandingBalance;
                        queryResponse.ResponseField8 = NssfResp.paymentMethod;
                        queryResponse.ResponseField9 = NssfResp.Employer;
                        queryResponse.ResponseField6 = "0";
                        queryResponse.ResponseField7 = NssfResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = NssfResp.StatusCode;
                        queryResponse.ResponseField7 = NssfResp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("MUK") || query.QueryField4.Equals("MUBS") || query.QueryField4.Equals("MUST") || query.QueryField4.Equals("KYU") || query.QueryField4.Equals("UMU") || query.QueryField4.Equals("MAK"))
                {
                    SchoolsQueryResponse kccaResp = pegpay.QuerySchoolStudentDetails(query.QueryField4, query.QueryField1, query.QueryField5, query.QueryField6);
                    if (kccaResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = kccaResp.CustomerReference;
                        queryResponse.ResponseField2 = kccaResp.CustomerName;
                        queryResponse.ResponseField4 = kccaResp.OutstandingBalance;
                        queryResponse.ResponseField5 = kccaResp.CustomerType;
                        queryResponse.ResponseField6 = kccaResp.StatusCode;
                        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = kccaResp.StatusCode;
                        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("STB_SCHOOL") || query.QueryField4.Equals("FLEXIPAY"))
                {


                    string studentNumber = query.QueryField1;
                    string schoolCode = query.QueryField2;
                    string vendorCode = query.QueryField5;
                    string password = query.QueryField6;
                    SchoolsQueryResponse resp = pegpay.ValidateStudentDetails(schoolCode, studentNumber, vendorCode, password);

                    queryResponse.ResponseField6 = resp.StatusCode;
                    queryResponse.ResponseField7 = resp.StatusDescription;
                    if (resp.StatusCode == "0")
                    {
                        //queryResponse.ResponseField2 = resp.CustomerName;
                        //queryResponse.ResponseField1 = resp.CustomerReference;
                        //queryResponse.ResponseField3 = resp.School;
                        //queryResponse.ResponseField5 = resp.Level;
                        //queryResponse.ResponseField4 = resp.Amount;
                        //queryResponse.ResponseField8 = resp.AccountNumber;
                        //queryResponse.ResponseField9 = resp.OutstandingBalance;

                        queryResponse.ResponseField2 = resp.CustomerName;
                        queryResponse.ResponseField1 = resp.CustomerReference;
                        queryResponse.ResponseField3 = resp.School;
                        queryResponse.ResponseField5 = resp.Level;
                        queryResponse.ResponseField4 = resp.Amount;
                        queryResponse.ResponseField8 = resp.AccountNumber;
                        queryResponse.ResponseField9 = resp.OutstandingBalance;
                        queryResponse.ResponseField10 = resp.BankCode;

                        queryResponse.ResponseField11 = resp.AllowPartialPayment;
                        queryResponse.ResponseField12 = resp.MinimumPayment;
                        queryResponse.ResponseField13 = resp.TranCharge;
                        queryResponse.ResponseField14 = resp.ClassName;
                    }
                    

                }
                else if (query.QueryField4.ToUpper().Equals("DSTV") || query.QueryField4.ToUpper().Equals("GOTV"))
                {
                    DSTVQueryResponse response = pegpay.QueryDSTVCustomerDetails(query.QueryField1, query.QueryField4, query.QueryField2, query.QueryField5, query.QueryField6);
                    if (response.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = response.CustomerReference;
                        queryResponse.ResponseField2 = response.CustomerName;
                        queryResponse.ResponseField3 = response.Area;
                        queryResponse.ResponseField4 = response.OutstandingBalance;
                        queryResponse.ResponseField5 = response.CustomerType;
                        queryResponse.ResponseField6 = response.StatusCode;
                        queryResponse.ResponseField7 = response.StatusDescription;
                        queryResponse.ResponseField8 = response.bouquetDetails.BouquetCode;
                        queryResponse.ResponseField9 = response.bouquetDetails.BouquetName;
                        queryResponse.ResponseField10 = response.bouquetDetails.BouquetPrice;
                        queryResponse.ResponseField11 = response.CurrentBouquet;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = response.StatusCode;
                        queryResponse.ResponseField7 = response.StatusDescription;
                    }
                }
                else if (query.QueryField4.Equals("KCCA"))
                {
                    KCCAQueryResponse kccaResp = pegpay.QueryKCCACustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                    if (kccaResp.StatusCode.Equals("0"))
                    {
                        queryResponse.ResponseField1 = kccaResp.PaymentReference;
                        queryResponse.ResponseField2 = kccaResp.CustomerName;
                        queryResponse.ResponseField4 = kccaResp.PaymentAmount;
                        queryResponse.ResponseField8 = "";
                        queryResponse.ResponseField9 = "";
                        queryResponse.ResponseField6 = "0";
                        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                    }
                    else
                    {
                        queryResponse.ResponseField1 = "";
                        queryResponse.ResponseField2 = "";
                        queryResponse.ResponseField5 = "";
                        queryResponse.ResponseField4 = "";
                        queryResponse.ResponseField6 = kccaResp.StatusCode;
                        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                    }
                }
                //else if (query.QueryField4.Equals("NEWKCCA"))
                //{
                //    query.QueryField4 = "KCCA";
                //    KCCAQueryResponse kccaResp = pegpay.NewQueryKCCACustomerDetails(query.QueryField1, query.QueryField5, query.QueryField6);
                //    if (kccaResp.StatusCode.Equals("0"))
                //    {
                //        //queryResponse.ResponseField1 = kccaResp.CustomerReference;
                //        queryResponse.ResponseField1 = kccaResp.PaymentReference;
                //        queryResponse.ResponseField2 = kccaResp.CustomerName;
                //        //queryResponse.ResponseField4 = kccaResp.OutstandingBalance;
                //        queryResponse.ResponseField4 = kccaResp.PaymentAmount;
                //        //queryResponse.ResponseField5 = kccaResp.CustomerType;
                //        queryResponse.ResponseField5 = kccaResp.PaymentType;
                //        queryResponse.ResponseField6 = kccaResp.StatusCode;
                //        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                //        queryResponse.ResponseField10 = kccaResp.sessionKey;
                //    }
                //    else
                //    {
                //        queryResponse.ResponseField1 = "";
                //        queryResponse.ResponseField2 = "";
                //        queryResponse.ResponseField5 = "";
                //        queryResponse.ResponseField4 = "";
                //        queryResponse.ResponseField6 = kccaResp.StatusCode;
                //        queryResponse.ResponseField7 = kccaResp.StatusDescription;
                //    }
                //}
                else
                {
                    queryResponse.ResponseField1 = "";
                    queryResponse.ResponseField2 = "";
                    queryResponse.ResponseField5 = "";
                    queryResponse.ResponseField4 = "";
                    queryResponse.ResponseField6 = "100";
                    queryResponse.ResponseField7 = "UTILITY " + query.QueryField4.ToString() + " NOT SUPPORTED BY PEGPAY AT THE MOMENT";
                }
            }
        }
        catch (WebException ex)
        {
            queryResponse.ResponseField1 = "";
            queryResponse.ResponseField2 = "";
            queryResponse.ResponseField5 = "";
            queryResponse.ResponseField4 = "";
            queryResponse.ResponseField6 = "102";
            queryResponse.ResponseField7 = "CONNECTIVITY ERROR AT " + query.QueryField4;
            queryResponse.ResponseField13 = "" + ex.Message;
        }
        catch (Exception ex)
        {
            queryResponse.ResponseField1 = "";
            queryResponse.ResponseField2 = "";
            queryResponse.ResponseField5 = "";
            queryResponse.ResponseField4 = "";
            queryResponse.ResponseField6 = "101";
            queryResponse.ResponseField7 = "GENERAL ERROR AT PEGPAY";
            queryResponse.ResponseField13 = "" + ex.Message;
        }

        string whatToLog = "Request Recieved: " + Environment.NewLine + query.ToString() +
                           Environment.NewLine +
                           "Response Sent: " + queryResponse.ToString() +
                           "------------------------------------------------" +
                           Environment.NewLine;
        //log.Info(whatToLog);

        return queryResponse;
    }

    [WebMethod]
    public Response QueryLastFiveDetails(QueryRequest query)
    {
        Response resp = new Response();
        try
        {

            if (query.QueryField5 == "MTN")
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();

                UmemeQueryResponse respo = pegpay.GetLastRegisteredTransaction(query.QueryField1, query.QueryField5, query.QueryField6);
                if (respo.StatusCode == "0")
                {
                    List<Transaction> respList = new List<Transaction>();
                    foreach (PegPayUtilityApi.PegPayApi.Transaction tran in respo.TranList)
                    {
                        Transaction tranResp = new Transaction();
                        //trand.transactionID = drow["VendorTranId"].ToString();
                        //trand.CustName = drow["CustomerName"].ToString();
                        //trand.CustomerTel = drow["CustomerTel"].ToString();
                        //trand.VendorTransactionRef = drow["UtilityTranRef"].ToString(); //
                        //trand.Tin = drow["Reason"].ToString();  // units
                        //trand.PaymentDate = drow["RecordDate"].ToString();
                        //trand.VendorCode = drow["Status"].ToString();

                        tranResp.CustName = tran.CustName; // Name
                        tranResp.CustomerTel = tran.CustomerTel; // Phone
                        tranResp.VendorTransactionRef = tran.VendorTransactionRef; // Token
                        tranResp.Tin = tran.Tin; //Unitis
                        tranResp.CustRef = tran.CustRef;
                        tranResp.PaymentDate = tran.PaymentDate; // PaymentDate
                        tranResp.VendorCode = tran.VendorCode;// status
                        tranResp.transactionID = tran.transactionID;

                        respList.Add(tranResp);
                    }
                    resp.TranList = respList;
                    resp.ResponseField6 = "0";
                    resp.ResponseField7 = "SUCCESS";
                }
                else
                {
                    resp.ResponseField5 = "100";
                    resp.ResponseField6 = respo.StatusDescription;
                }
            }
        }
        catch (Exception ee)
        {
            resp.ResponseField6 = "101";
            resp.ResponseField7 = "GENERAL ERROR AT PEGPAY";
        }
        return resp;
    }


    [WebMethod]
    public Response QuerySchoolDetails(QueryRequest query)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        Response queryResponse = new Response();
        try
        {
            SchoolsQueryResponse resp = pegpay.ValidateSchoolCode(query.QueryField3);
            if (resp.StatusCode == "0")
                queryResponse.ResponseField3 = resp.School;

            queryResponse.ResponseField1 = resp.StatusCode;
            queryResponse.ResponseField2 = resp.StatusDescription;
        }
        catch (Exception ee)
        {
            queryResponse.ResponseField1 = "100";
            queryResponse.ResponseField2 = ee.Message.ToUpper();
        }
        return queryResponse;


    }
    [WebMethod]
    public Response PostSchoolsTransaction(TransactionRequest trans)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        pegpay.Timeout = 120000;
        Response postResponse = new Response();
        try
        {
            if (trans.PostField4 == null)
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else if (trans.PostField4.Trim().Equals(""))
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else if (trans.PostField9.Equals("FLEXIPAY"))
            {
                schoolsTransaction schoolsTrans = GetSchoolsFeesTransaction(trans);
                SchoolsPostResponse resp = pegpay.PostSchoolFeesTransaction(schoolsTrans);
                if (resp.StatusCode.Equals("0"))
                {
                    postResponse.ResponseField6 = resp.StatusCode;
                    postResponse.ResponseField7 = resp.StatusDescription;
                    postResponse.ResponseField8 = resp.PegPayPostId;
                    postResponse.ResponseField9 = resp.ReceiptNumber;
                }
                else
                {
                    postResponse.ResponseField6 = resp.StatusCode;
                    postResponse.ResponseField7 = resp.StatusDescription;
                    postResponse.ResponseField8 = "";
                    postResponse.ResponseField9 = "";
                }

            }
            else
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "Invalid request";
            }

        }
        catch (WebException ex)
        {
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "102";
            postResponse.ResponseField7 = "CONNECTIVITY ERROR AT " + trans.PostField4;
            postResponse.ResponseField13 = "" + ex.Message;
        }
        catch (Exception ex)
        {
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "101";
            postResponse.ResponseField7 = "GENERAL ERROR AT PEGPAY";
            postResponse.ResponseField13 = "" + ex.Message;
        }
        string whatToLog = "Request Recieved: " + Environment.NewLine + trans.ToString() +
                          Environment.NewLine +
                          "Response Sent: " + postResponse.ToString() +
                          "------------------------------------------------" +
                          Environment.NewLine;
        //log.Info(whatToLog);
        return postResponse;
    }

    [WebMethod]
    public Response PostTransaction(TransactionRequest trans)
    {
        string ip = getRequestIp();
        handler.LogIp(ip, trans.PostField9);

        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        pegpay.Timeout = 120000;
        Response postResponse = new Response();
        try
        {
            if (trans.PostField4 == null)
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else if (trans.PostField4.Trim().Equals(""))
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else
            {
                if (trans.PostField4.Equals("NWSC"))
                {
                    NWSCTransaction nwscTrans = GetNWSCTrans(trans);
                    PostResponse resp = pegpay.MakeNWSCPayment(nwscTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else if (trans.PostField4.Equals("URA"))
                {
                    URATransaction uraTrans = GetURATrans(trans);
                    PostResponse resp = pegpay.MakeURAPayment(uraTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else if (trans.PostField4.Equals("UMEME"))
                {
                    if (VendorHasBlacklistedAccounts(trans.PostField9) && trans.PostField21.Equals("PREPAID"))
                    {
                        UmemeTransaction umemeTrans = GetUmemeTrans(trans);
                        Token resp = pegpay.MakeYakaPayment(umemeTrans);
                        if (resp.StatusCode.Equals("0"))
                        {
                            postResponse.ResponseField6 = resp.StatusCode;
                            postResponse.ResponseField7 = resp.StatusDescription;
                            postResponse.ResponseField8 = resp.PegPayPostId;
                            postResponse.ResponseField9 = resp.ReceiptNumber;
                        }
                        else
                        {
                            postResponse.ResponseField6 = resp.StatusCode;
                            postResponse.ResponseField7 = resp.StatusDescription;
                            postResponse.ResponseField8 = "";
                            postResponse.ResponseField9 = "";
                        }
                    }
                    else
                    {
                        UmemeTransaction umemeTrans = GetUmemeTrans(trans);
                        UmemePostResponse resp = pegpay.MakeUmemePayment(umemeTrans);
                        if (resp.StatusCode.Equals("0"))
                        {
                            postResponse.ResponseField6 = resp.StatusCode;
                            postResponse.ResponseField7 = resp.StatusDescription;
                            postResponse.ResponseField8 = resp.PegPayPostId;
                            postResponse.ResponseField9 = resp.ReceiptNumber;
                        }
                        else
                        {
                            postResponse.ResponseField6 = resp.StatusCode;
                            postResponse.ResponseField7 = resp.StatusDescription;
                            postResponse.ResponseField8 = "";
                            postResponse.ResponseField9 = "";
                        }
                    }
                }
                else if (trans.PostField4.ToUpper().Equals("WENRECO"))
                {
                    Transaction tr = GetSolarOneTransObj(trans);
                    PostResponse resp = pegpay.MakeWenrecoPayment(tr);
                    if (resp.StatusCode == "0")
                    {
                        postResponse.ResponseField8 = resp.PegPayPostId;
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                    }
                    else
                    {
                        postResponse.ResponseField8 = "";
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                    }

                }
                else if (trans.PostField4.Equals("STB_SCHOOL") || trans.PostField4.Equals("FLEXIPAY"))
                {
                    schoolsTransaction schoolsTrans = GetSchoolsFeesTransaction(trans);
                    SchoolsPostResponse resp = pegpay.PostSchoolFeesTransaction(schoolsTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                        postResponse.ResponseField9 = resp.ReceiptNumber;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                        postResponse.ResponseField9 = "";
                    }

                }
                else if (trans.PostField4.Equals("TUCKSEE"))
                {
                    KCCATransaction uraTrans = GetKCCATrans(trans);
                    KCCAResponse resp = pegpay.MakeTuckSeePayment(uraTrans);
                    if (resp.ErrorCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.ErrorCode;
                        postResponse.ResponseField7 = resp.ErrorDescription;
                        postResponse.ResponseField8 = resp.TransactionID;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.ErrorCode;
                        postResponse.ResponseField7 = resp.ErrorDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else if (trans.PostField4.Equals("STARTIMES"))
                {
                    Transaction starTrans = GetSolarOneTransObj(trans);
                    PostResponse resp = pegpay.MakeStartTimesPayment(starTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else if (trans.PostField4.ToUpper().Equals("DSTV") || trans.PostField4.ToUpper().Equals("DSTV BO") || trans.PostField4.ToUpper().Equals("GOTV"))
                {
                    DSTVTransaction dstvTtrans = GetDSTVTrans(trans);
                    PostResponse resp = pegpay.MakeDSTVPayment(dstvTtrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else if (trans.PostField4.Equals("SOLAR"))
                {
                    Transaction tr = GetSolarOneTransObj(trans);
                    TransactionResponse resp = pegpay.MakeSolarPayment(tr);
                    if (resp.ErrorCode == "0")
                    {
                        postResponse.ResponseField8 = resp.ReceiptNumber;
                        postResponse.ResponseField6 = resp.ErrorCode;
                        postResponse.ResponseField7 = resp.ErrorDescription;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.ErrorCode;
                        postResponse.ResponseField7 = resp.ErrorDescription;
                    }


                }
                else if (trans.PostField4.Equals("KCCA"))
                {
                    KCCATransaction kccaTrans = GetKCCATrans(trans);
                    KCCAPostResponse resp = pegpay.MakeKCCAPayment(kccaTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                        postResponse.ResponseField9 = resp.ReceiptNumber;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                        postResponse.ResponseField9 = "";
                    }
                }
                else if (trans.PostField4.Equals("NEWKCCA"))
                {
                    trans.PostField4 = "KCCA";
                    KCCATransaction kccaTrans = GetKCCATrans(trans);
                    KCCAPostResponse resp = pegpay.MakeKCCAPayment(kccaTrans);
                    //KCCAPostResponse resp = pegpay.NewMakeKCCAPayment(kccaTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                        postResponse.ResponseField9 = resp.ReceiptNumber;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                        postResponse.ResponseField9 = "";
                    }
                }

                else if (trans.PostField4.Equals("MUBS") || trans.PostField4.Equals("MAK") || trans.PostField4.Equals("KYU"))
                {
                    schoolsTransaction schoolsTrans = GetSchoolsTrans(trans);
                    SchoolsPostResponse resp = pegpay.MakeSchoolFeesPayment(schoolsTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                        postResponse.ResponseField9 = resp.ReceiptNumber;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                        postResponse.ResponseField9 = "";
                    }
                }
                else if (trans.PostField4.Equals("BETWAY")
                  || trans.PostField4.Equals("FORTUNEBET")
                  || trans.PostField4.Equals("MIXAKIDS")
                  || trans.PostField4.Equals("ZAKAT")
                  || trans.PostField4.Equals("WAQF")
                  || trans.PostField4.Equals("SADAQ")
                  || trans.PostField4.Equals("SUNKING")
                  || trans.PostField4.Equals("MAZIMARBS")
                  || trans.PostField4.Equals("TOPBET")
                  || IsGoodsAndServicesUtility(trans.PostField4))//future
                {
                    BETWAYTransaction betwayTrans = GetBETWAYTrans(trans);
                    PostResponse resp = pegpay.MakeBETWAYPayment(betwayTrans);
                    if (resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else
                {
                    postResponse.ResponseField1 = "";
                    postResponse.ResponseField2 = "";
                    postResponse.ResponseField5 = "";
                    postResponse.ResponseField4 = "";
                    postResponse.ResponseField6 = "100";
                    postResponse.ResponseField7 = "UTILITY " + trans.PostField4.ToString() + " NOT SUPPORTED BY PEGPAY AT THE MOMENT";
                }
            }
        }
        catch (WebException ex)
        {
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "102";
            postResponse.ResponseField7 = "CONNECTIVITY ERROR AT " + trans.PostField4;
            postResponse.ResponseField13 = "" + ex.Message;
        }
        catch (Exception ex)
        {
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "101";
            postResponse.ResponseField7 = "GENERAL ERROR AT PEGPAY";
            postResponse.ResponseField13 = "" + ex.Message;
        }
        string whatToLog = "Request Recieved: " + Environment.NewLine + trans.ToString() +
                          Environment.NewLine +
                          "Response Sent: " + postResponse.ToString() +
                          "------------------------------------------------" +
                          Environment.NewLine;
        //log.Info(whatToLog);
        return postResponse;
    }

    public bool IsGoodsAndServicesUtility(string utilitycode)
    {
        DatabaseHandler dh = new DatabaseHandler();
        List<String> goodsAndServices = dh.GetUtilities("GOODS&SERVICES");
        if (goodsAndServices.Contains(utilitycode))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private Transaction GetSolarOneTransObj(TransactionRequest trans)
    {
        Transaction transaction = new Transaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.UtilityCode = trans.PostField4;
        transaction.CustomerType = trans.PostField21;
        transaction.Tin = trans.PostField22;
        transaction.Area = trans.PostField3;
        transaction.PaymentDate = trans.PostField5;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;

        return transaction;
    }


    private URATransaction GetURATrans(TransactionRequest transaction)
    {
        URATransaction trans = new URATransaction();
        trans.BankCode = transaction.PostField9;
        trans.BranchCode = transaction.PostField23;
        trans.ChequeNumber = transaction.PostField17;
        trans.CustName = transaction.PostField2;
        trans.CustomerTel = transaction.PostField11;
        trans.CustRef = transaction.PostField1;
        trans.PaymentDate = transaction.PostField5;
        trans.Reversal = transaction.PostField12;
        trans.Status = transaction.PostField25;
        trans.Teller = transaction.PostField14;
        trans.TIN = transaction.PostField3;
        trans.TransactionAmount = transaction.PostField7;
        trans.transactionID = transaction.PostField13;
        trans.TransactionType = transaction.PostField8;
        trans.VendorTransactionRef = transaction.PostField20;
        trans.Password = transaction.PostField10;
        trans.VendorCode = transaction.PostField9;
        trans.DigitalSignature = transaction.PostField16;//trans.VendorCode + trans.TransactionType + trans.TransactionAmount + trans.CustName;

        return trans;
    }



    [WebMethod]
    public BouquetDetails[] GetPayTVBouquetDetails(QueryRequest query)
    {
        BouquetDetails[] allBouquets = { };
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
            pegpay.Timeout = 120000;
            allBouquets = pegpay.GetBouquetDetails(query.QueryField1, query.QueryField4, query.QueryField5, query.QueryField6);
        }
        catch (Exception ex)
        {
            BouquetDetails bouq = new BouquetDetails();
            bouq.StatusCode = "101";
            bouq.StatusDescription = "GENERAL ERROR AT PEGASUS";
            List<BouquetDetails> allB = new List<BouquetDetails>();
            allB.Add(bouq);
            allBouquets = allB.ToArray();
        }

        //log request resp
        string whatToLog = "BouquetDetailsRequest Recieved: " + Environment.NewLine + query.ToString() +
                           Environment.NewLine +
                           "BouquetDetailsResponse Sent: " + Environment.NewLine +
                           GetBouquetDetailsResponse(allBouquets) +
                           "------------------------------------------------" +
                           Environment.NewLine;
        //log.Info(whatToLog);
        return allBouquets;
    }

    private string GetBouquetDetailsResponse(BouquetDetails[] allBouquets)
    {
        string whatToLog = "";
        foreach (BouquetDetails bouq in allBouquets)
        {
            whatToLog = whatToLog + "BouquetName:" +
                        bouq.BouquetName + " BouquetCode:" +
                        bouq.BouquetCode + " Price:" +
                        bouq.BouquetPrice + " PayTV:" +
                        bouq.PayTvCode;
        }
        return whatToLog;
    }
    [WebMethod]
    public Response ReversePrepaidTransaction(TransactionRequest trans)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
        pegpay.Timeout = 120000;
        Response resp = new Response();
        try
        {
            if (string.IsNullOrEmpty(trans.PostField4))
            {
                resp.ResponseField1 = "";
                resp.ResponseField2 = "";
                resp.ResponseField5 = "";
                resp.ResponseField4 = "";
                resp.ResponseField6 = "103";
                resp.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else
            {
                if (trans.PostField4.Equals("NWSC") || trans.PostField4.Equals("UMEME"))
                {
                    ReversalRequest request = GetReversalRequest(trans);
                    ReversalResponse resps = pegpay.ReversePrepaidTransaction(request);
                    if (resps.StatusCode.Equals("0"))
                    {
                        resp.ResponseField6 = resps.StatusCode;
                        resp.ResponseField7 = resps.StatusDescription;
                        resp.ResponseField8 = resps.ReversalID;
                    }
                    else if (resps.StatusCode.Equals("1000"))
                    {
                        resp.ResponseField6 = resps.StatusCode;
                        resp.ResponseField7 = resps.StatusDescription;
                        resp.ResponseField8 = resps.ReversalID;
                    }
                    else
                    {
                        resp.ResponseField6 = resps.StatusCode;
                        resp.ResponseField7 = resps.StatusDescription;
                        resp.ResponseField8 = "";
                    }
                }
                else
                {
                    resp.ResponseField1 = "";
                    resp.ResponseField2 = "";
                    resp.ResponseField5 = "";
                    resp.ResponseField4 = "";
                    resp.ResponseField6 = "103";
                    resp.ResponseField7 = "REVERSALS FOR THIS UTILITY ARE NOT SUPPORTED";
                }
            }
        }
        catch (Exception)
        {
            resp.ResponseField1 = "";
            resp.ResponseField2 = "";
            resp.ResponseField5 = "";
            resp.ResponseField4 = "";
            resp.ResponseField6 = "101";
            resp.ResponseField7 = "GENERAL ERROR AT PEGPAY";
        }
        return resp;
    }

    private ReversalRequest GetReversalRequest(TransactionRequest trans)
    {
        ReversalRequest request = new ReversalRequest();
        //request.TransactionType = trans.PostField8;
        request.VendorCode = trans.PostField9;
        request.Password = trans.PostField10;
        //request.Reversal = "1";
        request.OriginalTransactionId = trans.PostField13;
        request.DigitalSignature = trans.PostField16;
        request.Reason = trans.PostField18;
        request.ReversalTransactionId = trans.PostField20;

        return request;
    }
    [WebMethod]
    public Response ReactivatePayTvCard(QueryRequest query)
    {
        Response queryResp = new Response();
        try
        {
            PegPayUtilityApi.PegPayApi.PegPay api = new PegPayUtilityApi.PegPayApi.PegPay();
            PegPayUtilityApi.PegPayApi.QueryResponse Resp = api.ReactivateSmartCard(query.QueryField1, query.QueryField5, query.QueryField6);
            queryResp.ResponseField6 = Resp.StatusCode;
            queryResp.ResponseField7 = Resp.StatusDescription;
        }
        catch (Exception ex)
        {
            queryResp.ResponseField1 = "";
            queryResp.ResponseField2 = "";
            queryResp.ResponseField5 = "";
            queryResp.ResponseField4 = "";
            queryResp.ResponseField6 = "101";
            queryResp.ResponseField7 = "GENERAL ERROR AT PEGPAY";
            queryResp.ResponseField13 = "" + ex.Message;
        }
        string whatToLog = "ReactivateRequest Recieved: " + Environment.NewLine + query.ToString() +
                           Environment.NewLine +
                           "ReactivateResponse Sent: " + queryResp.ToString() +
                           "------------------------------------------------" +
                           Environment.NewLine;
        //log.Info(whatToLog);
        return queryResp;
    }
    private BETWAYTransaction GetBETWAYTrans(TransactionRequest trans)
    {
        BETWAYTransaction transaction = new BETWAYTransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.Area = trans.PostField3;
        transaction.CustomerType = trans.PostField21;
        transaction.PaymentDate = trans.PostField5;
        transaction.PaymentType = trans.PostField3;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        transaction.UtilityCode = trans.PostField4;
        //transaction.MsmqRecordDate = trans.PostField24;
        return transaction;
    }

    private DSTVTransaction GetDSTVTrans(TransactionRequest trans)
    {
        DSTVTransaction transaction = new DSTVTransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.UtilityCode = "DSTV";
        transaction.CustomerType = trans.PostField21;
        transaction.Area = trans.PostField3;
        transaction.PaymentDate = trans.PostField5;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        return transaction;
    }



    private KCCATransaction GetKCCATrans(TransactionRequest trans)
    {
        KCCATransaction transaction = new KCCATransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.CustomerType = trans.PostField21;
        transaction.PaymentDate = trans.PostField5;
        transaction.PaymentType = trans.PostField21;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        //transaction.MsMqRecordDate = trans.PostField24;//date Pegasus Received Transaction
        return transaction;
    }
    private schoolsTransaction GetSchoolsFeesTransaction(TransactionRequest trans)
    {
        schoolsTransaction transaction = new schoolsTransaction();

        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.CustomerType = trans.PostField21;
        transaction.PaymentDate = trans.PostField5;
        transaction.PaymentType = trans.PostField22;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        transaction.UtilityCode = trans.PostField4;
        transaction.ChargeType = trans.PostField33;
        transaction.Area = trans.PostField34;

        return transaction;
    }
    private schoolsTransaction GetSchoolsTrans(TransactionRequest trans)
    {
        schoolsTransaction transaction = new schoolsTransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.CustomerType = trans.PostField21;
        transaction.PaymentDate = trans.PostField5;
        transaction.PaymentType = trans.PostField21;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        transaction.UtilityCode = trans.PostField4;
        return transaction;
    }

    private UmemeTransaction GetUmemeTrans(TransactionRequest trans)
    {
        UmemeTransaction transaction = new UmemeTransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.CustomerType = trans.PostField21;
        transaction.PaymentDate = trans.PostField5;
        transaction.PaymentType = trans.PostField6;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        return transaction;
    }

    private NWSCTransaction GetNWSCTrans(TransactionRequest trans)
    {
        NWSCTransaction transaction = new NWSCTransaction();
        transaction.CustRef = trans.PostField1;
        transaction.CustName = trans.PostField2;
        transaction.CustomerType = trans.PostField21;
        transaction.Area = trans.PostField3;
        transaction.utilityCompany = trans.PostField4;
        transaction.PaymentDate = trans.PostField5;
        transaction.TransactionAmount = trans.PostField7;
        transaction.TransactionType = trans.PostField8;
        transaction.VendorCode = trans.PostField9;
        transaction.Password = trans.PostField10;
        transaction.CustomerTel = trans.PostField11;
        transaction.Reversal = trans.PostField12;
        transaction.TranIdToReverse = trans.PostField13;
        transaction.Teller = trans.PostField14;
        transaction.Offline = trans.PostField15;
        transaction.DigitalSignature = trans.PostField16;
        transaction.ChequeNumber = trans.PostField17;
        transaction.Narration = trans.PostField18;
        transaction.Email = trans.PostField19;
        transaction.VendorTransactionRef = trans.PostField20;
        return transaction;


    }

    private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private bool VendorHasBlacklistedAccounts(string vendorCode)
    {
        switch (vendorCode)
        {
            case "ORANGE":
            case "Orange":
            case "orange":
                return true;
            default:
                return false;
        }
    }

    [WebMethod]
    public Response PrepaidVendorPostTransaction(TransactionRequest trans)
    {
        string ip = getRequestIp();
        handler.LogIp(ip, trans.PostField9);

        Response postResponse = new Response();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            PegPayUtilityApi.PegPayApi.PegPay pegpay = new PegPayUtilityApi.PegPayApi.PegPay();
            pegpay.Timeout = 120000;
            if (trans.PostField4 == null)
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else if (trans.PostField4.Trim().Equals(""))
            {
                postResponse.ResponseField1 = "";
                postResponse.ResponseField2 = "";
                postResponse.ResponseField5 = "";
                postResponse.ResponseField4 = "";
                postResponse.ResponseField6 = "103";
                postResponse.ResponseField7 = "PLEASE SUPPLY A UTILITY CODE";
            }
            else
            {
                if (trans.PostField4.Equals("NWSC") || trans.PostField4.Equals("UMEME") || trans.PostField4.Equals("STARTIMES") || trans.PostField4.Equals("URA")
                    || trans.PostField4.Equals("DSTV") || trans.PostField4.Equals("GOTV") || trans.PostField4.Equals("MUBS") || trans.PostField4.Equals("MAK") ||
                    trans.PostField4.Equals("KYU") || trans.PostField4.Equals("NSSF") || trans.PostField4.Equals("KCCA") || trans.PostField4.Equals("AIRTIME") || trans.PostField4.Equals("SMS"))
                {
                    NWSCTransaction nwscTrans = GetNWSCTrans(trans);
                    PostResponse resp = new PostResponse();
                    if (trans.PostField4.ToUpper() == "SMS")
                    {
                        resp = pegpay.SendSms(nwscTrans);
                    }
                    else
                    {
                        resp = pegpay.MakeUtilityPaymentPrepaidVendor(nwscTrans);
                    }


                    //success or pending
                    if (resp.StatusCode.Equals("1000") || resp.StatusCode.Equals("0"))
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = resp.PegPayPostId;
                    }
                    else
                    {
                        postResponse.ResponseField6 = resp.StatusCode;
                        postResponse.ResponseField7 = resp.StatusDescription;
                        postResponse.ResponseField8 = "";
                    }
                }
                else
                {
                    postResponse.ResponseField6 = "100";
                    postResponse.ResponseField7 = "UTILITY NOT SUPPORTED FOR PREPAID VENDORS AT THE MOMENT";
                    postResponse.ResponseField8 = "";
                    postResponse.ResponseField9 = "";
                }
            }
        }
        catch (WebException ex)
        {
            //force vendor to invoke get transaction status
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "1000";
            postResponse.ResponseField7 = "PENDING";
            postResponse.ResponseField13 = "" + ex.Message;
        }
        catch (Exception ex)
        {
            //force vendor to invoke get transaction status
            postResponse.ResponseField1 = "";
            postResponse.ResponseField2 = "";
            postResponse.ResponseField5 = "";
            postResponse.ResponseField4 = "";
            postResponse.ResponseField6 = "1000";
            postResponse.ResponseField7 = "PENDING";
            postResponse.ResponseField13 = "" + ex.Message;
        }

        string whatToLog = "Request Recieved: " + Environment.NewLine + trans.ToString() +
                          Environment.NewLine +
                          "Response Sent: " + postResponse.ToString() +
                          "------------------------------------------------" +
                          Environment.NewLine;
        //log.Info(whatToLog);

        return postResponse;
    }

}
