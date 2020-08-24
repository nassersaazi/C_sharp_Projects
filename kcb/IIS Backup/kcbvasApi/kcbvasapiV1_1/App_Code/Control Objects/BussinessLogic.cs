using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ThirdPartyInterfaces.PegPay;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.NetworkInformation;
using System.IO;

/// <summary>
/// Summary description for BussinessLogic
/// </summary>
public class BussinessLogic
{
    DatabaseHandler dh = new DatabaseHandler();

    public BussinessLogic()
    {
    }

    public ValidateCustomerResponse QueryCustomerDetails(ValidateCustomerRequest requestData)
    {
        ValidateCustomerResponse queryResp = new ValidateCustomerResponse();
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

            //pick correct service name from db
            DataTable dt = dh.GetServiceDetailsFromDB(requestData.PayLoad.ServiceCode);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string ServiceCode = dr["ServiceCode"].ToString();
                string ServiceName = dr["ServiceName"].ToString();
                string ServiceID = dr["ServiceID"].ToString();
                string requestCustType = "";

                if (ServiceCode.Equals("NWSC"))
                {
                    //read area from db
                    requestData.PayLoad.Area = ServiceName.Split(' ')[1];
                }
                else if (ServiceCode.Equals("UMEME"))
                {
                    requestCustType = ServiceName.Split(' ')[1].ToUpper();
                }
                //else if (ServiceCode == "FLEXIPAY-MERCHANT")
                //{
                //    return GetMerchantDetails(requestData, ServiceID);
                //}
                

                //generate level 1 query object
                QueryRequest levelOneQuery = GetPegpayQueryObject(requestData, ServiceCode);

                //query from level 1
                PegPay levelOneApi = new PegPay();
                ThirdPartyInterfaces.PegPay.Response levelOneResp = levelOneApi.QueryCustomerDetails(levelOneQuery);

                //return appropriate response
                if (levelOneResp.ResponseField6.Equals("0"))
                {
                    if (ServiceCode.Equals("UMEME"))
                    {
                        queryResp = ReturnUmemeResponse(levelOneResp, requestData,requestCustType);
                    }
                    else if (ServiceCode.Equals("FLEXIPAY"))
                    {
                        queryResp.Details.Status.StatusCode = "";
                        queryResp.Details.Status.Description = "";
                        queryResp.Details.Resultz.Area = levelOneResp.ResponseField3;
                        queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
                        queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
                        queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
                        queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
                        queryResp.Details.Resultz.Tin = levelOneResp.ResponseField8;//requestData.PayLoad.Area;
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "0";
                        queryResp.Details.Resultz.StatusDescription = "SUCCESS";
                    }
                    else if (ServiceCode.Equals("URA"))
                    {
                        queryResp.Details.Status.StatusCode = "";
                        queryResp.Details.Status.Description = "";
                        queryResp.Details.Resultz.Area = levelOneResp.ResponseField3;
                        queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
                        queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
                        queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
                        queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
                        queryResp.Details.Resultz.Tin = levelOneResp.ResponseField3;
                        queryResp.Details.Resultz.RegistrationDate = string.IsNullOrEmpty(levelOneResp.ResponseField8) ? "" : levelOneResp.ResponseField8;
                        queryResp.Details.Resultz.ExpiryDate = string.IsNullOrEmpty(levelOneResp.ResponseField9) ? "" : levelOneResp.ResponseField9;
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "0";
                        queryResp.Details.Resultz.StatusDescription = "SUCCESS";
                    }
                    else if (ServiceCode.Equals("NSSF"))
                    {
                        queryResp.Details.Status.StatusCode = "";
                        queryResp.Details.Status.Description = "";
                        queryResp.Details.Resultz.Area = levelOneResp.ResponseField10;
                        queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
                        queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
                        queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
                        queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
                        queryResp.Details.Resultz.Tin = levelOneResp.ResponseField9;
                        queryResp.Details.Resultz.RegistrationDate = "";
                        queryResp.Details.Resultz.ExpiryDate = levelOneResp.ResponseField3;
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "0";
                        queryResp.Details.Resultz.StatusDescription = "SUCCESS";
                    }
                    else if (ServiceCode.Equals("DSTV")||ServiceCode.Equals("GOTV"))
                    {
                        queryResp = ReturnDSTVResponse(levelOneResp, requestData, ServiceCode);
                    }
                    else
                    {
                        queryResp.Details.Status.StatusCode = "";
                        queryResp.Details.Status.Description = "";
                        queryResp.Details.Resultz.Area = levelOneResp.ResponseField3;
                        queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
                        queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
                        queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
                        queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
                        queryResp.Details.Resultz.Tin = levelOneResp.ResponseField3;
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "0";
                        queryResp.Details.Resultz.StatusDescription = "SUCCESS";
                    }
                }
                else
                {
                    if (ServiceCode.Equals("NSSF"))
                    {
                        if (levelOneResp.ResponseField7.ToUpper().Contains("EXPIRED"))
                        {
                            levelOneResp.ResponseField7 = "TRANSACTION NUMBER EXPIRED";
                        }
                        else if (levelOneResp.ResponseField7.ToUpper().Contains("DELETED"))
                        {
                            levelOneResp.ResponseField7 = "TRANSACTION NUMBER DELETED";
                        }
                        else if (levelOneResp.ResponseField7.ToUpper().Contains("EXISTS"))
                        {
                            levelOneResp.ResponseField7 = "NO TRANSACTION NUMBER";
                        }
                        else if (levelOneResp.ResponseField7.ToUpper().Contains("NUMBER PAID"))
                        {
                            levelOneResp.ResponseField7 = "TRANSACTION NUMBER ALREADY PAID";
                        }
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "100";
                        queryResp.Details.Resultz.StatusDescription = levelOneResp.ResponseField7;
                    }
                    else
                    {
                        queryResp.Details.Status.StatusCode = "200";
                        queryResp.Details.Status.Description = "Successful";
                        queryResp.Details.Resultz.StatusCode = "100";
                        queryResp.Details.Resultz.StatusDescription = levelOneResp.ResponseField7;
                    }

                }
            }
            else
            {
                queryResp.Details.Status.StatusCode = "200";
                queryResp.Details.Status.Description = "Successful";
                queryResp.Details.Resultz.StatusCode = "100";
                queryResp.Details.Resultz.StatusDescription = "INVALID MERCHANT ID";
            }

            return queryResp;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

   
    private ValidateCustomerResponse ReturnDSTVResponse(ThirdPartyInterfaces.PegPay.Response levelOneResp, ValidateCustomerRequest requestData, string requestCustType)
    {
        ValidateCustomerResponse queryResp = new ValidateCustomerResponse();
        //stanbic requested that for DSTV
        //we only return success if customer type returned matches
        //customer type in request e.g GOTV or DSTV
        if (levelOneResp.ResponseField5 == requestCustType)
        {
            queryResp.Details.Status.StatusCode = "";
            queryResp.Details.Status.Description = "";
            queryResp.Details.Resultz.Area = levelOneResp.ResponseField3;
            queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
            queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
            queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
            queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
            queryResp.Details.Resultz.Tin = levelOneResp.ResponseField3;
            queryResp.Details.Status.StatusCode = "200";
            queryResp.Details.Status.Description = "Successful";
            queryResp.Details.Resultz.StatusCode = "0";
            queryResp.Details.Resultz.StatusDescription = "SUCCESS";
        }
        else
        {
            queryResp.Details.Status.StatusCode = "200";
            queryResp.Details.Status.Description = "Successful";
            queryResp.Details.Resultz.StatusCode = "100";
            queryResp.Details.Resultz.StatusDescription = "INVALID CUSTOMER REFERENCE";
        }
        return queryResp;
    }

    private ValidateCustomerResponse ReturnUmemeResponse(ThirdPartyInterfaces.PegPay.Response levelOneResp, ValidateCustomerRequest requestData, string requestCustType)
    {
        ValidateCustomerResponse queryResp = new ValidateCustomerResponse();
        //stanbic requested that for UMEME
        //we only return success if customer type returned matches
        //customer type in request
        if (levelOneResp.ResponseField5 == requestCustType)
        {
            queryResp.Details.Status.StatusCode = "";
            queryResp.Details.Status.Description = "";
            queryResp.Details.Resultz.Area = levelOneResp.ResponseField3;
            queryResp.Details.Resultz.CustomerName = levelOneResp.ResponseField2;
            queryResp.Details.Resultz.CustomerReference = levelOneResp.ResponseField1;
            queryResp.Details.Resultz.CustomerType = levelOneResp.ResponseField5;
            queryResp.Details.Resultz.OutstandingBalance = levelOneResp.ResponseField4;
            queryResp.Details.Resultz.Tin = levelOneResp.ResponseField3;
            queryResp.Details.Status.StatusCode = "200";
            queryResp.Details.Status.Description = "Successful";
            queryResp.Details.Resultz.StatusCode = "0";
            queryResp.Details.Resultz.StatusDescription = "SUCCESS";
        }
        else
        {
            queryResp.Details.Status.StatusCode = "200";
            queryResp.Details.Status.Description = "Successful";
            queryResp.Details.Resultz.StatusCode = "100";
            queryResp.Details.Resultz.StatusDescription = "INVALID CUSTOMER REFERENCE";
        }
        return queryResp;
    }

    private QueryRequest GetPegpayQueryObject(ValidateCustomerRequest tran, string utilityCode)
    {
        QueryRequest req = new QueryRequest();
        req.QueryField1 = tran.PayLoad.CustomerRef;
        if (utilityCode.ToUpper() == "DSTV" || utilityCode.ToUpper() == "GOTV")
        {
            req.QueryField2 = "";
        }
        else
        {
            req.QueryField2 = tran.PayLoad.Area;
        }
        req.QueryField4 = utilityCode;
        req.QueryField5 = "STANBIC_VAS";
        req.QueryField6 = "53P48KU262";
        if (utilityCode.ToUpper() == "FLEXIPAY")
        {
            req.QueryField4 = "STB_SCHOOL";
        }
        return req;
    }


    private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    internal CheckServiceStatusResponse CheckServiceStatus(CheckServiceStatusRequest requestData)
    {
        CheckServiceStatusResponse resp = new CheckServiceStatusResponse();
        try
        {
            //they call this when they want to know if the utility is up
            PingResponse pingResp = PingService(requestData);
            if (pingResp.StatusCode.Equals("0"))
            {
                resp.Status = new Status();
                resp.Status.StatusCode = "200";
                resp.Status.Description = "Successfully Authenticated";
                resp.Payload.StatusCode = "100";
                resp.Payload.CountryID = requestData.Payload.CountryID;
                resp.Payload.LastTimeUp = pingResp.LastTimeUp;
                resp.Payload.LastWentoff = "";
                resp.Payload.NetworkID = requestData.Payload.NetworkID;
                resp.Payload.Reason = "NONE";
                resp.Payload.ServiceID = requestData.Payload.ServiceID;
                resp.Payload.ServiceType = pingResp.ServiceType;
            }
            else
            {
                resp.Status = new Status();
                resp.Status.StatusCode = "200";
                resp.Status.Description = "Successfully Authenticated";
                resp.Payload.StatusCode = "105";
                resp.Payload.CountryID = requestData.Payload.CountryID;
                resp.Payload.LastTimeUp = "";
                resp.Payload.LastWentoff = pingResp.LastWentOff;
                resp.Payload.NetworkID = requestData.Payload.NetworkID;
                resp.Payload.Reason = pingResp.Reason;
                resp.Payload.ServiceID = requestData.Payload.ServiceID;
                resp.Payload.ServiceType = pingResp.ServiceType;
            }

        }
        catch (Exception e)
        {
            resp.Status = new Status();
            resp.Status.StatusCode = "200";
            resp.Status.Description = "Successfully Authenticated";
            resp.Payload.StatusCode = "105";
            resp.Payload.Reason = e.Message;
        }
        return resp;
    }

    public PostTransactionResponse PostTransaction(PostTransactionRequest requestData)
    {
        PostTransactionResponse resp = new PostTransactionResponse();
        try
        {
            //string UtilityCode = dh.GetServiceNameFromDB(requestData.Payload.MerchantID);
            string Id = dh.InsertIntoStanbicRecieved(requestData, requestData.PayLoad.UtilityCode, "PENDING");
            resp.Status = new Status();
            resp.Status.StatusCode = "100";
            resp.Status.Description = "Successful request receipt";
            resp.PayLoad.C360UniqueID = Id;
            resp.PayLoad.Description = "Request successfully logged";
            resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
            resp.PayLoad.StatusCode = "113";
        }
        catch (Exception e)
        {
            resp.Status = new Status();
            resp.Status.StatusCode = "101";
            resp.Status.Description = "Technical error. Try and post again ";
            dh.LogError("Exception " + e.Message, "STANBIC_VAS", DateTime.Now, requestData.PayLoad.ReferenceID);
            //string path = @"E:\Logs\StanbicApiLogs\Errors\ErrorFile_" + DateTime.Today.ToString("yyyyddMM")+".txt";
            //string text = "\n";
            //if (File.Exists(path))
            //{
            //    text = File.ReadAllText(path);
            //}
           // File.WriteAllText(path, text+"\n"+e.Message);
            resp.PayLoad.StatusCode = "112";
        }
        return resp;
    }

    public QueryTransactionStatusResponse QueryTransactionStatus(QueryTransactionStatusRequest requestData)
    {
        QueryTransactionStatusResponse resp = new QueryTransactionStatusResponse();
        try
        {
            DataTable dt = dh.GetTransactionStatus(requestData);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                resp.Status = new Status();
                resp.Status.StatusCode = "100";
                resp.Status.Description = "Successful";
                resp.PayLoad.C360UniqueID = row["Id"].ToString();
                string status = row["Status"].ToString().ToUpper();

                //string
                if (status.Equals("SUCCESS"))
                {
                    string token = row["Reason"].ToString();
                    resp.PayLoad.Description = "SUCCESSFUL|" + token;
                    resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
                    resp.PayLoad.StatusCode = "200";
                }
                else if (status.Equals("PENDING"))
                {
                    resp.PayLoad.Description = "Request is pending. Scheduled for processing";
                    resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
                    resp.PayLoad.StatusCode = "0";
                }
                else if (status.Equals("FAILED"))
                {
                    resp.PayLoad.Description = "Failed";
                    resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
                    resp.PayLoad.StatusCode = "202";
                }
                else
                {
                    resp.PayLoad.Description = "Request is Pending.";
                    resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
                    resp.PayLoad.StatusCode = "0";
                }
            }
            else
            {
                resp.Status = new Status();
                resp.Status.StatusCode = "100";
                resp.Status.Description = "Successful";//"No Record of Transaction " + requestData.PayLoad.ReferenceID + " at Pegasus";
                resp.PayLoad.C360UniqueID = "";
                resp.PayLoad.Description = "No Record of Transaction " + requestData.PayLoad.ReferenceID + " at Pegasus";
                resp.PayLoad.ReferenceID = requestData.PayLoad.ReferenceID;
                resp.PayLoad.StatusCode = "203";
            }
        }
        catch (Exception e)
        {
            resp.Status = new Status();
            resp.Status.StatusCode = "106";
            resp.Status.Description = "Technical error. Try and query again";
        }
        return resp;
    }

    private PingResponse PingService(CheckServiceStatusRequest requestData)
    {
        PingResponse pingResp = new PingResponse();
        try
        {
            
            pingResp.ServiceType = dh.GetServiceType(requestData.Payload.ServiceID);
            string IpAddress = dh.GetServerIpAddress(requestData.Payload.ServiceID);
            Ping myPing = new Ping();
            PingReply reply = myPing.Send(IpAddress, 5000);
            if (reply == null)
            {
                pingResp.LastTimeDown = "" + DateTime.Now;
                pingResp.StatusCode = "100";
                pingResp.StatusDescription = "Service Is Unavailable";
            }
            else
            {
                pingResp.LastTimeUp = "" + DateTime.Now;
                pingResp.StatusCode = "0";
                pingResp.StatusDescription = "Service Is Up and Running";
            }
        }
        catch (Exception ex)
        {
            pingResp.LastTimeUp = "" + DateTime.Now;
            pingResp.StatusCode = "100";
            pingResp.Reason = ex.Message;
        }
        return pingResp;

    }

    public bool IsValidCredentials(Credentials credentials)
    {
        if (credentials.Username == "KCB" && credentials.Password == "63T25KG001")
        {
            return true;
        }
        return false;
    }

    internal bool IsDuplicateVendorRef(string referenceID)
    {
        DataTable dt = dh.GetDuplicateTranByReferenceID(referenceID);
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
