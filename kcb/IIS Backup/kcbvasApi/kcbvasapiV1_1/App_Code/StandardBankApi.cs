using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using ThirdPartyInterfaces.PegPay;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using log4net;
using ThirdPartyInterfaces.BillerEngineWebService;
using System.Security.Cryptography;
using System.Text;

[WebService(Namespace = "http://pegasustechnologies.co.ug/", Name = "Kenya Commercial Bank")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class StandardBank : System.Web.Services.WebService
{
    // Define a static logger variable so that it references the name of your class 
    private static readonly ILog log = LogManager.GetLogger(typeof(StandardBank));

    BussinessLogic bll = new BussinessLogic();

    
    DatabaseHandler dh = new DatabaseHandler();

    string password;

    public StandardBank()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //[WebMethod]
    //public billerTransactionResponse CallKCB(string UserName, string Password, string ServiceId, string ServiceName, string TransactionReference, string TransactionStatus, string TransactionStatusDescription, string ReversalStaus, string SessionId, customMap Payload)
    //{
    //    //customMap cust = new customMap();
    //    customMap cust = Payload;
    //    BillerEngineWebService bl = new BillerEngineWebService();
    //    Response resp = new Response();
    //    billerTransactionResponse res ;
    //    res = bl.KCBBillerTransactionCallBack(UserName, Password, ServiceId, ServiceName, TransactionReference, TransactionStatus, TransactionStatusDescription, ReversalStaus, SessionId,Payload);
    //    return res;
    //}

    [WebMethod]
    public ValidateCustomerResponse ValidateCustomer(ValidateCustomerRequest requestData)
    {
        string strCert = null;
        //Hashing the password
        try
        {
            string text = requestData.Credentials.Password;
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
            strCert = Convert.ToBase64String(digitalCert);

        }
        catch (Exception e)
        {
            dh.LogErrorKCB("", "", DateTime.Now, "", "", e.Message, "");

        }
        if (String.IsNullOrEmpty(strCert))
        {
            password = "";
        }
        else
        {
            password = strCert;
        }


        dh.SaveRequestlog("", requestData.Credentials.Username, "", password, "VALIDATION", DateTime.Now);
        ValidateCustomerResponse resp = new ValidateCustomerResponse();
        try
        {
            if (requestData.IsValidRequest())
            {
                resp = bll.QueryCustomerDetails(requestData);
            }
            else
            {
                resp.Details.Status.StatusCode = "200";
                resp.Details.Status.Description = "Successful";
                resp.Details.Resultz.StatusCode = requestData.PegPayStatusCode;
                resp.Details.Resultz.StatusDescription = requestData.PegPayStatusDescription;
            }
        }
        catch (Exception e)
        {
            requestData.PegPayStatusCode = "200";
            requestData.PegPayStatusDescription = "Successful";
            resp.Details.Status.StatusCode = "100";
            resp.Details.Status.Description = "UNABLE TO VERIFY CUSTOMER REFERENCE AT THE MOMENT";
            //  dh.LogError("SendPendingTransactionsToUtility TranId: " + tran.VendorTranId + " : " + e.Message, tran.VendorCode, DateTime.Now, tran.UtilityCompany);
        }
        return resp;
    }


    private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }



    private QueryRequest GetDSTVBouquetQueryObject()
    {
        QueryRequest query = new QueryRequest();
        query.QueryField1 = "";
        query.QueryField2 = "";
        query.QueryField5 = "MTN";
        query.QueryField6 = "83Y84KW560";
        return query;
    }

  

    [WebMethod]
    public PostTransactionResponse PostTransaction(PostTransactionRequest requestData)
    {
        string strCert = null;
        //Hashing the password
        try
        {
            string text = requestData.Credentials.Password;
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
            strCert = Convert.ToBase64String(digitalCert);

        }
        catch (Exception e)
        {
            dh.LogErrorKCB(requestData.PayLoad.ReferenceID, requestData.Credentials.Username, DateTime.Now, "POSTING", "", e.Message, "");
        }
        if (String.IsNullOrEmpty(strCert))
        {
            password = "";
        }
        else
        {
            password = strCert;
        }

        dh.SaveRequestlog(requestData.PayLoad.ReferenceID, requestData.Credentials.Username, requestData.PayLoad.Amount, password, "POSTING", DateTime.Now);

        PostTransactionResponse resp = new PostTransactionResponse();
        try
        {
            if (requestData.IsValidRequest())
            {
                resp = bll.PostTransaction(requestData);
            }
            else
            {
                resp.Status = new Status();
                resp.Status.StatusCode = "200";
                resp.Status.Description = "Successful";
                resp.PayLoad.StatusCode = requestData.PegPayStatusCode;
                resp.PayLoad.Description = requestData.PegPayStatusDescription;
            }
        }
        catch (Exception e)
        {
            requestData.PegPayStatusCode = "101";
            requestData.PegPayStatusDescription = "GENERAL ERROR AT PEGASUS";
            resp.Status.StatusCode = requestData.PegPayStatusCode;
            resp.Status.Description = requestData.PegPayStatusDescription;
        }
        return resp;
    }

    [WebMethod]
    public QueryTransactionStatusResponse QueryTransactionStatus(QueryTransactionStatusRequest requestData)
    {
        string strCert = null;
        //Hashing the password
        try
        {
            string text = requestData.Credentials.Password;
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
            strCert = Convert.ToBase64String(digitalCert);

        }
        catch (Exception e)
        {
            dh.LogErrorKCB("", "", DateTime.Now, "", "", e.Message, "");
        }
        if (String.IsNullOrEmpty(strCert))
        {
            password = "";
        }
        else
        {
            password = strCert;
        }

        dh.SaveRequestlog(requestData.PayLoad.ReferenceID, requestData.Credentials.Username, "", password, "VERIFICATION", DateTime.Now);

        QueryTransactionStatusResponse resp = new QueryTransactionStatusResponse();
        try
        {
            if (requestData.IsValidRequest())
            {
                resp = bll.QueryTransactionStatus(requestData);
            }
            else
            {
                resp.Status.StatusCode = "200";
                resp.Status.Description = "Successful";
                resp.PayLoad.StatusCode = requestData.PegPayStatusCode;
                resp.PayLoad.Description = requestData.PegPayStatusDescription;
            }
        }
        catch (Exception e)
        {
            requestData.PegPayStatusCode = "106";
            requestData.PegPayStatusDescription = "GENERAL ERROR AT PEGASUS";
            resp.Status.StatusCode = requestData.PegPayStatusCode;
            resp.Status.Description = requestData.PegPayStatusDescription;

            //dh.LogError("SendPendingTransactionsToUtility TranId: " + tran.VendorTranId + " : " + e.Message, tran.VendorCode, DateTime.Now, tran.UtilityCompany);
        }
        return resp;
    }

}
