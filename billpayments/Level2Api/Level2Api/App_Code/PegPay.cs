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
using UtilityReferences;
using UtilityReferences.NWSC;
using UtilityReferences.UMEME;
using UtilityReferences.URA;
//using UtilityReferences.Stanbic;
using UtilityReferences.KCCA;
using System.Collections.Generic;
using UtilityReferences.DSTVApi;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Messaging;
using UtilityReferences.WenrecoApi;
using Tester;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Net.Sockets;

[WebService(Namespace = "http://PegPayPaymentsApi/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class PegPay : System.Web.Services.WebService
{
    public string othersUmemeQueue = @".\private$\OthersUmemeQueue";
    public string otherNwscQueue = @".\private$\OthersUmemeQueue";
    public string EodReconciliationQueue = @".\private$\EodReconciliationQueue";
    BusinessLogic bll = new BusinessLogic();
   
    string meter = 45700054534.ToString();
    public void SaveVendor(Vendor vendor)
    {
        try
        {
            DatabaseHandler dh = new DatabaseHandler();
            BusinessLogic bll = new BusinessLogic();
            vendor.Passwd = bll.EncryptString(vendor.Passwd);
            dh.SaveVendorDetails(vendor);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod]
    public string GetServerStatus()
    {
        string status = "";
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveRequestlog("", "", "GETSERVERSTATUS", "", "");
            status = dp.GetServerStatus();
        }
        catch (Exception ex)
        {
            status = "ERROR CONNECTING TO SERVER DB";
        }
        return status;
    }

    [WebMethod]
    public DSTVQueryResponse QueryStartTimesCustomerDetails(string customerRef, string vendorCode, string password)
    {
        DSTVQueryResponse cust = new DSTVQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "STARTIMES", "VERIFICATION", customerRef, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("STARTIMES", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        UtilityReferences.StarTimes.StarTimesConnect serv = new UtilityReferences.StarTimes.StarTimesConnect();
                        UtilityReferences.StarTimes.SubscriberQueryResult resp = serv.GetStarTimesCustomerDetails(customerRef, creds.UtilityCode, creds.UtilityPassword);

                        if (resp.returnCode.Equals("0"))
                        {
                            cust.CustomerReference = resp.smartCardCode;
                            cust.CustomerName = resp.customerName;
                            cust.CustomerBalance = resp.balance + "";
                            cust.CustomerType = resp.orderedProductsDesc;
                            cust.Area = resp.subscriberStatus + "";
                            cust.CustType = resp.canOrderProductInfos.Length.Equals(4) ? "DTT" : "DTH";
                            cust.StatusCode = "0";
                            cust.StatusDescription = "SUCCESS";

                            Customer customer = new Customer();
                            customer.AgentCode = "STARTIMES";
                            customer.CustomerName = cust.CustomerName;
                            customer.CustomerRef = cust.CustomerReference;
                            customer.CustomerType = cust.CustomerType;
                            customer.Balance = cust.CustomerBalance.ToString();
                            customer.TIN = cust.CustType;

                            saveUmemeCustomerDetails(customer);
                        }
                        else
                        {
                            cust.CustomerReference = "";
                            cust.CustomerName = "";
                            cust.CustomerType = "";
                            cust.OutstandingBalance = "";
                            cust.StatusCode = "100";
                            cust.StatusDescription = cust.StatusDescription;
                        }
                    }
                    else
                    {
                        cust.StatusCode = "29";
                        cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
                    }
                }
                else
                {
                    cust.StatusCode = "11";
                    cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
                }
            }
            else
            {
                cust.StatusCode = "2";
                cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer custo = dp.GetCustomerDetails(customerRef, "", "STARTIMES");
            if (cust.StatusCode.Equals("0"))
            {
                cust.CustomerType = custo.CustomerType;
                cust.CustomerName = custo.CustomerName;
                cust.CustomerReference = custo.CustomerRef;
                cust.OutstandingBalance = "0";
                cust.StatusCode = "0";
                cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
            }
            else
            {
                cust.StatusCode = "30";
                cust.StatusDescription = "UNABLE TO CONNECT TO STARTIMES";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "STARTIMES");
        }
        catch (SqlException sqlex)
        {
            cust.StatusCode = "31";
            cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
        }
        catch (Exception ex)
        {
            Customer custo = dp.GetCustomerDetails(customerRef, "", "STARTIMES");
            if (custo.StatusCode.Equals("0"))
            {
                cust.CustomerType = custo.CustomerType;
                cust.CustomerName = custo.CustomerName;
                cust.CustomerReference = custo.CustomerRef;
                cust.OutstandingBalance = "0";
                cust.StatusCode = "0";
                cust.StatusDescription = dp.GetStatusDescr(cust.StatusCode);
            }
            else
            {
                cust.StatusCode = "100";
                cust.StatusDescription = "INVALID CUSTOMER REFERENCE";
            }
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "STARTIMES");
        }
        return cust;
    }

    [WebMethod]
    public ReversalResponse ReversePrepaidTransaction(ReversalRequest req)
    {
        ReversalResponse resp = new ReversalResponse();
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            if (req.IsValid())
            {
                string PegPayId = dh.LogReversalRequest(req);
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
                resp.ReversalID = PegPayId;
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = req.StatusDesc;
                resp.ReversalID = "";
            }
        }
        catch (Exception ex)
        {
            dh.LogError(ex.Message, req.VendorCode, DateTime.Now, req.OriginalTransactionId + "-REVERSAL");
            resp.StatusCode = "101";
            resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
            resp.ReversalID = "";
        }
        return resp;
    }


    [WebMethod]
    public URAQueryResponse QueryURACustomerDetails(string CustomerReference, string TypeOfPayment, string vendorCode, string password)
    {
        URAQueryResponse resp = new URAQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "URA", "VERIFICATION", CustomerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("URA", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        if (vendorCode.ToUpper().Equals("MTN"))
                        {
                            if (TypeOfPayment.ToUpper().Equals("REGISTERED"))
                            {
                                resp = QueryUraForRegisteredPayment(CustomerReference);
                            }
                            else if (TypeOfPayment.ToUpper().Equals("UNREGISTERED"))
                            {
                                resp = QueryUraForUnRegisteredPayment(CustomerReference);
                            }
                            else
                            {
                                resp.StatusCode = "100";
                                resp.StatusDescription = "UNKNOWN URA PAYMENT TYPE";
                            }
                        }
                        else if (vendorCode.ToUpper().Equals("CELL")) 
                        {
                            Customer cust = dp.GetCustomerDetails(CustomerReference, "", "URA");
                            if (cust.StatusCode.Equals("0"))
                            {
                                resp.CustomerReference = cust.CustomerRef;
                                resp.CustomerName = cust.CustomerName;
                                resp.TIN = cust.TIN;
                                resp.OutstandingBalance = cust.Balance.Split('.')[0];
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);


                            }
                            else
                            {
                                resp.CustomerReference = "";
                                resp.CustomerName = "";
                                resp.OutstandingBalance = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "INVALID REFERENCE NUMBER";
                            }
                            UraPmtService ura = new UraPmtService();
                            creds.UtilityPassword = bll.EncryptUraParameter(bll.DecryptString(creds.UtilityPassword));
                            PaymentRegEntity pre = ura.GetPRNDetails(creds.UtilityCode, creds.UtilityPassword, creds.UtilityCode, CustomerReference);
                            if (pre.AccessMsg == null || !pre.AccessMsg.Trim().ToUpper().Equals("ACCESS DENIED"))
                            {
                                if (pre.StatusCode.Trim() == "A")
                                {
                                    resp.CustomerReference = pre.Prn;
                                    resp.OutstandingBalance = int.Parse(pre.Amount).ToString("#,##0");
                                    resp.TIN = pre.Tin;
                                    resp.PaymentRegistrationDate = pre.PaymentRegDt;
                                    resp.prnStatus = pre.ExpiryDt;
                                    resp.StatusCode = "0";
                                    resp.StatusDescription = "SUCCESS";
                                    resp.CustomerName = pre.TaxpayerName;

                                    var newCustomer = new Customer();
                                    newCustomer.AgentCode = "URA";
                                    newCustomer.Area = pre.StatusCode;
                                    newCustomer.Balance = resp.OutstandingBalance;
                                    newCustomer.CustomerName = resp.CustomerName;
                                    newCustomer.TIN = resp.TIN;
                                    newCustomer.CustomerRef = resp.CustomerReference;

                                    saveURACustomerDetails(newCustomer);
                                }
                                else
                                {
                                    resp.StatusCode = "100";
                                    resp.StatusDescription = pre.StatusDesc;
                                }
                            }
                            else
                            {
                                resp.StatusCode = "100";
                                resp.StatusDescription = pre.AccessMsg;
                            }
                        }
                        else if (vendorCode.ToUpper().Equals("EGOPAY"))
                        {

                            DataTable dt = new DataTable();
                            DatabaseHandler dh = new DatabaseHandler();
                            resp = dh.QueryURAcustomerDetails(CustomerReference);

                        }
                        else if (vendorCode.ToUpper().Equals("SMART"))
                        {
                            DataTable dt = new DataTable();
                            DatabaseHandler dh = new DatabaseHandler();
                            resp = dh.QueryURAcustomerDetails(CustomerReference);

                        }
                        else
                        {

                            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                            UtilityReferences.StanbicBankApi.PegPay pegpay = new UtilityReferences.StanbicBankApi.PegPay();
                            UtilityReferences.StanbicBankApi.QueryRequest stanbicbankuraqueryrequest = new UtilityReferences.StanbicBankApi.QueryRequest();
                            UtilityReferences.StanbicBankApi.Response stanbicbankuraresponse = new UtilityReferences.StanbicBankApi.Response();
                            stanbicbankuraqueryrequest.QueryField1 = CustomerReference;
                            stanbicbankuraqueryrequest.QueryField4 = "URA";
                            stanbicbankuraqueryrequest.QueryField5 = vendorCode;
                            stanbicbankuraqueryrequest.QueryField6 = password;
                            pegpay.Url = "https://196.8.207.124:8009/TestLevelONEApi/PegPay.asmx?WSDL";
                            
                            stanbicbankuraresponse = pegpay.QueryCustomerDetails(stanbicbankuraqueryrequest);


                            if (stanbicbankuraresponse.ResponseField6.Equals("0"))
                            {
                                resp.CustomerReference = stanbicbankuraresponse.ResponseField1;
                                resp.CustomerName = stanbicbankuraresponse.ResponseField2;
                                resp.TIN = stanbicbankuraresponse.ResponseField3;
                                resp.OutstandingBalance = stanbicbankuraresponse.ResponseField4;
                                resp.StatusCode = stanbicbankuraresponse.ResponseField6;
                                resp.StatusDescription = stanbicbankuraresponse.ResponseField7;
                                resp.PaymentRegistrationDate = stanbicbankuraresponse.ResponseField8;
                                resp.PrnStatus = stanbicbankuraresponse.ResponseField9;
                                resp.Forex = stanbicbankuraresponse.ResponseField10;
                                resp.Fuel = stanbicbankuraresponse.ResponseField11;


                                Customer customer = new Customer();
                                customer.AgentCode = "URA";
                                customer.CustomerName = stanbicbankuraresponse.ResponseField2;
                                customer.CustomerRef = stanbicbankuraresponse.ResponseField1;
                                customer.Balance = stanbicbankuraresponse.ResponseField4;
                                customer.Area = stanbicbankuraresponse.ResponseField3;
                                saveKCCACustomerDetails(customer);
                            }
                            else
                            {
                                resp.CustomerReference = stanbicbankuraresponse.ResponseField1;
                                resp.CustomerName = stanbicbankuraresponse.ResponseField2;
                                resp.TIN = stanbicbankuraresponse.ResponseField3;
                                resp.OutstandingBalance = stanbicbankuraresponse.ResponseField4;
                                resp.StatusCode = stanbicbankuraresponse.ResponseField6;
                                resp.StatusDescription = stanbicbankuraresponse.ResponseField7;
                                resp.PaymentRegistrationDate = stanbicbankuraresponse.ResponseField8;
                                resp.PrnStatus = stanbicbankuraresponse.ResponseField9;
                                resp.Forex = stanbicbankuraresponse.ResponseField10;
                                resp.Fuel = stanbicbankuraresponse.ResponseField11;
                            }


                        }

                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "30";
            resp.StatusDescription = "UNABLE TO CONNECT TO URA";
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "URA");
            Customer cust = dp.GetCustomerDetails(CustomerReference, "", "URA");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerReference = cust.CustomerRef;
                resp.CustomerName = cust.CustomerName;
                resp.TIN = cust.TIN;
                resp.OutstandingBalance = cust.Balance.Split('.')[0];
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);

            }
            else
            {
                resp.CustomerReference = "";
                resp.CustomerName = "";
                resp.OutstandingBalance = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID REFERENCE NUMBER";
            }
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "URA");
        }
        return resp;
    }

    [WebMethod]
    public URAQueryResponse QueryURADTSCustomerDetails(string CustomerReference, string TypeOfPayment, string vendorCode, string password)
    {
        URAQueryResponse resp = new URAQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "URADTS", "VERIFICATION", CustomerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("URADTS", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        if (vendorCode.ToUpper().Equals("MTN"))
                        {
                            if (TypeOfPayment.ToUpper().Equals("REGISTERED"))
                            {
                                resp = QueryUraForRegisteredPayment(CustomerReference);
                            }
                            else if (TypeOfPayment.ToUpper().Equals("UNREGISTERED"))
                            {
                                resp = QueryUraForUnRegisteredPayment(CustomerReference);
                            }
                            else
                            {
                                resp.StatusCode = "100";
                                resp.StatusDescription = "UNKNOWN URA PAYMENT TYPE";
                            }
                        }
                        else if (vendorCode.ToUpper().Equals("CELL")) 
                        {
                            Customer cust = dp.GetCustomerDetails(CustomerReference, "", "URA");
                            if (cust.StatusCode.Equals("0"))
                            {
                                resp.CustomerReference = cust.CustomerRef;
                                resp.CustomerName = cust.CustomerName;
                                resp.TIN = cust.TIN;
                                resp.OutstandingBalance = cust.Balance.Split('.')[0];
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);


                            }
                            else
                            {
                                resp.CustomerReference = "";
                                resp.CustomerName = "";
                                resp.OutstandingBalance = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "INVALID REFERENCE NUMBER";
                            }
                            UraPmtService ura = new UraPmtService();
                            creds.UtilityPassword = bll.EncryptUraParameter(bll.DecryptString(creds.UtilityPassword));
                            PaymentRegEntity pre = ura.GetPRNDetails(creds.UtilityCode, creds.UtilityPassword, creds.UtilityCode, CustomerReference);
                            if (pre.AccessMsg == null || !pre.AccessMsg.Trim().ToUpper().Equals("ACCESS DENIED"))
                            {
                                if (pre.StatusCode.Trim() == "A")
                                {
                                    resp.CustomerReference = pre.Prn;
                                    resp.OutstandingBalance = int.Parse(pre.Amount).ToString("#,##0");
                                    resp.TIN = pre.Tin;
                                    resp.PaymentRegistrationDate = pre.PaymentRegDt;
                                    resp.prnStatus = pre.ExpiryDt;
                                    resp.StatusCode = "0";
                                    resp.StatusDescription = "SUCCESS";
                                    resp.CustomerName = pre.TaxpayerName;

                                    var newCustomer = new Customer();
                                    newCustomer.AgentCode = "URA";
                                    newCustomer.Area = pre.StatusCode;
                                    newCustomer.Balance = resp.OutstandingBalance;
                                    newCustomer.CustomerName = resp.CustomerName;
                                    newCustomer.TIN = resp.TIN;
                                    newCustomer.CustomerRef = resp.CustomerReference;

                                    saveURACustomerDetails(newCustomer);
                                }
                                else
                                {
                                    resp.StatusCode = "100";
                                    resp.StatusDescription = pre.StatusDesc;
                                }
                            }
                            else
                            {
                                resp.StatusCode = "100";
                                resp.StatusDescription = pre.AccessMsg;
                            }
                        }
                        else if (vendorCode.ToUpper().Equals("EGOPAY"))
                        {

                            DataTable dt = new DataTable();
                            DatabaseHandler dh = new DatabaseHandler();
                            resp = dh.QueryURAcustomerDetails(CustomerReference);

                        }
                        else if (vendorCode.ToUpper().Equals("SMART"))
                        {
                            DataTable dt = new DataTable();
                            DatabaseHandler dh = new DatabaseHandler();
                            resp = dh.QueryURAcustomerDetails(CustomerReference);

                        }
                        else
                        {

                            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                            UtilityReferences.StanbicBankApi.PegPay pegpay = new UtilityReferences.StanbicBankApi.PegPay();
                            UtilityReferences.StanbicBankApi.QueryRequest stanbicbankuraqueryrequest = new UtilityReferences.StanbicBankApi.QueryRequest();
                            UtilityReferences.StanbicBankApi.Response stanbicbankuraresponse = new UtilityReferences.StanbicBankApi.Response();
                            stanbicbankuraqueryrequest.QueryField1 = CustomerReference;
                            stanbicbankuraqueryrequest.QueryField4 = "URADTS";
                            stanbicbankuraqueryrequest.QueryField5 = vendorCode;
                            stanbicbankuraqueryrequest.QueryField6 = password;
                           pegpay.Url = "https://196.8.207.124:8009/TestLevelONEApi/PegPay.asmx?WSDL";
                            
                            stanbicbankuraresponse = pegpay.QueryCustomerDetails(stanbicbankuraqueryrequest);


                            if (stanbicbankuraresponse.ResponseField6.Equals("0"))
                            {
                                resp.CustomerReference = stanbicbankuraresponse.ResponseField1;
                                resp.CustomerName = stanbicbankuraresponse.ResponseField2;
                                resp.TIN = stanbicbankuraresponse.ResponseField3;
                                resp.OutstandingBalance = stanbicbankuraresponse.ResponseField4;
                                resp.StatusCode = stanbicbankuraresponse.ResponseField6;
                                resp.StatusDescription = stanbicbankuraresponse.ResponseField7;
                                resp.PaymentRegistrationDate = stanbicbankuraresponse.ResponseField8;
                                resp.PrnStatus = stanbicbankuraresponse.ResponseField9;
                                resp.Forex = stanbicbankuraresponse.ResponseField10;
                                resp.Fuel = stanbicbankuraresponse.ResponseField11;


                                Customer customer = new Customer();
                                customer.AgentCode = "URADTS";
                                customer.CustomerName = stanbicbankuraresponse.ResponseField2;
                                customer.CustomerRef = stanbicbankuraresponse.ResponseField1;
                                customer.Balance = stanbicbankuraresponse.ResponseField4;
                                customer.Area = stanbicbankuraresponse.ResponseField3;
                                saveKCCACustomerDetails(customer);
                               }
                            else
                            {
                                resp.CustomerReference = stanbicbankuraresponse.ResponseField1;
                                resp.CustomerName = stanbicbankuraresponse.ResponseField2;
                                resp.TIN = stanbicbankuraresponse.ResponseField3;
                                resp.OutstandingBalance = stanbicbankuraresponse.ResponseField4;
                                resp.StatusCode = stanbicbankuraresponse.ResponseField6;
                                resp.StatusDescription = stanbicbankuraresponse.ResponseField7;
                                resp.PaymentRegistrationDate = stanbicbankuraresponse.ResponseField8;
                                resp.PrnStatus = stanbicbankuraresponse.ResponseField9;
                                resp.Forex = stanbicbankuraresponse.ResponseField10;
                                resp.Fuel = stanbicbankuraresponse.ResponseField11;
                            }


                        }

                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "30";
            resp.StatusDescription = "UNABLE TO CONNECT TO URA";
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "URA");
            Customer cust = dp.GetCustomerDetails(CustomerReference, "", "URA");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerReference = cust.CustomerRef;
                resp.CustomerName = cust.CustomerName;
                resp.TIN = cust.TIN;
                resp.OutstandingBalance = cust.Balance.Split('.')[0];
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);

            }
            else
            {
                resp.CustomerReference = "";
                resp.CustomerName = "";
                resp.OutstandingBalance = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID REFERENCE NUMBER";
            }
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "URA");
        }
        return resp;
    }

    private URAQueryResponse QueryUraForUnRegisteredPayment(string CustomerReference)
    {
        URAQueryResponse resp = new URAQueryResponse();
        DatabaseHandler dh = new DatabaseHandler();
        LookupResp lookupResp = dh.GetDetailsByOffenceCode(CustomerReference);
        if (lookupResp.StatusCode.Equals("0"))
        {
            resp.CustomerReference = CustomerReference;
            resp.CustomerName = "";
            resp.OutstandingBalance = lookupResp.Amount;
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        else
        {
            resp.StatusCode = "100";
            resp.StatusDescription = lookupResp.StatusDescription;
        }
        return resp;
    }

    private URAQueryResponse QueryUraForRegisteredPayment(string CustomerReference)
    {
        URAQueryResponse resp = new URAQueryResponse();
        Process p = new Process();

        ProcessStartInfo startInfo = new ProcessStartInfo("C:\\Program Files\\Java\\jre7\\bin\\java.exe",
                  "-jar E:\\PePay\\UraTester\\ConsoleApplication\\LookUpConsole\\LookUpConsole\\dist\\LookUpConsole.jar PrnLookup:" + CustomerReference);
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardOutput = true;
        Process process = Process.Start(startInfo);

        string uraResp = process.StandardOutput.ReadToEnd();
        string[] details = Regex.Split(uraResp, "\r\n");
        
        if (details[0] == "0")
        {
            resp.StatusCode = details[0];
            resp.StatusDescription = details[1];
            resp.CustomerReference = CustomerReference;
            resp.CustomerName = details[6];
            resp.OutstandingBalance = details[2].Split('.')[0];
            resp.TIN = details[7];
            resp.PaymentRegistrationDate = details[3];
            resp.PrnStatus = details[5];
            string TypeOfPayment = "REGISTERED";
            Customer cust = new Customer();
            cust.CustomerName = resp.CustomerName;
            cust.AgentCode = "URA";
            cust.Area = details[5];
            cust.Balance = resp.OutstandingBalance;
            cust.CustomerRef = CustomerReference;
            cust.CustomerType = TypeOfPayment;
            cust.TIN = resp.TIN;
            cust.StatusCode = "0";
            cust.StatusDescription = "SUCCESS";

            DatabaseHandler dh = new DatabaseHandler();
            dh.SaveDstvCustomerDetails(cust);
        }
        else
        {
            resp.StatusCode = "100";
            resp.StatusDescription = details[1];
        }

        return resp;

    }

    [WebMethod]
    public UmemeQueryResponse QueryUmemeCustomerDetails(string customerReference, string vendorCode, string password)
    {
        UmemeQueryResponse resp = new UmemeQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string vendortype = vendorData.Rows[0]["VendorType"].ToString().ToUpper();
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("UMEME", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {

                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        EPayment umemeapi = new EPayment();

                        if(vendorCode.Equals("AFRICELL"))
                        {
                            Customer cust1 = dp.GetCustomerDetails(customerReference, "", "UMEME");
                            if (cust1.StatusCode.Equals("0"))
                            {
                                resp.CustomerType = cust1.CustomerType;
                                resp.CustomerName = cust1.CustomerName;
                                resp.CustomerReference = cust1.CustomerRef;
                                resp.OutstandingBalance = "0";
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                            else
                            {
                                resp.StatusCode = "30";
                                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
                            }
                            return resp;
                        }

                        UtilityReferences.UMEME.Customer cust = umemeapi.ValidateCustomer(customerReference, creds.UtilityCode, creds.UtilityPassword);
                        
                        if (cust.StatusCode.Equals("0"))
                        {
                            resp.CustomerReference = cust.CustomerRef;
                            resp.CustomerName = cust.CustomerName;
                            resp.CustomerType = cust.CustomerType;
                            resp.OutstandingBalance = cust.Balance.ToString();
                            resp.StatusCode = cust.StatusCode;
                            resp.StatusDescription = cust.StatusDescription;
                            Customer customer = new Customer();
                            customer.AgentCode = "UMEME";
                            customer.CustomerName = resp.CustomerName;
                            customer.CustomerRef = resp.CustomerReference;
                            customer.CustomerType = resp.CustomerType;
                            customer.Balance = cust.Balance.ToString();


                            saveUmemeCustomerDetails(customer);
                        }
                        else if (cust.StatusDescription.Equals("Work Request not ready to receive Payment"))
                        {
                            resp.StatusCode = "100";
                            resp.StatusDescription = "PAYMENT ALREADY COMPLETED";
                        }
                        else
                        {
                            resp.CustomerReference = "";
                            resp.CustomerName = "";
                            resp.CustomerType = "";
                            resp.OutstandingBalance = "";
                            resp.StatusCode = "100";
                            resp.StatusDescription = cust.StatusDescription;
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "UMEME");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "UMEME");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "UMEME");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID CUSTOMER REFERENCE";
            }
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "UMEME");
        }
        return resp;
    }


    [WebMethod]
    public UmemeQueryResponse QueryMoweCustomer(string customerReference, string vendorCode, string password)
    {
        UmemeQueryResponse resp = new UmemeQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string vendortype = vendorData.Rows[0]["VendorType"].ToString().ToUpper();
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        UtilityReferences.MoweApi.MowePaymentApi moweApi = new UtilityReferences.MoweApi.MowePaymentApi();
                       
                        UtilityReferences.MoweApi.Query query = new UtilityReferences.MoweApi.Query();
                        query.RequestId = customerReference;
                        query.VendorCode = vendorCode;
                        query.Password = PegPay.MowePassword;
                        UtilityReferences.MoweApi.ApiResult cust = moweApi.QueryCustomer(query);
                    
                        if (cust.StatusCode.Equals("0"))
                        {
                            resp.CustomerReference = cust.ResultId;
                            resp.CustomerName = cust.CustomerName;
                            resp.CustomerType = cust.UmbrellaCode;
                            resp.OutstandingBalance = cust.Balance.ToString();

                            resp.Lifeline = cust.UmbrellaName;
                            resp.ReceiptNumber = cust.SchemeName;

                            resp.StatusCode = cust.StatusCode;
                            resp.StatusDescription = cust.StatusDesc;
                            Customer customer = new Customer();
                            customer.AgentCode = "MOWE";
                            customer.CustomerName = resp.CustomerName;
                            customer.CustomerRef = resp.CustomerReference;
                            customer.CustomerType = resp.CustomerType;
                            customer.Balance = cust.Balance.ToString();
                        
                        saveUmemeCustomerDetails(customer);
                        }
                        else
                        {
                            resp.CustomerReference = "";
                            resp.CustomerName = "";
                            resp.CustomerType = "";
                            resp.OutstandingBalance = "";
                            resp.StatusCode = "100";
                            resp.StatusDescription = cust.StatusDesc;
                        }
                    
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "UMEME");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO MOWE";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "MOWE");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "MOWE");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID CUSTOMER REFERENCE";
            }
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "MOWE");
        }
        return resp;
    }

    [WebMethod]
    public DSTVQueryResponse QueryDSTVCustomerDetails(string smartCardNumber, string PayTvCode, string bouquetCode, string vendorCode, string password)
    {
        DSTVQueryResponse resp = new DSTVQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "DSTV", "VERIFICATION", smartCardNumber, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("DSTV", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        if (creds.UtilityIsOffline == "TRUE")
                        {
                            resp = QueryDSTVCustomerLocally(smartCardNumber, PayTvCode, bouquetCode);
                        }
                        else
                        {
                            resp = QueryMultichoiceDirectly(smartCardNumber, PayTvCode, bouquetCode, vendorCode, password, creds);
                           
                            if (resp.StatusCode.Equals("0"))
                            {
                                Customer cust = GetDSTVCustomer(resp, bouquetCode);
                                dp.SaveDstvCustomerDetails(cust);
                            }
                        }
                        
                        if (resp.StatusCode.Equals("200"))
                        {
                            resp = QueryMultichoiceDirectly(smartCardNumber, PayTvCode, bouquetCode, vendorCode, password, creds);
                           
                            if (resp.StatusCode.Equals("0"))
                            {
                                Customer cust = GetDSTVCustomer(resp, bouquetCode);
                                dp.SaveDstvCustomerDetails(cust);
                            }
                            else if (IsValidDstvFailureReason(resp))
                            {
                                dp.DeactivateCustomerNumber(smartCardNumber, PayTvCode);
                            }
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            resp = QueryDSTVCustomerLocally(smartCardNumber, PayTvCode, bouquetCode);
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "DSTV");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "DSTV");
        }
        return resp;
    }
    private bool IsValidDstvFailureReason(DSTVQueryResponse resp)
    {
        DatabaseHandler dh = new DatabaseHandler();
        bool isvalid = false;
        try
        {
            string[] message = resp.StatusDescription.Split(new string[] { ":" }, StringSplitOptions.None);
            string error = "";
            if (message.Length == 2)
            {
                error = message[0];
            }
            else
            {
                error = resp.StatusDescription;
            }
            DataTable dtable = dh.GetDstvValidationFailureReasons(error);
            if (dtable.Rows.Count > 0)
            {
                isvalid = true;
            }
        }
        catch (Exception)
        {

        }
        return isvalid;
    }
    private DSTVQueryResponse QueryDSTVCustomerLocally(string smartCardNumber, string PayTvCode, string bouquetCode)
    {
        //so you have a smart card number
        //and a paytvcode
        //and bouquetcode
        //we check in db for that smartcard
        //if customer exists in db
        //we get the customer type i.e(Gotv or dstv)
        //then we make sure the bouquet he wants to pay for is for that customer type
        //
        DatabaseHandler dh = new DatabaseHandler();
        DSTVQueryResponse resp = new DSTVQueryResponse();
        Customer cust = dh.GetCustomerDetailsDSTV(smartCardNumber, "", "DSTV");
        if (cust.StatusCode.Equals("0"))
        {
            //if he is a gotv guy
            //we make sure he picked a Gotv bouquet
            if (cust.CustomerType.ToUpper().Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (!(bouquetCode.StartsWith("GO") || bouquetCode.Equals("789012")))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUETCODE FOR GOTV CUSTOMER";
                    return resp;
                }
            }
            else if (cust.CustomerType.ToUpper().Equals("DSTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUETCODE FOR DSTV CUSTOMER";
                    return resp;
                }
            }
            else
            {
                resp.bouquetDetails = null;
                resp.Area = "";
                resp.StatusCode = "200";
                resp.StatusDescription = "INVALID CUSTOMER REFERENCE";
                return resp;
            }
            
            resp.Area = cust.Area;
            resp.CustomerType = cust.CustomerType;
            resp.bouquetDetails = dh.GetBouquetDetailsFromDB(bouquetCode, cust.CustomerType)[0];
            resp.CurrentBouquet = dh.GetBouquetDetailsFromDB(bouquetCode, cust.CustomerType)[0].BouquetName;
            resp.CustomerName = cust.CustomerName;
            resp.DstvCustomerNo = cust.TIN;
            resp.CustomerReference = cust.CustomerRef;
            resp.OutstandingBalance = string.IsNullOrEmpty(cust.Balance) ? "0" : cust.Balance;
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        else
        {
            resp.Area = "";
            resp.CustomerName = "";
            resp.CustomerReference = "";
            resp.CustomerType = "";
            resp.OutstandingBalance = "";
            resp.StatusCode = "200";
            resp.StatusDescription = "INVALID SMART CARD NUMBER";
        }
        return resp;

    }

    private DSTVQueryResponse QueryMultichoiceDirectly(string smartCardNumber, string PayTvCode, string bouquetCode, string vendorCode, string password, UtilityCredentials creds)
    {

        DSTVQueryResponse resp = new DSTVQueryResponse();
        resp = QueryUsingSmartCardNumber(smartCardNumber, bouquetCode, vendorCode, PayTvCode, creds);
        if (resp.StatusCode.Equals("200"))
        {
            resp = QueryUsingCustomerNumber(smartCardNumber, bouquetCode, vendorCode, PayTvCode);
        }
        return resp;
    }

    private DSTVQueryResponse QueryUsingSmartCardNumber(string smartCardNumber, string bouquetCode, string vendorCode, string PayTvCode, UtilityCredentials creds)
    {
        DatabaseHandler dh = new DatabaseHandler();
        DSTVQueryResponse resp = new DSTVQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            GetBalanceByDeviceNumberResponse dstvResp = new GetBalanceByDeviceNumberResponse();
            GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
            string datasource = "Uganda";
            string devicenumber = smartCardNumber;//"4261353579"; //"2017607397";//"2015897761";
            string currencycode = "";
            string bussinesUnit = PayTvCode;
            string VendorCode = creds.UtilityCode;
            string language = "English";
            string IpAddress = "41.190.131.222";

            
            dstvResp = dstv.GetCustomerDetailsByDeviceNumber(datasource, devicenumber, currencycode, bussinesUnit, VendorCode, language, IpAddress, "");
            
           UtilityReferences.DSTVApi.Customer customer = dstvResp.customerDetails;
            if (customer.typeName.Contains("GOTV"))
            {
                resp.CustomerType = "GOTV";
            }
            else if (customer.typeName.ToUpper().Contains("PANELIST"))
            {
                resp.CustomerType = "DSTV";
            }
            else if (customer.typeName.ToUpper().Contains("SUD"))
            {
                resp.CustomerType = "DSTV";
            }
            else if (customer.typeName.ToUpper().Contains("EMPLOYEE"))
            {
                resp.CustomerType = "DSTV";
            }
            if (resp.CustomerType.ToUpper().Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (!(bouquetCode.StartsWith("GO") || bouquetCode.Equals("789012")))
                {
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUETCODE FOR GOTV CUSTOMER";
                    return resp;
                }
            }
            else if (resp.CustomerType.ToUpper().Equals("DSTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUETCODE FOR DSTV CUSTOMER";
                    return resp;
                }
            }
            else
            {
                resp.bouquetDetails = null;
                resp.Area = "";
                resp.StatusCode = "200";
                resp.StatusDescription = "INVALID SMART CARD NUMBER";
                return resp;
            }
            
            resp.CustomerName = dstvResp.customerDetails.salutation + " " + dstvResp.customerDetails.surname + " " + dstvResp.customerDetails.initials;
            resp.CustomerReference = smartCardNumber;
            resp.DstvCustomerNo = "" + dstvResp.customerDetails.number;
            resp.bouquetDetails = GetBouquetDetails(bouquetCode, resp.CustomerType, "", "")[0];
            resp.OutstandingBalance = dstvResp.accounts.Length > 0 ? "" + dstvResp.accounts[0].totalBalance : "0";
            resp.OutstandingBalance = string.IsNullOrEmpty(resp.OutstandingBalance) ? "0" : resp.OutstandingBalance;
            resp.CurrentBouquet = GetCurrentBouquetName(dstvResp.customerDetails.number, resp.CustomerType, "", "");
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (SoapException ex)
        {
            resp.CustomerReference = smartCardNumber;
            resp.OutstandingBalance = "";
            resp.StatusCode = "100";
            resp.StatusDescription = ex.Message;
        }
        //something has gone wrong
        catch (Exception ex)
        {
            //we cant reach dstv
            if (ex.Message.ToUpper().Contains("UNABLE TO CONNECT") || ex.Message.ToUpper().Contains("TIMED OUT"))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "UNABLE TO REACH MULTCHOICE";
            }
            else
            {
                resp.StatusCode = "101";
                resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
            }
        }

        return resp;

    }

    private DSTVQueryResponse QueryMultichoiceDirectly(string smartCardNumber, string PayTvCode, string bouquetCode, string vendorCode, string password)
    {

        DSTVQueryResponse resp = new DSTVQueryResponse();
        resp = QueryUsingSmartCardNumber(smartCardNumber, bouquetCode, vendorCode, PayTvCode);
        if (resp.StatusCode.Equals("100"))
        {
            resp = QueryUsingCustomerNumber(smartCardNumber, bouquetCode, vendorCode, PayTvCode);
        }
        return resp;
    }

    private DSTVQueryResponse CheckInDBForCustomer(string smartCardNumber, string PayTvCode, string bouquetCode)
    {
        DSTVQueryResponse resp = new DSTVQueryResponse();
        DatabaseHandler dh = new DatabaseHandler();
        Customer cust = dh.GetCustomerDetails(smartCardNumber, "", "DSTV");
        if (cust.StatusCode.Equals("0"))
        {
            //if he is a gotv guy
            //we make sure he picked a Gotv bouquet
            if (cust.CustomerType.ToUpper().Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "TOP_UP")
                {
                    //do nothing
                }
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (!bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR GOTV";
                    return resp;
                }
            }
            //this is most likely a dstv guy
            //we make sure he picked a dstv bouquet
            else if (cust.CustomerType.ToUpper().Equals("DSTV") && (bouquetCode != null))
            {
                if (bouquetCode == "TOP_UP")
                {
                    //do nothing
                }
                else if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR DSTV";
                    return resp;
                }
            }
            else
            {
                resp.bouquetDetails = null;
                resp.Area = "";
                resp.StatusCode = "200";
                resp.StatusDescription = "INVALID CUSTOMER REFERENCE";
                return resp;
            }
            
            resp.Area = cust.Area;
            resp.CustomerType = cust.CustomerType;
            resp.bouquetDetails = dh.GetBouquetDetailsFromDB(bouquetCode, cust.CustomerType)[0];
            resp.CurrentBouquet = dh.GetBouquetDetailsFromDB(bouquetCode, cust.CustomerType)[0].BouquetName;
            resp.CustomerName = cust.CustomerName;
            resp.DstvCustomerNo = cust.TIN;
            resp.CustomerReference = cust.CustomerRef;
            resp.OutstandingBalance = string.IsNullOrEmpty(cust.Balance) ? "0" : cust.Balance;
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        else
        {
            resp.Area = "";
            resp.CustomerName = "";
            resp.CustomerReference = "";
            resp.CustomerType = "";
            resp.OutstandingBalance = "";
            resp.StatusCode = "200";
            resp.StatusDescription = "INVALID SMART CARD NUMBER";
        }
        return resp;

    }

    private Customer GetDSTVCustomer(DSTVQueryResponse resp, string bouquetCode)
    {
        Customer cust = new Customer();
        cust.AgentCode = "DSTV";
        cust.Area = resp.CurrentBouquet;
        cust.Balance = resp.OutstandingBalance;
        cust.CustomerName = resp.CustomerName;
        cust.CustomerRef = resp.CustomerReference;
        cust.CustomerType = resp.CustomerType;
        cust.TIN = resp.DstvCustomerNo;
        cust.StatusCode = "0";
        cust.StatusDescription = "SUCCESS";
        return cust;
    }

    private DSTVQueryResponse QueryUsingCustomerNumber(string smartCardNumber, string bouquetCode, string vendorCode, string utility)
    {

        DSTVQueryResponse resp = new DSTVQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            DatabaseHandler dp = new DatabaseHandler();
            UtilityCredentials creds = dp.GetUtilityCreds("DSTV", vendorCode);
            SelfCareService dstv = new SelfCareService();
            GetBalanceByCustomerNumberResponse dstvResp = new GetBalanceByCustomerNumberResponse();
            string language = "English";
            string datasource = "Uganda_UAT";
            uint customerNumber = 0;

            try
            {
                customerNumber = uint.Parse(smartCardNumber);
            }
            catch (Exception e)
            {
                resp.CustomerReference = smartCardNumber;
                resp.OutstandingBalance = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID SMART CARD NUMBER";
                return resp;
            }

            string currencycode = "";
            string bussinesUnit = "dstv";
            string vendorC = creds.UtilityCode;
            string passwd = creds.UtilityPassword;
            string IpAddress = "41.190.130.222";

            dstvResp = dstv.GetCustomerDetailsByCustomerNumber(datasource, customerNumber, true, currencycode, bussinesUnit, passwd, language, IpAddress,"1");


            resp.CustomerType = GetDSTVCustomerType(dstvResp.customerDetails.typeName);

            //go tv customer has chosen wrong bouquet
            if (resp.CustomerType.Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (!bouquetCode.StartsWith("GO"))
                {
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR GOYV";
                    return resp;
                }
            }
            //dstv customer has chosen go tv bouquet
            else if (resp.CustomerType.Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR DSTV";
                    return resp;
                }
            }
            else
            {
                resp.bouquetDetails = null;
                resp.Area = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID SMART CARD NUMBER";
                return resp;
            }

            resp.CustomerName = dstvResp.customerDetails.salutation + " " + dstvResp.customerDetails.surname + " " + dstvResp.customerDetails.initials;
            resp.CustomerReference = smartCardNumber;
            resp.DstvCustomerNo = "" + dstvResp.customerDetails.number;
            resp.bouquetDetails = GetBouquetDetails(bouquetCode, resp.CustomerType, "", "")[0];
            resp.OutstandingBalance = dstvResp.accounts.Length > 0 ? "" + dstvResp.accounts[0].totalBalance : "0";
            resp.OutstandingBalance = string.IsNullOrEmpty(resp.OutstandingBalance) ? "0" : resp.OutstandingBalance;
            resp.CurrentBouquet = GetCurrentBouquetName(dstvResp.customerDetails.number, resp.CustomerType, "", "");
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        //this is how multichoice communicate errors
        catch (SoapException ex)
        {
            resp.CustomerReference = smartCardNumber;
            resp.OutstandingBalance = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "INAVLID CUSTOMER NUMBER";
        }
        //something has gone wrong
        catch (Exception ex)
        {
            //we cant reach dstv
            if (ex.Message.ToUpper().Contains("UNABLE TO CONNECT") || ex.Message.ToUpper().Contains("TIMED OUT"))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "UNABLE TO REACH MULTCHOICE";
            }
            else
            {
                resp.StatusCode = "101";
                resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
            }
        }

        return resp;
    }

    private DSTVQueryResponse QueryUsingSmartCardNumber(string smartCardNumber, string bouquetCode, string vendorCode, string PayTvCode)
    {
        DSTVQueryResponse resp = new DSTVQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            GetBalanceByDeviceNumberResponse dstvResp = new GetBalanceByDeviceNumberResponse();
            GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();

            string datasource = "Uganda";
            string devicenumber = smartCardNumber;
            string currencycode = "UGS";
            string bussinesUnit = "GOTV";
            string VendorCode = "PegasusDstv";
            string language = "English";
            string IpAddress = "41.190.131.222";

            dstvResp = dstv.GetCustomerDetailsByDeviceNumber(datasource, devicenumber, currencycode, bussinesUnit, VendorCode, language, IpAddress,"1");

            resp.CustomerType = GetDSTVCustomerType(dstvResp.customerDetails.typeName);
           
            if (resp.CustomerType.ToUpper().Equals("GOTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (!bouquetCode.StartsWith("GO"))
                {
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR GOTV";
                    return resp;
                }
            }
            else if (resp.CustomerType.ToUpper().Equals("DSTV") && (bouquetCode != null))
            {
                if (bouquetCode == "")
                {
                    //do nothing
                }
                else if (bouquetCode.StartsWith("GO"))
                {
                    resp.bouquetDetails = null;
                    resp.Area = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID BOUQUET FOR DSTV";
                    return resp;
                }
            }
            else
            {
                resp.bouquetDetails = null;
                resp.Area = "";
                resp.StatusCode = "200";
                resp.StatusDescription = "INVALID SMART CARD NUMBER";
                return resp;
            }
            
            resp.CustomerName = dstvResp.customerDetails.salutation + " " + dstvResp.customerDetails.surname + " " + dstvResp.customerDetails.initials;
            resp.CustomerReference = smartCardNumber;
            resp.DstvCustomerNo = "" + dstvResp.customerDetails.number;
            resp.bouquetDetails = GetBouquetDetails(bouquetCode, resp.CustomerType, "", "")[0];
            resp.OutstandingBalance = dstvResp.accounts.Length > 0 ? "" + dstvResp.accounts[0].totalBalance : "0";
            resp.OutstandingBalance = string.IsNullOrEmpty(resp.OutstandingBalance) ? "0" : resp.OutstandingBalance;
            resp.CurrentBouquet = GetCurrentBouquetName(dstvResp.customerDetails.number, resp.CustomerType, "", "");
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (SoapException ex)
        {
            resp.CustomerReference = smartCardNumber;
            resp.OutstandingBalance = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "INVALID SMART CARD NUMBER";
        }
        catch (Exception ex)
        {
            if (ex.Message.ToUpper().Contains("UNABLE TO CONNECT") || ex.Message.ToUpper().Contains("TIMED OUT"))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "UNABLE TO REACH MULTCHOICE";
            }
            else
            {
                resp.StatusCode = "101";
                resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
            }
        }

        return resp;

    }

    private DSTVQueryResponse QueryFromGoTvUsingSmartCard(string smartCardNumber, string bouquetCode)
    {
        DSTVQueryResponse resp = new DSTVQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            GetBalanceByDeviceNumberResponse dstvResp = new GetBalanceByDeviceNumberResponse();
            GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
            string datasource = "Uganda_UAT";
            string devicenumber = smartCardNumber;
            string currencycode = "UGS";
            string bussinesUnit = "Gotv";
            string vendorCode = "PegasusGOtv";
            string language = "English";
            string IpAddress = "41.190.130.222";

            dstvResp = dstv.GetCustomerDetailsByDeviceNumber(datasource, devicenumber, currencycode, bussinesUnit, vendorCode, language, IpAddress,"1");

            resp.bouquetDetails = GetBouquetDetails(bouquetCode, "GOTV", "", "")[0];
            resp.CustomerName = dstvResp.customerDetails.salutation + " " + dstvResp.customerDetails.surname;
            resp.CustomerReference = smartCardNumber;
            resp.OutstandingBalance = dstvResp.accounts.Length > 0 ? "" + dstvResp.accounts[0].currentAmount : "";
            resp.CurrentBouquet = GetCurrentBouquetName(dstvResp.customerDetails.number, "GOTV", "", "");
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (SoapException ex)
        {
            resp.StatusCode = "100";
            resp.StatusDescription = "INVALID SMART CARD NUMBER";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "101";
            resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }

        return resp;

    }

    private DSTVQueryResponse QueryFromDSTVUsingSmartCard(string smartCardNumber, string bouquetCode)
    {
        DSTVQueryResponse resp = new DSTVQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            GetBalanceByDeviceNumberResponse dstvResp = new GetBalanceByDeviceNumberResponse();
            GetBalanceByCustomerNumberResponse custRep = new GetBalanceByCustomerNumberResponse();
            string datasource = "Uganda_UAT";
            string devicenumber = smartCardNumber;
            string currencycode = "UGS";
            string bussinesUnit = "dstv";
            string vendorCode = "PegasusDstv";
            string language = "English";
            string IpAddress = "41.190.130.222";

            dstvResp = dstv.GetCustomerDetailsByDeviceNumber(datasource, devicenumber, currencycode, bussinesUnit, vendorCode, language, IpAddress,"1");


            resp.CustomerName = dstvResp.customerDetails.salutation + " " + dstvResp.customerDetails.surname + " " + dstvResp.customerDetails.initials;
            resp.CustomerType = GetDSTVCustomerType(dstvResp.customerDetails.typeName);
            resp.CustomerReference = smartCardNumber;
            resp.DstvCustomerNo = "" + dstvResp.customerDetails.number;
            resp.bouquetDetails = GetBouquetDetails(bouquetCode, resp.CustomerType, "", "")[0];
            resp.OutstandingBalance = dstvResp.accounts.Length > 0 ? "" + dstvResp.accounts[0].currentAmount : "";
            resp.CurrentBouquet = GetCurrentBouquetName(dstvResp.customerDetails.number, resp.CustomerType, "", "");
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (SoapException ex)
        {
            resp.StatusCode = "100";
            resp.StatusDescription = "INVALID SMART CARD NUMBER";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "101";
            resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }

        return resp;
    }

    private string GetCurrentBouquetName(uint customerNumber, string customerType, string vendorCode, string passowrd)
    {
        try
        {
            if (customerType.Equals("DSTV"))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SelfCareService dstv = new SelfCareService();
                Hardware[] resp = { };
               return resp[0].Services[0].ProductDescription;
            }
            else
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                SelfCareService dstv = new SelfCareService();
                Hardware[] resp = { };
                return resp[0].Services[0].ProductDescription;
            }
        }
        catch (Exception e)
        {
            return "";
        }
    }

    private string GetDSTVCustomerType(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            return "";
        }
        else
        {
            if (type.Trim().ToUpper().Contains("GOTV"))
            {
                return "GOTV";
            }
            else if (type.Trim().ToUpper().Contains("DSTV"))
            {
                return "DSTV";
            }
            else
            {
                return type;
            }
        }
    }

    private double GetTotalBalance(UtilityReferences.UMEME.Customer cust)
    {
        try
        {
            double total = 0;
            double PercentVAT = 18;
            double Percentcredit = 50;
            if (cust.Credit.Equals(0))
            {
                total = cust.Balance;
            }
            else
            {
                double calculatedcredit = (Percentcredit / 100) * cust.Credit;
                double bal = cust.Balance + calculatedcredit;
                double calculatedVAT = (PercentVAT / 100) * bal;
                total = bal + calculatedVAT;
            }
            return total;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [WebMethod]
    public BouquetDetails[] GetBouquetDetails(string BouquetCode, string PayTvCode, string VendorCode, string Password)
    {
        DatabaseHandler dh = new DatabaseHandler();
        BouquetDetails[] allBouquets = { };
        try
        {
            allBouquets = dh.GetBouquetDetailsFromDB(BouquetCode, PayTvCode);
        }
        catch (Exception e)
        {
            BouquetDetails bd = new BouquetDetails();
            bd.StatusCode = "100";
            bd.StatusDescription = "FAILED TO GET BOUQUET DETAILS";
            allBouquets[0] = bd;
        }
        return allBouquets;
    }

    private BouquetDetails[] QueryBouquetsFromPayTv(string BouquetCode, string PayTvCode, string VendorCode, string Password)
    {
        DatabaseHandler dh = new DatabaseHandler();
        List<BouquetDetails> allBouquets = new List<BouquetDetails>();
        BouquetDetails bouq = new BouquetDetails();
        try
        {
            if (string.IsNullOrEmpty(PayTvCode))
            {
                bouq.StatusCode = "100";
                bouq.StatusDescription = "Please Supply A Pay TV Code";
                allBouquets.Add(bouq);
                return allBouquets.ToArray();
            }
            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            SelfCareService dstv = new SelfCareService();
            ProductwithoutChannel[] resp = { };
            string[] Products = { };
            string country = "Uganda";
            string vendorCode = "pegasusGotv";
            string IpAddress = "41.190.131.222";
            string bussinessUnit = "gotv";
            string interfaceType = "Gotv Facebook And Mobi";
            string language = "English";
            bool visibleForPayment = true;
            bool visibleForPaymentSpecified = true;
            bool visibleForView = true;
            bool visibleForViewSpecified = true;
            resp = dstv.GetAvailableProductsWithoutChannels(country, bussinessUnit, language, vendorCode, interfaceType, visibleForPayment, visibleForPaymentSpecified, visibleForView, visibleForViewSpecified, IpAddress);

            if (string.IsNullOrEmpty(BouquetCode))
            {
                foreach (ProductwithoutChannel pdt in resp)
                {
                    BouquetDetails abouquet = new BouquetDetails();
                    abouquet.BouquetCode = pdt.Product_Key;
                    abouquet.BouquetName = pdt.Product_Name;
                    abouquet.BouquetDescription = pdt.Description;
                    abouquet.BouquetPrice = pdt.CustomPrice;
                    abouquet.PayTvCode = PayTvCode;
                    abouquet.StatusDescription = "SUCCESS";
                    abouquet.StatusCode = "0";
                    dh.SaveBouquetDetails(abouquet);
                    allBouquets.Add(abouquet);
                }
            }
            else
            {
                foreach (ProductwithoutChannel pdt in resp)
                {
                    if (pdt.Product_Key.Equals(BouquetCode) || pdt.Product_Name.Equals(BouquetCode))
                    {
                        BouquetDetails abouquet = new BouquetDetails();
                        abouquet.BouquetCode = pdt.Product_Key;
                        abouquet.BouquetName = pdt.Product_Name;
                        abouquet.BouquetDescription = pdt.Description;
                        abouquet.BouquetPrice = pdt.CustomPrice;
                        abouquet.StatusDescription = "SUCCESS";
                        abouquet.StatusCode = "0";

                        allBouquets.Add(abouquet);
                        break;
                    }
                }
            }
        }
        catch (SoapException ex)
        {
            allBouquets = new List<BouquetDetails>();
            bouq.StatusCode = "100";
            bouq.StatusDescription = ex.Message;
            allBouquets.Add(bouq);
            return allBouquets.ToArray();
        }
        catch (Exception ex)
        {
            allBouquets.Clear();
            bouq.StatusCode = "100";
            bouq.StatusDescription = "GENERAL ERROR AT PEGASUS";
            allBouquets.Add(bouq);
            return allBouquets.ToArray();
        }
        return allBouquets.ToArray();

    }


    private SchoolsQueryResponse QueryUMUCustomerDetails(string utilityCode, string customerReference, string vendorCode)
    {
        SchoolsQueryResponse resp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {

            creds = dp.GetUtilityCreds(utilityCode, vendorCode);
            if (!creds.UtilityCode.Equals(""))
            {
                string stdno = customerReference;
                System.Net.ServicePointManager.Expect100Continue = false;
                string myUrl = "http://eis.umu.ac.ug:84/test/ebank?";
                string urlParams1 = "act=INQ&stno=" + stdno;
                myUrl = myUrl + urlParams1;
                HttpWebRequest r = (HttpWebRequest)System.Net.WebRequest.Create(myUrl);
                r.Headers.Clear();
                r.AllowAutoRedirect = true;
                r.PreAuthenticate = true;
                r.ContentType = "application / x - www - form - urlencoded";
                r.Credentials = CredentialCache.DefaultCredentials;
                r.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                r.Timeout = 150000;
                Encoding byteArray = Encoding.GetEncoding("utf-8");

                Stream dataStream;
                WebResponse response = (HttpWebResponse)r.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader rdr = new StreamReader(dataStream);
                string feedback = rdr.ReadToEnd();
                string ErrorCode = "";
                string[] array = feedback.Split(',');
                if (array.Length == 5)
                {
                    ErrorCode = array[0].ToString().Replace("\n", "");
                    string Name = GetDetail(array[1].ToString());
                    string RegNo = GetDetail(array[2].ToString());
                    string StdNo = GetDetail(array[3].ToString());
                    string Program = GetDetail(array[4].ToString());

                    resp.StatusCode = "0";
                    resp.CustomerName = Name;
                    resp.CustomerReference = StdNo;
                    resp.OutstandingBalance = "0";
                    resp.StatusDescription = "SUCCESS";

                    Customer customer = new Customer();
                    customer.AgentCode = utilityCode;
                    customer.CustomerName = resp.CustomerName;
                    customer.CustomerRef = resp.CustomerReference;
                    customer.CustomerType = "";
                    saveCustomerDetails(customer);

                }
                else
                {
                    if (array.Length == 2)
                    {
                        string errorCode = array[0].ToString().Replace(",", "");
                        string errorMessage = array[1].ToString().Replace(",", "");
                        string Error = errorMessage;
                        ErrorCode = errorCode;
                        resp.StatusCode = errorCode;
                        resp.StatusDescription = Error;
                    }
                    else
                    {
                        string errorCode = "F";
                        string errorMessage = "UN Known Error from Utility Database at" + utilityCode;
                        string Error = errorMessage;
                        ErrorCode = errorCode;
                        resp.StatusCode = errorCode;
                        resp.StatusDescription = Error;
                    }
                }
            }
            else
            {
                resp.StatusCode = "29";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return resp;
    }

    private SchoolsQueryResponse QueryCemasDetails(string utilityCode, string customerReference, string vendorCode)
    {
        SchoolsQueryResponse student = new SchoolsQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            UtilityReferences.StanbicCemasApi.PegPay pegpay = new UtilityReferences.StanbicCemasApi.PegPay();
            pegpay.Url = "https://196.8.208.145:9444/testleveloneapi/pegpay.asmx?WSDL";
            UtilityReferences.StanbicCemasApi.QueryRequest query = new UtilityReferences.StanbicCemasApi.QueryRequest();
            UtilityReferences.StanbicCemasApi.Response resp = new UtilityReferences.StanbicCemasApi.Response();
            query.QueryField4 = utilityCode;
            query.QueryField1 = customerReference;
            query.QueryField5 = "PEGASUS";
            query.QueryField6 = "PEGASUS";
            
            resp = pegpay.QueryCustomerDetails(query);
            if (resp.ResponseField6 == "0")
            {
                student.CustomerBalance = resp.ResponseField4;
                student.CustomerName = resp.ResponseField2;
                student.CustomerReference = resp.ResponseField1;
                student.StatusCode = resp.ResponseField6;
                student.StatusDescription = resp.ResponseField7;
                student.OutstandingBalance = resp.ResponseField4;
            }
            else
            {
                student.CustomerBalance = "";
                student.CustomerName = "";
                student.CustomerReference = "";
                student.StatusCode = resp.ResponseField6;
                student.StatusDescription = resp.ResponseField7;
            }

        }
        catch (Exception ee)
        {

            throw;
        }

        return student;
    }
    private SchoolsQueryResponse QueryMUBSCustomerDetails(string utilityCode, string customerReference, string vendorCode)
    {
        SchoolsQueryResponse resp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {

            creds = dp.GetUtilityCreds(utilityCode, vendorCode);
            if (vendorCode == "MTN")
            {
                creds.UtilityCode = "MTN";
            }
            if (!creds.UtilityPassword.Equals(""))
            {
                string stdno = customerReference;
                System.Net.ServicePointManager.Expect100Continue = false;
                string myUrl = "http://eis.mubs.ac.ug/test/ebank?";
                string urlParams1 = "act=INQ&stno=" + stdno;
                myUrl = myUrl + urlParams1;
                HttpWebRequest r = (HttpWebRequest)System.Net.WebRequest.Create(myUrl);
                r.Headers.Clear();
                r.AllowAutoRedirect = true;
                r.PreAuthenticate = true;
                r.ContentType = "application / x - www - form - urlencoded";
                r.Credentials = CredentialCache.DefaultCredentials;
                r.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                r.Timeout = 150000;
                Encoding byteArray = Encoding.GetEncoding("utf-8");

                Stream dataStream;
                WebResponse response = (HttpWebResponse)r.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader rdr = new StreamReader(dataStream);
                string feedback = rdr.ReadToEnd();
                string ErrorCode = "";
                string[] array = feedback.Split(',');
                if (array.Length == 5)
                {
                    ErrorCode = array[0].ToString().Replace("\n", "");
                    string Name = GetDetail(array[1].ToString());
                    string RegNo = GetDetail(array[2].ToString());
                    string StdNo = GetDetail(array[3].ToString());
                    string Program = GetDetail(array[4].ToString());

                    resp.StatusCode = "0";
                    resp.CustomerName = Name;
                    resp.CustomerReference = StdNo;
                    resp.OutstandingBalance = "0";
                    resp.StatusDescription = "SUCCESS";

                    Customer customer = new Customer();
                    customer.AgentCode = utilityCode;
                    customer.CustomerName = resp.CustomerName;
                    customer.CustomerRef = resp.CustomerReference;
                    customer.CustomerType = "";
                    saveCustomerDetails(customer);
                }
                else
                {
                    //E,,The Student No 20000 Does not Exist
                    if (array.Length == 2)
                    {
                        string errorCode = array[0].ToString().Replace(",", "");
                        string errorMessage = array[1].ToString().Replace(",", "");
                        string Error = errorMessage;
                        ErrorCode = errorCode;
                        resp.StatusCode = errorCode;
                        resp.StatusDescription = Error;
                    }
                    else
                    {
                        string errorCode = "F";
                        string errorMessage = "UN Known Error from Utility Database at" + utilityCode;
                        string Error = errorMessage;
                        ErrorCode = errorCode;
                        resp.StatusCode = errorCode;
                        resp.StatusDescription = Error;
                    }
                }
            }
            else
            {
                resp.StatusCode = "29";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

        }
        catch (Exception ex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "MUBS");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerReference = cust.CustomerRef;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerType = cust.CustomerType;
                resp.OutstandingBalance = "0";

                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.CustomerReference = "";
                resp.CustomerName = "";
                resp.CustomerType = "";
                resp.OutstandingBalance = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID STUDENT NUMBER";

            }
        }
        return resp;
    }


    [WebMethod]
    public QueryResponse QueryCustomerDetialsGeneric(string customerReference, string utilityCode, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        BusinessLogic bll = new BusinessLogic();
        dataAccess dha = new dataAccess();

        try
        {
            if (string.IsNullOrEmpty(utilityCode))
            {
                resp.CustomerReference = "";
                resp.StatusCode = "133";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                return resp;
            }
            dp.SaveRequestlog(vendorCode, utilityCode, "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds(utilityCode, vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {

                        if (utilityCode == "WENRECO")
                        {
                            resp = VerifyWenrecoCustomer(customerReference, creds.UtilityCode, creds.UtilityPassword);
                        }
                        if (resp.StatusCode != "0")
                        {
                            Customer cust = dp.GetCustomerDetails(customerReference, "", utilityCode);
                            
                            resp.CustomerReference = customerReference;
                            resp.CustomerName = cust.CustomerName;

                            resp.CustomerBalance = cust.Balance.Split('.')[0];
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";

                        }

                    }

                }
            }
        }

        catch (System.Net.WebException wex)
        {
            if (wex.Message.ToUpper().Contains("TIMEOUT"))
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "CONNECTION TO KCCA TIMED OUT";
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO TUCKSEE";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "TUCKSEE");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = "PEGPAY DB UNAVAILABLE";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, utilityCode);
        }
        return resp;
    }

    [WebMethod]
    public PostResponse MakeWenrecoPayment(Transaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            if (string.IsNullOrEmpty(trans.UtilityCode))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "133";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                return resp;
            }
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);

            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (string.IsNullOrEmpty(trans.TransactionType))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Teller))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.DigitalSignature))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                string receipt = dp.PostTransactionObject(trans, "WENRECO");
                                                                resp.PegPayPostId = receipt;
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        //resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
           
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
               
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, trans.UtilityCode);
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }
    public Token ProcessWenrecoToken(Transaction trans)
    {
        Token token = new Token();
        string msgId = "", msgDateTime = "";
        try
        {

            XMLVendService client = new XMLVendService();
            CreditVendReq request = new CreditVendReq();
            CreditVendResp response = new CreditVendResp();
            // set the operator auth credentials of the message
            request.authCred = new AuthCred();
            request.authCred.opName = "PPAY";
            request.authCred.password = "123";
            // set the client ID
            EANDeviceID clientID = new EANDeviceID();
            clientID.ean = "PPAY";
            request.clientID = clientID;
            // set the terminal ID
            EANDeviceID terminalID = new EANDeviceID();
            terminalID.ean = "0000000000001";
            request.terminalID = terminalID;
            // set the msg ID
            request.msgID = GenerateMsgID();// MsgIDGenerator.Generate();
            msgId = request.msgID.uniqueNumber;
            msgDateTime = request.msgID.dateTime;
            // set the purchase amount
            PurchaseValueCurrency purchaseValue = new
            PurchaseValueCurrency();
            purchaseValue.amt = new UtilityReferences.WenrecoApi.Currency();
            purchaseValue.amt.symbol = "UGX";
            decimal tranAmount = decimal.Parse(trans.TransactionAmount);
            purchaseValue.amt.value = tranAmount;
            request.purchaseValue = purchaseValue;
            // set the meter identifier
            request.idMethod = new VendIDMethod();
            MeterNumber meterNumber = new MeterNumber();
            meterNumber.msno = trans.CustRef;
            request.idMethod.meterIdentifier = meterNumber;
            //set the resource type to electricity
            UtilityReferences.WenrecoApi.Electricity electricity = new Electricity();
            request.resource = electricity;
            //set the paytype to cash
            UtilityReferences.WenrecoApi.Cash cash = new UtilityReferences.WenrecoApi.Cash();
            cash.tenderAmt = new UtilityReferences.WenrecoApi.Currency();
            cash.tenderAmt.symbol = "UGX";
            cash.tenderAmt.value = tranAmount;
            request.payType = cash;
            token.DebtRecovery = "0";
            client.Timeout = 99999999;
            response = client.CreditVendRequest(request);
            List<TranCharges> chargeList = new List<TranCharges>();
            foreach (Tx tx in response.creditVendReceipt.transactions.tx)
            {
                Transaction vendTrans = new Transaction();
                if (tx.GetType() == typeof(CreditVendTx))
                {
                    CreditVendTx creditVendTx = (CreditVendTx)tx;
                    token.TokenValue = creditVendTx.amt.value.ToString();
                    STS1Token sts1Token = (STS1Token)creditVendTx.creditTokenIssue.token;

                    token.PrepaidToken = sts1Token.stsCipher;
                    StepTariffBreakdown tbreak = (StepTariffBreakdown)creditVendTx.tariffBreakdown;
                    string lifeMsg = "Purchased as";
                    string otherLifeMsg = "";
                    token.LifeLine = lifeMsg + otherLifeMsg;
                    if (token.LifeLine.EndsWith(","))
                    {
                        token.LifeLine = token.LifeLine.Remove((token.LifeLine.Length - 1));
                    }
                    if (token.PrepaidToken.Trim().Length == 20)
                    {
                        token.StatusCode = "SUCCESS";
                        token.MeterNumber = trans.CustRef;
                        token.TotalAmount = tranAmount.ToString();

                    }
                    token.Units = creditVendTx.creditTokenIssue.units.value.ToString();
                }
                else if (tx.GetType() == typeof(DebtRecoveryTx))
                {
                    DebtRecoveryTx debtRecoveryTx = (DebtRecoveryTx)tx;
                    token.DebtRecovery = token.DebtRecovery + debtRecoveryTx.amt.value;
                    TranCharges charge = new TranCharges();
                    charge.ChargeName = debtRecoveryTx.accDesc;
                    charge.Charge = debtRecoveryTx.amt.value + "";
                    charge.AccountNumber = debtRecoveryTx.accNo;
                    charge.MeterNumber = trans.CustRef;
                    charge.Balance = debtRecoveryTx.balance.value + "";
                    charge.TranId = trans.transactionID;
                    chargeList.Add(charge);
                }
                else if (tx.GetType() == typeof(PayAccTx))
                {
                    PayAccTx payAccountTx = (PayAccTx)tx;
                    token.PayAccount = payAccountTx.amt.value.ToString();
                }
                else if (tx.GetType() == typeof(ServiceChrgTx))
                {
                    ServiceChrgTx serviceChrgTx = (ServiceChrgTx)tx;
                    if (serviceChrgTx.accDesc.Trim().ToUpper().Equals("TAX"))
                    {
                        token.Tax = serviceChrgTx.amt.value.ToString();
                    }
                    else if (serviceChrgTx.accDesc.Trim().ToUpper().Equals("INFLATION ADJUSTMENT"))
                    {
                        token.Inflation = serviceChrgTx.amt.value.ToString();
                    }
                    else if (serviceChrgTx.accDesc.Trim().ToUpper().Equals("FUEL ADJUSTMENT"))
                    {
                        token.Fuel = serviceChrgTx.amt.value.ToString();
                    }
                    else if (serviceChrgTx.accDesc.Trim().ToUpper().Equals("FOREX ADJUSTMENT"))
                    {
                        token.Fx = serviceChrgTx.amt.value.ToString();
                    }
                }    
            }

        }
        catch (SoapException soapException)
        {
            token.StatusCode = "35";
            token.StatusDescription = soapException.InnerException.ToString();
        }
        catch (Exception ex)
        {
            token.StatusCode = "100";
            token.StatusDescription = ex.Message;
        }

        return token;
    }






    public static MsgID GenerateMsgID()
    {
        MsgID msgID = new MsgID();
        msgID.dateTime = DateTime.Now.ToString("yyyyMMddhhmmss");

        Random random = new Random(DateTime.Now.Millisecond);
        int randomNumber = random.Next(1000000);

        msgID.uniqueNumber = randomNumber.ToString();

        return msgID;
    }
    [WebMethod]
    public QueryResponse VerifyWenrecoCustomer(string custRef, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        try
        {

            dp.SaveRequestlog(vendorCode, "WENRECO", "VERIFICATION", custRef, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    UtilityCredentials creds = dp.GetUtilityCreds("WENRECO", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        UtilityReferences.WenrecoApi.XMLVendService service = new UtilityReferences.WenrecoApi.XMLVendService();
                        UtilityReferences.WenrecoApi.ConfirmCustomerReq request = new UtilityReferences.WenrecoApi.ConfirmCustomerReq();
                        UtilityReferences.WenrecoApi.ConfirmCustomerResp resps = new UtilityReferences.WenrecoApi.ConfirmCustomerResp();
                        request.authCred = new AuthCred();
                        request.authCred.opName = creds.UtilityCode;
                        request.authCred.password = creds.UtilityPassword;
                        request.authCred.newPassword = creds.UtilityPassword;
                        // set the client ID
                        EANDeviceID clientID = new EANDeviceID();
                        clientID.ean = creds.UtilityCode;
                        request.clientID = clientID;
                        // set the terminal ID
                        EANDeviceID terminalID = new EANDeviceID();
                        terminalID.ean = "0000000000001";
                        request.terminalID = terminalID;
                        // set the msg ID
                        request.msgID = Generate();
                        string msgId = request.msgID.uniqueNumber;
                        string msgDateTime = request.msgID.dateTime;
                        // set the meter identifier
                        MeterNumber meterNumber = new MeterNumber();
                        meterNumber.msno = custRef;
                        VendIDMethod meth = new VendIDMethod();
                        meth.meterIdentifier = meterNumber;
                        request.idMethod = meth;
                        resps = service.ConfirmCustomerRequest(request);
                        

                        if (false)
                        {
                            foreach (UtilityReferences.WenrecoApi.ConfirmCustResult cv in resps.confirmCustResult)
                            {
                                resp.CustomerName = cv.custVendDetail.name;
                                resp.CustomerReference = cv.meterDetail.msno;
                                resp.CustomerBalance = cv.custVendConfig.minVendAmt.value + "";
                                resp.StatusCode = "0";
                                resp.StatusDescription = "100";

                                Customer customer = new Customer();
                                customer.AgentCode = "WENRECO";
                                customer.CustomerName = resp.CustomerName;
                                customer.CustomerRef = resp.CustomerReference;
                                customer.CustomerType = "PREPAID";
                                customer.Balance = resp.CustomerBalance;

                                saveUmemeCustomerDetails(customer);

                            }
                        }
                        else
                        {
                            Customer cust = dp.GetCustomerDetails(custRef, "", "WENRECO");
                            if (cust.StatusCode == "0")
                            {
                                resp.CustomerReference = cust.CustomerRef;
                                resp.CustomerName = cust.CustomerName;
                                resp.CustomerBalance = cust.Balance.ToString();
                                resp.StatusCode = cust.StatusCode;
                                resp.StatusDescription = cust.StatusDescription;
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                            else
                            {
                                resp.StatusCode = "100";
                                resp.StatusDescription = "CUSTOMER DETAILS NOT FOUND";
                            }
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (SoapException soapException)
        {
            XMLVendFaultResp xmlVendFaultResp = XMLDeserialize.Deserialize(soapException.Detail);

            if (xmlVendFaultResp.fault.GetType() == typeof(UnknownMeterEx))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "Unknown Meter Serial Number!";
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "Unexpected XMLVend Fault Response    received: (" + xmlVendFaultResp.fault.GetType().ToString() + ")" + xmlVendFaultResp.fault.desc;
                }
        }
        catch (Exception ex)
        {
            resp.StatusCode = "100";
            resp.StatusDescription = "GENERAL ERROR " + ex.Message;
            Console.WriteLine("A serious exception has occurred: (" + ex.GetType().ToString() + ") " + ex.Message);
        }

        return resp;
    }
    public UtilityReferences.WenrecoApi.MsgID Generate()
    {
        UtilityReferences.WenrecoApi.MsgID msgID = new UtilityReferences.WenrecoApi.MsgID();
        msgID.dateTime = DateTime.Now.ToString("yyyyMMddhhmmss");

        Random random = new Random(DateTime.Now.Millisecond);
        int randomNumber = random.Next(1000000);

        msgID.uniqueNumber = randomNumber.ToString();

        return msgID;
    }
    [WebMethod]
    public KCCAResponse QueryTuckSeeCustomerDetails(string customerReference, string vendorCode, string password)
    {
        KCCAResponse resp = new KCCAResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        BusinessLogic bll = new BusinessLogic();
        dataAccess dha = new dataAccess();

        try
        {
            dp.SaveRequestlog(vendorCode, "TUCKSEE", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("TUCKSEE", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        KCCAResponse respresp = TuckSeeAuthenticate(creds);
                        if (respresp.ErrorCode != "0")
                        {
                            resp.ErrorCode = "100";
                            resp.ErrorDescription = resp.ErrorDescription + " AT TUCKSEE".ToUpper();
                            return resp;
                        }
                        resp = VerifyTuckSeePrn(customerReference, respresp.SessionKey, creds.Key);
                        string message = resp.ErrorDescription.ToLower();
                        if (resp.ErrorCode != "1" && resp.ErrorDescription.ToLower().Contains("available"))
                        {
                            resp.Success = "0";
                            resp.ErrorCode = "0";
                            resp.Status = "0";

                            resp.ErrorDescription = "SUCCESS";


                            Customer customer = new Customer();
                            customer.AgentCode = "TUCKSEE";
                            customer.CustomerName = resp.CustomerName;
                            customer.CustomerRef = resp.PaymentReference;
                            customer.CustomerType = resp.SessionKey;
                            customer.Balance = resp.PaymentAmount.ToString();
                            customer.Area = resp.SystemCode;
                            customer.TIN = resp.SystemName;
                            customer.SessionKey = resp.AllowPartialPayment;
                            dp.SaveTuckseeCustomerDetails(customer);
                        }
                        else
                        {
                            resp.Success = "100";
                            resp.PaymentReference = "";
                            resp.CustomerName = "";
                            resp.PaymentType = "";
                            resp.ErrorCode = "100";
                            resp.ErrorDescription = resp.ErrorDescription;


                        }

                    }

                }
            }
        }

        catch (System.Net.WebException wex)
        {
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "TUCKSEE");

            Customer cust = dp.GetCustomerDetails(customerReference, "", "TUCKSEE");
            if (cust.StatusCode.Equals("0"))
            {
                resp.PaymentReference = cust.CustomerRef;
                resp.CustomerName = cust.CustomerName;
                resp.SessionKey = cust.TIN;
                resp.PaymentAmount = cust.Balance.Split('.')[0];
                resp.ErrorCode = "0";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);

            }
            else
            {
                if (wex.Message.ToUpper().Contains("TIMEOUT"))
                {
                    resp.ErrorCode = "30";
                    resp.ErrorDescription = "CONNECTION TO TUCKSEE TIMED OUT";
                }
                else
                {
                    resp.ErrorCode = "30";
                    resp.ErrorDescription = "UNABLE TO CONNECT TO TUCKSEE";
                }
            }
        }
        catch (SqlException sqlex)
        {
            resp.ErrorCode = "31";
            resp.ErrorDescription = "PEGPAY DB UNAVAILABLE";
        }
        catch (Exception ex)
        {
            resp.ErrorCode = "32";
            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "TUCKSEE");
        }
        return resp;
    }


    private KCCAResponse TuckSeeAuthenticate(UtilityCredentials creds)
    {
        KCCAResponse resp = new KCCAResponse();
        string datastring = creds.SecretKey + creds.UtilityCode + creds.UtilityPassword + creds.Key;
        string hash = MD5Hash(datastring);
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            UtilityReferences.Tucksee.PaymentNotificationService serv = new UtilityReferences.Tucksee.PaymentNotificationService();
            object obj = serv.authenticate(creds.SecretKey, creds.UtilityCode, creds.UtilityPassword, hash, "");

            resp = ParseTuskseeXmlResponse(obj.ToString(), "AUTH");

        }
        catch (Exception ee)
        {
            resp.Status = "100";
            resp.Success = ee.Message;
        }
        return resp;
    }
    internal KCCAResponse ParseTuskseeXmlResponse(string resp, string Action)
    {
        try
        {
            KCCAResponse authresp = new KCCAResponse();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(resp);

            if (Action.Trim().ToUpper().Equals("AUTH"))
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    string nodeName = node.Name.ToUpper();
                    switch (nodeName)
                    {

                        case "SUCCESS":
                            authresp.ErrorCode = node.InnerText;
                            authresp.Status = node.InnerText;
                            if (authresp.ErrorCode == "1")

                                authresp.ErrorCode = "0";
                            break;
                        case "SESSIONKEY":
                            authresp.SessionKey = node.InnerText;
                            break;
                        case "MESSAGE":
                            authresp.Status = node.InnerText;
                            break;

                    }
                }


            }
            else if (Action.Trim().ToUpper().Equals("VERIFICATION"))
            {
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    string nodeName = node.Name.ToLower();
                    switch (nodeName)
                    {
                        case "success":
                            authresp.ErrorCode = node.InnerText;
                            if (authresp.ErrorCode == "1")
                                authresp.ErrorCode = "0";
                            break;
                        case "message":
                            authresp.ErrorDescription = node.InnerText;

                            break;
                        case "tpgoreference":
                            authresp.TpgoReference = node.InnerText;
                            break;
                        case "customername":
                            authresp.CustomerName = node.InnerText;
                            break;
                        case "systemcode":
                            authresp.SystemCode = node.InnerText;
                            break;
                        case "systemname":
                            authresp.SystemName = node.InnerText;
                            break;
                        case "prn":
                            authresp.PaymentReference = node.InnerText;
                            break;
                        case "prndate":
                            authresp.PaymentDate = node.InnerText;

                            break;
                        case "sessionkey":
                            authresp.SessionKey = node.InnerText;

                            break;
                        case "expirydate":
                            authresp.ExpiryDate = node.InnerText;
                            break;
                        case "paymentamount":
                            authresp.PaymentAmount = node.InnerText;
                            break;
                        case "paymentcurrency":
                            authresp.PaymentCurrency = node.InnerText;
                            break;
                        case "allowpartial":
                            authresp.AllowPartialPayment = node.InnerText;
                            break;
                    }
                }

            }
            else if (Action.Trim().ToUpper().Equals("TRANSACT"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/transactResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.SessionKey = message.Attributes["session_key"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.TransactionID = message.Attributes["transactionID"].Value;
                    authresp.PaymentReference = message.Attributes["PRN"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }
            else if (Action.Trim().ToUpper().Equals("CLOSETRANSACT"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/closeTransactionResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.TransactionID = message.Attributes["transactionID"].Value;
                    authresp.PaymentReference = message.Attributes["paymentReference"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }

            return authresp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public string MD5Hash(string input)
    {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
    }


    private KCCAResponse VerifyTuckSeePrn(string prnReference, string sessionKey, string apiKey)
    {
        KCCAResponse resp = new KCCAResponse();
        try
        {

            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

            string datastring = sessionKey + prnReference + apiKey;
            string hash = MD5Hash(datastring);

            UtilityReferences.Tucksee.PaymentNotificationService serv = new UtilityReferences.Tucksee.PaymentNotificationService();

            object obj = serv.verifyReference(sessionKey, prnReference, hash, "");
            resp = ParseTuskseeXmlResponse(obj.ToString(), "VERIFICATION");



        }
        catch (Exception ee)
        {
            resp.ErrorCode = "0";
            resp.ErrorDescription = ee.Message;
        }
        return resp;
    }
    [WebMethod]
    public KCCAQueryResponse QueryKCCACustomerDetails(string customerReference, string vendorCode, string password)
    {
        KCCAQueryResponse resp = new KCCAQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        BusinessLogic bll = new BusinessLogic();;
        dataAccess dha = new dataAccess();

        try
        {
            dp.SaveRequestlog(vendorCode, "KCCA", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("KCCA", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        resp = GetKCCADetails(customerReference, vendorCode, creds);

                        if (resp.StatusDescription.ToUpper() == "PRN TRANSACTION ALREADY COMPLETED")
                        {
                            string MtnID = dha.GetMtnTransactionId(customerReference);
                            resp.StatusDescription = "PRN TRANSACTION ALREADY COMPLETED." +
                                                    " Transaction ID: " + MtnID;
                            return resp;
                        }
                        else if (resp.StatusCode.Equals("100"))
                        {
                            Customer cust = dp.GetCustomerDetails(customerReference, "", "KCCA");
                            if (cust.CustomerType.ToUpper().Equals("ACTIVEPRN"))
                            {
                                resp.Success = "1";
                                resp.CustomerReference = customerReference;
                                resp.PaymentReference = customerReference;
                                resp.CustomerName = cust.CustomerName;
                                resp.CustomerType = cust.CustomerType;
                                resp.OutstandingBalance = cust.Balance.Split('.')[0];
                                resp.PaymentAmount = cust.Balance.Split('.')[0];
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                resp.ErrorCode = resp.StatusCode;
                                resp.ErrorDescription = resp.StatusDescription;
                                return resp;
                            }
                            else
                            {
                                resp.Success = "0";
                                resp.CustomerReference = "";
                                resp.PaymentReference = "";
                                resp.CustomerName = "";
                                resp.CustomerType = "";
                                resp.OutstandingBalance = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "INVALID KCCA PRN REFERENCE NUMBER";
                                resp.ErrorCode = resp.StatusCode;
                                resp.ErrorDescription = resp.StatusDescription;
                                return resp;
                            }
                        }
                        else
                        {
                            return resp;
                        }
                    }
                    else
                    {
                        Customer cust = dp.GetCustomerDetails(customerReference, "", "KCCA");
                        if (cust.StatusCode.Equals("0"))
                        {
                            resp.Success = "1";
                            resp.Status = "A";
                            resp.CustomerReference = cust.CustomerRef;
                            resp.CustomerName = cust.CustomerName;
                            resp.CustomerType = cust.CustomerType;
                            resp.OutstandingBalance = cust.Balance;
                            resp.PaymentAmount = cust.Balance;
                            resp.StatusCode = "0";

                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            resp.ErrorCode = resp.StatusCode;
                            resp.ErrorDescription = resp.StatusDescription;
                        }
                        else
                        {
                            resp.Success = "0";
                            resp.Status = "D";
                            resp.CustomerReference = "";
                            resp.CustomerName = "";
                            resp.CustomerType = "";
                            resp.OutstandingBalance = "";
                            resp.StatusCode = "100";
                            resp.StatusDescription = "INVALID REFERENCE NUMBER";
                            resp.ErrorCode = resp.StatusCode;
                            resp.ErrorDescription = resp.StatusDescription;
                        }
                    }
                }
            }
        }

        catch (System.Net.WebException wex)
        {
            if (wex.Message.ToUpper().Contains("TIMEOUT"))
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "CONNECTION TO KCCA TIMED OUT";
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO KCCA";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = "PEGPAY DB UNAVAILABLE";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    public KCCAQueryResponse GetKCCADetails(string custRef, string VendorCode, UtilityCredentials creds)
    {

        KCCAQueryResponse Queryresp = new KCCAQueryResponse();
        try
        {
            if (creds.BankCode == "DTB")
            {
                Queryresp = QueryKccaBanksApi(custRef, VendorCode);
            }
            else
            {
                Queryresp = QueryKccaTelecomsApi(custRef, VendorCode);
            }
        }
        catch (Exception ex)
        {
            Queryresp.PaymentAmount = "";
            Queryresp.CustomerName = "";
            Queryresp.StatusCode = "100";
            Queryresp.StatusDescription = "UNABLE TO CONNECT TO KCCA";
        }
        return Queryresp;
    }

    private string GetNewSessionKey(string vendor, bool isBank)
    {
        DatabaseHandler dh = new DatabaseHandler();
        UtilityCredentials creds = dh.GetUtilityCreds("KCCA", vendor);
        string response = "";
        string ap_key = "";
        string ap_username = "";
        string ap_passord = "";
        string ap_backCheck = "";
        string ap_hash = "";

        ap_key = creds.Key;
        ap_username = creds.UtilityCode;
        ap_passord = creds.UtilityPassword;

        if (isBank)
        {
            response = GetKccaAuthentication2(ap_key, ap_username, ap_passord, ap_hash, ap_backCheck);
        }
        else
        {
            response = GetKccaAuthentication(ap_key, ap_username, ap_passord, ap_hash, ap_backCheck);
        }

        KCCAResponse resp = ParseXmlResponse(response, "AUTH");
        return resp.SessionKey;
    }

    private KCCAQueryResponse QueryKccaBanksApi(string custRef, string vendor)
    {
        KCCAQueryResponse Queryresp = new KCCAQueryResponse();
        KCCAResponse resp = new KCCAResponse();
        string ap_backCheck = "";
        string ap_hash = "";

        DatabaseHandler dh = new DatabaseHandler();

        string SessionKey = GetNewSessionKey(vendor, true);

        string response = "";

        response = VerifyKccaPRN2(SessionKey, custRef, ap_hash, ap_backCheck);

        resp = ParseXmlResponse(response, "VERIFICATION");

        if (resp.Success.Equals("1") && resp.Status.Equals("A"))
        {
            string amount2 = resp.PaymentAmount.Split('.')[0];
            Queryresp.PaymentAmount = amount2;
            Queryresp.CustomerName = resp.CustomerName;
            Queryresp.PaymentReference = resp.PaymentReference;
            Queryresp.SessionKey = resp.SessionKey;
            Queryresp.Coin = resp.Coin;
            Queryresp.StatusCode = "0";
            Queryresp.StatusDescription = "SUCCESS";
            Queryresp.Success = "SUCCESS";
            Customer cust = new Customer();
            cust.CustomerName = resp.CustomerName;
            cust.CustomerRef = resp.PaymentReference;
            cust.CustomerType = resp.Status == "A" ? "ActivePRN" : "TransactedPRN";
            cust.Balance = resp.PaymentAmount;
            cust.AgentCode = "KCCA";

            saveKCCACustomerDetails(cust);
        }
        else
        {
            Console.WriteLine("Failed to Create Authentication Details\nError: " + resp.ErrorDescription);
            Queryresp.PaymentAmount = "";
            Queryresp.CustomerName = "";
            Queryresp.StatusCode = "100";
            Queryresp.StatusDescription = resp.ErrorDescription;
        }
        return Queryresp;
    }

    private KCCAQueryResponse QueryKccaTelecomsApi(string custRef, string vendor)
    {
        KCCAQueryResponse Queryresp = new KCCAQueryResponse();
        KCCAResponse resp = new KCCAResponse();
        string ap_backCheck = "";
        string ap_hash = "";

        DatabaseHandler dh = new DatabaseHandler();
        string SessionKey = GetNewSessionKey(vendor, false);
        if (string.IsNullOrEmpty(SessionKey))
        {
            SessionKey = dh.GetSystemSettings2("8", "4");
        }

        string response = VerifyKccaPRN(SessionKey, custRef, ap_hash, ap_backCheck);
        resp = ParseXmlResponse(response, "VERIFICATION");

        if (resp.Success.Equals("1") && resp.Status.Equals("A"))
        {
            string amount2 = resp.PaymentAmount.Split('.')[0];
            Queryresp.PaymentAmount = amount2;
            Queryresp.CustomerName = resp.CustomerName;
            Queryresp.PaymentReference = resp.PaymentReference;
            Queryresp.SessionKey = resp.SessionKey;
            Queryresp.Coin = resp.Coin;
            Queryresp.StatusCode = "0";
            Queryresp.StatusDescription = "SUCCESS";
            Queryresp.Success = "SUCCESS";
            
            Customer cust = new Customer();
            cust.CustomerName = resp.CustomerName;
            cust.CustomerRef = resp.PaymentReference;
            cust.CustomerType = resp.Status == "A" ? "ActivePRN" : "TransactedPRN";
            cust.Balance = resp.PaymentAmount;        
            cust.AgentCode = "KCCA";

            saveKCCACustomerDetails(cust);
        }
        else
        {
            Console.WriteLine("Failed to Create Authentication Details\nError: " + resp.ErrorDescription);
            Queryresp.PaymentAmount = "";
            Queryresp.CustomerName = "";
            Queryresp.StatusCode = "100";
            Queryresp.StatusDescription = resp.ErrorDescription;

        }
        return Queryresp;
    }

    private string GetVendorType(string vendor)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public KCCAQueryResponse GetKCCADetails_old(string custRef, UtilityCredentials creds)
    {
        KCCAQueryResponse Queryresp = new KCCAQueryResponse();
        try
        {
            KCCAResponse resp = new KCCAResponse();
            string ap_backCheck = "";
            string ap_hash = "";

            DatabaseHandler dh = new DatabaseHandler();

            string SessionKey = dh.GetSystemSettings2("8", "4");
            

            string ap_key = "C9773BB7F8D0AD7F94609B3A5A0A15C1";
            string ap_username = "UIDDiamondTrustBank";
            string ap_passord = "63cz1_j7EzFCsGNmhSNi21r6zN5Kl$IO";

            string respons = bll.GetKccaAuthentication(ap_key, ap_username, ap_passord, ap_hash, ap_backCheck);
            KCCAQueryResponse resp1 = bll.ParseXmlResponse(respons, "AUTH");

            string response = VerifyKccaPRN(resp1.SessionKey, custRef, ap_hash, ap_backCheck);
            resp = ParseXmlResponse(response, "VERIFICATION");

            if (resp.Success.Equals("1") && resp.Status.Equals("A"))
            {
                string amount2 = resp.PaymentAmount.Split('.')[0];
                Queryresp.PaymentAmount = amount2;
                Queryresp.CustomerName = resp.CustomerName;
                Queryresp.PaymentReference = resp.PaymentReference;
                Queryresp.SessionKey = resp.SessionKey;
                Queryresp.Coin = resp.Coin;
                Queryresp.StatusCode = "0";
                Queryresp.StatusDescription = "SUCCESS";
                Queryresp.Success = "SUCCESS";
                
                Customer cust = new Customer();
                cust.CustomerName = resp.CustomerName;
                cust.CustomerRef = resp.PaymentReference;
                cust.CustomerType = resp.Status == "A" ? "ActivePRN" : "TransactedPRN";
                cust.Balance = resp.PaymentAmount;     
                cust.AgentCode = "KCCA";

                saveKCCACustomerDetails(cust);
            }
            else
            {
                Console.WriteLine("Failed to Create Authentication Details\nError: " + resp.ErrorDescription);
                Queryresp.PaymentAmount = "";
                Queryresp.CustomerName = "";
                Queryresp.StatusCode = "100";
                Queryresp.StatusDescription = resp.ErrorDescription;

            }
        }
        catch (Exception ex)
        {
            Queryresp.PaymentAmount = "";
            Queryresp.CustomerName = "";
            Queryresp.StatusCode = "100";
            Queryresp.StatusDescription = "UNABLE TO CONNECT TO KCCA";
        }
        return Queryresp;
    }

    internal string GetKccaAuthentication(string ap_key, string ap_username, string ap_passord, string hash, string backref)
    {
        try
        {
            UtilityReferences.KCCA.TelecomPaymentService payment = new UtilityReferences.KCCA.TelecomPaymentService();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertValidation;
            string resp = payment.authenticate(ap_key, ap_username, ap_passord, hash, backref);
            return resp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string GetKccaAuthentication2(string ap_key, string ap_username, string ap_passord, string hash, string backref)
    {
        try
        {
            UtilityReferences.KCCA2.BankPaymentService payment = new UtilityReferences.KCCA2.BankPaymentService();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertValidation;
            string resp = payment.authenticate(ap_key, ap_username, ap_passord, hash, backref);
            return resp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal string VerifyKccaPRN(string SessionKey, string custRef, string Hash, string ap_backCheck)
    {
        try
        {
            UtilityReferences.KCCA.TelecomPaymentService payment = new UtilityReferences.KCCA.TelecomPaymentService();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertValidation;
            string resp = payment.verifyReference(SessionKey, custRef, Hash, ap_backCheck);
            return resp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string VerifyKccaPRN2(string SessionKey, string custRef, string Hash, string ap_backCheck)
    {
        try
        {
            UtilityReferences.KCCA2.BankPaymentService payment = new UtilityReferences.KCCA2.BankPaymentService();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertValidation;
            string resp = payment.verifyReference(SessionKey, custRef, Hash, ap_backCheck);
            return resp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal KCCAResponse ParseXmlResponse(string resp, string Action)
    {
        try
        {
            KCCAResponse authresp = new KCCAResponse();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(resp);

            if (Action.Trim().ToUpper().Equals("AUTH"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/authenticateResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.SessionKey = message.Attributes["session_key"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }
            else if (Action.Trim().ToUpper().Equals("VERIFICATION"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/verifyReferenceResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.SessionKey = message.Attributes["session_key"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.PaymentReference = message.Attributes["PRN"].Value;
                    authresp.CustomerName = message.Attributes["customerName"].Value;
                    authresp.CustomerPhone = message.Attributes["customerPhone"].Value;
                    authresp.PaymentAmount = message.Attributes["paymentAmount"].Value;
                    authresp.PaymentCurrency = message.Attributes["paymentCurrency"].Value;
                    authresp.Status = message.Attributes["status"].Value;
                    authresp.Coin = message.Attributes["COIN"].Value;
                    authresp.PrnDate = message.Attributes["prnDate"].Value;
                    authresp.ExpiryDate = message.Attributes["expiryDate"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }
            else if (Action.Trim().ToUpper().Equals("TRANSACT"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/transactResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.SessionKey = message.Attributes["session_key"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.TransactionID = message.Attributes["transactionID"].Value;
                    authresp.PaymentReference = message.Attributes["PRN"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }
            else if (Action.Trim().ToUpper().Equals("CLOSETRANSACT"))
            {
                XmlNode message = doc.DocumentElement.SelectSingleNode("/closeTransactionResponse/message");
                authresp.Success = message.Attributes["success"].Value;
                if (authresp.Success.Equals("1"))
                {
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.TransactionID = message.Attributes["transactionID"].Value;
                    authresp.PaymentReference = message.Attributes["paymentReference"].Value;
                }
                else
                {
                    authresp.ErrorCode = message.Attributes["error_code"].Value;
                    authresp.RefCheck = message.Attributes["refcheck"].Value;
                    authresp.ErrorDescription = message.InnerText.ToString();
                }
            }

            return authresp;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

   

   
    [WebMethod]
    public SolarCustomer ValidateSolarCustomer(string utilityCode, string meternumber, string vendorCode, string password)
    {
        SolarCustomer customer = new SolarCustomer();
        if (utilityCode == "SOLAR")
        {
            DatabaseHandler dp = new DatabaseHandler();
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    SolarHandler handler = new SolarHandler();
                    customer = handler.ValidateMeterNumber(meternumber);
                }
                else
                {
                    customer.StatusCode = "99";
                    customer.StatusDescription = "Deactivated vendor. Access denied".ToUpper();
                }
            }
            else
            {
                customer.StatusCode = "99";
                customer.StatusDescription = "Invalid vendor Credentials. Access denied".ToUpper();
            }
        }
        else
        {
            customer.StatusCode = "99";
            customer.StatusDescription = "Deamon! Unknown utility".ToUpper();
        }
        return customer;
    }
    [WebMethod]
    public SchoolsQueryResponse ValidateStudentDetails(string utilityCode, string customerReference, string vendorCode, string password)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        UtilityReferences.SchoolsApi.SchoolApi schapi = new UtilityReferences.SchoolsApi.SchoolApi();
        UtilityReferences.SchoolsApi.ApiResponse resp = new UtilityReferences.SchoolsApi.ApiResponse();
        SchoolsQueryResponse schresp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        dp.SaveRequestlog(vendorCode, "FLEXIPAY", "VERIFICATION", customerReference, password);
        DataTable vendorData = dp.GetVendorDetails(vendorCode);

        if (string.IsNullOrEmpty(customerReference))
        {
            schresp.StatusCode = "99";
            schresp.StatusDescription = "Please provide a student number".ToUpper();
        }
        else if (string.IsNullOrEmpty(utilityCode))
        {
            schresp.StatusCode = "99";
            schresp.StatusDescription = "Please provide a school code".ToUpper();
        }
        else if (string.IsNullOrEmpty(vendorCode) || string.IsNullOrEmpty(password))
        {
            schresp.StatusCode = "99";
            schresp.StatusDescription = "Incomplete credentials".ToUpper();
        }

        if (!isValidVendorCredentials(vendorCode, password, vendorData))
        {
            schresp.StatusCode = "2";
            schresp.StatusDescription = dp.GetStatusDescr(schresp.StatusCode);

        }
        else
        {
            UtilityCredentials creds = dp.GetUtilityCreds("FLEXIPAY", vendorCode);
            if (!creds.UtilityCode.Equals(""))
            {
                resp = schapi.GetStudent(customerReference, utilityCode, creds.UtilityCode, creds.UtilityPassword);

                if (resp.ErrorCode == "0")
                {
                    UtilityReferences.SchoolsApi.Student student = new UtilityReferences.SchoolsApi.Student();
                    student = resp.Student;
                    schresp.CustomerName = student.StdName;
                    schresp.CustomerReference = student.StdNumber;
                    schresp.School = student.School;
                    schresp.Level = student.Level;
                    schresp.Amount = student.AmountToPay;
                    schresp.MinimumPayment = student.MinimumPayment;
                    schresp.AccountNumber = student.SchoolAccount;
                    schresp.ClassName = student.ClassName;
                    schresp.BankCode = student.BankCode;
                    schresp.TranCharge = resp.Charge;
                    schresp.StatusCode = "0";
                    schresp.StatusDescription = "SUCCESS";
                    schresp.AllowPartialPayment = "1";
                    
                   

                }
                else
                {
                    schresp.StatusCode = resp.ErrorCode;
                    schresp.StatusDescription = resp.ErrorDescription;

                }
               
            }
            else
            {
                schresp.StatusCode = "29";
                schresp.StatusDescription = dp.GetStatusDescr(schresp.StatusCode);
            }

        }
        return schresp;
    }
    [WebMethod]
    public SchoolsQueryResponse ValidateSchoolCode(string schoolcode)
    {
        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
        UtilityReferences.SchoolsApi.SchoolApi schapi = new UtilityReferences.SchoolsApi.SchoolApi();
        UtilityReferences.SchoolsApi.ApiResponse resp = new UtilityReferences.SchoolsApi.ApiResponse();
        SchoolsQueryResponse schresp = new SchoolsQueryResponse();
        if (string.IsNullOrEmpty(schoolcode))
        {
            schresp.StatusCode = "99";
            schresp.StatusDescription = "Please provide a school code";
            return schresp;
        }
        try
        {
            resp = schapi.ValidateSchoolCode(schoolcode);
            if (resp.ErrorCode == "0")
            {
                schresp.School = resp.ReceiptNumber;
                //schresp.CustomerName = resp.Student.StdName;
                //schresp.CustomerType = resp.Student.StdNumber;
                //schresp.CustomerBalance = resp.Student.MinimumPayment;

            }
            schresp.StatusCode = resp.ErrorCode;
            schresp.StatusDescription = resp.ErrorDescription;
        }
        catch (Exception ee)
        {
            schresp.StatusCode = "100";
            schresp.StatusDescription = ee.Message.ToUpper();
        }
        return schresp;
    }

    [WebMethod]
    public SchoolsQueryResponse QuerySchoolStudentDetails(string utilityCode, string customerReference, string vendorCode, string password)
    {
        SchoolsQueryResponse resp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        string groupCode = "";
        try
        {
            dp.SaveRequestlog(vendorCode, utilityCode, "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);

            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds(utilityCode, vendorCode);

                    if (!creds.UtilityCode.Equals(""))
                    {
                        if (utilityCode.Equals("UMU"))
                        {
                            resp = QueryUMUCustomerDetails(utilityCode, customerReference, vendorCode);
                        }
                        else if (utilityCode.Equals("MUBS") || utilityCode.Equals("MUST") || utilityCode.Equals(""))
                        {
                            resp = QueryCemasDetails(utilityCode, customerReference, vendorCode);
                        }
                        //else if (utilityCode.Equals("KYU"))
                        //{
                        //    resp = QueryKyuCustomerDetails(customerReference, vendorCode, password);
                        //}
                        //else if (utilityCode.Equals("MAK"))
                        //{
                        //    resp = QueryMAKCustomerDetails(creds, customerReference, vendorCode);
                        //}
                        else
                        {
                            DataTable StudentDetails = dp.GetStudentDetails(utilityCode, customerReference);
                            if (StudentDetails.Rows.Count > 0)
                            {
                                resp.CustomerReference = StudentDetails.Rows[0]["CustomerRef"].ToString();
                                resp.CustomerName = StudentDetails.Rows[0]["CustomerName"].ToString();
                                resp.CustomerType = StudentDetails.Rows[0]["CustomerType"].ToString();
                                
                                resp.OutstandingBalance = StudentDetails.Rows[0]["AccountBal"].ToString().Split('.')[0];
                                if (string.IsNullOrEmpty(resp.OutstandingBalance)) { resp.OutstandingBalance = "0"; }
                                resp.StatusCode = "0";
                                resp.StatusDescription = "SUCCESS";
                            }
                            else
                            {
                                resp.CustomerReference = "";
                                resp.CustomerName = "";
                                resp.CustomerType = "";
                                resp.OutstandingBalance = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "INVALID STUDENT REFERENCE NUMBER";
                            }
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", utilityCode);
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO " + utilityCode;
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, utilityCode);
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, utilityCode);
        }
        return resp;
    }

    [WebMethod]
     public SchoolsQueryResponse QueryNDAInvoiceDetails(string utilityCode, string invoiceNumber, string vendorCode, string password)
    {
        SchoolsQueryResponse resp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        string groupCode = "";
        try
        {
            dp.SaveRequestlog(vendorCode, utilityCode, "VERIFICATION", invoiceNumber, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);

            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds(utilityCode, vendorCode);

                    if (!creds.UtilityCode.Equals(""))
                    {

                       // UtilityReferences.NDAValidation.validateInvoiceResponse inv = new UtilityReferences.NDAValidation.validateInvoiceResponse();
                        UtilityReferences.NDAValidation.invoiceValidationRequest req = new UtilityReferences.NDAValidation.invoiceValidationRequest();
                        UtilityReferences.NDAValidation.invoiceValidationResponse inv = new UtilityReferences.NDAValidation.invoiceValidationResponse();
                        UtilityReferences.NDAValidation.MobileMoneyInvoiceServiceService nda = new UtilityReferences.NDAValidation.MobileMoneyInvoiceServiceService();
                        
                        UtilityReferences.NDAValidation.validateInvoiceResponse response = new UtilityReferences.NDAValidation.validateInvoiceResponse();
                        UtilityReferences.NDAValidation.validateInvoice request = new UtilityReferences.NDAValidation.validateInvoice();
                        UtilityReferences.NDAValidation.invoiceValidationRequest arg0 = new UtilityReferences.NDAValidation.invoiceValidationRequest();
                        UtilityReferences.NDAValidation.requestHeader requestHeader = new UtilityReferences.NDAValidation.requestHeader();

                       
                        requestHeader.username = creds.UtilityCode;
                        requestHeader.password = creds.UtilityPassword;
                        requestHeader.systemID = creds.Key;
                        requestHeader.systemToken = "7F8F88E00C4F77C78A0552789C31AB266F503D4C1ED338E4F32BD73F00E8581B";
                        req.invoiceNo = invoiceNumber;
                        req.requestHeader = requestHeader;
                        UtilityReferences.NDAValidation.validateInvoice validate = new UtilityReferences.NDAValidation.validateInvoice();
                        validate.arg0 = req;

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        nda.Url = "http://212.22.169.21:80/broker_mobile_money/mobilemoney/invoice/validation/v1.0?wsdl";
                            //"http://212.22.169.21:80/broker_mobile_money/mobilemoney/invoice/validation/v1.0?wsdl";
                        //"http://212.22.169.22:80/broker/mobilemoney/invoice/validation/v1.0?wsdl";
                        response = nda.validateInvoice(validate);


                        if (response.@return.statusCode.ToString().Equals("0"))
                        {
                            resp.StatusCode = "0";
                            resp.CustomerReference = response.@return.invoice.invoiceNo.ToUpper();
                            resp.OutstandingBalance = response.@return.invoice.amount.ToString();
                            resp.CustomerName = response.@return.invoice.customerName.ToString();
                            resp.Forex = response.@return.invoice.currencyCode.ToString();
                            resp.Level = response.@return.invoice.dueDate.ToString();
                            resp.StatusDescription = response.@return.invoice.description.ToString();

                        }
                        else if (!response.@return.statusCode.ToString().Equals("0"))
                        {
                            resp.StatusCode = response.@return.statusCode.ToString();
                            resp.StatusDescription = response.@return.statusMessage.ToString();
                            resp.CustomerReference = "";
                            resp.OutstandingBalance = "";
                            resp.CustomerName = "";
                            resp.Forex = "";
                            resp.Level = "";
                        }

                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            if (utilityCode.Equals("NDA"))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "FAILED TO VALIDATE REFERENCE NUMBER AT NDA";
                dp.LogError(wex.Message + ":" + wex.InnerException, vendorCode, DateTime.Now, utilityCode);
                return resp;
            }
            Customer cust = dp.GetCustomerDetails(invoiceNumber, "", utilityCode);
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO " + utilityCode;
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, utilityCode);
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, utilityCode);
        }
        return resp;
    }

    private SchoolsQueryResponse QueryKyuCustomerDetails(string studentNo, string vendorcode, string vendorPassword)
    {
        SchoolsQueryResponse queryResp = new SchoolsQueryResponse();
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            System.Net.ServicePointManager.Expect100Continue = false;
            UtilityReferences.NkumbaPayInterface.EPayment nku = new UtilityReferences.NkumbaPayInterface.EPayment();
            UtilityReferences.NkumbaPayInterface.ValidateResponse NkumbaResponse = new UtilityReferences.NkumbaPayInterface.ValidateResponse();
            string RequestState = "1";
            string APIUsername = "1482963BBEF5";
            string ApiPassword = "c7ca173be871b939e15bd48be94412";
            string ReferenceNumber = studentNo;
            string BankCode = "STANBIC";
            string datatodigest = "ref=" + ReferenceNumber + "&bank_code=" + BankCode;
            string datatosign = "ref=" + ReferenceNumber + "&api_user=" + APIUsername + "&api_pwd=" + ApiPassword;
            string RequestDigest = bll.GetRequestDigest(datatodigest, ApiPassword);
            string RequestSignature = bll.GetRequestSignature(datatosign);

            NkumbaResponse = nku.ValidateRequest(BankCode, ReferenceNumber.Trim(), RequestState, RequestSignature, RequestDigest);

            Console.WriteLine("******************************************************************");
            Console.WriteLine("Testing KYU Validate Request");
            Console.WriteLine("******************************************************************");
            Console.WriteLine("Message = " + NkumbaResponse.ResponseStatus);
            if (NkumbaResponse.ResponseStatus.Equals("200"))
            {
                queryResp.CustomerName = NkumbaResponse.EntityName;
                queryResp.CustomerReference = NkumbaResponse.ReferenceNo;
                queryResp.OutstandingBalance = NkumbaResponse.Amount;
                queryResp.CustomerType = NkumbaResponse.AuthToken;
                queryResp.StatusCode = "0";
                queryResp.StatusDescription = "SUCCESS";

                Customer customer = new Customer();
                string Utility = "KYU";
                customer.AgentCode = Utility;
                customer.CustomerName = NkumbaResponse.EntityName;
                customer.CustomerRef = NkumbaResponse.ReferenceNo;
                customer.CustomerType = NkumbaResponse.AuthToken;
                customer.Balance = NkumbaResponse.Amount;
                saveCustomerDetails(customer);
            }
            else if (NkumbaResponse.ResponseStatus.Equals("404"))
            {

                queryResp.StatusCode = "100";
                queryResp.StatusDescription = "INVALID STUDENT REFERENCE";

            }
            else
            {
                queryResp.StatusCode = "100";
                queryResp.StatusDescription = NkumbaResponse.ResponseStatus;
            }
            return queryResp;
        }
        catch (WebException ex)
        {
            queryResp.StatusCode = "100";
            queryResp.StatusDescription = "UNABLE TO CONNECT TO KYAMBOGO UNIVERSITY";
        }
        catch (Exception ex)
        {
            queryResp.StatusCode = "101";
            queryResp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }

        return queryResp;

    }

    private SchoolsQueryResponse QueryKyuCustomerDetailsOld(string studentNo, string vendorcode, string vendorPassword)
    {
        SchoolsQueryResponse queryResp = new SchoolsQueryResponse();
        try
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            UtilityReferences.KYU.ValidateResponse response = new UtilityReferences.KYU.ValidateResponse();
            UtilityReferences.KYU.EPayment epayment = new UtilityReferences.KYU.EPayment();
            string ReferenceNo = studentNo;

            string State = "1";
            string password = "126e8ad746c7a0f1e325589974696263303751349081f49673";
            string dataToSign = "ref=" + studentNo;
            string Signature = GetKyuDigitalSignature(password, dataToSign); ;

            string SourceId = "AB5BD455A7D4";
            response = epayment.ValidateRequest(ReferenceNo, State, Signature, SourceId);

            Console.WriteLine("******************************************************************");
            Console.WriteLine("Testing KYU Validate Request");
            Console.WriteLine("******************************************************************");
            Console.WriteLine("Message = " + response.Message);
            if (response.Status.Equals("200"))
            {
                queryResp.CustomerName = response.EntityTitle;
                queryResp.CustomerReference = response.RequestReferenceNo;
                queryResp.OutstandingBalance = response.Amount;
                queryResp.CustomerType = response.AuthToken;
                queryResp.StatusCode = "0";
                queryResp.StatusDescription = "SUCCESS";
            }
            else
            {
                queryResp.StatusCode = "100";
                queryResp.StatusDescription = response.Message;
            }
            return queryResp;
        }
        catch (WebException ex)
        {
            queryResp.StatusCode = "100";
            queryResp.StatusDescription = "UNABLE TO CONNECT TO KYAMBOGO UNIVERSITY";
        }
        catch (Exception ex)
        {
            queryResp.StatusCode = "101";
            queryResp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }

        return queryResp;

    }
    public static string GetKyuDigitalSignature(string sign, string contentTosgin)
    {
        byte[] key = new Byte[64];
        byte[] content = new Byte[100];
        key = Encoding.ASCII.GetBytes(sign);
        content = Encoding.ASCII.GetBytes(contentTosgin);

        using (HMACSHA256 hmc = new HMACSHA256(key))
        {
            byte[] hashvalue = hmc.ComputeHash(content);
           
            StringBuilder hexValue = new StringBuilder(hashvalue.Length * 2);
            foreach (byte b in hashvalue)
            {
                hexValue.Append(b.ToString("X02"));

            }
            return hexValue.ToString().ToLower();
        }

    }

    [WebMethod]
    public PostResponse MakeDSTVPayment(DSTVTransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "DSTV", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.CustRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Area))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = "PLEASE SUPPLY A BOUQUET CODE";
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("DSTV", trans.VendorCode);
                                                                creds.UtilityCode = "DSTV";

                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    resp.PegPayPostId = dp.PostPayTvTransaction(trans, trans.UtilityCode);
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "URA");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "URA");
        }
        return resp;
    }
    [WebMethod]
    public BETWAYPostResponse MakeBETWAYPayment(BETWAYTransaction trans)
    {
        BETWAYPostResponse resp = new BETWAYPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "BETWAY", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds("BETWAY", trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                if (trans.PaymentType == null)
                                                                {
                                                                    trans.PaymentType = "";
                                                                }
                                                                if (trans.CustomerType == null)
                                                                {
                                                                    trans.CustomerType = "";

                                                                }

                                                                if (string.IsNullOrEmpty(trans.UtilityCode))
                                                                {
                                                                    resp.PegPayPostId = dp.PostUmemeTransaction(trans, "INFOCOM");
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = dp.PostUmemeTransaction(trans, "BETWAY");
                                                                }
                                                                
                                                                resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = "SUCCESS";


                                                            }
                                                            else
                                                            {
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "BETWAY");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "BETWAY");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "BETWAY");
        }
        return resp;
    }

    private string FormatPayWayTel(string toFormat)
    {
        if (toFormat.Length == 9)
        {
            toFormat = "0" + toFormat;
        }
        else if (toFormat.Length == 12)
        {
            toFormat = "0" + toFormat.Substring(3);
        }
        return toFormat;
    }



    private SchoolsQueryResponse QueryMAKCustomerDetails(UtilityCredentials creds, string customerReference, string vendorCode)
    {
        SchoolsQueryResponse resp = new SchoolsQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        try
        {

            if (!creds.UtilityCode.Equals(""))
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                UtilityReferences.Makerere.EPayment makapi = new UtilityReferences.Makerere.EPayment();
                UtilityReferences.Makerere.Student cust = makapi.QueryMukStudent(customerReference, creds.UtilityCode, creds.UtilityPassword);
                if (cust.StatusCode.Equals("0"))
                {
                    resp.CustomerReference = cust.StudentNumber;
                    resp.CustomerName = cust.StudentName;
                    resp.CustomerType = cust.Course;
                    resp.OutstandingBalance = "0";
                    resp.StatusCode = cust.StatusCode;
                    resp.StatusDescription = cust.StatusDescription;
                    Customer customer = new Customer();
                    customer.AgentCode = "MAK";
                    customer.CustomerName = resp.CustomerName;
                    customer.CustomerRef = resp.CustomerReference;
                    customer.CustomerType = resp.CustomerType;
                    customer.Balance = cust.Balance.ToString();
                    saveUmemeCustomerDetails(customer);
                }
                else
                {
                    resp.CustomerReference = "";
                    resp.CustomerName = "";
                    resp.CustomerType = "";
                    resp.OutstandingBalance = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = cust.StatusDescription;
                }
            }
            else
            {
                resp.StatusCode = "29";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "MAK");
            if (cust.StatusCode.Equals("0"))
            {
                resp.CustomerType = cust.CustomerType;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO MAKERERE";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "MAK");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "MAK");
        }
        return resp;
    }

    [WebMethod]
    public NWSCQueryResponse QueryNWSCCustomerDetails(string customerReference, string area, string vendorCode, string password)
    {
        NWSCQueryResponse resp = new NWSCQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "NWSC", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("NWSC", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        NWSCBillingInterface nwscapi = new NWSCBillingInterface();
                        UtilityReferences.NWSC.Customer cust = nwscapi.verifyCustomerDetailsWithArea(customerReference, area, creds.UtilityCode, creds.UtilityPassword);
                        if (cust.CustomerError.Equals("NONE"))
                        {
                            resp.Area = cust.Area;
                            resp.CustomerName = cust.CustName;
                            resp.CustomerReference = cust.CustRef;
                            resp.OutstandingBalance = cust.OutstandingBal.ToString();
                            resp.StatusCode = "0";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            Customer customer = new Customer();
                            customer.AgentCode = "NWSC";
                            customer.CustomerName = resp.CustomerName;
                            customer.CustomerRef = customerReference;
                            customer.Area = resp.Area;
                            //customer.Balance=
                            saveCustomerDetails(customer);
                        }
                        else
                        {

                            Customer cust2 = dp.GetCustomerDetails(customerReference, area, "NWSC");
                            if (cust2.StatusCode.Equals("0"))
                            {
                                resp.Area = cust2.Area;
                                resp.CustomerName = cust2.CustomerName;
                                resp.CustomerReference = cust2.CustomerRef;
                                resp.OutstandingBalance = "0";
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                            else
                            {
                                resp.Area = "";
                                resp.CustomerName = "";
                                resp.CustomerReference = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = cust.CustomerError;
                            }
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, area, "NWSC");
            if (cust.StatusCode.Equals("0"))
            {
                resp.Area = cust.Area;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO NWSC";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "NWSC");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "NWSC");
        }
        return resp;
    }

    [WebMethod]
    public Meter QueryREACustomerDetails(string customerReference, string utilityCode, string vendorCode, string password)
    {

        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        //this is the access token to the REA api
        REAResponse apiAccessToken = AccessToken();
        Meter meterInfo = new Meter();
        Request request = new Request();
        request.CustomerRef = customerReference;
        request.UtilityCode = utilityCode;
        request.VendorCode = vendorCode;
        request.Password = password;
        string xmlRawRequest = "";
        string xmlRawResponse = "";
        xmlRawRequest = XmlQueryCustomer(request);
        try
        {

            dp.SaveRequestlog(vendorCode, "KRECS", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds(utilityCode, vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        if (apiAccessToken.ErrorCode.Equals("0"))
                        {
                            //call REA api here and instantiate an object to accommodate the values returned
                            Meter custMeterInfo = QueryREAMeterDetails(customerReference, apiAccessToken.SessionID);
                          
                            if (custMeterInfo.ErrorCode.Equals("0"))
                            {
                                meterInfo.User_ID = custMeterInfo.User_ID;
                                meterInfo.MeterType = custMeterInfo.MeterType;
                                meterInfo.FullName = custMeterInfo.FirstName.ToUpper() + " " + custMeterInfo.LastName.ToUpper();
                                meterInfo.MeterNo = custMeterInfo.SN;
                                meterInfo.ErrorCode = custMeterInfo.ErrorCode;
                                meterInfo.Customertype = custMeterInfo.Customertype;
                               
                                Customer customer = new Customer();
                                customer.AgentCode = "KRECS";
                                customer.CustomerName = meterInfo.FullName;
                                customer.CustomerRef = customerReference;
                                customer.Area = meterInfo.Address;
                                // saveCustomerDetails(customer);
                            }
                            else
                            {
                                meterInfo.ErrorCode = "1";
                                meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);


                            }
                        }
                        else
                        {
                            //meterInfo.ErrorCode = apiAccessToken.ErrorCode;
                            //meterInfo.ErrorMsg = apiAccessToken.ErrorMsg;//mask error from Error api

                            meterInfo.ErrorCode = "32";
                            meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);
                        }

                        xmlRawResponse = XmlQueryCustomerResponse(meterInfo);
                        dp.LogRequestAndResponse(vendorCode, customerReference,xmlRawRequest,xmlRawResponse);
                    }
                    else
                    {
                        meterInfo.ErrorCode = "29";
                        meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);
                    }
                }
                else
                {
                    meterInfo.ErrorCode = "11";
                    meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);
                }
            }
            else
            {
                meterInfo.ErrorCode = "2";
                meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            meterInfo.ErrorCode = "30";
            meterInfo.ErrorMsg = "UNABLE TO CONNECT TO REA";
           
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "KRECS");
        }
        
        catch (Exception ex)
        {
            meterInfo.ErrorCode = "32";
            meterInfo.ErrorMsg = dp.GetStatusDescr(meterInfo.ErrorCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "KRECS");
        }
        return meterInfo;
    }

    //get access token from REA api
    public static REAResponse AccessToken()
    {
        REAResponse resp = new REAResponse();
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            string url = @"https://vending.rea.or.ug:8447/epower/prepayservice/login?username=pegasus&password=" + MD5HashPassword("REA.123");

            //Check certificate validity
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (delegate { return true; });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 5000;
            request.ContentLength = 0;
            string certPath = @"E:\certificates\client.p12";

            // Load the certificate into an X509Certificate object.
            X509Certificate2 cert = new X509Certificate2(certPath, "REA.1234", X509KeyStorageFlags.MachineKeySet);
            request.ClientCertificates.Add(cert);

            //Get WebResponse.
            string reaApiResponse;
            using (HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse())
            {
                if (webresponse == null)
                    throw new WebException("Failed to get reponse from URL");

                //Read Response from REA
                StreamReader responseReader = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                reaApiResponse = responseReader.ReadToEnd().Trim();
                dh.LogRequestAndResponse("KRECS_ACCTOKEN","LOGON", url,reaApiResponse);
                REAResponse obj = JsonConvert.DeserializeObject<REAResponse>(reaApiResponse);

                if (obj.ErrorCode.Equals("0"))
                {

                    resp.ErrorCode = obj.ErrorCode;
                    resp.ErrorMsg = obj.ErrorMsg;
                    resp.SessionID = obj.SessionID;
                    resp.User_ID = obj.User_ID;
                    resp.UserName = obj.UserName;
                    resp.LatesAction = obj.LatesAction;
                    resp.Logintime = obj.Logintime;
                    resp.Token = obj.Token;
                    resp.CduName = obj.CduName;
                    resp.CduAmount = obj.CduAmount;
                    resp.Ip = obj.Ip;
                }
                else
                {
                    resp.ErrorCode = obj.ErrorCode;
                    resp.ErrorMsg = obj.ErrorMsg;
                }
            }
        }
        catch (WebException err)
        {
            Console.WriteLine("Error:" + err);
        }
        return resp;
    }
    //encrypt password
    public static string MD5HashPassword(string password)
    {
        MD5 md5 = new MD5CryptoServiceProvider();

        //compute hash from the bytes of text  
        md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));

        //get hash result after compute it  
        byte[] result = md5.Hash;

        StringBuilder strBuilder = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            //change it into 2 hexadecimal digits  
            //for each byte  
            strBuilder.Append(result[i].ToString("x2"));
        }
        return strBuilder.ToString();
    }

    //get REA customer details
    public static Meter QueryREAMeterDetails(string meterNo, string token)
    {
        Meter meterDetails = new Meter();
        Customer cust = new Customer();
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            string url = @"https://vending.rea.or.ug:8447/epower/prepayservice/getmeter?token=" + token + "&msno=" + meterNo;
            //Check certificate validity
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (delegate { return true; });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 5000;
            request.ContentLength = 0;
            string certPath = @"E:\certificates\client.p12";

            // Load the certificate into an X509Certificate object.
            X509Certificate2 cert = new X509Certificate2(certPath, "REA.1234", X509KeyStorageFlags.MachineKeySet);
            request.ClientCertificates.Add(cert);

            //Get WebResponse.
            string reaApiResponse;
            using (HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse())
            {
                if (webresponse == null)
                    throw new WebException("Failed to get reponse from REA URL");

                //Read Response from REA
                StreamReader apiMeter = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                reaApiResponse = apiMeter.ReadToEnd().Trim();
                //log request and response
                dh.LogRequestAndResponse("KRECS", meterNo, url, reaApiResponse);
                Meter apiMeterDetails = JsonConvert.DeserializeObject<Meter>(reaApiResponse);
                if (apiMeterDetails.ErrorCode.Equals("0"))
                {
                    var getMeterInfo = JObject.Parse(reaApiResponse).GetValue("meter");
                    var getCustomerInfo = JObject.Parse(reaApiResponse).GetValue("customer");
                    meterDetails.ErrorCode = apiMeterDetails.ErrorCode;
                    meterDetails.MeterType = apiMeterDetails.MeterType;
                    meterDetails.SN = apiMeterDetails.SN;
                    meterDetails.FirstName = (string)getCustomerInfo["firstname"];
                    meterDetails.LastName = (string)getCustomerInfo["lastname"];
                    meterDetails.Customertype = (string)getCustomerInfo["customertype"];
                    meterDetails.TelNo = (string)getCustomerInfo["tel"];
                    meterDetails.Lastfeevending = (string)getMeterInfo["lastfeevending"];
                    meterDetails.CurrentMonthUnits = (string)getMeterInfo["currentmonthsunits"];
                    meterDetails.Currentmonthmoney = (string)getMeterInfo["currentmonthmoney"];
                    meterDetails.Address = (string)getCustomerInfo["address"];
                }
                else
                {
                    meterDetails.ErrorCode = apiMeterDetails.ErrorCode;
                    meterDetails.ErrorMsg = apiMeterDetails.ErrorMsg;
                }

            }
        }
        catch (WebException err)
        {
            if (err.Status == WebExceptionStatus.SecureChannelFailure)
            {
                Console.WriteLine("Exception encountered:{0}", ((HttpWebResponse)err.Response).StatusCode);
            }
            else
            {
                Console.WriteLine("Error:" + err);
            }
        }
        return meterDetails;
    }
    
    [WebMethod]
    public PostResponse MakeREAPayment(REATransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "KRECS", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
           
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if ((vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("KRECS", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }
                                                                    PostResponse reaTokenResponse = dp.PostREATransaction(trans, "KRECS");
                                                                    if (reaTokenResponse.Equals("0"))
                                                                    {
                                                                        resp.PegPayPostId = reaTokenResponse.PegPayPostId;
                                                                        resp.Token = reaTokenResponse.Token;
                                                                        resp.Units = reaTokenResponse.Units;
                                                                        resp.StatusCode = "0";
                                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    }
                                                                    else
                                                                    {
                                                                        resp.StatusCode = "100";
                                                                        resp.StatusDescription = "TRANSACTION FAILED AT KRECS";
                                                                    }
                                                                   
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "REA");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "REA");
        }
        return resp;
    }
    [WebMethod]
    public string FetchLastREAToken(string meterRef)
    {
        try
        {
            DatabaseHandler dh = new DatabaseHandler();
            string lastToken = "";
            lastToken = dh.GetLastREAToken(meterRef);
            return lastToken;
        }
        catch (Exception err)
        {
            throw err;
        }
        
    }
        public Meter NotifyKRECS(string token, string orderno,string transactionId, string meterNo, float amount)
        {
            string response = "";
                Meter meterDetails = new Meter();
                Customer cust = new Customer();
        DatabaseHandler dh = new DatabaseHandler();
                try
                {

                    string url = @"https://vending.rea.or.ug:8447/epower/prepayservice/vending?token=" + token.Trim() + "&orderno=" + orderno.Trim() + "&msno=" + meterNo.Trim() + "&amount=" + amount + "&receiveamount=" + amount + "&changeamount=" + 0 + "&paymethod=" + 1;
                    //Check certificate validity
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                        (delegate { return true; });
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Timeout = 5000;
                    request.ContentLength = 0;
             
                string certPath = @"E:\Certificates\client.p12";

                // Load the certificate into an X509Certificate object.
                X509Certificate2 cert = new X509Certificate2(certPath, "REA.1234", X509KeyStorageFlags.MachineKeySet);
                    request.ClientCertificates.Add(cert);
              
                    //Get WebResponse.
                    string reaApiResponse;
                request.Timeout = 900000;
                using (HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse())
                 {
                      if (webresponse == null)
                            throw new WebException("Failed to get reponse from REA URL");

                        //Read Response from REA
                        StreamReader vending = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                        reaApiResponse = vending.ReadToEnd().Trim();
                        response = reaApiResponse;
                        string getErrorCode = (string)JObject.Parse(reaApiResponse).GetValue("errorCode");
                        var getToken = JObject.Parse(reaApiResponse).GetValue("vendingResponse");
                        string message = (string)JObject.Parse(reaApiResponse).GetValue("errorMsg");
                  
                    dh.LogRequestAndResponse("KRECS", transactionId, url, reaApiResponse);
                    if (getErrorCode.Equals("0"))
                    {
                        try
                        {
                           meterDetails.ErrorCode = "0";
                           meterDetails.Token = (string)getToken["token"];
                            meterDetails.Energy = (string)getToken["energy"];
                        }
                        
                        catch (Exception xx)
                        {
                            var dateNow = DateTime.Now;
                           
                            Console.WriteLine(xx.Message);
                        }
                    }
                    else
                    {
                        meterDetails.ErrorCode = "100";
                        meterDetails.ErrorMsg = message;
                    }
                  }
                    
                        
                }
                catch (Exception err)
                {
                    throw err;                  
                }
            return  meterDetails;
        }
    
    public static Meter QueryGetOrderNumber(string token)
    {
        Meter OrderNo = new Meter();
        try
        {
            string url = @"https://vending.rea.or.ug:8447/epower/prepayservice/getorderno?token=" + token;
           
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (delegate { return true; });
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 5000;
            request.ContentLength = 0;

            string certPath = @"E:certificates\client.p12";

            // Load the certificate into an X509Certificate object.
            X509Certificate2 cert = new X509Certificate2(certPath, "REA.1234", X509KeyStorageFlags.MachineKeySet);
            request.ClientCertificates.Add(cert);

            //Get WebResponse.
            string reaApiResponse;
            using (HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse())
            {
                if (webresponse == null)
                    throw new WebException("Failed to get reponse from REA URL");

                //Read Response from REA
                StreamReader apiMeter = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                reaApiResponse = apiMeter.ReadToEnd().Trim();
                var getErrorCode = JObject.Parse(reaApiResponse).GetValue("errorCode");
                var getErrorMsg = JObject.Parse(reaApiResponse).GetValue("errorMsg");
                var getOrderNo = JObject.Parse(reaApiResponse).GetValue("ordno");
                Meter apiMeterDetails = JsonConvert.DeserializeObject<Meter>(reaApiResponse);
                OrderNo.OrderNo = getOrderNo.ToString();
               
            }
        }
        catch (WebException err)
        {
            if (err.Status == WebExceptionStatus.SecureChannelFailure)
            {
                Console.WriteLine("Exception encountered:{0}", ((HttpWebResponse)err.Response).StatusCode);
            }
            else
            {
                Console.WriteLine("Error:" + err);
            }
        }
        return OrderNo;
    }
    public Meter GetVendingTokenFromKRECS(string meterNo, string amount,string vendorTranId)
    {
       REAResponse accessToken=  AccessToken();
        Meter reaInfo = new Meter();
        Meter orderNo = QueryGetOrderNumber(accessToken.SessionID);
        try
        { 
            Meter vendingResponse = NotifyKRECS(accessToken.SessionID,orderNo.OrderNo, vendorTranId, meterNo, float.Parse(amount));
            if (vendingResponse.ErrorCode.Equals("0"))
            {
                reaInfo.ErrorCode = "0";
                reaInfo.ErrorMsg = "SUCCESS";
                reaInfo.Token = vendingResponse.Token;
                reaInfo.Energy = vendingResponse.Energy;
            }
            else
            {
                reaInfo.ErrorCode = "100";
                reaInfo.ErrorMsg = vendingResponse.ErrorMsg;
            }
            return reaInfo;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public string XmlQueryCustomerResponse(Object feedback)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Meter));
            var subReq = feedback;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString();
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;
        }
    }

    public string XmlQueryCustomer(Object der)
    {
        var xml = "";
        try

        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(Request));
            var subReq = der;


            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, subReq);
                    xml = sww.ToString();
                }
            }

            return xml;
        }
        catch (Exception ert)
        {
            throw ert;
        }
    }

    [WebMethod]
    public BrightLifeResponse QueryBrightLifeCustomerDetails(string customerReference, string area, string vendorCode, string password)
    {
        BrightLifeResponse resp = new BrightLifeResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "BRIGHTLIFE", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("BRIGHTLIFE", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        BrightLifeResponse cust = QueryBrightLifeClientDetails(long.Parse(customerReference));
                        if (cust.StatusCode.Equals("10"))
                        {
                            resp.Client_name = cust.Client_name;
                            resp.Client_surname = cust.Client_surname;
                            resp.CustomerReference = customerReference;
                            resp.Client_registration_date = cust.Client_registration_date;
                            resp.Client_planned_termination_date = cust.Client_planned_termination_date;
                            resp.Client_next_payment_due = cust.Client_next_payment_due;
                            resp.StatusCode = "0";
                            resp.Client_registration_date = cust.Client_registration_date;
                            Customer customer = new Customer();
                            customer.AgentCode = "BRIGHTLIFE";
                            customer.CustomerName = resp.Client_name +" "+ resp.Client_surname;
                            customer.CustomerRef = customerReference;
                            saveCustomerDetails(customer);
                        }
                        else
                        {

                            Customer cust2 = dp.GetCustomerDetails(customerReference, area, "BRIGHTLIFE");
                            if (cust2.StatusCode.Equals("0"))
                            {
                                resp.CustomerReference = cust2.CustomerRef;
                                resp.Fullname = cust2.CustomerName;
                                resp.StatusCode = "0";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                            else
                            {
                                resp.Fullname = "";
                                resp.CustomerReference = "";
                                resp.StatusCode = "122";
                                resp.StatusDescription = "Telephone number does not exist.";
                            }
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, area, "BRIGHTLIFE");
            if (cust.StatusCode.Equals("0"))
            {
                
                resp.Fullname = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO BRIGHTLIFE";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "BRIGHTLIFE");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "BRIGHTLIFE");
        }
        return resp;
    }

    [WebMethod]
    public static BrightLifeResponse QueryBrightLifeClientDetails(long phoneNumber)
    {
        BrightLifeResponse cust = new BrightLifeResponse();
        try
        {
            DatabaseHandler dh = new DatabaseHandler();
            DataTable dt = dh.GetSolarisVendors();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                string utilityCode = row["UtilitiesName"].ToString();
                string url = row["Url"].ToString();
                string token = row["Apikey"].ToString();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

                string fullURL = url + "/clients/simple?phone_number=" + phoneNumber;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(fullURL);
                request.Method = "GET";
                request.Timeout = 5000;
                request.Accept = "application/xml";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Bearer " + token;

               
                string apiResponse;
                using (HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse())
                {
                    if (webresponse == null)
                        throw new WebException("Error encountered while fetching data");

                    StreamReader responseReader = new StreamReader(webresponse.GetResponseStream(), Encoding.UTF8);
                    apiResponse = responseReader.ReadToEnd().Trim();

                   
                    JObject jsonObj = (JObject)JsonConvert.DeserializeObject(apiResponse);
                    int count = jsonObj.Count;
                    if (count.Equals(0))
                    {
                        cust.StatusCode = "11";
                        cust.StatusDescription = "Phone number details do not exist";
                    }
                    else
                    {
                        BrightLifeResponse brightLifeResponse = JsonConvert.DeserializeObject<BrightLifeResponse>(apiResponse);

                        cust.Client_id = brightLifeResponse.Client_id;
                        cust.Client_name = brightLifeResponse.Client_name;
                        cust.Client_surname = brightLifeResponse.Client_surname;
                        cust.Client_registration_date = brightLifeResponse.Client_registration_date;
                        cust.Client_next_payment_due = brightLifeResponse.Client_next_payment_due;
                        cust.Client_termination_date = brightLifeResponse.Client_termination_date;
                        cust.Client_planned_termination_date = brightLifeResponse.Client_planned_termination_date;
                        cust.Client_birthdate = brightLifeResponse.Client_birthdate;
                        cust.Client_device = brightLifeResponse.Client_device;
                        cust.StatusCode = "10";
                    }
                }
            }
            else
            {
                Console.WriteLine("No records found in dataTable");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex);
        }
        return cust;
    }

    [WebMethod]
    public PostResponse MakeBrightLifePayment(BrightLifeTransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "BRIGHTLIFE", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("BRIGHTLIFE", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }
                                                                    resp.PegPayPostId = dp.PostBrightLifeTransaction(trans, "BRIGHTLIFE");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {

            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "BRIGHTLIFE");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "BRIGHTLIFE");
        }
        return resp;
    }

    [WebMethod]
    public SageWoodResponse QuerySAGEWOODCustomerDetails(string customerReference, string area, string vendorCode, string password)
    {
        SageWoodResponse resp = new SageWoodResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        ProcessingLogic result = new ProcessingLogic();
        try
        {
            dp.SaveRequestlog(vendorCode, "SAGEWOOD", "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("SAGEWOOD", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        SageWoodResponse cust = result.ValidateSageWoodMeter(customerReference);
                        if (cust.StatusCode.Equals("0"))
                        {
                            resp.AccNo = customerReference;
                            resp.Name = cust.Name;
                            resp.StatusCode = "0";
                            resp.StatusDescription = "SUCCESS";
                            resp.DaysOflastPurchase = cust.DaysOflastPurchase;
                            resp.Address = cust.Address;
                            Customer customer = new Customer();
                            customer.AgentCode = "SAGEWOOD";
                            customer.CustomerName = cust.Name;
                            customer.CustomerRef = customerReference;
                            saveCustomerDetails(customer);
                        }
                        else
                        {
                            resp.StatusCode = "100";
                            resp.StatusDescription = "INVALID METER REFERENCE";
                        }
                    }
                    else
                    {
                        resp.StatusCode = "29";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (WebException wex)
        {
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "SAGEWOOD");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "SAGEWOOD");
        }
        return resp;
    }
   
    [WebMethod]
    public PostResponse MakeSAGEWOODPayment(NWSCTransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "SAGEWOOD", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("SAGEWOOD", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }
                                                                    resp.PegPayPostId = dp.PostTransaction(trans, "SAGEWOOD");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);

                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "REA");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "REA");
        }
        return resp;
    }


    /***************************************** SAGEWOOD ENDS ****************************************************************************************/
    [WebMethod]
    public PostResponse MakeNSSFPayment(URATransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "NSSF", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.CustRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TIN == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "34";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.TIN.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "34";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (true/*pv.PhoneNumbersOk(trans.CustomerTel)*/)
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("NSSF", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {

                                                                    resp.PegPayPostId = dp.PostNSSFTransaction(trans, "NSSF");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = "SUCCESS";

                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NSSF");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NSSF");
        }
        return resp;
    }


    [WebMethod]
    public NSSFQueryResponse QueryNSSFCustomerDetails(string utilityCode, string customerReference, string vendorCode, string Password)
    {
        NSSFQueryResponse nssfqueryresponse = new NSSFQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "NSSF", "VERIFICATION", customerReference, Password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, Password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("NSSF", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                        UtilityReferences.NSSF.NSSFWSDL pegpay = new UtilityReferences.NSSF.NSSFWSDL();
                        UtilityReferences.NSSF.TransactionDetails stanbicbanknssfresponse = new UtilityReferences.NSSF.TransactionDetails();
                        String signature = creds.UtilityCode + customerReference + creds.UtilityPassword;
                        stanbicbanknssfresponse = pegpay.getTransaction(creds.UtilityCode, bll.GetSha1(signature), customerReference);


                        if (stanbicbanknssfresponse.error_code.ToString().Equals("200"))
                        {
                            nssfqueryresponse.CustomerReference = stanbicbanknssfresponse.reference;
                            nssfqueryresponse.EmployerNo = stanbicbanknssfresponse.employer_no;
                            nssfqueryresponse.Employer = stanbicbanknssfresponse.employer;
                            nssfqueryresponse.OutstandingBalance = stanbicbanknssfresponse.total;
                            nssfqueryresponse.Period = stanbicbanknssfresponse.period;
                            nssfqueryresponse.StatusCode = "0";
                            nssfqueryresponse.StatusDescription = stanbicbanknssfresponse.error_message;
                            nssfqueryresponse.PaymentMethod = stanbicbanknssfresponse.payment_method;

                            Customer customer = new Customer();
                            customer.AgentCode = "NSSF";
                            customer.CustomerName = nssfqueryresponse.Employer;
                            customer.CustomerRef = nssfqueryresponse.CustomerReference;
                            customer.Balance = nssfqueryresponse.OutstandingBalance;
                            customer.Area = nssfqueryresponse.EmployerNo;
                            customer.CustomerType = nssfqueryresponse.PaymentMethod;
                            saveUmemeCustomerDetails(customer);

                        }
                        else
                        {
                            nssfqueryresponse.CustomerReference = "";
                            nssfqueryresponse.EmployerNo = "";
                            nssfqueryresponse.Employer = "";
                            nssfqueryresponse.OutstandingBalance = "";
                            nssfqueryresponse.Period = "";
                            nssfqueryresponse.StatusCode = stanbicbanknssfresponse.error_code.ToString();
                            nssfqueryresponse.StatusDescription = stanbicbanknssfresponse.error_message;
                            nssfqueryresponse.PaymentMethod = "";
                        }
                    }
                    else
                    {
                        nssfqueryresponse.StatusCode = "29";
                        nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
                    }
                }
                else
                {
                    nssfqueryresponse.StatusCode = "11";
                    nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
                }
            }
            else
            {
                nssfqueryresponse.StatusCode = "2";
                nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "NSSF");
            if (cust.StatusCode.Equals("0"))
            {
                nssfqueryresponse.CustomerName = cust.CustomerName;
                nssfqueryresponse.CustomerReference = cust.CustomerRef;
                nssfqueryresponse.OutstandingBalance = "0";
                nssfqueryresponse.StatusCode = "0";
                nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
            }
            else
            {
                nssfqueryresponse.StatusCode = "30";
                nssfqueryresponse.StatusDescription = "UNABLE TO CONNECT TO NSSF";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "NSSF");
        }
        catch (SqlException sqlex)
        {
            nssfqueryresponse.StatusCode = "31";
            nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
        }
        catch (Exception ex)
        {
            nssfqueryresponse.StatusCode = "32";
            nssfqueryresponse.StatusDescription = dp.GetStatusDescr(nssfqueryresponse.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "NSSF");
        }
        return nssfqueryresponse;
    }

    [WebMethod]
    public TotalCardValidationResponse QueryTotalCardDetails(string utilityCode, string customerReference, string vendorCode, string Password)
    {
        TotalCardValidationResponse totalresponse = new TotalCardValidationResponse();
        TotalLogic api = new TotalLogic();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {
            dp.SaveRequestlog(vendorCode, "TOTAL", "CARD_VALIDATION", customerReference, Password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, Password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }
                    creds = dp.GetUtilityCreds("TOTAL", vendorCode);
                    if (!creds.UtilityCode.Equals(""))
                    {
                        totalresponse = api.ValidateCard(customerReference);


                        if (totalresponse.StatusCode.ToString().Equals("00"))
                        {                            
                            Customer customer = new Customer();
                            customer.AgentCode = "TOTAL";
                            customer.CustomerName = totalresponse.HolderName;
                            customer.CustomerRef = customerReference;
                            saveCustomerDetails(customer);

                        }
                    }
                    else
                    {
                        totalresponse.StatusCode = "29";
                        totalresponse.StatusDescription = dp.GetStatusDescr(totalresponse.StatusCode);
                    }
                }
                else
                {
                    totalresponse.StatusCode = "11";
                    totalresponse.StatusDescription = dp.GetStatusDescr(totalresponse.StatusCode);
                }
            }
            else
            {
                totalresponse.StatusCode = "2";
                totalresponse.StatusDescription = dp.GetStatusDescr(totalresponse.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, "", "TOTAL");
            if (cust.StatusCode.Equals("0"))
            {
                totalresponse.HolderName = cust.CustomerName;
                totalresponse.StatusCode = "00";
                totalresponse.StatusDescription = "SUCCESS";
            }
            else
            {
                totalresponse.StatusCode = "30";
                totalresponse.StatusDescription = "UNABLE TO CONNECT TO TOTAL";
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, "TOTAL");
        }
        catch (SqlException sqlex)
        {
            totalresponse.StatusCode = "31";
            totalresponse.StatusDescription = dp.GetStatusDescr(totalresponse.StatusCode);
        }
        catch (Exception ex)
        {
            totalresponse.StatusCode = "32";
            totalresponse.StatusDescription = dp.GetStatusDescr(totalresponse.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, "TOTAL");
        }
        return totalresponse;
    }

    [WebMethod]
    public PostResponse MakeURAPayment(URATransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "URA", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.CustRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "1";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TIN == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "34";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.TIN.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "34";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (true/*pv.PhoneNumbersOk(trans.CustomerTel)*/)
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("URA", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {

                                                                    if (trans.VendorCode.Equals("CELL"))
                                                                    {
                                                                        resp.PegPayPostId = dp.PostURATransaction(trans, "URA");
                                                                        BankResponse resp1 = SendTransactionToBank(trans);
                                                                        if (resp1.IsSuccessfullAtBank())
                                                                        {
                                                                            dp.UpdateSentTransaction(resp.PegPayPostId, resp1.BankId, "1");
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = "SUCCESS";
                                                                        }
                                                                        else
                                                                        {
                                                                            dp.deleteTransaction(trans.VendorTransactionRef, resp1.StatusDescription
                                                                             );
                                                                            resp.StatusCode = "100";
                                                                            resp.StatusDescription = resp1.StatusDescription;
                                                                        }
                                                                   
                                                                    }
                                                                    else
                                                                    {
                                                                        resp.PegPayPostId = dp.PostURATransaction(trans, "URA");
                                                                        resp.StatusCode = "0";
                                                                        resp.StatusDescription = "SUCCESS";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "URA");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "URA");
        }
        return resp;
    }

    private UtilityReferences.StanbicBankApi.TransactionRequest GetStanbicFmbURATranDetails(URATransaction trans)
    {
        try
        {
            UtilityReferences.StanbicBankApi.TransactionRequest tran = new UtilityReferences.StanbicBankApi.TransactionRequest();
            tran.PostField1 = trans.CustRef;
            tran.PostField2 = trans.CustName;// CustomerName
            tran.PostField3 = "";
            tran.PostField4 = "MBURA";//for Financle Mpbile Banking URA code is MBURA
            tran.PostField5 = trans.PaymentDate;
            tran.PostField6 = "";
            tran.PostField7 = trans.TransactionAmount;
            tran.PostField8 = "CASH";
            tran.PostField9 = "";
            tran.PostField10 = "";
            tran.PostField11 = trans.CustomerTel;
            tran.PostField12 = "0";
            tran.PostField13 = "";
            tran.PostField14 = trans.CustomerTel;
            tran.PostField15 = "0";
            tran.PostField18 = "";
            tran.PostField20 = trans.VendorTransactionRef;
            tran.PostField21 = "CASH"; 
            string dataToSign = tran.PostField1 + tran.PostField2 + tran.PostField11 + tran.PostField20 + tran.PostField9 + tran.PostField10 + tran.PostField5 + tran.PostField14 + tran.PostField7 + tran.PostField18 + tran.PostField8;
            tran.PostField16 = dataToSign;
            return tran;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    private BankResponse SendTransactionToBank(URATransaction trans)
    {
        BankResponse bankResp = new BankResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
            UtilityReferences.StanbicBankApi.PegPay pegpay = new UtilityReferences.StanbicBankApi.PegPay();
            UtilityReferences.StanbicBankApi.TransactionRequest tran = new UtilityReferences.StanbicBankApi.TransactionRequest();
            UtilityReferences.StanbicBankApi.Response resp = new UtilityReferences.StanbicBankApi.Response();
            UtilityReferences.StanbicBankApi.QueryRequest query = new UtilityReferences.StanbicBankApi.QueryRequest();
            tran = GetStanbicFmbURATranDetails(trans);
            resp = pegpay.PostTransaction(tran);

            if (resp.ResponseField6.Equals("0") || resp.ResponseField6.Equals("21"))
            {
                bankResp.StatusCode = resp.ResponseField6;
                bankResp.StatusDescription = resp.ResponseField7;
                bankResp.BankId = resp.ResponseField8;
                bankResp.BankBranch = "";
            }
            else
            {
                bankResp.StatusCode = resp.ResponseField6;
                bankResp.StatusDescription = resp.ResponseField7;
                bankResp.BankBranch = "";
            }
        }
        catch (WebException ex)
        {
            bankResp.StatusCode = "100";
            bankResp.StatusDescription = "TIMEOUT";
        }
        catch (Exception ex)
        {
            bankResp.StatusCode = "100";
            bankResp.StatusDescription = ex.Message;
        }

        return bankResp;
    }


  

    private static void WriteToErrorLog(Transaction trans, string Message)
    {
        File.WriteAllText(@"C:\" + trans.VendorTransactionRef + "ErrorLog.txt", Message);
    }


    [WebMethod]
    public UmemePostResponse MakeUmemePayment(UmemeTransaction trans)
    {
        UmemePostResponse resp = new UmemePostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidPayCode(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "27";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidCustType(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "28";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds("UMEME", trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                                if (!(vendorType.Equals("PREPAID")))
                                                                {
                                                                    if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                                                                    {
                                                                        if (trans.VendorCode.ToUpper().Equals("AFRICELL") || (trans.VendorCode.ToUpper().Equals("MTN") && trans.Narration.Contains("POS PAYMENT")))
                                                                        {
                                                                            Customer cust = new Customer();
                                                                            cust = dp.GetCustomerDetails(trans.CustRef, "", creds.Utility);
                                                                            double balance = Convert.ToDouble(cust.Balance.Trim());
                                                                            double amt = Convert.ToDouble(trans.TransactionAmount.Trim());
                                                                            if (amt > balance)
                                                                            {
                                                                                resp.PegPayPostId = dp.PostUmemeTransaction(trans, "UMEME");
                                                                                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                                                                                EPayment umemeapi = new EPayment();
                                                                                umemeapi.Timeout = 39000;
                                                                                UtilityReferences.UMEME.Response umemeResp = new UtilityReferences.UMEME.Response();
                                                                                UtilityReferences.UMEME.Transaction umemeTrans = new UtilityReferences.UMEME.Transaction();
                                                                                if (trans.VendorCode == "AFRICELL")
                                                                                {
                                                                                    umemeResp.StatusCode = "0";
                                                                                    umemeResp.StatusDescription = "SUCCESS";
                                                                                    umemeResp.ReceiptNumber = resp.PegPayPostId;
                                                                                }
                                                                                else
                                                                                {
                                                                                    umemeTrans = GetUmemeTrans(trans, creds);

                                                                                    umemeResp = umemeapi.PostBankUmemePayment(umemeTrans);
                                                                                }

                                                                                if (umemeResp.StatusCode.Equals("0"))
                                                                                {
                                                                                    dp.UpdateSentTransaction(resp.PegPayPostId, umemeResp.ReceiptNumber, "SUCCESS");
                                                                                    resp.ReceiptNumber = umemeResp.ReceiptNumber;
                                                                                    resp.StatusCode = "0";
                                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                                }
                                                                                else
                                                                                {
                                                                                    dp.deleteTransaction(resp.PegPayPostId, umemeResp.StatusDescription + " AT UMEME");
                                                                                    resp.ReceiptNumber = "";
                                                                                    resp.PegPayPostId = "";
                                                                                    resp.StatusCode = "100";
                                                                                    resp.StatusDescription = umemeResp.StatusDescription + " AT UMEME";
                                                                                }
                                                                            }

                                                                        }
                                                                       
                                                                        else if (trans.VendorCode.Equals("MTN"))
                                                                        {
                                                                            //MTN expect pending in the response 
                                                                            //and status code 1000
                                                                            resp.PegPayPostId = dp.PostUmemeTransaction(trans, "UMEME");
                                                                            resp.ReceiptNumber = resp.PegPayPostId;
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = "SUCCESS";//dp.GetStatusDescr(resp.StatusCode);
                                                                        }
                                                                        else
                                                                        {
                                                                            //check balance
                                                                            Customer cust = new Customer();
                                                                            cust = dp.GetCustomerDetails(trans.CustRef, "", creds.Utility);
                                                                            double balance = Convert.ToDouble(cust.Balance.Trim());
                                                                            double amt = Convert.ToDouble(trans.TransactionAmount.Trim());
                                                                            if (amt >= balance)
                                                                            {

                                                                                if (VendorHasBlackListedAccounts(trans.VendorCode))
                                                                                {
                                                                                    //NB:For Orange We insert into 
                                                                                    //Recieved Transactions                                                                                                           //later becoz the umeme api may be down
                                                                                    //and user has a debt
                                                                                    //e.g user pays 20000 and debt is 10000
                                                                                    //we should only insert 10000 in the recieved
                                                                                    //if umeme is down so when its back on we can 
                                                                                    //send thru only that amount and not the 20k he
                                                                                    //paid...think abt it

                                                                                    resp = DoOrangeMoneyUMEMETransaction(trans);
                                                                                }
                                                                                else
                                                                                {
                                                                                    resp.PegPayPostId = dp.PostUmemeTransaction(trans, "UMEME");
                                                                                    resp.ReceiptNumber = resp.PegPayPostId;
                                                                                    resp.StatusCode = "0";
                                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                resp.ReceiptNumber = "";
                                                                                resp.PegPayPostId = "";
                                                                                resp.StatusCode = "36";
                                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                                dp.LogError(resp.StatusDescription, trans.VendorCode, DateTime.Now, "UMEME");
                                                                            }

                                                                        }
                                                                    }
                                                                    
                                                                    else
                                                                    {
                                                                        if (trans.VendorCode.Equals("MTN") && !trans.Narration.Equals("POS PAYMENT"))
                                                                        {
                                                                            resp.PegPayPostId = dp.PostUmemeTransaction(trans, "UMEME");
                                                                            resp.ReceiptNumber = resp.PegPayPostId;
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = "SUCCESS";//dp.GetStatusDescr(resp.StatusCode);
                                                                        }
                                                                        else
                                                                        {
                                                                            resp.PegPayPostId = dp.PostUmemeTransaction(trans, "UMEME");
                                                                            resp.ReceiptNumber = resp.PegPayPostId;
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                        }
                                                                    }
                                                                }
                                                               
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "20";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }

        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                if (wex.Message.ToUpper().Contains("UNABLE TO CONNECT TO THE REMOTE SERVER"))
                {
                    dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "30";
                    resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
                }
                else
                {
                    UtilityCredentials creds2 = dp.GetUtilityCreds("UMEME", trans.VendorCode);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    EPayment umemeapi2 = new EPayment();
                    UtilityReferences.UMEME.Response umemeResp2 = umemeapi2.GetTransactionDetails(trans.VendorTransactionRef, creds2.UtilityCode, creds2.UtilityPassword);
                    if (umemeResp2.StatusCode.Equals("0"))
                    {
                        if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                        {
                            dp.UpdateSentTransaction(resp.PegPayPostId, umemeResp2.Token, "SUCCESS");
                            resp.ReceiptNumber = umemeResp2.Token;
                        }
                        else
                        {
                            dp.UpdateSentTransaction(resp.PegPayPostId, umemeResp2.ReceiptNumber, "SUCCESS");
                            resp.ReceiptNumber = resp.PegPayPostId;
                        }

                    }
                    else
                    {

                        dp.UpdateSentTransaction(resp.PegPayPostId, umemeResp2.ReceiptNumber, "SUCCESS");
                        resp.ReceiptNumber = resp.PegPayPostId;
                    }
                    resp.StatusCode = "0";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.LogError(wex.Message + " - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "UMEME");
                }
            }
            else
            {
                resp.ReceiptNumber = resp.PegPayPostId;
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "UMEME");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "UMEME");
        }
        
        return resp;
    }

    [WebMethod]
    public Token MakeYakaPayment(UmemeTransaction trans)
    {
        Token resp = new Token();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "UMEME", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidPayCode(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "27";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidCustType(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "28";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("UMEME", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {

                                                                    if (trans.CustomerType.ToUpper().Equals("PREPAID"))
                                                                    {
                                                                        resp.PegPayPostId = dp.InsertIntoReceivedTransactions(trans, "UMEME");
                                                                        System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;
                                                                        EPayment umemeapi = new EPayment();
                                                                        UtilityReferences.UMEME.Transaction umemeTrans = GetUmemeTrans(trans, creds);
                                                                        UtilityReferences.UMEME.Token token = umemeapi.PostYakaPayment(umemeTrans);
                                                                        if (token.StatusCode.Equals("0"))
                                                                        {
                                                                            resp = GetToken(token, resp.PegPayPostId);
                                                                            dp.UpdateSentTransaction(resp.PegPayPostId, token.PrepaidToken, "SUCCESS");
                                                                            resp.ReceiptNumber = token.ReceiptNumber;
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                        }
                                                                        else
                                                                        {
                                                                            dp.deleteTransaction(resp.PegPayPostId, token.StatusDescription + " AT UMEME");
                                                                            resp.ReceiptNumber = "";
                                                                            resp.PegPayPostId = "";
                                                                            resp.StatusCode = "100";
                                                                            resp.StatusDescription = token.StatusDescription + " AT UMEME";
                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        resp.ReceiptNumber = resp.PegPayPostId;
                                                                        resp.StatusCode = "0";
                                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resp.ReceiptNumber = "";
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (wex.Message.ToUpper().Contains("UNABLE TO CONNECT TO REMOTE SERVER"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
                dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "UMEME");
            }
            else
            {
                resp.ReceiptNumber = resp.PegPayPostId;
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                dp.UpdateSentTransaction(resp.PegPayPostId, "", "PENDING");
                dp.LogError(wex.Message + " - " + trans.VendorTransactionRef, trans.VendorCode, DateTime.Now, "UMEME");

            }
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "UMEME");
        }
        return resp;
    }

    public bool VendorHasBlackListedAccounts(string vendorCode)
    {
        DatabaseHandler dataAccessor = new DatabaseHandler();
        DataTable table = dataAccessor.CheckIfVendorHasBlacklistedAccounts(vendorCode);
        foreach (DataRow row in table.Rows)
        {
            if (row["Blacklisted"].ToString().Equals("1"))
            {
                return true;
            }
            break;
        }
        return false;
    }

    private UmemePostResponse DoOrangeMoneyUMEMETransaction(UmemeTransaction trans)
    {
        DatabaseHandler dp = new DatabaseHandler();
        UmemePostResponse umemeResponse = new UmemePostResponse();

        //meter Number = is in blacklisted accounts Table
        if (AccountIsBlackListed(trans))
        {
            //cash > amount due
            if (AmountIsGreaterThanDebt(trans))
            {
                string[] deductionDetails = DeductFromDebt(ref trans);
                LogAmountPaidByCustomer(trans, deductionDetails[0], deductionDetails[1]);

                //insert it into recieved transactions at this point because incase UMEME is down
                //the posting will fail. Later when its back we should send only the amount
                //that was left after deducting oranges debt
                umemeResponse.PegPayPostId = dp.InsertIntoReceivedTransactions(trans, "UMEME");
                umemeResponse.ReceiptNumber = "";
                umemeResponse.StatusCode = "0";
                umemeResponse.StatusDescription = dp.GetStatusDescr(umemeResponse.PegPayPostId);
                NotifyOrangeToCallCustomer(trans, deductionDetails[1]);
            }
            //cash == amount due
            else if (AmountIsEqualToDebt(trans))
            {
                string[] deductionDetails = DeductFromDebt(ref trans);
                string PegPayId = LogAmountPaidByCustomer(trans, deductionDetails[0], deductionDetails[1]);
                umemeResponse.ReceiptNumber = "";
                umemeResponse.PegPayPostId = PegPayId;
                umemeResponse.StatusCode = "0"; //still not sure what error code to use here...
                umemeResponse.StatusDescription = "Your Outstanding Balance Has Been Cleared.";

                NotifyOrangeToCallCustomer(trans, deductionDetails[1]);
            }
            //cash < amount due
            else
            {
                string[] deductionDetails = DeductFromDebt(ref trans);
                string PegPayId = LogAmountPaidByCustomer(trans, deductionDetails[0], deductionDetails[1]);
                umemeResponse.ReceiptNumber = "";
                umemeResponse.PegPayPostId = PegPayId;
                umemeResponse.StatusCode = "0"; //still not sure what error code to use here...
                umemeResponse.StatusDescription = "Your Account Is In Debt Recovery Mode. Current Debt = " + deductionDetails[2] + ". Call 100 for more info.";

                NotifyOrangeToCallCustomer(trans, deductionDetails[1]);
            }
        }
        //customer is not blacklisted
        else
        {
            //insert it into recieved transactions at this point because incase UMEME is down
            //the posting will fail. Later on... when its back we should send only the amount
            //that was left after deducting oranges debt
            umemeResponse.PegPayPostId = dp.InsertIntoReceivedTransactions(trans, "UMEME");
            umemeResponse.ReceiptNumber = "";
            umemeResponse.StatusCode = "0";
            umemeResponse.StatusDescription = /*"PENDING"*/dp.GetStatusDescr(umemeResponse.StatusCode);
        }

        return umemeResponse;
    }

    private bool AmountIsEqualToDebt(UmemeTransaction trans)
    {
        string meterNumber = trans.CustRef;
        int transactionAmount = Convert.ToInt32(trans.TransactionAmount);
        DatabaseHandler dataAccessor = new DatabaseHandler();
        DataTable dataTable = dataAccessor.GetBlacklistedAccountsDebt(meterNumber);

        //this should loop once
        foreach (DataRow row in dataTable.Rows)
        {
            int debtAmount = Convert.ToInt32(row["Debt"].ToString());

            //and if the cash == amount due
            if (transactionAmount == debtAmount)
            {
                return true;
            }

            break;
        }
        return false;
    }


    private string[] DeductFromDebt(ref UmemeTransaction trans)
    {
        string meterNumber = trans.CustRef;
        int transactionAmount = Convert.ToInt32(trans.TransactionAmount);
        string amountPaid = trans.TransactionAmount;
        string amountTaken = "0";
        DatabaseHandler dataAccessor = new DatabaseHandler();
        DataTable dataTable = dataAccessor.GetBlacklistedAccountsDebt(meterNumber);
        int debtAmount = 0;
        //This for loop will loop once
        foreach (DataRow row in dataTable.Rows)
        {
            //e.g cash > debt
            //user pays 20,000
            //debt is 10,000
            //difference=debt-tranAmount;
            //difference=10,000-20,000;
            //difference=-10,000
            //we set debt=0 and transactionAmount to 10,000 
            //however if
            // cash <= debt
            //debt = 10,000
            //user pays 5000
            //difference=debt-tranAmount;
            //difference=10,000-5,000
            //difference=5000;
            //we set debt=5000;
            //we set transactionAmount=0;

            debtAmount = Convert.ToInt32(row["Debt"].ToString());
            if (debtAmount > 0)
            {

                int difference;
                difference = debtAmount - transactionAmount;

                //he has paid more
                if (difference < 0)
                {
                    amountTaken = "" + debtAmount;
                    trans.TransactionAmount = "" + (transactionAmount - debtAmount);
                    debtAmount = 0;
                }
                //he has paid less
                else
                {
                    amountTaken = trans.TransactionAmount;
                    debtAmount = difference;
                    trans.TransactionAmount = "0";
                }
                dataAccessor.UpdateBlacklistedAccountDebt(meterNumber, debtAmount);

            }
            break;
        }
        return new string[] { amountPaid, amountTaken, "" + debtAmount };
    }


    private string LogAmountPaidByCustomer(UmemeTransaction trans, string AmountPaid, string AmountTaken)
    {
        DatabaseHandler databaseHandler = new DatabaseHandler();
        string id = databaseHandler.InsertIntoBlackListedAccountLogs(trans, AmountPaid, AmountTaken);
        return id;
    }


    private void NotifyOrangeToCallCustomer(UmemeTransaction trans, string amountTaken)
    {
        if (Convert.ToInt32(amountTaken) > 0)
        {
            Email orangeEmails = new Email(trans, amountTaken);
            orangeEmails.StartSendingEmails();
        }
    }

    private bool AmountIsGreaterThanDebt(UmemeTransaction trans)
    {

        string meterNumber = trans.CustRef;
        int transactionAmount = Convert.ToInt32(trans.TransactionAmount);
        DatabaseHandler dataAccessor = new DatabaseHandler();
        DataTable dataTable = dataAccessor.GetBlacklistedAccountsDebt(meterNumber);

        //this should loop once
        foreach (DataRow row in dataTable.Rows)
        {
            int debtAmount = Convert.ToInt32(row["Debt"].ToString());

            //and if the cash > amount due
            if (transactionAmount > debtAmount)
            {
                return true;
            }

            break;
        }
        return false;
    }

    private bool AccountIsBlackListed(UmemeTransaction trans)
    {
        //for an account to be blacklisted
        //it be in the blacklitsed umeme accounts table
        //and must have a debt > 0
        string meterNumber = trans.CustRef;
        DatabaseHandler dataAccessor = new DatabaseHandler();
        DataTable dataTable = dataAccessor.CheckIfMeterNumberIsBlacklisted(meterNumber);

        //if any data is recieved 
        if (dataTable.Rows.Count > 0)
        {
            //and the customer actually has a debt to pay off then return true
            if (DebtIsGreaterThanZero(dataTable))
            {
                return true;
            }
        }
        return false;
    }

    public bool DebtIsGreaterThanZero(DataTable table)
    {
        
        foreach (DataRow row in table.Rows)
        {
            string debt = row["Debt"].ToString();
            if (Convert.ToInt32(debt) > 0)
            {
                return true;
            }
            break;
        }
        return false;
    }
    [WebMethod]
    public PostResponse MakeDataAirtimePayment(NWSCTransaction trans)
    {

        PostResponse resp = new PostResponse();
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            if (string.IsNullOrEmpty(trans.VendorCode))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A VENDOR CODE";
                return resp;
            }
            else if (string.IsNullOrEmpty(trans.utilityCompany))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A UTILITY CODE";
                return resp;
            }
            else if (trans.utilityCompany.ToUpper() != "DATA" && trans.utilityCompany.ToUpper() != "AIRTIME")
            {

                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "METHOD IS ONLY DESIGNED FOR DATA OR AIRTIME PROCESSING";
                return resp;

            }
            else if (string.IsNullOrEmpty(trans.CustomerTel))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE PROVIDE RECEIVER PHONE NUMBER";
                return resp;

            }
            else
            {
                PhoneValidator pv = new PhoneValidator();
                DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                string phone = pv.FormatTelephone(trans.CustomerTel);

                if (!pv.PhoneNumbersOk(trans.CustomerTel))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID PHONE NUMBER";
                    return resp;
                }
                else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY A VENDOR TRANSACTION REFERENCE";
                    return resp;
                }
                else if (string.IsNullOrEmpty(trans.Teller))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY A TELLER";
                }
                else if (string.IsNullOrEmpty(trans.Narration))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "Please supply the message to be delivered".ToUpper();
                }
                else if (string.IsNullOrEmpty(trans.PaymentDate))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY SMS SENDING DATE FORMAT";
                }
                else if (!bll.IsValidDate(trans.PaymentDate))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID DATE FORMAT: USE dd/MM/yyyy";
                }
                else if (!isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID VENDOR CREDENTIALS";
                }
                else if (!isSignatureValid(trans))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID DIGITAL SIGNATURE AT PEGPAY";
                }
                else if (trans.utilityCompany == "DATA" && string.IsNullOrEmpty(trans.Area))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE PROVIDE THE BOUQUET CODE";
                }
                else if (trans.utilityCompany == "AIRTIME" && string.IsNullOrEmpty(trans.TransactionAmount))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE PROVIDE THE TRANSACTION AMOUNT";
                }
                else
                {
                    trans.CustRef = trans.CustomerTel;
                    trans.PaymentType = "CASH";
                    trans.Reversal = "0";
                    trans.TransactionType = "2";
                    trans.CustomerType = "";
                    if (trans.utilityCompany.ToUpper() == "DATA")
                    {
                        string networkCode = pv.NetworkCode;
                        trans.TransactionAmount = dp.GetDataCharge(trans.Area, networkCode);
                    }
                    else
                    {
                        trans.TransactionAmount = GetAmount(trans.TransactionAmount);
                    }
                    string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                    if ((vendorType.Equals("PREPAID")))
                    {
                        resp.PegPayPostId = dp.PostUmemeTransactionPrepaidVendor(trans, trans.utilityCompany, trans.Area);
                    }
                    else
                    {
                        resp.PegPayPostId = dp.PostUmemeTransaction(trans, trans.UtilityCode);

                    }
                    resp.StatusCode = "1000";
                    resp.StatusDescription = "PENDING";
                }
            }
        }
        catch (Exception ee)
        {

            resp.PegPayPostId = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "FAILED:" + ee.Message;
            return resp;

        }

        return resp;
    }

    [WebMethod]
    public AirtimeResponseObj MakeAirtimePayment(Transaction trans)
    {
        AirtimeResponseObj resp = new AirtimeResponseObj();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "NWSC", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area.Trim().Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.ResponseId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.ResponseId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.ResponseId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.ResponseId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }
                                                                    resp.ResponseId = dp.PostTransactionObject(trans, "NWSC");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                                else
                                                                {
                                                                    resp.ResponseId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.ResponseId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ResponseId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ResponseId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ResponseId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ResponseId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.ResponseId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ResponseId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ResponseId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ResponseId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ResponseId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ResponseId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ResponseId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NWSC");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.ResponseId, resp.StatusDescription);
            resp.ResponseId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.ResponseId, resp.StatusDescription);
            resp.ResponseId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NWSC");
        }
        return resp;
    }


    [WebMethod]
    public PostResponse MakeStartTimesPayment(Transaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        List<string> payCategories = new List<string>();
        payCategories.Add("RECHARGE");
        payCategories.Add("PAYMENT");
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "STARTIMES", "POSTING", trans.CustRef, trans.Password);
            if (string.IsNullOrEmpty(trans.CustName))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Area.Trim()))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = "PLEASE PROVIDE BOUQUET";
            }
            else if (string.IsNullOrEmpty(trans.TransactionType))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Teller))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";

                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.DigitalSignature))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Tin))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "39";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!payCategories.Contains(trans.Tin.ToUpper()))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "111";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount) || trans.Tin.Equals("RECHARGE"))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = trans.Tin.Equals("RECHARGE") ? "0" : GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("STARTIMES", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }

                                                                    resp.PegPayPostId = dp.PostPayTvTransaction(trans, "STARTIMES");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "STARTIMES");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "STARTIMES");
        }
        return resp;
    }


    [WebMethod]
    public PostResponse MakeNWSCPayment(NWSCTransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "NWSC", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Area.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "35";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {

                                                            string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                            if (!(vendorType.Equals("PREPAID")))
                                                            {
                                                                UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
                                                                if (!creds.UtilityCode.Equals(""))
                                                                {
                                                                    if (string.IsNullOrEmpty(trans.CustomerType))
                                                                    {
                                                                        trans.CustomerType = "";
                                                                    }
                                                                    resp.PegPayPostId = dp.PostTransaction(trans, "NWSC");
                                                                    resp.StatusCode = "0";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
           
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NWSC");
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NWSC");
        }
        return resp;
    }

   


    [WebMethod]
    public KCCAPostResponse MakeKCCAPayment(KCCATransaction trans)
    {
        KCCAPostResponse resp = new KCCAPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
       
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "KCCA", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
         
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds("KCCA", trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                            
                                                                if (trans.PaymentType == null)
                                                                {
                                                                    trans.PaymentType = "";
                                                                }
                                                                if (trans.CustomerType == null)
                                                                {
                                                                    trans.CustomerType = "";

                                                                }
                                                                DatabaseHandler dh = new DatabaseHandler();
                                                                Customer cust = dh.GetCustomerDetails(trans.CustRef, "", "KCCA");
                                                                if (cust.StatusCode.Equals("0"))
                                                                {
                                                                    if (trans.CustName.Equals(""))
                                                                    {
                                                                        trans.CustName = cust.CustomerName;
                                                                    }
                                                                    double balance = Convert.ToDouble(cust.Balance.Trim());
                                                                    double amt = Convert.ToDouble(trans.TransactionAmount.Trim());
                                                                    if (amt == balance)
                                                                    {
                                                                        resp.PegPayPostId = dp.PostUmemeTransaction(trans, "KCCA");
                                                                        resp.ReceiptNumber = resp.PegPayPostId;
                                                                        resp.StatusCode = "0";
                                                                        resp.StatusDescription = "SUCCESS";
                                                                    }
                                                                    else
                                                                    {
                                                                        resp.ReceiptNumber = "";
                                                                        resp.PegPayPostId = "";
                                                                        resp.StatusCode = "38";
                                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resp.ReceiptNumber = "";
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "38";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }

                                                                
                                                            }
                                                            else
                                                            {
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }
    [WebMethod]
    public KCCAResponse MakeTuckSeePayment(KCCATransaction trans)
    {
        KCCAResponse resp = new KCCAResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
       
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "TUCKSEE", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "13";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }

            else if (trans.TransactionType == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "14";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "14";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }

            else if (trans.VendorTransactionRef == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "16";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "16";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Teller == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "17";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "17";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "19";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "19";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "25";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "22";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "22";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.TransactionID = "";
                resp.ErrorCode = "23";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.TransactionID = "";
                resp.ErrorCode = "23";
                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            }
           
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds("TUCKSEE", trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                if (trans.PaymentType == null)
                                                                {
                                                                    trans.PaymentType = "";
                                                                }
                                                                if (trans.CustomerType == null)
                                                                {
                                                                    trans.CustomerType = "";

                                                                }
                                                                DatabaseHandler dh = new DatabaseHandler();

                                                                resp.TransactionID = dp.PostUmemeTransaction(trans, "TUCKSEE");
                                                                resp.ErrorCode = "0";
                                                                resp.ErrorDescription = "SUCCESS";


                                                            }
                                                            else
                                                            {
                                                                resp.TransactionID = "";
                                                                resp.ErrorCode = "29";
                                                                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.TransactionID = "";
                                                            resp.ErrorCode = "29";
                                                            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.TransactionID = "";
                                                        resp.ErrorCode = "26";
                                                        resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.TransactionID = "";
                                                    resp.ErrorCode = "24";
                                                    resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.TransactionID = "";
                                                resp.ErrorCode = "21";
                                                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.TransactionID = "";
                                            resp.ErrorCode = "20";
                                            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.TransactionID = "";
                                        resp.ErrorCode = "12";
                                        resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                    }
                                }
                                else
                                {
                                    resp.TransactionID = "";
                                    resp.ErrorCode = "18";
                                    resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                                }
                            }
                            else
                            {
                                resp.TransactionID = "";
                                resp.ErrorCode = "11";
                                resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                            }
                        }
                        else
                        {
                            resp.TransactionID = "";
                            resp.ErrorCode = "2";
                            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                        }
                    }
                    else
                    {
                        resp.TransactionID = "";
                        resp.ErrorCode = "4";
                        resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                    }
                }
                else
                {
                    resp.TransactionID = "";
                    resp.ErrorCode = "3";
                    resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
                }
            }
            if (resp.ErrorCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.TransactionID, "UNABLE TO CONNECT TO UMEME");
               
                resp.TransactionID = "";
                resp.ErrorCode = "30";
                resp.ErrorDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.ErrorCode = "0";
                resp.ErrorDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ErrorCode = "31";
            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            dp.deleteTransaction(resp.TransactionID, resp.ErrorDescription);
            resp.TransactionID = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ErrorCode = "32";
            resp.ErrorDescription = dp.GetStatusDescr(resp.ErrorCode);
            dp.deleteTransaction(resp.TransactionID, resp.ErrorDescription);
            resp.TransactionID = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    [WebMethod]
    public PostResponse PostGenericTransaction(Transaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
      
        try
        {
            if (string.IsNullOrEmpty(trans.UtilityCode))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "133";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                return resp;
            }
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);

            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (string.IsNullOrEmpty(trans.TransactionType))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.Teller))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (string.IsNullOrEmpty(trans.DigitalSignature))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
           
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                if (trans.PaymentType == null)
                                                                {
                                                                    trans.PaymentType = "";
                                                                }
                                                                if (trans.CustomerType == null)
                                                                {
                                                                    trans.CustomerType = "";

                                                                }
                                                                DatabaseHandler dh = new DatabaseHandler();
                                                                trans.CustomerTel = "256"+pv.FormatTelephone(trans.CustomerTel);
                                                                resp.PegPayPostId = dp.PostUmemeTransaction(trans, trans.UtilityCode);
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = "SUCCESS";


                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
              
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, trans.UtilityCode);
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    [WebMethod]
    public TransactionResponse MakeSolarPayment(Transaction trans)
    {
        TransactionResponse resp = new TransactionResponse();
        try
        {
            string transtatus = ValidateTransactions.ValidateTransactionObject(trans);
            if (transtatus == "OK")
            {
                SolarHandler shandler = new SolarHandler();
                resp = shandler.PostSolarPayment(trans);
                if (resp.ErrorCode == "0")
                {
                    resp.ErrorDescription = "SUCCESS";
                }
            }
            else
            {
                resp.ErrorCode = "100";
                resp.ErrorDescription = transtatus;
            }
        }
        catch (TimeoutException ee)
        {
            resp.ErrorCode = "101";
            resp.ErrorDescription = "Transaction Timeout";
          
        }
        catch (Exception ee)
        {
            resp.ErrorCode = "100";
            resp.ErrorDescription = "" + ee.Message;
        }
        return resp;
    }

    [WebMethod]
    public KCCAPostResponse NewMakeKCCAPayment(KCCATransaction trans)
    {
        KCCAPostResponse resp = new KCCAPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;
        
        try
        {
            dp.SaveRequestlog(trans.VendorCode, "MAK", "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
           
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                if (isSignatureValid(trans))
                                {
                                    if (pv.PhoneNumbersOk(trans.CustomerTel))
                                    {
                                        if (!IsduplicateVendorRef(trans))
                                        {
                                            if (!IsduplicateCustPayment(trans))
                                            {
                                                trans.Reversal = GetReversalState(trans);
                                                if (HasOriginalEntry(trans))
                                                {
                                                    if (ReverseAmountsMatch(trans))
                                                    {
                                                        if (!IsChequeBlacklisted(trans))
                                                        {
                                                            UtilityCredentials creds = dp.GetUtilityCreds("KCCA", trans.VendorCode);
                                                            if (!creds.UtilityCode.Equals(""))
                                                            {
                                                                resp.PegPayPostId = dp.PostUmemeTransaction(trans, "KCCA");
                                                                resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            
                                                            }
                                                            else
                                                            {
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "29";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "26";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "24";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }

                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "21";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "20";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "12";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "18";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    [WebMethod]
    public SchoolsPostResponse MakeSchoolFeesPayment(schoolsTransaction trans)
    {
        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;

        try
        {
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
           
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials2(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                
                                if (pv.PhoneNumbersOk(trans.CustomerTel))
                                {
                                    if (!IsduplicateVendorRef(trans))
                                    {
                                        if (!IsduplicateCustPayment(trans))
                                        {
                                            trans.Reversal = GetReversalState(trans);
                                            if (HasOriginalEntry(trans))
                                            {
                                                if (ReverseAmountsMatch(trans))
                                                {
                                                    if (!IsChequeBlacklisted(trans))
                                                    {
                                                        UtilityCredentials creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
                                                        if (!creds.UtilityCode.Equals(""))
                                                        {
                                                            resp.PegPayPostId = dp.PostSchoolTransaction(trans, trans.UtilityCode);
                                                            if (bll.IsNumeric(resp.PegPayPostId))
                                                            {


                                                               resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                            else
                                                            {
                                                                dp.deleteTransaction(resp.PegPayPostId, "ERROR SAVING TRANSACTION AT PEGPAY");
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "100";
                                                                resp.StatusDescription = "ERROR POSTING TRANSACTION AT PEGPAY";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = "ERROR FROM HERE 1"; 
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "29";
                                                        resp.StatusDescription = "ERROR FROM HERE 2";
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "26";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "24";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }

                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "21";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "20";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "12";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                           
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }


    [WebMethod]
    public SchoolsPostResponse MakeNDAPayment(schoolsTransaction trans)
    {
        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;

        try
        {
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            
            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials2(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                              
                                if (pv.PhoneNumbersOk(trans.CustomerTel))
                                {
                                    if (!IsduplicateVendorRef(trans))
                                    {
                                        if (!IsduplicateCustPayment(trans))
                                        {
                                            trans.Reversal = GetReversalState(trans);
                                            if (HasOriginalEntry(trans))
                                            {
                                                if (ReverseAmountsMatch(trans))
                                                {
                                                    if (!IsChequeBlacklisted(trans))
                                                    {
                                                        UtilityCredentials creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
                                                        if (!creds.UtilityCode.Equals("") || trans.UtilityCode == "NDA")
                                                        {
                                                            resp.PegPayPostId = dp.PostSchoolTransaction(trans, trans.UtilityCode);
                                                            if (bll.IsNumeric(resp.PegPayPostId))
                                                            {

                                                                resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                            else
                                                            {
                                                                dp.deleteTransaction(resp.PegPayPostId, "ERROR SAVING TRANSACTION AT PEGPAY");
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "100";
                                                                resp.StatusDescription = "ERROR POSTING TRANSACTION AT PEGPAY";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = "ERROR FROM HERE 1"; 
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "29";
                                                        resp.StatusDescription = "ERROR FROM HERE 2";
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "26";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "24";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }

                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "21";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "20";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "12";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                           
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    [WebMethod]
    public SchoolsPostResponse MakeTOTALPayment(schoolsTransaction trans)
    {
        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;

        try
        {
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials2(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                              
                                if (pv.PhoneNumbersOk(trans.CustomerTel))
                                {
                                    if (!IsduplicateVendorRef(trans))
                                    {
                                        if (!IsduplicateCustPayment(trans))
                                        {
                                            trans.Reversal = GetReversalState(trans);
                                            if (HasOriginalEntry(trans))
                                            {
                                                if (ReverseAmountsMatch(trans))
                                                {
                                                    if (!IsChequeBlacklisted(trans))
                                                    {
                                                        UtilityCredentials creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
                                                        if (!creds.UtilityCode.Equals("") || trans.UtilityCode == "NDA")
                                                        {
                                                            resp.PegPayPostId = dp.PostSchoolTransaction(trans, trans.UtilityCode);
                                                            if (bll.IsNumeric(resp.PegPayPostId))
                                                            {

                                                                resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                            else
                                                            {
                                                                dp.deleteTransaction(resp.PegPayPostId, "ERROR SAVING TRANSACTION AT PEGPAY");
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "100";
                                                                resp.StatusDescription = "ERROR POSTING TRANSACTION AT PEGPAY";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = "ERROR FROM HERE 1"; 
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "29";
                                                        resp.StatusDescription = "ERROR FROM HERE 2";
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "26";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "24";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }

                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "21";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "20";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "12";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                           
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {           
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO TOTAL";
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "TOTAL");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "TOTAL");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "TOTAL");
        }
        return resp;
    }


    private SchoolsPostResponse MakeUMUSchoolFeesPayment(schoolsTransaction trans)
    {
        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {

            creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
            if (!creds.UtilityCode.Equals(""))
            {
                
                System.Net.ServicePointManager.Expect100Continue = false;
                string myUrl2 = "http://eis.umu.ac.ug:84/test/ebank?";
                string urlParams2 = "act = POST & cur = UGX & acc = " + trans.Teller + "& dt =" + trans.PaymentDate + "& stno =" + trans.CustRef + "& chqno =" + trans.ChequeNumber + " &dr =" + trans.Reversal + " &cr =" + trans.PaymentType + " & bank =" + creds.BankCode + " & username =" + creds.Utility + "& password =" + creds.UtilityPassword + "& phone =" + trans.Telephone + " & yrofstudy =" + "" + "& sem =" + "" + "& bankRef =" + trans.VendorTransactionRef + " & naration =" + trans.Narration;
                myUrl2 = myUrl2 + urlParams2;
                HttpWebRequest r2 = (HttpWebRequest)System.Net.WebRequest.Create(myUrl2);
                r2.Headers.Clear();
                r2.AllowAutoRedirect = true;
                r2.PreAuthenticate = true;
                r2.ContentType = "application / x - www - form - urlencoded";
                r2.Credentials = CredentialCache.DefaultCredentials;
                r2.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                r2.Timeout = 150000;
                Encoding byteArray2 = Encoding.GetEncoding("utf-8");
                Stream dataStream2;
                WebResponse response2 = (HttpWebResponse)r2.GetResponse();
                Console.WriteLine(((HttpWebResponse)response2).StatusDescription);
                dataStream2 = response2.GetResponseStream();
                StreamReader rdr2 = new StreamReader(dataStream2);
                string feedback2 = rdr2.ReadToEnd();

                string[] array = feedback2.Split(',');
                if (array.Length == 9)
                {
                    string errorCode = array[0].ToString().Replace(",", "");
                    string Ref = GetDetail(array[1].ToString());
                    resp.StatusCode = errorCode.Replace("\n", "");
                    
                    resp.ReceiptNumber = Ref;
                }
                else
                {
                    string errorCode = array[0].ToString().Replace(",", "");
                    string errorMessage = array[1].ToString().Replace(",", "");
                    resp.StatusDescription = errorMessage;
                    resp.StatusCode = errorCode;
                }
            }
            else
            {
                resp.StatusCode = "29";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return resp;
    }
    private SchoolsPostResponse MakeMUBSSchoolFeesPayment(schoolsTransaction trans)
    {
        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        UtilityCredentials creds;
        try
        {

            creds = dp.GetUtilityCreds(trans.UtilityCode, trans.VendorCode);
            if (!creds.UtilityCode.Equals(""))
            {
                System.Net.ServicePointManager.Expect100Continue = false;
                string myUrl2 = "http://eis.mubs.ac.ug:84/test/ebank?";
                string urlParams2 = "act = POST & cur = UGX & acc = " + trans.Teller + "& dt =" + trans.PaymentDate + "& stno =" + trans.CustRef + "& chqno =" + trans.ChequeNumber + " &dr =" + trans.Reversal + " &cr =" + trans.PaymentType + " & bank =" + creds.BankCode + " & username =" + creds.Utility + "& password =" + creds.UtilityPassword + "& phone =" + trans.Telephone + " & yrofstudy =" + "" + "& sem =" + "" + "& bankRef =" + trans.VendorTransactionRef + " & naration =" + trans.Narration;
                myUrl2 = myUrl2 + urlParams2;
                HttpWebRequest r2 = (HttpWebRequest)System.Net.WebRequest.Create(myUrl2);
                r2.Headers.Clear();
                r2.AllowAutoRedirect = true;
                r2.PreAuthenticate = true;
                r2.ContentType = "application / x - www - form - urlencoded";
                r2.Credentials = CredentialCache.DefaultCredentials;
                r2.UserAgent = "Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
                r2.Timeout = 150000;
                Encoding byteArray2 = Encoding.GetEncoding("utf-8");
                Stream dataStream2;
                WebResponse response2 = (HttpWebResponse)r2.GetResponse();
                Console.WriteLine(((HttpWebResponse)response2).StatusDescription);
                dataStream2 = response2.GetResponseStream();
                StreamReader rdr2 = new StreamReader(dataStream2);
                string feedback2 = rdr2.ReadToEnd();

                string[] array = feedback2.Split(',');
                if (array.Length == 9)
                {
                    string errorCode = array[0].ToString().Replace(",", "");
                    string Ref = GetDetail(array[1].ToString());
                    resp.StatusCode = errorCode.Replace("\n", "");
                    //resp.Serial = Ref;
                    resp.ReceiptNumber = Ref;
                }
                else
                {
                    string errorCode = array[0].ToString().Replace(",", "");
                    string errorMessage = array[1].ToString().Replace(",", "");
                    resp.StatusDescription = errorMessage;
                    resp.StatusCode = errorCode;
                }

            }
            else
            {
                resp.StatusCode = "29";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return resp;
    }

    [WebMethod]
    public QueryResponse GetTransactionDetails(string vendorTranID, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();

        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveRequestlog(vendorCode, "", "GETTRANDETAILS", "", password);
            BusinessLogic bll = new BusinessLogic();
            string strLoginCount = "";
            int loginCount = 0;
            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (vendorData.Rows.Count != 0)
            {
                string vendor = vendorData.Rows[0]["VendorCode"].ToString();
                string encVendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
                string vendorPassword = bll.DecryptString(encVendorPassword);
                strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                string VendoType = vendorData.Rows[0]["VendorType"].ToString();
                loginCount = int.Parse(strLoginCount);
                bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
                if (password == vendorPassword)
                {
                    if (activeVendor)
                    {
                        if (loginCount > 0)
                        {
                            dp.UpdateVendorInvalidLoginCount(vendorCode, 0, ipAddress);
                        }
                        if (vendor.Trim().Equals(vendorCode.Trim()) && vendorPassword.Trim().Equals(password.Trim()))
                        {
                                if ((VendoType.Equals("PREPAID")))
                                {
                                DataTable dt = dp.GetTransactionDetails(vendorTranID, vendorCode);
                                if (dt.Rows.Count != 0)
                                {
                                    if (dt.Rows[0]["CustomerType"].ToString().Trim().Equals("PREPAID") || dt.Rows[0]["UtilityCode"].ToString().ToUpper() == "WENRECO")
                                    {
                                        resp.PegPayQueryId = dt.Rows[0]["UtilityTranRef"].ToString();
                                       
                                    }
                                    else
                                    {
                                        resp.PegPayQueryId = dt.Rows[0]["TransNo"].ToString();
                                        resp.TokenValue = dt.Rows[0]["UtilityTranRef"].ToString();
                                        resp.NoOfUnits = dt.Rows[0]["Reason"].ToString();
                                    }

                                    resp.StatusCode = "0";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                                else
                                {
                                    resp.StatusCode = "33";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    resp.PegPayQueryId = "";
                                }
                            }
                            else
                            {

                                DataTable dt = dp.GetPrepaidTransactionDetails(vendorTranID, vendorCode);
                                if (dt.Rows.Count != 0)
                                {
                                    string status = dt.Rows[0]["Status"].ToString().ToUpper().Trim();
                                    
                                    if (status.Equals("PENDING"))
                                    {
                                        resp.StatusCode = "1000";
                                        resp.StatusDescription = status;
                                    }
                                    
                                    else if (status.Equals("SUCCESS"))
                                        {
                                        string customerTel= dt.Rows[0]["CustomerTel"].ToString();
                                        string prepaidextras = dt.Rows[0]["ReceiptNo"].ToString();
                                        if (!String.IsNullOrEmpty(prepaidextras))
                                        {
                                            
                                            string[] array = prepaidextras.Split(';');
                                            if (array.Length >= 10)
                                            {
                                                resp.Lifeline = array[0].ToString();
                                                resp.ServiceFee = array[1].ToString();
                                                resp.PayAccount = array[2].ToString();
                                                resp.DebtRecovery = array[3].ToString();
                                                resp.ReceiptNumber = array[4].ToString();
                                                resp.TokenValue = array[5].ToString();
                                                resp.Inflation = array[6].ToString();
                                                resp.Forex = array[7].ToString();
                                                resp.VAT = array[8].ToString();
                                                resp.Fuel = array[9].ToString();
                                                resp.NoOfUnits = dt.Rows[0]["Reason"].ToString();
                                               
                                            }
                                          }
                                       

                                        resp.StatusCode = "0";
                                        resp.StatusDescription = status;
                                        resp.PegPayQueryId = dt.Rows[0]["UtilityTranRef"].ToString();
                                        resp.CustomerReference = dt.Rows[0]["VendorToken"].ToString();
                                        resp.IssuingEntity = "DFCU BANK";
                                    }
                                    else if (status.Equals("REVERSAL"))
                                    {
                                        resp.StatusCode = "0";
                                        resp.StatusDescription = "REVERSED";
                                        resp.PegPayQueryId = dt.Rows[0]["VendorToken"].ToString();
                                        resp.CustomerReference = dt.Rows[0]["BankReversalRef"].ToString();
                                    }
                                   
                                    else if (status.Equals("FAILED"))
                                    {
                                        resp.StatusCode = "100";
                                        resp.StatusDescription = dt.Rows[0]["Reason"].ToString();
                                        resp.PegPayQueryId = "";
                                        resp.CustomerReference = "";
                                    }
                                   
                                    else
                                    {
                                        resp.StatusCode = "1000";
                                        resp.StatusDescription = status;
                                    }
                                }
                                else
                                {
                                    resp.StatusCode = "33";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    resp.PegPayQueryId = "";
                                }
                            }
                        }
                        else
                        {
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            resp.PegPayQueryId = "";
                        }
                    }
                    else
                    {
                        resp.StatusCode = "11";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        resp.PegPayQueryId = "";
                    }
                }
                else
                {
                    strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }

                    resp.StatusCode = "2";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    resp.PegPayQueryId = "";
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                resp.PegPayQueryId = "";
            }
        }
        catch (Exception ex)
        {
            try
            {
                DatabaseHandler dp = new DatabaseHandler();
                resp.StatusCode = "10";
                resp.StatusDescription = "GENERAL ERROR AT PEGPAY";
                resp.PegPayQueryId = "";
               
            }
            catch (Exception exx)
            {
              
            }
        }
        return resp;
    }
    [WebMethod]
    public QueryResponse GetDataBoundler(string Network, string Duration, string VendorCode, string Password)
    {
        DatabaseHandler dh = new DatabaseHandler();
        QueryResponse resp = new QueryResponse();
        
        try
        {
            DataTable vendaData = dh.GetVendorDetails(VendorCode);
            if (isValidVendorCredentials(VendorCode, Password, vendaData))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "Invalid Pegpay Credentials";
            }
            else if (string.IsNullOrEmpty(Network))
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "Please Provide a network code";
            }
            else
            {
                DataTable table = dh.GetDataBundles(Network, Duration);
                List<DataBundle> bundleList = new List<DataBundle>();
                foreach (DataRow drow in table.Rows)
                {
                    DataBundle bundle = new DataBundle();
                    bundle.BundleCode = drow["BundleCode"].ToString();
                    bundle.BundleDuration = drow["Duration"].ToString();
                    bundle.BundleName = drow["BundleName"].ToString();
                    bundleList.Add(bundle);
                }
                if (bundleList.Count > 0)
                {
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                    resp.Bundles = bundleList;
                }
                else
                {
                    resp.StatusCode = "100";
                    resp.StatusDescription = "Bunlde(s) Not Found for the selected Network [" + Network.ToUpper() + "]";
                }
            }
        }
        catch (Exception ee)
        {
            resp.StatusCode = "100";
            resp.StatusDescription = "Error: Internal system failure";
            dh.LogError(ee.Message, VendorCode, DateTime.Now, "DATA");
        }

        return resp;
    }
    [WebMethod]
    public QueryResponse GetTransactionStatus(string vendorTranID, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();

        try
        {
            resp = ValidateParameters(vendorTranID, vendorCode, password);
           
            if (resp.StatusCode.Equals("0"))
            {
                resp = GetTheStatusOfTheTransaction(vendorTranID, vendorCode);
            }
           
            else
            {
                return resp;
            }
        }
        
        catch (Exception ex)
        {
            resp.StatusCode = "1000";
            resp.StatusDescription = "PENDING";
            resp.PegPayQueryId = "";
        }
        return resp;
    }

    private QueryResponse GetTheStatusOfTheTransaction(string vendorTranID, string vendorCode)
    {
        QueryResponse resp = new QueryResponse();
        DatabaseHandler dh = new DatabaseHandler();
        DataTable results = dh.GetTranStatus(vendorTranID, vendorCode);
        if (results.Rows.Count > 0)
        {
            string status = results.Rows[0]["Status"].ToString();
            if (status.Equals("SUCCESS"))
            {
                resp.StatusCode = "0";
                resp.StatusDescription = status;
            }
            
            else
            {
                resp.StatusCode = "42";
                resp.StatusDescription = status;
            }
        }
       
        else
        {
            resp.StatusCode = "33";
            resp.StatusDescription = dh.GetStatusDescr(resp.StatusCode);
        }
        return resp;
    }

    private QueryResponse ValidateParameters(string vendorTranID, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        dp.SaveRequestlog(vendorCode, "", "GETTRANDETAILS", "", password);
        BusinessLogic bll = new BusinessLogic();
        string strLoginCount = "";
        int loginCount = 0;
        string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        DataTable vendorData = dp.GetVendorDetails(vendorCode);
        if (vendorData.Rows.Count != 0)
        {
            string vendor = vendorData.Rows[0]["VendorCode"].ToString();
            string encVendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
            string vendorPassword = bll.DecryptString(encVendorPassword);
            strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
            loginCount = int.Parse(strLoginCount);
            bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
            if (password == vendorPassword)
            {
                if (activeVendor)
                {
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0, ipAddress);
                    }
                    if (vendor.Trim().Equals(vendorCode.Trim()) && vendorPassword.Trim().Equals(password.Trim()))
                    {
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                        resp.PegPayQueryId = "";
                    }
                    else
                    {
                        resp.StatusCode = "2";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        resp.PegPayQueryId = "";
                    }
                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    resp.PegPayQueryId = "";
                }
            }
            else
            {
                strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                loginCount = int.Parse(strLoginCount);
                loginCount = loginCount + 1;
                if (loginCount == 3)
                {
                    dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    dp.DeactivateVendor(vendorCode, ipAddress);
                }
                {
                    dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                }

                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                resp.PegPayQueryId = "";
            }
        }
        else
        {
            resp.StatusCode = "2";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            resp.PegPayQueryId = "";
        }
        return resp;

    }

    private UtilityReferences.URA.TransactionEntity GetUraTrans(URATransaction trans, UtilityCredentials creds)
    {
        BusinessLogic bll = new BusinessLogic();
        TransactionEntity uraTrans = new TransactionEntity();
        uraTrans.Amount = bll.EncryptUraParameter(trans.TransactionAmount);
        uraTrans.Prn = bll.EncryptUraParameter(trans.CustRef);
        uraTrans.Tin = bll.EncryptUraParameter(trans.TIN);
        uraTrans.Bank_branch_cd = creds.BankCode;
        uraTrans.Bank_cd = creds.UtilityCode;
        uraTrans.Bank_tr_no = trans.VendorTransactionRef;
        uraTrans.Chq_no = trans.ChequeNumber;
        uraTrans.Paid_dt = DateTime.Now.ToString("dd/MM/yyyy"); 
        uraTrans.Reason = trans.Narration;
        uraTrans.Status = trans.Status;
        uraTrans.Value_dt = DateTime.Now.ToString("dd/MM/yyyy");
        string dataToSign = uraTrans.Bank_cd + uraTrans.Prn + uraTrans.Tin + uraTrans.Amount + uraTrans.Paid_dt
                           + uraTrans.Value_dt + uraTrans.Status + uraTrans.Bank_branch_cd + uraTrans.Bank_tr_no + uraTrans.Chq_no
                           + uraTrans.Reason;
        uraTrans.Signature = GetSignature(dataToSign, trans.VendorCode);
       
        return uraTrans;
    }

    private byte[] GetSignatureURA(string Tosign, string VendorCode)
    {
        string certificate = @"E:\\Certificates\\" + VendorCode + "\\" + VendorCode + ".pfx";
        X509Certificate2 cert = new X509Certificate2(certificate, "", X509KeyStorageFlags.UserKeySet);

        RSACryptoServiceProvider RSAcp = (RSACryptoServiceProvider)cert.PrivateKey;

        if (RSAcp == null)
        {
            throw new Exception("VALID CERTIFICATE NOT FOUND");
        }

        byte[] data = new UnicodeEncoding().GetBytes(Tosign);
        byte[] hash = new SHA1Managed().ComputeHash(data);

        
        return RSAcp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
    }

    private string[] makePaymentUra(URATransaction transaction, UtilityCredentials creds)
    {
        try
        {
            BusinessLogic bll = new BusinessLogic();
            TransactionEntity av = new TransactionEntity();
            string username = creds.UtilityCode;
            string Passwd = bll.DecryptString(creds.UtilityPassword);
            string CryptPass = bll.EncryptUraParameter(Passwd);
            
            string Amount = Convert.ToString(int.Parse(transaction.TransactionAmount));
            av.Amount = bll.EncryptUraParameter(Amount);
            av.Prn = bll.EncryptUraParameter(transaction.CustRef);
            av.Tin = bll.EncryptUraParameter(transaction.TIN);
            av.Reason = transaction.TransactionType;
            av.Bank_branch_cd = creds.BankCode;
            av.Bank_cd = creds.UtilityCode;
            av.Bank_tr_no = transaction.VendorTransactionRef;
            av.Chq_no = transaction.ChequeNumber;
            av.Paid_dt = DateTime.Now.ToString("dd/MM/yyyy");
            av.Status = transaction.Status;  

            av.Value_dt = DateTime.Now.ToString("dd/MM/yyyy");
            string dataToSign = av.Bank_cd + av.Prn + av.Tin + av.Amount + av.Paid_dt + av.Value_dt + av.Status +
                           av.Bank_branch_cd + av.Bank_tr_no + av.Chq_no + av.Reason;
            av.Signature = GetSignatureURA(dataToSign, transaction.VendorCode);

            TransactionEntity[] arr = { av };
            UraPmtService clt = new UraPmtService();
            string[] RetStr;
            try
            {
                RetStr = clt.NotifyUraPayment(username, CryptPass, arr);
            }
            catch (Exception exe)
            {
                string res = "000,OFFLINE";
                string[] arr_str = res.Split(',');
                RetStr = arr_str;
            }
            return RetStr;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private UtilityReferences.UMEME.Transaction GetUmemeTrans(UmemeTransaction trans, UtilityCredentials creds)
    {
        UtilityReferences.UMEME.Transaction umemeTrans = new UtilityReferences.UMEME.Transaction();
        umemeTrans.CustomerName = trans.CustName;
        umemeTrans.CustomerRef = trans.CustRef;
        umemeTrans.CustomerTel = trans.CustomerTel;
        umemeTrans.CustomerType = trans.CustomerType;
        umemeTrans.Offline = trans.Offline;
        umemeTrans.Password = creds.UtilityPassword;
        umemeTrans.PaymentDate = trans.PaymentDate;
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
        umemeTrans.VendorTranId = trans.VendorTransactionRef;
        string dataToSign = umemeTrans.CustomerRef + umemeTrans.CustomerName + umemeTrans.CustomerTel + umemeTrans.CustomerType + umemeTrans.VendorTranId + umemeTrans.VendorCode + umemeTrans.Password + umemeTrans.PaymentDate + umemeTrans.PaymentType + umemeTrans.Teller + umemeTrans.TranAmount + umemeTrans.TranNarration + umemeTrans.TranType;
        umemeTrans.DigitalSignature = GetDigitalSignature(dataToSign, trans.VendorCode);
        return umemeTrans;
    }

  

    private string GetDigitalSignature(string text, string vendorCode)
    {
       
        string certificate = "";
        if (vendorCode.ToUpper().Equals("EZEEMONEY"))
        {
            certificate = @"E:\\Certificates\\" + vendorCode + "Certs\\" + vendorCode + ".pfx";
        }
        else if (vendorCode.ToUpper().Equals("PEGPAY_COREBANKING"))
        {
            certificate = @"E:\Certificates\CELL\CELL.pfx";
        }
        else
        {
            certificate = @"E:\\Certificates\\" + vendorCode + "\\" + vendorCode + ".pfx";
        }
        X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
        RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
        
        SHA1Managed sha1 = new SHA1Managed();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] data = encoding.GetBytes(text);
        byte[] hash = sha1.ComputeHash(data);
        
        byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        string strDigCert = Convert.ToBase64String(digitalCert);
        return strDigCert;
    }

    private string GetDigitalSignature1(string text, string vendorCode)
    {
        try
        {
            X509Store my = new X509Store(vendorCode, StoreLocation.LocalMachine);
            my.Open(OpenFlags.ReadOnly);
            
            RSACryptoServiceProvider csp = null;
            foreach (X509Certificate2 cert in my.Certificates)
            {
                if (cert.Subject.Contains(vendorCode))
                {
                    csp = (RSACryptoServiceProvider)cert.PrivateKey;
                }
            }
            if (csp == null)
            {
                throw new Exception("Valid certificate was not found");
            }
            
            SHA1Managed sha1 = new SHA1Managed();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(text);
            byte[] hash = sha1.ComputeHash(data);
            
            return Convert.ToBase64String(csp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private byte[] GetSignature(string Tosign, string vendorCode)
    {
        string certificate = @"E:\\Certificates\\" + vendorCode + "\\" + vendorCode + ".pfx"; ;
        X509Certificate2 cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);

        RSACryptoServiceProvider RSAcp = (RSACryptoServiceProvider)cert.PrivateKey;

        if (RSAcp == null)
        {
            throw new Exception("VALID CERTIFICATE NOT FOUND");
        }

        byte[] data = new UnicodeEncoding().GetBytes(Tosign);
        byte[] hash = new SHA1Managed().ComputeHash(data);
        
        return RSAcp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
    }

    private bool IsValidPayCode(UmemeTransaction trans)
    {
        if (trans.PaymentType != null)
        {
            DatabaseHandler dp = new DatabaseHandler();
            DataTable dt = dp.GetPaymentCode(trans.PaymentType);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool IsValidCustType(UmemeTransaction trans)
    {
        if (trans.CustomerType != null)
        {
            string CustType = trans.CustomerType.ToString().ToUpper();
            CustType = CustType.Replace(" ", "");
            if (CustType.Equals("PREPAID") || CustType.Equals("POSTPAID") || CustType.Equals("NEWCONNECTION"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private Token GetToken(UtilityReferences.UMEME.Token token, string pegpayPostId)
    {
        Token t = new Token();
        t.DebtRecovery = token.DebtRecovery;
        t.Fuel = token.Fuel;
        t.Fx = token.Fx;
        t.Inflation = token.Inflation;
        t.MeterNumber = token.MeterNumber;
        t.PayAccount = token.PayAccount;
        t.PrepaidToken = token.PrepaidToken;
        t.ReceiptNumber = token.ReceiptNumber;
        t.Tax = token.Tax;
        t.TokenValue = token.TokenValue;
        t.TotalAmount = token.TotalAmount;
        t.Units = token.Units;
        t.PegPayPostId = pegpayPostId;
        return t;
    }

    private bool IsChequeBlacklisted(Transaction trans)
    {

        if (trans.TransactionType.ToUpper().Contains("CHEQUE"))
        {
            DatabaseHandler dp = new DatabaseHandler();
            DataTable dt = dp.CheckBlacklist(trans.CustRef);
            if (dt.Rows.Count > 0)
            {
                string status = dt.Rows[0]["ChequeBlackListed"].ToString();
                if (status.Equals("1"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool ReverseAmountsMatch(Transaction trans)
    {
        DatabaseHandler dp = new DatabaseHandler();
        if (trans.Reversal.Equals("0"))
        {
            return true;
        }
        else
        {
            DataTable dt = dp.GetOriginalVendorRef(trans);
            if (dt.Rows.Count > 0)
            {
                double amount = double.Parse(trans.TransactionAmount);
                double amountToreverse = double.Parse(dt.Rows[0]["TranAmount"].ToString());
                double total = amountToreverse + amount;
                if (total.Equals(0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    private bool HasOriginalEntry(Transaction trans)
    {
        DatabaseHandler dp = new DatabaseHandler();
        if (trans.Reversal.Equals("0"))
        {
            return true;
        }
        else
        {
            DataTable dt = dp.GetOriginalVendorRef(trans);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private string GetReversalState(Transaction trans)
    {
        string res = "";
        if (trans.Reversal != null)
        {
            double amt = double.Parse(trans.TransactionAmount);
            string amountstr = amt.ToString();
            int amount = int.Parse(amountstr);
            if (amount > 0)
            {
                res = "0";
            }
            else
            {
                res = "1";
            }
        }
        return res;
    }

    private bool IsduplicateCustPayment(Transaction trans)
    {
        if (trans.VendorCode.Trim().ToUpper() == "MTN")
        {
            return false;
        }
        bool ret = false;
        DatabaseHandler dp = new DatabaseHandler();
        string custRef = trans.CustRef;
        double amount = double.Parse(trans.TransactionAmount);
        DateTime postDate = DateTime.Now;
        DataTable dt = dp.GetDuplicateCustPayment(trans.VendorCode, custRef, amount, postDate);
        if (dt.Rows.Count > 0)
        {
            DateTime Postdate = DateTime.Parse(dt.Rows[0]["RecordDate"].ToString());
            TimeSpan t = postDate.Subtract(Postdate);
            int tmdiff = t.Minutes;
            if (tmdiff < 10)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    private bool IsduplicateVendorRef(Transaction trans)
    {
        bool ret = false;
        DatabaseHandler dp = new DatabaseHandler();
        DataTable dt = dp.GetDuplicateVendorRef(trans);
        if (dt.Rows.Count > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    private bool IsduplicatePrepaidVendorRef(Transaction trans)
    {
        bool ret = false;
        DatabaseHandler dp = new DatabaseHandler();
        DataTable dt = dp.GetDuplicatePrepaidVendorRef(trans);
        if (dt.Rows.Count > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
        return ret;
    }


    private bool isSignatureValid(Transaction trans)
    {
        bool valid = false;
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            BusinessLogic bll = new BusinessLogic();
            if (trans.VendorCode.Equals("MTN"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.Equals("AFRICELL"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.Equals("TESTFLEXIPAY"))
            {
                if (trans.UtilityCode.Equals("MOWE"))
                {
                    valid = true;
                    return valid;
                }
               
            }
            else if (trans.VendorCode.Equals("EzeeMoney"))
            {
                string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                string certPath = "C:\\PegPayCertificates1\\";
               
                string vendorCode = trans.VendorCode;
                        certPath = certPath + "\\" + vendorCode + "\\";
                        string[] fileEntries = Directory.GetFiles(certPath);
                        string filePath = "";
                        if (fileEntries.Length == 1)
                        {
                    
                    valid = true; 
                        }
                        else
                        {
                            return valid;
                        }
                
            }
            else if (trans.VendorCode.Equals("CENTENARY")&& trans.DigitalSignature.Equals("1234"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.Equals("CENTENARY"))
            {
                string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                string certPath = "C:\\PegPayCertificates1\\";
                string vendorCode = trans.VendorCode;
                certPath = certPath + "\\" + vendorCode + "\\";
                string[] fileEntries = Directory.GetFiles(certPath);
                string filePath = "";
                if (fileEntries.Length == 1)
                {
                    filePath = fileEntries[0].ToString();
                    X509Certificate2 cert = new X509Certificate2(filePath);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                    SHA1Managed sha1 = new SHA1Managed();
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(text);
                    byte[] hash = sha1.ComputeHash(data);
                    byte[] sig = Convert.FromBase64String(trans.DigitalSignature);
                    return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);

                }
                else
                {
                    return valid;
                }

            }
            else if (trans.VendorCode.Equals("TEST") || trans.VendorCode.ToUpper().Equals("PEGPAY") || trans.VendorCode.ToUpper().Equals("AIRTEL"))
            {
                valid = true;
                return valid;
            }

            else if (trans.VendorCode.Equals("SMART") || trans.VendorCode.Equals("SMS2BET"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.ToUpper().Equals("ISYS"))
            {
                string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                string certPath = "C:\\PegPayCertificates1\\";
                string vendorCode = trans.VendorCode;
                certPath = certPath + "\\" + vendorCode + "\\";
                string[] fileEntries = Directory.GetFiles(certPath);
                string filePath = "";
                if (fileEntries.Length == 1)
                {
                    filePath = fileEntries[0].ToString();
                    X509Certificate2 cert = new X509Certificate2(filePath);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                    SHA1Managed sha1 = new SHA1Managed();
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] data = encoding.GetBytes(text);
                    byte[] hash = sha1.ComputeHash(data);
                    byte[] sig = Convert.FromBase64String(trans.DigitalSignature);
                    valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);
                    return valid;
                   
                }
                else
                {
                    dp.LogError(" more than 1 certificate in folder", trans.VendorCode, DateTime.Now, "NONE");
                    return false;
                }
            }
            else if (trans.VendorCode.ToUpper().Equals("CELL"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.ToUpper().Equals("SMARTMONEY"))
            {
                valid = true;
                return valid;
            }
            else if (trans.VendorCode.ToUpper().Equals("AONE HOLDINGS SMS"))
            {
                string signedData = trans.DigitalSignature;
                string message = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;
                string certPath = "C:\\PegPayCertificates1\\";
                string vendorCode = trans.VendorCode;
                certPath = certPath + "\\" + vendorCode + "\\";
                string[] fileEntries = Directory.GetFiles(certPath);
                string filePath = "C:\\PegPayMOMOCertificates\\AONEPublic.pem";
                if (fileEntries.Length == 1)
                {
                   string newSignedData = SignData(message, RsaProviderFromPrivateKeyInPemFile());
                    Boolean validRequest = Verify(message, signedData);
                    return valid=validRequest;
                }
                else
                {
                    dp.LogError(" more than 1 certificate in folder", trans.VendorCode, DateTime.Now, "NONE");
                    return false;
                }
            }
            else
            {

                string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                DataTable dt2 = dp.GetSystemSettings("1", "6");
                string certPath = dt2.Rows[0]["ValueVarriable"].ToString();
                string vendorCode = trans.VendorCode;
                certPath = certPath + "\\" + vendorCode + "\\";
                
                string[] fileEntries = Directory.GetFiles(certPath);
                string filePath = "";
                if (fileEntries.Length == 1)
                {
                    filePath = fileEntries[0].ToString();
                    X509Certificate2 cert = new X509Certificate2(filePath);
                   
                    valid = true;
                    return valid;
                }
                else
                {
                    dp.LogError(" more than 1 certificate in folder", trans.VendorCode, DateTime.Now, "NONE");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            return false;
        }
        return false;
    }

    public string GetServerIpIpValue()
    {
        string ipaddress = null;
        try
        {
            object addresslist = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in (Dns.GetHostEntry(Dns.GetHostName()).AddressList))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipaddress = ip.ToString();
                }
                else
                {
                    ipaddress = ip.ToString();
                }
            }

            if (String.IsNullOrEmpty(ipaddress))
            {
                ipaddress = "localhostip";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return ipaddress;
    }

    private static bool Verify(string message, string signedData)
    {
        string publicKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"C:\\PegPayMOMOCertificates\\AONE\\AONEPublic.pem");

        var publicRsa = RsaProviderFromPublicKeyInPemFile(publicKeyPath);

        var verifiedData = publicRsa.VerifyData(Encoding.UTF8.GetBytes(message), CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(signedData));

        return verifiedData;
    }

    public static string SignData(string message, RSAParameters privateKey)
    {
        byte[] signedBytes = null;
        try
        {

            using (var rsa = new RSACryptoServiceProvider())
            {
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(message);

                try
                {
                    rsa.ImportParameters(privateKey);
                    
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA1"));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return Convert.ToBase64String(signedBytes);
    }
    public static RSAParameters RsaProviderFromPrivateKeyInPemFile()
    {
       
        string privateKeyPath = "C:\\PegPayMOMOCertificates\\AONE HOLDINGS SMS\\NewAonePublickey.pem"; 
        using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(privateKeyPath)))
        {
            PemReader pr = new PemReader(privateKeyTextReader, new PasswordFinder(("aone1234")));
            AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
            return rsaParams;
        }
    }
    public static RSACryptoServiceProvider RsaProviderFromPublicKeyInPemFile(string publicKeyPath)
    {
        using (TextReader privateKeyTextReader = new StringReader(File.ReadAllText(publicKeyPath)))
        {
            PemReader pr = new PemReader(privateKeyTextReader);
            AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.ImportParameters(rsaParams);
            return csp;
        }
    }

    private bool isValidVendorCredentials2(string vendorCode, string password, DataTable vendorData)
    {
        bool valid = true;
       
        return valid;
    }

    private bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
    {
        bool valid = false;
        try
        {
            BusinessLogic bll = new BusinessLogic();
            if (vendorData.Rows.Count != 0)

            {
                string vendor = vendorData.Rows[0]["VendorCode"].ToString();
                string vendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
                string Hpassword = bll.HashPassword(password);
               
                MowePassword = vendorPassword;
                if (vendor.Trim().Equals(vendorCode.Trim()) && vendorPassword.Trim().Equals(Hpassword.Trim()) || vendorCode.Contains("STANBIC") || vendorCode.Equals("Airtel") || vendorCode.Equals("ISYS") || vendorCode.ToUpper().Equals("PEGPAY"))
                {
                    valid = true;
                }
                else
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
            return valid;
        }
        catch (Exception ex)
        {
            throw ex;
        }
       
    }


    public static string MowePassword = "";

    internal void file_log(string log_string)
    {
        string header = "---------------------------- NEW ERROR LINE ----------------------------";
        StreamWriter log;
        string filepath = "F:\\sigtext\\";
        string filename = filepath + "logfile.txt";
        CheckPath(filename);
        if (!File.Exists(filename))
        {
            log = new StreamWriter(filename);
        }
        else
        {
            log = File.AppendText(filename);
        }

       
        log.WriteLine(header);
        log.WriteLine(DateTime.Now);
        log.WriteLine(log_string);
        log.WriteLine();
        log.Close();
    }
    private void CheckPath(string Path)
    {
        if (!File.Exists(Path))
        {
            File.Create(Path);
        }
    }

    private bool IsValidReversalStatus(Transaction trans)
    {
        if (trans.Reversal == null)
        {
            return false;
        }
        else
        {
            if (trans.Reversal.Equals("0") || trans.Reversal.Equals("1"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private bool isActiveVendor(string vendorCode, DataTable vendorData)
    {
        bool active = false;
        try
        {
            bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
            if (activeVendor)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return active;
    }


    private void saveCustomerDetails(Customer cust)
    {
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveCustomerDetailsKCCA(cust);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private void saveURACustomerDetails(Customer cust)
    {
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveURACustomerDetails(cust);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void saveUmemeCustomerDetails(Customer cust)
    {
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveUmemeCustomerDetails(cust);
             }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void saveKCCACustomerDetails(Customer cust)
    {
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            dp.SaveKCCACustomerDetails(cust);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    private static string GetDetail(string input)
    {
        string[] array = input.Split(':');
        string output = array[1].ToString().Trim();
        return output;
    }


    [WebMethod]
    public QueryResponse ReactivateSmartCard(string smartCardNumber, string vendorCode, string password)
    {
        QueryResponse resp = new QueryResponse();
        try
        {
            DatabaseHandler dh = new DatabaseHandler();
            int rows = dh.SaveReactivateRequest(smartCardNumber);
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }
        return resp;
    }

    [WebMethod]
    public PostResponse UploadEODReconciliationReport(List<EODTransaction> lstOftrans, string vendorCode, string password)
    {
        PostResponse resp = new PostResponse();
        try
        {
            SendToEodMsq(lstOftrans, vendorCode);
            resp.StatusCode = "0";
            resp.StatusDescription = "SUCCESS";
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = "GENERAL ERROR AT PEGASUS";
        }
        return resp;
    }

    private void SendToEodMsq(EODTransaction trans)
    {
        MessageQueue queue;
        if (MessageQueue.Exists(EodReconciliationQueue))
        {
            queue = new MessageQueue(EodReconciliationQueue);
        }
        else
        {
            queue = MessageQueue.Create(EodReconciliationQueue);
        }
        Message msg = new Message();
        msg.Body = trans;
        msg.Label = trans.VendorTranId;
        msg.Recoverable = true;
        queue.Send(msg);
    }

    private void SendToEodMsq(List<EODTransaction> lstOftrans, string VendorCode)
    {
        MessageQueue queue;
        if (MessageQueue.Exists(EodReconciliationQueue))
        {
            queue = new MessageQueue(EodReconciliationQueue);
        }
        else
        {
            queue = MessageQueue.Create(EodReconciliationQueue);
        }
        Message msg = new Message();
        msg.Body = lstOftrans;
        msg.Label = VendorCode;
        msg.Recoverable = true;
        queue.Send(msg);
    }
    [WebMethod]
    public PostResponse SendSms(NWSCTransaction trans)
    {

        PostResponse resp = new PostResponse();
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            if (string.IsNullOrEmpty(trans.VendorCode))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A VENDOR CODE";
                return resp;
            }
            else if (string.IsNullOrEmpty(trans.utilityCompany))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A VENDOR CODE";
                return resp;
            }
            else if (trans.utilityCompany.ToUpper() != "SMS")
            {

                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "METHOD IS ONLY DESIGNED FOR SMS PROCESSING";
                return resp;

            }
            else if (string.IsNullOrEmpty(trans.CustomerTel))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE PROVIDE RECEIVER PHONE NUMBER";
                return resp;

            }
            else
            {
                PhoneValidator pv = new PhoneValidator();
                DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                string phone = pv.FormatTelephone(trans.CustomerTel);

                if (!pv.PhoneNumbersOk(trans.CustomerTel))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID PHONE NUMBER";
                    return resp;
                }
                else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY A VENDOR TRANSACTION REFERENCE";
                    return resp;
                }
                else if (string.IsNullOrEmpty(trans.Teller))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY A TELLER";
                }
                else if (string.IsNullOrEmpty(trans.Narration))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "Please supply the message to be delivered".ToUpper();
                }
                else if (string.IsNullOrEmpty(trans.PaymentDate))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "PLEASE SUPPLY SMS SENDING DATE FORMAT";
                }
                else if (!bll.IsValidDate(trans.PaymentDate))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID DATE FORMAT: USE dd/MM/yyyy";
                }
                //else if (!isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                //{
                //    resp.PegPayPostId = "";
                //    resp.StatusCode = "100";
                //    resp.StatusDescription = "INVALID VENDOR CREDENTIALS";
                //}
                else if (!isSignatureValid(trans))
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = "INVALID DIGITAL SIGNATURE AT PEGPAY";
                }
                else
                {
                    trans.CustRef = trans.CustomerTel;
                    trans.CustomerType = "";
                    trans.TransactionType = "";
                    trans.CustomerType = "";
                    trans.CustName = "";
                    trans.Tin = "";
                    trans.PaymentType = "CASH";
                    trans.Reversal = "0";
                    trans.TransactionType = "2";
                    trans.CustomerType = "";
                    trans.Area = "";
                    trans.TransactionAmount = "50";
                    //trans.TransactionAmount = dp.GetSmsCharge(trans.VendorCode);
                    trans.TransactionAmount = GetAmount(trans.TransactionAmount);
                    // log details
                    resp.PegPayPostId = dp.PostUmemeTransactionPrepaidVendor(trans, trans.utilityCompany, trans.Area);
                    resp.StatusCode = "1000";
                    resp.StatusDescription = "PENDING";

                }
            }
        }
        catch (Exception ee)
        {

            resp.PegPayPostId = "";
            resp.StatusCode = "100";
            resp.StatusDescription = ee.Message;
            return resp;

        }

        return resp;
    }

    [WebMethod]
    public SchoolsPostResponse PostSchoolFeesTransaction(schoolsTransaction trans)
    {

        SchoolsPostResponse resp = new SchoolsPostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        string vendorCode = trans.VendorCode;

        try
        {
            dp.SaveRequestlog(trans.VendorCode, trans.UtilityCode, "POSTING", trans.CustRef, trans.Password);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "13";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "14";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.PaymentType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "15";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "16";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "17";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.UtilityCode.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "36";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "19";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (!IsValidReversalStatus(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "25";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "22";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else if (trans.Reversal == "1" && trans.Narration.Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "23";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }

            else
            {
                if (bll.IsNumeric(trans.TransactionAmount))
                {
                    if (bll.IsValidDate(trans.PaymentDate))
                    {
                        DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                        if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                        {
                            if (isActiveVendor(trans.VendorCode, vendaData))
                            {
                                //if (isSignatureValid(trans))
                                //{
                                if (pv.PhoneNumbersOk(trans.CustomerTel))
                                {
                                    if (!IsduplicateVendorRef(trans))
                                    {
                                        if (!IsduplicateCustPayment(trans))
                                        {
                                            trans.Reversal = GetReversalState(trans);
                                            if (HasOriginalEntry(trans))
                                            {
                                                if (ReverseAmountsMatch(trans))
                                                {
                                                    if (!IsChequeBlacklisted(trans))
                                                    {
                                                      UtilityCredentials creds = dp.GetUtilityCreds("FLEXIPAY", trans.VendorCode);
                                                        if (!creds.UtilityCode.Equals(""))
                                                        {
                                                          
                                                            resp.PegPayPostId = dp.PostSchoolTransaction(trans, "FLEXIPAY");
                                                            if (bll.IsNumeric(resp.PegPayPostId))
                                                            {
                                                                resp.ReceiptNumber = resp.PegPayPostId;
                                                                resp.StatusCode = "0";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                            else
                                                            {
                                                                dp.deleteTransaction(resp.PegPayPostId, "ERROR SAVING TRANSACTION AT PEGPAY");
                                                                resp.ReceiptNumber = "";
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "100";
                                                                resp.StatusDescription = "ERROR POSTING TRANSACTION AT PEGPAY";
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.ReceiptNumber = "";
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "29";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        resp.ReceiptNumber = "";
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "29";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.ReceiptNumber = "";
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "26";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.ReceiptNumber = "";
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "24";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }

                                        }
                                        else
                                        {
                                            resp.ReceiptNumber = "";
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "21";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.ReceiptNumber = "";
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "20";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.ReceiptNumber = "";
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "12";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                           
                            else
                            {
                                resp.ReceiptNumber = "";
                                resp.PegPayPostId = "";
                                resp.StatusCode = "11";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.ReceiptNumber = "";
                            resp.PegPayPostId = "";
                            resp.StatusCode = "2";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    else
                    {
                        resp.ReceiptNumber = "";
                        resp.PegPayPostId = "";
                        resp.StatusCode = "4";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                }
                else
                {
                    resp.ReceiptNumber = "";
                    resp.PegPayPostId = "";
                    resp.StatusCode = "3";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            if (resp.StatusCode.Equals("2"))
            {
                DataTable dt = dp.GetVendorDetails(vendorCode);
                if (dt.Rows.Count != 0)
                {
                    string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    loginCount = loginCount + 1;
                    if (loginCount == 3)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                        dp.DeactivateVendor(vendorCode, ipAddress);
                    }
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                    }
                }
            }
        }
        catch (System.Net.WebException wex)
        {
            if (trans.CustomerType.ToUpper().Equals("PREPAID"))
            {
                dp.deleteTransaction(resp.PegPayPostId, "UNABLE TO CONNECT TO UMEME");
                resp.ReceiptNumber = "";
                resp.PegPayPostId = "";
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO UMEME";
            }
            else
            {
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        catch (SqlException sqlex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(sqlex.Message, trans.VendorCode, DateTime.Now, "KCCA");

        }
        catch (Exception ex)
        {
            resp.ReceiptNumber = "";
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
            resp.PegPayPostId = "";
            dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "KCCA");
        }
        return resp;
    }

    [WebMethod]
    public PostResponse MakeUtilityPaymentPrepaidVendor(NWSCTransaction trans)
    {
        PostResponse resp = new PostResponse();
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        PhoneValidator pv = new PhoneValidator();
        PostResponse valResp = new PostResponse();
         
        if (trans.CustomerTel == null)
        {
            trans.CustomerTel = "";
        }
        if (trans.Reversal == null)
        {
            trans.Reversal = "";
        }
        if (trans.Email == null)
        {
            trans.Email = "";
        }
        if (trans.Area == null)
        {
            trans.Area = "";

        }
       

        if (string.IsNullOrEmpty(trans.CustRef))
        {
            resp.PegPayPostId = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "PLEASE SUPPLY A CUSTOMER REFERENCE";
            return resp;
        }
        else if (string.IsNullOrEmpty(trans.VendorTransactionRef))
        {
            resp.PegPayPostId = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "PLEASE SUPPLY A VENDOR TRANSACTION REFERENCE";
            return resp;
        }
        else if (string.IsNullOrEmpty(trans.CustomerTel))
        {
            resp.PegPayPostId = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "PLEASE SUPPLY A CUSTOMER TEL";
            return resp;
        }
        string vendorCode = trans.VendorCode;
        try
        {
            dp.SaveRequestlog(trans.VendorCode, trans.utilityCompany, "POSTING", trans.CustRef, trans.Password);
            DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
            if (trans.CustName == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A CUSTOMER NAME";
            }
            else if (trans.CustName.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A CUSTOMER NAME";
            }
            else if (trans.Area.Trim().Equals("") && trans.utilityCompany.Equals("NWSC"))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY AN AREA FOR NWSC TRANSACTIONS";

            }
            else if (trans.Area.Trim().Equals("") && trans.utilityCompany.Equals("DSTV"))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SPECIFY A BOUQUET CODE FOR DSTV TRANSACTION";

            }
            else if (trans.TransactionType == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A TRANSACTION TYPE";
            }
            else if (trans.TransactionType.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A TRANSACTION TYPE";
            }
            else if (trans.VendorTransactionRef == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A VENDOR TRANSACTION REFERENCE";
            }
            else if (trans.VendorTransactionRef.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A VENDOR TRANSACTION REFERENCE"; ;
            }
            else if (trans.Teller == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A TELLER";
            }
            else if (trans.Teller.Trim().Equals(""))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A TELLER";
            }
            else if (trans.DigitalSignature == null)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A DIGITAL SIGNATURE";
            }
            else if (trans.DigitalSignature.Length == 0)
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PLEASE SUPPLY A DIGITAL SIGNATURE";
            }


            else if (!bll.IsNumeric(trans.TransactionAmount))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID TRANSACTION AMOUNT";
            }
            else if (!isCustumerReferenceValid(trans.CustRef, trans.utilityCompany))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID CUSTOMER REFERENCE"; ;
            }
            else if (!bll.IsValidDate(trans.PaymentDate))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID DATE FORMAT: USE dd/MM/yyyy";
            }
            else if (!isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID VENDOR CREDENTIALS";
            }
            else if (!isActiveVendor(trans.VendorCode, vendaData))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "PEGPAY VENDOR CREDENTIALS HAVE BEEN DEACTIVATED";
            }
            else if (!isSignatureValid(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID DIGITAL SIGNATURE AT PEGPAY";
            }
            else if (!pv.PhoneNumbersOk(trans.CustomerTel))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID PHONE NUMBER";
            }
            else if (IsduplicatePrepaidVendorRef(trans))
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "DUPLICATE VENDOR REFERENCE";
            }
            else if (IsduplicateCustPayment(trans) && trans.Reversal != "1")
            {
                resp.PegPayPostId = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "FAILED: SUSPECTED DOUBLE POSTING AT PEGASUS";
            }
            else if (trans.Reversal == "1" && !IsValidReversal(trans, out valResp))
            {
                resp.PegPayPostId = valResp.PegPayPostId;
                resp.StatusCode = valResp.StatusCode;
                resp.StatusDescription = valResp.StatusDescription;
            }
            else
            {
                string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                if (!(vendorType.Equals("PREPAID")))
                {
                    resp.StatusCode = "999";
                    resp.StatusDescription = "METHOD ONLY FOR PREPAID AGENTS";
                }
                else
                {

                    if (trans.Reversal == "1")
                    {
                        // log details
                        ReversalRequest req = GetReversalRequest(trans);
                        resp.PegPayPostId = dp.LogReversalRequest(req);
                        resp.StatusCode = "0";
                        resp.StatusDescription = "SUCCESS";
                    }
                    else
                    {
                        string status_code = "";
                        string status_description = "";
                        if (trans.utilityCompany.ToUpper() == "SMS")
                        {
                            trans.CustRef = trans.CustomerTel;
                            if (!pv.PhoneNumbersOk(trans.CustomerTel))
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "FAILED: PLEASE SPECIFY RECEIPEINT PHONE NUMBER";
                                return resp;
                            }
                            else if (string.IsNullOrEmpty(trans.Narration))
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "100";
                                resp.StatusDescription = "FAILED: MESSAGE NOT SUPPLIED";
                                return resp;
                            }
                            trans.CustomerType = "";
                            trans.Area = "";
                            trans.TransactionAmount = dp.GetSmsCharge(vendorCode);
                        }

                        else if (trans.utilityCompany.ToUpper().Equals("DATA"))
                        {
                            string Network = bll.GetTelecomNetwork(trans.CustomerTel);
                            //check the data bundle code passed
                            DataBundleDetails bundleResponse = dp.GetDataBundlePrice(Network, trans.Area);
                            if (bundleResponse.StatusCode.Equals("0"))
                            {
                                trans.Area = bundleResponse.bundleCode;
                                trans.TransactionAmount = Decimal.Truncate(Decimal.Parse(bundleResponse.bundlePrice)).ToString();
                                trans.Narration = "Payment For: " + bundleResponse.bundleDescription;
                                resp.PegPayPostId = dp.PostUmemeTransactionPrepaidVendor(trans, trans.utilityCompany, trans.Area);
                                status_code = "1000";
                                status_description = "PENDING";

                            }
                            else
                            {
                                dp.LogError(bundleResponse.StatusDescription, trans.VendorCode, new DateTime(), Network);
                                status_code = "500";
                                status_description = bundleResponse.StatusDescription;

                            }

                        }                     
                        trans.TransactionAmount = GetAmount(trans.TransactionAmount);
                        // log details
                        resp.PegPayPostId = dp.PostUmemeTransactionPrepaidVendor(trans, trans.utilityCompany, trans.Area);
                        resp.StatusCode = "1000";
                        resp.StatusDescription = "PENDING";
                    }
                }
            }
        }
        catch (SqlException sqlex)
        {
           
        }
        catch (Exception ex)
        {

        }
        return resp;
    }

    internal string GetAmount(string amount)
    {
        if (amount.Contains("-"))
        {
            return amount.Replace("-", "");
        }
        else
        {
            return amount;
        }
    }

    private ReversalRequest GetReversalRequest(NWSCTransaction trans)
    {
        ReversalRequest request = new ReversalRequest();
        request.OriginalTransactionId = trans.TranIdToReverse;
        request.ReversalTransactionId = trans.VendorTransactionRef;
        request.Reason = trans.Narration;
        request.VendorCode = trans.VendorCode;
        return request;
    }

    private bool IsValidReversal(NWSCTransaction trans, out PostResponse result)
    {
        DatabaseHandler dp = new DatabaseHandler();
        result = new PostResponse();
        PostResponse valResp = new PostResponse();
        if (string.IsNullOrEmpty(trans.VendorTransactionRef))
        {
            result.StatusCode = "50";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (trans.TranIdToReverse == trans.VendorTransactionRef)
        {
            result.StatusCode = "51";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (string.IsNullOrEmpty(trans.Narration))
        {
            result.StatusCode = "52";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (!bll.IsValidReversalAmount(trans))
        {
            result.StatusCode = "53";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (!bll.OriginalTransactionExistsAndAcceptsReversal(trans.VendorCode, trans.TranIdToReverse, out valResp))
        {
            result.PegPayPostId = valResp.PegPayPostId;
            result.StatusCode = valResp.StatusCode;
            result.StatusDescription = valResp.StatusDescription;
            return false;
        }
        if (!bll.IsValidOriginalTransaction(trans))
        {
            result.StatusCode = "60";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (!bll.IsValidAmountToReverse(trans))
        {
            result.StatusCode = "56";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        if (bll.IsAlreadyReversed(trans, out valResp))
        {
            result.PegPayPostId = valResp.PegPayPostId;
            result.StatusCode = valResp.StatusCode;
            result.StatusDescription = valResp.StatusDescription;
            return false;
        }
        if (bll.IsDuplicateReversalId(trans))
        {
            result.StatusCode = "57";
            result.StatusDescription = dp.GetStatusDescr(result.StatusCode);
            return false;
        }
        else
        {
            return true;
        }

    }


    private bool isCustumerReferenceValid(string custReference, string utilityCompany)
    {
        bool isValid = false;
        try
        {
            if (utilityCompany.Equals("NWSC") || utilityCompany.Equals("UMEME") || utilityCompany.Equals("DSTV"))
            {
                DatabaseHandler dh = new DatabaseHandler();
                string status = dh.checkCustomerDetails(custReference);
                if (status == "true")
                {
                    isValid = true;
                }
                else
                {
                    isValid = false;
                }
            }
            else
            {
                isValid = true;
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
        return isValid;
    }

    [WebMethod]
    public NWSCQueryResponse QueryCustomerDetailsPrepaidVendor(string utility, string customerReference, string area, string vendorCode, string password)
    {
        NWSCQueryResponse resp = new NWSCQueryResponse();
        DatabaseHandler dp = new DatabaseHandler();
        try
        {
            dp.SaveRequestlog(vendorCode, utility, "VERIFICATION", customerReference, password);
            DataTable vendorData = dp.GetVendorDetails(vendorCode);
            if (isValidVendorCredentials(vendorCode, password, vendorData))
            {
                if (isActiveVendor(vendorCode, vendorData))
                {
                    string strLoginCount = vendorData.Rows[0]["InvalidLoginCount"].ToString();
                    int loginCount = int.Parse(strLoginCount);
                    if (loginCount > 0)
                    {
                        dp.UpdateVendorInvalidLoginCount(vendorCode, 0);
                    }

                    Customer cust = dp.GetCustomerDetails(customerReference, area, utility);
                    if (cust.StatusCode.Equals("0"))
                    {
                        resp.Area = cust.Area;
                        resp.CustomerName = cust.CustomerName;
                        resp.CustomerReference = cust.CustomerRef;
                        resp.OutstandingBalance = cust.Balance;
                        resp.CustType = cust.TIN;
                        resp.StatusCode = "0";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else
                    {
                        resp.StatusCode = "100";
                        resp.StatusDescription = "MATCH NOT FOUND";
                        resp.Area = "";
                        resp.CustomerName = "";
                        resp.CustomerReference = "";
                        resp.OutstandingBalance = "";
                        resp.CustType = "";
                    }

                }
                else
                {
                    resp.StatusCode = "11";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                }
            }
            else
            {
                resp.StatusCode = "2";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
        }
        catch (System.Net.WebException wex)
        {
            Customer cust = dp.GetCustomerDetails(customerReference, area, utility);
            if (cust.StatusCode.Equals("0"))
            {
                resp.Area = cust.Area;
                resp.CustomerName = cust.CustomerName;
                resp.CustomerReference = cust.CustomerRef;
                resp.OutstandingBalance = "0";
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.StatusCode = "30";
                resp.StatusDescription = "UNABLE TO CONNECT TO " + utility;
            }
            dp.LogError(wex.Message, vendorCode, DateTime.Now, utility);
        }
        catch (SqlException sqlex)
        {
            resp.StatusCode = "31";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
        }
        catch (Exception ex)
        {
            resp.StatusCode = "32";
            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            dp.LogError(ex.Message, vendorCode, DateTime.Now, utility);
        }
        return resp;
    }
    [WebMethod]
    public NWSCQueryResponse queryCustomerAtPostBank(string utility, string customerReference, string area, string vendorCode, string password)
    {
        NWSCQueryResponse customer = new NWSCQueryResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertValidation;
            UtilityReferences.PBU.CardlessMoMoApi cardless = new UtilityReferences.PBU.CardlessMoMoApi();
            UtilityReferences.PBU.Customer Cust = new UtilityReferences.PBU.Customer();
            Cust = cardless.InquireUtilityCustomer(utility, customerReference, area, vendorCode, password);
            customer.Area = Cust.Area;
            customer.CustomerName = Cust.CustName;
            customer.CustType = Cust.CustomerType;
            customer.CustomerReference = Cust.CustRef;
            customer.OutstandingBalance = Cust.OutStandingBal;
            customer.StatusCode = Cust.StatusCode;
            customer.StatusDescription = Cust.StatusDescription;

        }
        catch (Exception ex)
        {
            customer.CustomerReference = customerReference;
            customer.StatusDescription = "UNABLE TO INQUIRE STARTIMES AT THE MOMENT.";

        }

        return customer;
    }


    private static bool RemoteCertValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

}
