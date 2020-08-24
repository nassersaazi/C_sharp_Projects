using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using System.Collections.Generic;
using UtilityReferences.DbApi;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Messaging;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using UtilityReferences.MailApi;
/// <summary>
/// Summary description for DatabaseHandler
/// </summary>
public class DatabaseHandler
{
    //private Database PegPayInterface;
    private DbCommand command;
    private const String constring = "TestPegPay";
    private DbAccess PegPayInterface = new DbAccess ();
    private Database pegpaydbase;
   
    public DatabaseHandler()
    {
        try
        {
            pegpaydbase = DatabaseFactory.CreateDatabase(constring);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public DataTable GetTranStatus(string VendorTranId, string VendorCode)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetTranStatus", VendorTranId, VendorCode);
            string[] parameters ={ VendorTranId, VendorCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetTranStatus", parameters).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void LogRequestAndResponse(string utilitycode, string tranId, string request, string response)
    {
        try
        {
            command = pegpaydbase.GetStoredProcCommand("LogXmlRequestResponseNew", utilitycode, tranId, request, response);
            pegpaydbase.ExecuteNonQuery(command);
        }
        catch (Exception ee)
        {

        }
    }

    public void LogRequestResponse(string cardNo, string method, string request, string response)
    {
        try
        {
            DbCommand mycommand = pegpaydbase.GetStoredProcCommand("LogTotalXmlRequestResponse", method, cardNo, request, response);
            pegpaydbase.ExecuteNonQuery(mycommand);
        }
        catch (Exception ex)
        {
            LogTotalError(cardNo, "", "LogRequestResponse: " + ex.Message);
        }
    }

    public string CheckTransaction(string VendorTranId, string VendorCode)
    {
        string receiptnumber = "";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetTranStatus", VendorTranId, VendorCode);
            string[] parameters ={ VendorTranId, VendorCode };

            DataTable dt = PegPayInterface.ExecuteDataSet("CheckTransactionStatus", parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                receiptnumber = row["receiptnumber"].ToString();

            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptnumber;
       
    }

    internal string GetStatusDescr(string statusCode)
    {
        string descr = "";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetStatusDescr", statusCode);
            string[] parameters ={ statusCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetStatusDescr", parameters).Tables[0];
            if (dt.Rows.Count != 0)
            {
                descr = dt.Rows[0]["StatusDescription"].ToString();
            }
            else
            {
                descr = "GENERAL ERROR";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return descr;
    }

    internal void SaveCredebtialsLog(string VendorCode, string password)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("SaveCredebtialsLog", VendorCode, password);
            //PegPayInterface.ExecuteNonQuery(command);
            string[] parameters ={ VendorCode, password };
            PegPayInterface.ExecuteNonQuery("SaveCredebtialsLog", parameters);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void logKCCAPostHit(KCCATransaction Tran)
    {
        try
        {
            string[] parameters ={ Tran.CustName, Tran.CustomerTel, Tran.CustRef, Tran.DigitalSignature, Tran.Email, Tran.Narration, Tran.Offline, Tran.Password, Tran.PaymentDate, Tran.PaymentType, Tran.Reversal, Tran.Teller, Tran.TranIdToReverse, Tran.TransactionAmount, Tran.TransactionType, Tran.VendorCode, Tran.VendorTransactionRef };
            PegPayInterface.ExecuteNonQuery("logKCCAPostHit", parameters);
            //command = PegPayInterface.GetStoredProcCommand("logKCCAPostHit", parameters);
            //PegPayInterface.ExecuteNonQuery(command);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveRequestlog(string VendorCode, string Utility, string LogType, string CustRef, string password)
    {
        try
        {
            string[] parameters ={ VendorCode, Utility, LogType, CustRef, password };
            command = pegpaydbase.GetStoredProcCommand("SaveRequestlog", parameters);
            command.CommandTimeout = 300000000;
            ////PegPayInterface.ExecuteNonQuery("SaveRequestlog", parameters);
            pegpaydbase.ExecuteNonQuery(command);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string GetKCCASession(string custref, string vendorCode)
    {
        string sessionKey = "";
        try
        {
            string[] parameters ={ custref, vendorCode };
            //DataTable dt = PegPayInterface.ExecuteDataSet("GetKCCASession", parameters).Tables[0];
            command = pegpaydbase.GetStoredProcCommand("GetKCCASession", parameters);
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];
            if (dt.Rows.Count > 0)
            {
                sessionKey = dt.Rows[0]["Area"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sessionKey;
    }

    internal DataTable GetVendorDetails(string vendorCode)
    {
        try
        {
            string[] parameters ={ vendorCode };
            //DataTable dt = PegPayInterface.ExecuteDataSet("GetVendorDetails", parameters).Tables[0];
            command = pegpaydbase.GetStoredProcCommand("GetVendorDetails", parameters);
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    //get solaris vendors
    internal DataTable GetSolarisVendors()
    {
        DataTable datatable = new DataTable();
        try
        {
            command = pegpaydbase.GetStoredProcCommand("GetSolarisVendors1");
            datatable = pegpaydbase.ExecuteDataSet(command).Tables[0];
        }
        catch (Exception ex)
        {
        }
        return datatable;
    }

    public DataSet ExecuteDataSet(string procedure, params object[] parameters)
    {
        try
        {

            command = pegpaydbase.GetStoredProcCommand(procedure, parameters);
            return pegpaydbase.ExecuteDataSet(command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void ExecuteNonQuery(string procedure, params object[] parameters)
    {
        try
        {

            command = pegpaydbase.GetStoredProcCommand(procedure, parameters);
            pegpaydbase.ExecuteNonQuery(command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    internal URAQueryResponse QueryURAcustomerDetails(string custRef)
    {
        URAQueryResponse resp = new URAQueryResponse();
        try
        {
            string[] parameters ={ custRef };
            DataTable dt = PegPayInterface.ExecuteDataSet("QueryURAcustomerDetails", parameters).Tables[0];
            if (dt.Rows.Count == 1)
            {
                resp.CustomerReference = dt.Rows[0]["CustomerRef"].ToString();
                resp.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                resp.TIN = "";
                resp.OutstandingBalance = dt.Rows[0]["AccountBal"].ToString();
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
                resp.PaymentRegistrationDate = dt.Rows[0]["LastUpdateDate"].ToString();
                resp.PrnStatus = "SUCCESS";
            }
            else if (dt.Rows.Count > 1)
            {
                resp.CustomerReference = dt.Rows[0]["CustomerRef"].ToString();
                resp.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                resp.TIN = "";
                resp.OutstandingBalance = dt.Rows[0]["AccountBal"].ToString();
                resp.StatusCode = "100";
                resp.StatusDescription = "MORE THAN ONE PRN HOLDER";
                resp.PaymentRegistrationDate = dt.Rows[0]["LastUpdateDate"].ToString();
                resp.PrnStatus = "EXPIRED";
            }
            else
            {
                resp.CustomerReference = "";
                resp.CustomerName = "";
                resp.TIN = "";
                resp.OutstandingBalance = "";
                resp.StatusCode = "100";
                resp.StatusDescription = "NO ONE PRN HOLDER";
                resp.PaymentRegistrationDate = "";
                resp.PrnStatus = "NOT FOUND";
            }

        }
        catch (Exception ex)
        {
            resp.CustomerReference = "";
            resp.CustomerName = "";
            resp.TIN = "";
            resp.OutstandingBalance = "";
            resp.StatusCode = "100";
            resp.StatusDescription = "ERROR: NOT FOUND";
            resp.PaymentRegistrationDate = "";
            resp.PrnStatus = "NOT FOUND";
        }
        return resp;
    }

    internal DataTable GetPrepaidVendorDetails(string vendorCode)
    {
        try
        {
            string[] parameters ={ vendorCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetPrepaidVendorDetails", parameters).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetRechargeUsers(string meterno)
    {
        DataTable table = new DataTable();
        try
        {
            //command = PegPay_DB.GetStoredProcCommand("GetRechargeUsers", meterno);
            table = PegPayInterface.ExecuteDataSet("GetRechargeUsers", new string[] { meterno }).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return table;
    }
    internal Customer GetCustomerDetailsDSTV(string customerReference, string area, string UtilityCode)
    {
        Customer cust = new Customer();
        try
        {
            DataTable dt = PegPayInterface.ExecuteDataSet("GetCustomerDetailsDSTV2", new object[] { customerReference, area, UtilityCode }).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cust.CustomerRef = dt.Rows[0]["CustomerRef"].ToString();
                cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
                cust.Area = dt.Rows[0]["Area"].ToString();
                cust.AgentCode = dt.Rows[0]["AgentCode"].ToString();
                cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                cust.StatusCode = "0";
                cust.StatusDescription = "SUCCESS";
            }
            else
            {
                cust.StatusCode = "1";
                cust.StatusDescription = "CUSTOMER DETAILS DON'T EXIST";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return cust;
    }

    internal DataTable GetDstvValidationFailureReasons(string error)
    {
        DataTable table = new DataTable();
        try
        {
            table = PegPayInterface.ExecuteDataSet("IsValideFailureReason", new object[] { error }).Tables[0];

        }
        catch (Exception ee)
        {
        }
        return table;
    }
    internal void DeactivateCustomerNumber(string customerRef, string utilityCode)
    {
        try
        {
            PegPayInterface.ExecuteNonQuery("DeactivateCustomerNumber", new object[] { customerRef, utilityCode });
        }
        catch (Exception ex)
        {

        }
    }


    internal UtilityCredentials GetUtilityCreds(string utilityCode, string vendorCode)
    {//"DSTV", vendorCode);
        UtilityCredentials creds = new UtilityCredentials();
        try
        {
            string[] parameters ={ vendorCode, utilityCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetUtilityCredentials", parameters).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
            if (dt.Rows.Count > 0)
            {
                creds.UtilityCode = dt.Rows[0]["UtilityUsername"].ToString();
                creds.UtilityPassword = dt.Rows[0]["UtilityPassword"].ToString();
                creds.Utility = dt.Rows[0]["UtilityCode"].ToString();
                creds.BankCode = dt.Rows[0]["BankCode"].ToString();
                creds.SecretKey = dt.Rows[0]["SecretKey"].ToString();
                creds.Key = dt.Rows[0]["Key"].ToString();
                creds.UtilityIsOffline = dt.Rows[0]["IsOffline"].ToString().ToUpper();
            }
            else
            {
                creds.UtilityCode = "";
                creds.UtilityPassword = "";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return creds;
    }

    internal void UpdateVendorInvalidLoginCount(string vendorCode, int loginCount)
    {
        try
        {
            string[] parameters ={ vendorCode, "" + loginCount };
            PegPayInterface.ExecuteNonQuery("UpdateVendorInvalidLoginCount", parameters);
            //PegPayInterface.ExecuteNonQuery(command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetNWSCCustomerDetails(string CustRef)
    {
        try
        {
            string[] parameters ={ CustRef };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetCustomerDetails", parameters).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveVendorDetails(Vendor vendor)
    {
        try
        {
            string[] parameters ={ ""+vendor.Vendorid, vendor.VendorCode, vendor.BillSysCode, vendor.VendorName,
             vendor.Contact, vendor.Passwd, vendor.Email, ""+vendor.Active, vendor.User};
            PegPayInterface.ExecuteNonQuery("SaveVendorDetails", parameters);
            //PegPayInterface.ExecuteNonQuery(command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal ArrayList GetBlackListedNumbers()
    {
        ArrayList blacklisted = new ArrayList();
        try
        {
            string[] parameters ={ };
            DataSet ds = PegPayInterface.ExecuteDataSet("GetBlacklistedNumbers", parameters);
            //DataSet ds = PegPayInterface.ExecuteDataSet(command);
            int recorcCount = ds.Tables[0].Rows.Count;
            for (int i = 0; i < recorcCount; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                string number = dr["Phone"].ToString();
                blacklisted.Add(number);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return blacklisted;
    }
    internal Hashtable GetNetworkCodes()
    {
        Hashtable networkCodes = new Hashtable();
        try
        {

            string[] parameters ={ };
            DataSet ds = PegPayInterface.ExecuteDataSet("GetNetworkCodes", parameters);
            //DataSet ds = PegPayInterface.ExecuteDataSet(command);
            int recordCount = ds.Tables[0].Rows.Count;
            if (recordCount != 0)
            {
                for (int i = 0; i < recordCount; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string network = dr["Network"].ToString();
                    string code = dr["Code"].ToString();
                    networkCodes.Add(code, network);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return networkCodes;
    }

    internal Hashtable GetNetworkRates()
    {
        Hashtable networkRates = new Hashtable();
        try
        {

            string[] parameters ={ };
            DataSet ds = PegPayInterface.ExecuteDataSet("GetNetworkRates", parameters);
            //DataSet ds = PegPayInterface.ExecuteDataSet(command);
            int recorcCount = ds.Tables[0].Rows.Count;
            for (int i = 0; i < recorcCount; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                string network = dr["Network"].ToString();
                int rate = int.Parse(dr["Rate(UShs.)"].ToString());
                networkRates.Add(network, rate);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return networkRates;
    }

    internal DataTable GetSystemSettings(string Valuecode, string ValueGroupcode)
    {
        try
        {

            string[] parameters ={ Valuecode, ValueGroupcode };
            //command = PegPayInterface.GetStoredProcCommand("GetSystemSettings", Valuecode, ValueGroupcode);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetSystemSettings", parameters).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    internal string GetSystemSettings2(string Valuecode, string ValueGroupcode)
    {
        string value = "";
        try
        {

            string[] parameters ={ Valuecode, ValueGroupcode };
            //command = PegPayInterface.GetStoredProcCommand("GetSystemSettings", Valuecode, ValueGroupcode);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetSystemSettings", parameters).Tables[0];
            if (returndetails.Rows.Count > 0)
            {
                value = returndetails.Rows[0]["ValueVarriable"].ToString();
            }
            return value;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    internal DataTable GetDuplicateVendorRef(Transaction trans)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDuplicateVendorRef", trans.VendorCode, trans.VendorTransactionRef);

            string[] parameters ={ trans.VendorCode, trans.VendorTransactionRef };
            command = pegpaydbase.GetStoredProcCommand("GetDuplicateVendorRef", parameters);
            DataTable returndetails = pegpaydbase.ExecuteDataSet(command).Tables[0];// PegPayInterface.ExecuteDataSet("GetDuplicateVendorRef", parameters).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetDuplicatePrepaidVendorRef(Transaction trans)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDuplicatePrepaidVendorRef", trans.VendorCode, trans.VendorTransactionRef);
            string[] p ={ trans.VendorCode, trans.VendorTransactionRef };
            command = pegpaydbase.GetStoredProcCommand("GetDuplicatePrepaidVendorRef", p);
            DataTable returndetails = pegpaydbase.ExecuteDataSet(command).Tables[0];
            //DataTable returndetails = PegPayInterface.ExecuteDataSet("GetDuplicatePrepaidVendorRef", p).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetDuplicateCustPayment(string vendorCode, string custRef, double amount, DateTime postDate)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDuplicateCustPayment", vendorCode, custRef, amount, postDate);
            string[] p ={ vendorCode, custRef, "" + amount, "" + postDate };
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetDuplicateCustPayment", p).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetOriginalVendorRef(Transaction trans)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDuplicateVendorRef", trans.VendorCode, trans.TranIdToReverse);
            string[] p ={ trans.VendorCode, trans.TranIdToReverse };
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetDuplicateVendorRef", p).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable CheckBlacklist(string customerRef)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetCustBlacklistStatus", customerRef);
            string[] p ={ customerRef };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetCustBlacklistStatus", p).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void deleteTransaction(string vendorTranId, string reason)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("DeleteTransation2", vendorTranId, reason);
            string[] p ={ vendorTranId, reason };
            PegPayInterface.ExecuteNonQuery("DeleteTransation2", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string PostTransaction(NWSCTransaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            string[] parameters ={trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, trans.Area, "", trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", parameters).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }
    internal string PostBrightLifeTransaction(BrightLifeTransaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            string[] parameters ={trans.CustRef, trans.CustName, "", trans.CustomerTel, trans.Area, "", trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", parameters).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    internal PostResponse PostREATransaction(REATransaction trans, string utilityCode)
    {
        string receiptNo = "";
        PostResponse response = new PostResponse();
        
        PegPay pg = new PegPay();
        try
        {
            //insert transaction into received, secondly update Transaction in received with token,units, success where pegpayId is same as receipt number
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            string[] parameters ={trans.CustRef, trans.CustName, "", trans.CustomerTel, trans.Area, "", trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", parameters).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
                
                //after successfully inserting transaction into received, go to krecs and 
                Meter rearesponse = pg.GetVendingTokenFromKRECS(trans.CustRef, trans.TransactionAmount, trans.VendorTransactionRef);
                if (rearesponse.ErrorCode.Equals("0"))
                {
                    string token = rearesponse.Token;
                    string units = rearesponse.Energy; ;
                    //update Transaction in received
                    PostResponse tokenDetails = UpdateKRECSTransaction(receiptNo, token, units,"SUCCESS");
                    response.PegPayPostId = receiptNo;
                    response.Token = tokenDetails.Token;
                    response.Units = tokenDetails.Units;
                    response.StatusCode = "0";
                    //dispatch sms with token and units
                    string Message = "Dear " + trans.CustName + ", payment of " + trans.TransactionAmount + "has been received by KRECS. Token No. is " + response.Token + ", Units " + response.Units + "KWH. Thank you for Paying";
                    SendSms(trans.CustomerTel, Message, trans.VendorTransactionRef);
                }
                else
                {
                    response.StatusCode = "100";
                    response.StatusDescription = "FAILED AT KRECS: REASON" + rearesponse.ErrorMsg;
                    //transfer transaction to failed
                    TransferFailedTransaction(receiptNo, rearesponse.ErrorMsg);
                    //dispatch sms with failure reason
                    string Message = "Dear " + trans.CustName + ", Token Generation for Meter " + trans.CustRef + " with amount  " + trans.TransactionAmount + " Failed with Reason " + rearesponse.ErrorMsg.ToString().ToUpper() + " AT KRECS";
                    SendSms(trans.CustomerTel, Message, trans.VendorTransactionRef);
                }
            }
            else
            {
                response.StatusCode = "100";
                response.StatusDescription = "ERROR ENCOUNTERED WHILE INSERTING TRANSACTION WITH REF: " + trans.VendorTransactionRef;
            }
          
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return response;
    }
    public bool SendSms(string msisdn, string message, string transactionId)
    {
        bool sent = false;
        try
        {
            UtilityReferences.MailApi.Messenger msg = new Messenger();
            SMS sms = new SMS();
            sms.Mask = "PEGASUS";
            sms.Message = message;
            sms.Phone = msisdn;
            sms.Reference = transactionId;
            sms.Sender = "PEGASUS";
            sms.VendorTranId = transactionId;
            msg.Url = "http://192.168.23.15:5099/MailApi/Messenger.asmx?WSDL";
            UtilityReferences.MailApi.Result result = msg.SendSMS(sms);
            if (result.StatusCode == "0")
            {
                sent = true;
                SmsSentStatus(transactionId);
            }

        }
        catch (Exception ee)
        {
            LogError(ee.Message,"CENTENARY_"+ transactionId,DateTime.Now,"KRECS");
        }
        return sent;
    }




    public void SmsSentStatus(string transactionId)
    {
        try
        {
            string[] data = { transactionId };
            ExecuteNonQuery("changesmsstatus", data);
            Console.WriteLine("SMS successfully Sent");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error" + e.Message);
        }
        finally
        {
            // conn.Close();
        }

    }

    public PostResponse UpdateKRECSTransaction(string pegpayId,string token, string units, string status)
    {
        string[] parameters = { pegpayId, token, units, status };
        PostResponse results = new PostResponse();
        try
        {
           PegPayInterface.ExecuteNonQuery("UpdateSentTransaction_KRECS", parameters);
            
                results.PegPayPostId = pegpayId;
                results.Token = token;
                results.Units= units;
          
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return results;
    }
    public void TransferFailedTransaction(string PegPayId, string reason)
    {
        try
        {
             PegPayInterface.ExecuteNonQuery("TransferFailedTransaction2", PegPayId, reason);
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public string PostTransactionObject(Transaction trans, string utilitycode)
    {
        string receiptNo = "";
        string format = "dd/MM/yyyy";
        try
        {
           
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            object [] parameters ={trans.CustRef, trans.CustName, "", trans.CustomerTel, trans.Area, "", trans.TransactionAmount, trans.PaymentDate, DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false, ""+false, ""+false, utilitycode, "", ""+false};
            command = pegpaydbase.GetStoredProcCommand("InsertReceivedTransactions");
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ee)
        {

            throw ee;
        }
        return receiptNo;
    }

    internal void UpdateVendorInvalidLoginCount(string vendorCode, int loginCount, string ip)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdateVendorInvalidLoginCount", vendorCode, loginCount);
            string[] p ={ vendorCode, "" + loginCount };
            PegPayInterface.ExecuteNonQuery("UpdateVendorInvalidLoginCount", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void UpdatePrepaidVendorBalance(string vendorCode, string balance)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidVendorBalance", vendorCode, balance);
            string[] p ={ vendorCode, balance };
            PegPayInterface.ExecuteNonQuery("UpdatePrepaidVendorBalance", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void UpdatePrepaidVendorRunningBalance(string pegpayId, string balance)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidVendorRunningBalance", pegpayId, balance);
            string[] p ={ pegpayId, balance };
            PegPayInterface.ExecuteNonQuery("UpdatePrepaidVendorRunningBalance", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void DeactivateVendor(string vendorCode, string ip_address)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("DeactivateVendor", vendorCode, ip_address);
            string[] p ={ vendorCode, ip_address };
            PegPayInterface.ExecuteNonQuery("DeactivateVendor", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataBundleDetails GetDataBundlePrice(string network, string bundleName)
    {
        DataBundleDetails bundleDetails = new DataBundleDetails();
        try
        {
            //   command = PegPayInterface.GetStoredProcCommand("GetDataBundlePrice", network, bundleCode);
            string[] p = { network, bundleName };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetDataBundlePrice", p).Tables[0];
            if (dt.Rows.Count > 0)
            {
                bundleDetails.bundleCode = dt.Rows[0]["BundleCode"].ToString();
                bundleDetails.bundleDescription = dt.Rows[0]["BundleName"].ToString();
                bundleDetails.bundlePrice = dt.Rows[0]["BundleAmount"].ToString();
                bundleDetails.StatusCode = "0";
                bundleDetails.StatusDescription = "SUCCESS";
            }
            else
            {
                bundleDetails.StatusCode = "100";
                bundleDetails.StatusDescription = "DATA BUNDLE WITH NAME " + bundleName + " DOES NOT EXIST!";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return bundleDetails;
    }

    internal string LogError(string error, string vendorCode, DateTime now, string AgentCode)
    {
        string ret = "";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
            string[] p ={ error, vendorCode, "" + now, AgentCode };
            PegPayInterface.ExecuteNonQuery("LogError", p);
            ret = "YES";
        }
        catch (Exception ex)
        {
            ret = ex.Message;
        }
        return ret;
    }

    public void LogTotalError(string cardNo, string tranid, string error)
    {
        try
        {
            string[] p = { cardNo, tranid, error };
            PegPayInterface.ExecuteNonQuery("LogTotalError", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetPaymentCode(string PaymentCode)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetPayTypeByCode", PaymentCode);
            string[] p ={ PaymentCode };
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetPayTypeByCode", p).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string PostUmemeTransaction(Transaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);           

            if (string.IsNullOrEmpty(trans.Area))
            {
                trans.Area = "";

               
            }
            string[] parameters ={trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, trans.Area, "", trans.TransactionAmount, ""+payDate, 
                                 ""+DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
                                 ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            command = pegpaydbase.GetStoredProcCommand("InsertReceivedTransactions", parameters);//.Tables[0];
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }
    internal string PostSchoolTransaction(Transaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);           

            string[] parameters ={trans.CustRef,
                trans.CustName,
                trans.Offline,//trans.CustomerType, 
                trans.CustomerTel, 
                trans.Area,
                "",//trans.ChargeType,
                trans.TransactionAmount, ""+payDate, 
                                 ""+DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
                                 ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions2", parameters).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }
    internal string PostUmemeTransactionPrepaidVendor(Transaction trans, string utilityCode, string area)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            //string transactionCharge = trans.PegpayCharge.ToString();
            string[] p =
                {
                trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, area, "", 
                trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, trans.PaymentType, 
                trans.VendorTransactionRef, trans.Narration, ""+false, trans.VendorCode, trans.Teller, 
                ""+false, ""+false, ""+false, utilityCode, "", ""+false};
            command = pegpaydbase.GetStoredProcCommand("InsertPrepaidReceivedTransactions2", p);
          
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }


    internal string PostPayTvTransaction(Transaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            if (string.IsNullOrEmpty(trans.Tin))
            {
                trans.Tin = "";
            }
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            object [] parameters ={trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, trans.Area, trans.Tin, trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            command = pegpaydbase.GetStoredProcCommand("InsertReceivedTransactions", parameters);
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];// PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", p).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    internal string PostNSSFTransaction(URATransaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            string[] p ={trans.CustRef, trans.CustName, "", trans.CustomerTel, "", trans.TIN, trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", p).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    internal string PostURATransaction(URATransaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            string[] p ={trans.CustRef, trans.CustName, "", trans.CustomerTel, "", trans.TIN, trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", p).Tables[0];
            //DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    internal void UpdateSentTransaction(string pegPayReceiptNumber, string utilityReceipt, string Status)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdateSentTransaction2", pegPayReceiptNumber, utilityReceipt, Status);
            string[] p ={ pegPayReceiptNumber, utilityReceipt, Status };
            PegPayInterface.ExecuteNonQuery("UpdateSentTransaction2", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void UpdatePBUSentTransaction(string pegPayReceiptNumber, string utilityReceipt, string Status, string pbuRef)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdatePBUSentTransaction", pegPayReceiptNumber, utilityReceipt, Status, pbuRef);
            string[] p ={ pegPayReceiptNumber, utilityReceipt, Status, pbuRef };
            PegPayInterface.ExecuteNonQuery("UpdatePBUSentTransaction", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetTransactionDetails(string vendorTranId, string vendorCode)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetTransactionDetails", vendorCode, vendorTranId);
            string[] p ={ vendorCode, vendorTranId };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetTransactionDetails", p).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    // []

    internal DataTable GetPrepaidTransactionDetails(string vendorTranId, string vendorCode)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetPrepaidTransactionDetails", vendorCode, vendorTranId);
            string[] p ={ vendorCode, vendorTranId };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetPrepaidTransactionDetails2", p).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string GetServerStatus()
    {
        string status = "";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetServerStatus");
            string[] p ={ };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetServerStatus", p).Tables[0];
            if (dt.Rows.Count > 0)
            {
                status = dt.Rows[0]["ServerStatus"].ToString();
            }
            return status;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveCustomerDetailsKCCA(Customer cust)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("SaveCustomerDetailsKCCA", cust.CustomerRef, cust.CustomerName, cust.Balance, cust.AgentCode);
            string[] p ={ cust.CustomerRef, cust.CustomerName, cust.Balance, cust.AgentCode };
            PegPayInterface.ExecuteNonQuery("SaveCustomerDetailsKCCA", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveDstvCustomerDetails(Customer cust)
    {
        try
        {
            command = pegpaydbase.GetStoredProcCommand("SaveCustomerDetails2", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN);
            //string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN };
            int count = pegpaydbase.ExecuteNonQuery(command);//"SaveCustomerDetails2", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveURACustomerDetails(Customer cust)
    {
        try
        {
            command = pegpaydbase.GetStoredProcCommand("SaveCustomerDetails2", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN);
            //string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN };
            int count = pegpaydbase.ExecuteNonQuery(command);//"SaveCustomerDetails2", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveUmemeCustomerDetails(Customer cust)
    {
        try
        {
            double balance = Convert.ToDouble(cust.Balance.Trim());
            //command = PegPayInterface.GetStoredProcCommand("SaveUmemeCustomerDetails", cust.CustomerRef, cust.CustomerName, cust.CustomerType, balance, cust.AgentCode);
            string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, "" + balance, cust.AgentCode };
            string rowz = PegPayInterface.ExecuteNonQuery("SaveUmemeCustomerDetails", p).RowsAffected;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //fetch rea token
    internal string GetLastREAToken(string meterNo)
    {
        string token = "";
        try
        {
            DataTable dt = PegPayInterface.ExecuteDataSet("GetLastREAToken", meterNo).Tables[0];
            if (dt.Rows.Count != 0)
            {
                token = dt.Rows[0][0].ToString();
            }
            else
            {
                token = "NO TOKENS FOUND FOR: " + meterNo;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error encountered in fetching token details)- " + ex.ToString());
        }
        return token;
    }

    internal Customer GetCustomerDetails(string customerReference, string area, string UtilityCode)
    {
        Customer cust = new Customer();
        try
        {
            command = pegpaydbase.GetStoredProcCommand("GetCustomerDetails1", customerReference, area, UtilityCode);
            string[] p ={ customerReference, area, UtilityCode };
            DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];//("GetCustomerDetails1", p).Tables[0];
            if (dt.Rows.Count > 0)
            {
                cust.CustomerRef = dt.Rows[0]["CustomerRef"].ToString();
                cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
                cust.Area = dt.Rows[0]["Area"].ToString();
                cust.AgentCode = dt.Rows[0]["AgentCode"].ToString();
                cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                cust.TIN = dt.Rows[0]["MeterNo"].ToString();
                cust.StatusCode = "0";
                cust.StatusDescription = "SUCCESS";
            }
            else
            {
                cust.CustomerRef = "CUSTOMER DETAILS DON'T EXIST";
                cust.CustomerName = "CUSTOMER DETAILS DON'T EXIST";
                cust.CustomerType = "CUSTOMER DETAILS DON'T EXIST";
                cust.Area = "CUSTOMER DETAILS DON'T EXIST";
                cust.AgentCode = "CUSTOMER DETAILS DON'T EXIST";
                cust.Balance = "CUSTOMER DETAILS DON'T EXIST";
                cust.TIN = "CUSTOMER DETAILS DON'T EXIST";
                cust.StatusCode = "1";
                cust.StatusDescription = "CUSTOMER DETAILS DON'T EXIST";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return cust;
    }

    internal DataTable GetStudentDetails(string utilityCode, string customerReference)
    {
        DataTable stdDetails = new DataTable();
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetStudentDetails", utilityCode, customerReference);
            string[] p ={ utilityCode, customerReference };
            stdDetails = PegPayInterface.ExecuteDataSet("GetStudentDetails", p).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return stdDetails;
    }

    internal void UpdateTransactionStatus(string pegPayReceiptNumber, string Status)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdateTransactionStatus", pegPayReceiptNumber, Status);
            string[] p ={ pegPayReceiptNumber, Status };
            PegPayInterface.ExecuteNonQuery("UpdateTransactionStatus", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveKCCACustomerDetails(Customer cust)
    {
        try
        {
            double balance = Convert.ToDouble(cust.Balance.Trim());
            //command = PegPayInterface.GetStoredProcCommand("SaveKCCACustomerDetails", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, balance, cust.AgentCode);
            string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, "" + balance, cust.AgentCode };
            PegPayInterface.ExecuteNonQuery("SaveKCCACustomerDetails", p);
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    internal void SaveTuckseeCustomerDetails(Customer cust)
    {
        try
        {
            double balance = Convert.ToDouble(cust.Balance.Trim());
            command = pegpaydbase.GetStoredProcCommand("SaveTuckseeCustomerDetails", cust.CustomerRef, cust.CustomerName, cust.Area, cust.CustomerType, cust.TIN, balance, cust.SessionKey, cust.AgentCode);
            //string[] p ={ cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, "" + balance, cust.AgentCode };
            pegpaydbase.ExecuteNonQuery(command);
        }
        catch (Exception ex)
        {
            throw ex;
        }


    }



//    create proc [dbo].[SaveTuckseeCustomerDetails]
//@CustRef varchar(50),
//@CustName varchar(100),
//@DivisionCode varchar(50),
//@sessionKey varchar(50),
//@TpgoReference varchar(50),
//@AccountBal money,
//@AgentCode varchar(50)


    public DataTable CheckIfMeterNumberIsBlacklisted(string meterNumber)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("CheckIfMeterNumberIsBlacklisted", meterNumber);
            string[] p ={ meterNumber };
            DataTable result = PegPayInterface.ExecuteDataSet("CheckIfMeterNumberIsBlacklisted", p).Tables[0];
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void UpdateBlacklistedAccountDebt(string meterNumber, int debtAmount)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("UpdateBlacklistedAccountDebt",
            //new string[] { meterNumber, debtAmount });
            PegPayInterface.ExecuteNonQuery("UpdateBlacklistedAccountDebt",
                                                            new string[] { meterNumber, "" + debtAmount });
            return;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetBlacklistedAccountsDebt(string meterNumber)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetBlacklistedAccountsDebt", meterNumber);
            string[] p ={ meterNumber };
            DataTable result = PegPayInterface.ExecuteDataSet("GetBlacklistedAccountsDebt", p).Tables[0];
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable CheckIfVendorHasBlacklistedAccounts(string vendorCode)
    {

        try
        {
            //command = PegPayInterface.GetStoredProcCommand("CheckIfVendorHasBlacklistedAccounts", vendorCode);
            string[] p ={ vendorCode };
            DataTable table = PegPayInterface.ExecuteDataSet("CheckIfVendorHasBlacklistedAccounts", p).Tables[0];
            return table;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string InsertIntoBlackListedAccountLogs(UmemeTransaction trans, string AmountPaid, string AmountTaken)
    {
        string receiptNo = "";
        try
        {
            int amountTaken = Convert.ToInt32(AmountTaken);
            //command = PegPayInterface.GetStoredProcCommand("InsertIntoBlacklistedAccountsLog");
            //PegPayInterface.AddInParameter(command, "@custRef", DbType.String, trans.CustRef);
            //PegPayInterface.AddInParameter(command, "@custName", DbType.String, trans.CustName);
            //PegPayInterface.AddInParameter(command, "@paymentDate", DbType.String, FormatDate(trans.PaymentDate));
            //PegPayInterface.AddInParameter(command, "@amountPaid", DbType.String, AmountPaid);
            //PegPayInterface.AddInParameter(command, "@amountTaken", DbType.String, amountTaken);
            //PegPayInterface.AddInParameter(command, "@tranId", DbType.String, trans.VendorTransactionRef);
            //PegPayInterface.AddOutParameter(command, "@RecordId", DbType.Int32, 32);
            string[] p ={ trans.CustRef, trans.CustName, trans.PaymentDate, AmountPaid, "" + amountTaken, trans.VendorTransactionRef, "" + 32 };
            PegPayInterface.ExecuteNonQuery("InsertIntoBlacklistedAccountsLog", p);
            //Read the Output Parameter Value afte execution
            //int GeneratedId = Convert.ToInt32(PegPayInterface.GetParameterValue(command, "@RecordId"));
            receiptNo = "32";
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    private string FormatDate(string date)
    {
        string[] parts = date.Split('/');
        string day = parts[0];
        string month = parts[1];
        string year = parts[2];
        return year + "-" + month + "-" + day;
    }

    internal string InsertIntoReceivedTransactions(UmemeTransaction trans, string utilityCode)
    {
        string receiptNo = "";
        try
        {
            string format = "dd/MM/yyyy";
            DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
            //command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, "", "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
            //false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
            string[] p ={ trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, "", "", trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertReceivedTransactions", p).Tables[0];

            if (dt.Rows.Count != 0)
            {
                receiptNo = dt.Rows[0][0].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return receiptNo;
    }

    internal string GetDSTVAccountType(string p)
    {
        string customertype = "";
        try
        {
            DataTable dt = PegPayInterface.ExecuteDataSet("GetCustomerTypes", p).Tables[0];
            if (dt.Rows.Count != 0)
            {
                customertype = dt.Rows[0][0].ToString();
            }
            else
            {
                customertype = "UNKNOWN CUSTOMER TYPE: " + p;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error in method:GetDSTVAccountType (Method to get DSTV Customer Types in DB)- "+ex.ToString());
        }
        return customertype;
    }

    public BouquetDetails[] GetBouquetDetailsFromDB(string bouquetCode, string PayTvCode)
    {
        List<BouquetDetails> allBouquets = new List<BouquetDetails>();
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetBouquetDetails", bouquetCode, PayTvCode);
            string[] p ={ bouquetCode, PayTvCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetBouquetDetails", p).Tables[0];

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    //we are getting details for 1 bouquet
                    if (!string.IsNullOrEmpty(bouquetCode))
                    {
                        BouquetDetails bd = new BouquetDetails();
                        bd.BouquetCode = row["BouquetCode"].ToString();
                        bd.BouquetDescription = row["BouquetDescription"].ToString();
                        bd.BouquetName = row["BouquetName"].ToString();
                        bd.BouquetPrice = row["BouquetPrice"].ToString();
                        bd.StatusCode = "0";
                        //bd.StatusDescription = "SUCCESS";
                        //if ((bd.BouquetCode.ToUpper().Trim() == bouquetCode.ToUpper().Trim())||(bd..ToUpper().Trim() == bouquetCode.ToUpper().Trim()))
                        //{
                        allBouquets.Add(bd);
                        return allBouquets.ToArray();
                        //}
                    }
                    //we are getting details for all bouquets
                    else
                    {
                        BouquetDetails bd = new BouquetDetails();
                        bd.BouquetCode = row["BouquetCode"].ToString();
                        bd.BouquetDescription = row["BouquetDescription"].ToString();
                        bd.BouquetName = row["BouquetName"].ToString();
                        bd.BouquetPrice = row["BouquetPrice"].ToString();
                        bd.StatusCode = "0";
                        bd.StatusDescription = "SUCCESS";
                        allBouquets.Add(bd);

                    }
                }
            }
            else
            {
                allBouquets.Clear();
                BouquetDetails bd = new BouquetDetails();
                bd.StatusCode = "100";
                bd.StatusDescription = "NO BOUQUET DETAILS FOR BOUQUET CODE";
                allBouquets.Add(bd);
            }
            return allBouquets.ToArray();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        //return allBouquets.ToArray();

    }

    public int SaveBouquetDetails(BouquetDetails bd)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("SaveBouquetDetails", bd.BouquetCode, bd.BouquetName, bd.BouquetDescription, bd.BouquetPrice, bd.PayTvCode);
            string[] p ={ bd.BouquetCode, bd.BouquetName, bd.BouquetDescription, bd.BouquetPrice, bd.PayTvCode };
            string strrows = PegPayInterface.ExecuteNonQuery("SaveBouquetDetails", p).RowsAffected;
            int rows = Convert.ToInt32(strrows);
            return rows;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal int SaveReactivateRequest(string smartCardNumber)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("SaveReactivateRequest", smartCardNumber);
            string[] p ={ smartCardNumber };
            string rowstr = PegPayInterface.ExecuteNonQuery("SaveReactivateRequest", p).RowsAffected;
            int rows = Convert.ToInt32(rowstr);
            return rows;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }

    internal string checkCustomerDetails(string custReference)
    {
        string status = "";
        DataTable returnTable = new DataTable();
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails", custReference);
            string[] p ={ custReference };
            returnTable = PegPayInterface.ExecuteDataSet("GetCustomerDetails", p).Tables[0];
            if (returnTable.Rows.Count > 0)
            {
                status = "true";
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return status;// 
    }

    internal bool PRNAlreadyExistsInPegPay(string CustomerRef, string utility, string vendorCode)
    {
        bool exists = false;
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("PRNAlreadyExistsInPegPay", CustomerRef, utility, vendorCode);
            string[] p ={ CustomerRef, utility, vendorCode };
            DataTable dt = PegPayInterface.ExecuteDataSet("PRNAlreadyExistsInPegPay", p).Tables[0];
            if (dt.Rows.Count > 0)
            {
                exists = true;
            }
            else
            {
                exists = false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return exists;
    }

    internal LookupResp GetDetailsByOffenceCode(string CustomerReference)
    {
        LookupResp resp = new LookupResp();
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDetailsByOffenceCode", CustomerReference);
            string[] p ={ CustomerReference };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetDetailsByOffenceCode", p).Tables[0];
            if (dt.Rows.Count > 0)
            {
                resp.Amount = dt.Rows[0]["Amount"].ToString();
                resp.OffenceCode = CustomerReference;
                resp.StatusCode = "0";
                resp.StatusDescription = "SUCCESS";
            }
            else
            {
                resp.StatusCode = "100";
                resp.StatusDescription = "INVALID OFFENCE CODE";
            }
        }
        catch (Exception ex)
        {
            resp.StatusCode = "100";
            resp.StatusDescription = ex.Message;
        }
        return resp;
    }
    internal DataTable GetPrePaidVendorPegasusCharge(string VendorCode)
    {
        DataTable dt = new DataTable();
        try
        {
            string[] parameters ={ VendorCode };
            dt = PegPayInterface.ExecuteDataSet("GetPrePaidVendorPegasusCharge", parameters).Tables[0];

        }
        catch (Exception ex)
        {
            GetPrePaidVendorPegasusCharge(VendorCode);
        }
        return dt;
    }

    internal DataTable GetPrepaidVendorAccountBalance(string VendorCode)
    {
        DataTable dt = new DataTable();
        try
        {
            string[] parameters ={ VendorCode };
            dt = PegPayInterface.ExecuteDataSet("GetPegPayAccount", parameters).Tables[0];


        }
        catch (Exception ex)
        {
            GetPrepaidVendorAccountBalance(VendorCode);
        }
        return dt;
    }

    internal DataTable CheckIfNumberIsTestNumber(string testNumber)
    {
        DataTable dt = new DataTable();
        try
        {
            string[] parameters = { testNumber };
            dt = PegPayInterface.ExecuteDataSet("CheckIfTestNumber", parameters).Tables[0];


        }
        catch (Exception ex)
        {
            CheckIfNumberIsTestNumber(testNumber);
        }
        return dt;
    }

    //private Database PegPayInterface;
    //private DbCommand command;

    //public DatabaseHandler()
    //{
    //    try
    //    {
    //        //PegPayInterface = DatabaseFactory.CreateDatabase("Ronnie");
    //        PegPayInterface = DatabaseFactory.CreateDatabase("TestPegPay");
    //        //PegPayInterface = DatabaseFactory.CreateDatabase("LivePegPay");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public DataTable GetTranStatus(string VendorTranId, string VendorCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetTranStatus", VendorTranId, VendorCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string GetStatusDescr(string statusCode)
    //{
    //    string descr = "";
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetStatusDescr", statusCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count != 0)
    //        {
    //            descr = dt.Rows[0]["StatusDescription"].ToString();
    //        }
    //        else
    //        {
    //            descr = "GENERAL ERROR";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return descr;
    //}

    //internal void SaveCredebtialsLog(string VendorCode, string password)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveCredebtialsLog", VendorCode, password);
    //        PegPayInterface.ExecuteNonQuery(command);

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void logKCCAPostHit(KCCATransaction Tran)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("logKCCAPostHit", Tran.CustName, Tran.CustomerTel, Tran.CustRef, Tran.DigitalSignature, Tran.Email, Tran.Narration, Tran.Offline, Tran.Password, Tran.PaymentDate, Tran.PaymentType, Tran.Reversal, Tran.Teller, Tran.TranIdToReverse, Tran.TransactionAmount, Tran.TransactionType, Tran.VendorCode, Tran.VendorTransactionRef);
    //        PegPayInterface.ExecuteNonQuery(command);

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    
    //internal void SaveRequestlog(string VendorCode, string Utility, string LogType, string CustRef, string password)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveRequestlog", VendorCode, Utility, LogType, CustRef, password);
    //        PegPayInterface.ExecuteNonQuery(command);

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string GetKCCASession(string custref, string vendorCode)
    //{
    //    string sessionKey = "";
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetKCCASession", custref, vendorCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            sessionKey = dt.Rows[0]["Area"].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return sessionKey;
    //}
    //internal DataTable GetVendorDetails(string vendorCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetVendorDetails", vendorCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable GetPrepaidVendorDetails(string vendorCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetPrepaidVendorDetails", vendorCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal UtilityCredentials GetUtilityCreds(string utilityCode, string vendorCode)
    //{
    //    UtilityCredentials creds = new UtilityCredentials();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetUtilityCredentials", vendorCode, utilityCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            creds.UtilityCode = dt.Rows[0]["UtilityUsername"].ToString();
    //            creds.UtilityPassword = dt.Rows[0]["UtilityPassword"].ToString();
    //            creds.Utility = dt.Rows[0]["UtilityCode"].ToString();
    //            creds.BankCode = dt.Rows[0]["BankCode"].ToString();
    //            creds.SecretKey = dt.Rows[0]["SecretKey"].ToString();
    //            creds.Key = dt.Rows[0]["Key"].ToString();
    //        }
    //        else
    //        {
    //            creds.UtilityCode = "";
    //            creds.UtilityPassword = "";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return creds;
    //}

    //internal void UpdateVendorInvalidLoginCount(string vendorCode, int loginCount)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdateVendorInvalidLoginCount", vendorCode, loginCount);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void SaveVendorDetails(Vendor vendor)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveVendorDetails", vendor.Vendorid, vendor.VendorCode, vendor.BillSysCode, vendor.VendorName,
    //         vendor.Contact, vendor.Passwd, vendor.Email, vendor.Active, vendor.User);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal ArrayList GetBlackListedNumbers()
    //{
    //    ArrayList blacklisted = new ArrayList();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetBlacklistedNumbers");
    //        DataSet ds = PegPayInterface.ExecuteDataSet(command);
    //        int recorcCount = ds.Tables[0].Rows.Count;
    //        for (int i = 0; i < recorcCount; i++)
    //        {
    //            DataRow dr = ds.Tables[0].Rows[i];
    //            string number = dr["Phone"].ToString();
    //            blacklisted.Add(number);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return blacklisted;
    //}
    //internal Hashtable GetNetworkCodes()
    //{
    //    Hashtable networkCodes = new Hashtable();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetNetworkCodes");
    //        DataSet ds = PegPayInterface.ExecuteDataSet(command);
    //        int recordCount = ds.Tables[0].Rows.Count;
    //        if (recordCount != 0)
    //        {
    //            for (int i = 0; i < recordCount; i++)
    //            {
    //                DataRow dr = ds.Tables[0].Rows[i];
    //                string network = dr["Network"].ToString();
    //                string code = dr["Code"].ToString();
    //                networkCodes.Add(code, network);
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return networkCodes;
    //}

    //internal Hashtable GetNetworkRates()
    //{
    //    Hashtable networkRates = new Hashtable();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetNetworkRates");
    //        DataSet ds = PegPayInterface.ExecuteDataSet(command);
    //        int recorcCount = ds.Tables[0].Rows.Count;
    //        for (int i = 0; i < recorcCount; i++)
    //        {
    //            DataRow dr = ds.Tables[0].Rows[i];
    //            string network = dr["Network"].ToString();
    //            int rate = int.Parse(dr["Rate(UShs.)"].ToString());
    //            networkRates.Add(network, rate);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return networkRates;
    //}

    //internal DataTable GetSystemSettings(string Valuecode, string ValueGroupcode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetSystemSettings", Valuecode, ValueGroupcode);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //}

    //internal string GetSystemSettings2(string Valuecode, string ValueGroupcode)
    //{
    //    string value = "";
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetSystemSettings", Valuecode, ValueGroupcode);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (returndetails.Rows.Count > 0)
    //        {
    //            value = returndetails.Rows[0]["ValueVarriable"].ToString();
    //        }
    //        return value;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //}


    //internal DataTable GetDuplicateVendorRef(Transaction trans)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetDuplicateVendorRef", trans.VendorCode, trans.VendorTransactionRef);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable GetDuplicatePrepaidVendorRef(Transaction trans)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetDuplicatePrepaidVendorRef", trans.VendorCode, trans.VendorTransactionRef);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable GetDuplicateCustPayment(string vendorCode, string custRef, double amount, DateTime postDate)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetDuplicateCustPayment", vendorCode, custRef, amount, postDate);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable GetOriginalVendorRef(Transaction trans)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetDuplicateVendorRef", trans.VendorCode, trans.TranIdToReverse);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable CheckBlacklist(string customerRef)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetCustBlacklistStatus", customerRef);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void deleteTransaction(string vendorTranId, string reason)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("DeleteTransation2", vendorTranId, reason);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string PostTransaction(NWSCTransaction trans, string utilityCode)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, "", trans.CustomerTel, trans.Area, "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //internal void UpdateVendorInvalidLoginCount(string vendorCode, int loginCount, string ip)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdateVendorInvalidLoginCount", vendorCode, loginCount);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void UpdatePrepaidVendorBalance(string vendorCode, string balance)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidVendorBalance", vendorCode, balance);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void UpdatePrepaidVendorRunningBalance(string pegpayId, string balance)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidVendorRunningBalance", pegpayId, balance);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void DeactivateVendor(string vendorCode, string ip_address)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("DeactivateVendor", vendorCode, ip_address);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string LogError(string error, string vendorCode, DateTime now, string AgentCode)
    //{
    //    string ret = "";
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
    //        PegPayInterface.ExecuteNonQuery(command);
    //        ret = "YES";
    //    }
    //    catch (Exception ex)
    //    {
    //        ret = ex.Message;
    //    }
    //    return ret;
    //}

    //internal DataTable GetPaymentCode(string PaymentCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetPayTypeByCode", PaymentCode);
    //        DataTable returndetails = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return returndetails;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string PostUmemeTransaction(Transaction trans, string utilityCode)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, "", "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //internal string PostUmemeTransactionPrepaidVendor(Transaction trans, string utilityCode, string area)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertPrepaidReceivedTransactions2", trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, area, "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //internal string PostDstvTransaction(Transaction trans, string utilityCode)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, trans.Area, "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //internal string PostURATransaction(URATransaction trans, string utilityCode)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, "", trans.CustomerTel, "", trans.TIN, trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //internal void UpdateSentTransaction(string pegPayReceiptNumber, string utilityReceipt, string Status)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdateSentTransaction2", pegPayReceiptNumber, utilityReceipt, Status);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void UpdatePBUSentTransaction(string pegPayReceiptNumber, string utilityReceipt, string Status, string pbuRef)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdatePBUSentTransaction", pegPayReceiptNumber, utilityReceipt, Status, pbuRef);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal DataTable GetTransactionDetails(string vendorTranId, string vendorCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetTransactionDetails", vendorCode, vendorTranId);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}
    //// []


    //internal DataTable GetPrepaidTransactionDetails(string vendorTranId, string vendorCode)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetPrepaidTransactionDetails", vendorCode, vendorTranId);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal string GetServerStatus()
    //{
    //    string status = "";
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetServerStatus");
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            status = dt.Rows[0]["ServerStatus"].ToString();
    //        }
    //        return status;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void SaveCustomerDetailsKCCA(Customer cust)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveCustomerDetailsKCCA", cust.CustomerRef, cust.CustomerName, cust.Balance, cust.AgentCode);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void SaveDstvCustomerDetails(Customer cust)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveCustomerDetails2", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, cust.AgentCode, cust.TIN);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void SaveUmemeCustomerDetails(Customer cust)
    //{
    //    try
    //    {
    //        double balance = Convert.ToDouble(cust.Balance.Trim());
    //        command = PegPayInterface.GetStoredProcCommand("SaveUmemeCustomerDetails", cust.CustomerRef, cust.CustomerName, cust.CustomerType, balance, cust.AgentCode);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal Customer GetCustomerDetails(string customerReference, string area, string UtilityCode)
    //{
    //    Customer cust = new Customer();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails1", customerReference, area, UtilityCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            cust.CustomerRef = dt.Rows[0]["CustomerRef"].ToString();
    //            cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
    //            cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
    //            cust.Area = dt.Rows[0]["Area"].ToString();
    //            cust.AgentCode = dt.Rows[0]["AgentCode"].ToString();
    //            cust.Balance = dt.Rows[0]["AccountBal"].ToString();
    //            cust.TIN = dt.Rows[0]["MeterNo"].ToString();
    //            cust.StatusCode = "0";
    //            cust.StatusDescription = "SUCCESS";
    //        }
    //        else
    //        {
    //            cust.StatusCode = "1";
    //            cust.StatusDescription = "CUSTOMER DETAILS DON'T EXIST";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return cust;
    //}

    //internal DataTable GetStudentDetails(string utilityCode, string customerReference)
    //{
    //    DataTable stdDetails = new DataTable();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetStudentDetails", utilityCode, customerReference);
    //        stdDetails = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return stdDetails;
    //}

    //internal void UpdateTransactionStatus(string pegPayReceiptNumber, string Status)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdateTransactionStatus", pegPayReceiptNumber, Status);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal void SaveKCCACustomerDetails(Customer cust)
    //{
    //    try
    //    {
    //        double balance = Convert.ToDouble(cust.Balance.Trim());
    //        command = PegPayInterface.GetStoredProcCommand("SaveKCCACustomerDetails", cust.CustomerRef, cust.CustomerName, cust.CustomerType, cust.Area, balance, cust.AgentCode);
    //        PegPayInterface.ExecuteNonQuery(command);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }


    //}

    //public DataTable CheckIfMeterNumberIsBlacklisted(string meterNumber)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("CheckIfMeterNumberIsBlacklisted", meterNumber);
    //        DataTable result = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public void UpdateBlacklistedAccountDebt(string meterNumber, int debtAmount)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("UpdateBlacklistedAccountDebt",
    //                                                        new string[] { meterNumber, debtAmount });
    //        PegPayInterface.ExecuteNonQuery(command);
    //        return;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public DataTable GetBlacklistedAccountsDebt(string meterNumber)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetBlacklistedAccountsDebt", meterNumber);
    //        DataTable result = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public DataTable CheckIfVendorHasBlacklistedAccounts(string vendorCode)
    //{

    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("CheckIfVendorHasBlacklistedAccounts", vendorCode);
    //        DataTable table = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        return table;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //public string InsertIntoBlackListedAccountLogs(UmemeTransaction trans, string AmountPaid, string AmountTaken)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        int amountTaken = Convert.ToInt32(AmountTaken);
    //        command = PegPayInterface.GetStoredProcCommand("InsertIntoBlacklistedAccountsLog");
    //        PegPayInterface.AddInParameter(command, "@custRef", DbType.String, trans.CustRef);
    //        PegPayInterface.AddInParameter(command, "@custName", DbType.String, trans.CustName);
    //        PegPayInterface.AddInParameter(command, "@paymentDate", DbType.String, FormatDate(trans.PaymentDate));
    //        PegPayInterface.AddInParameter(command, "@amountPaid", DbType.String, AmountPaid);
    //        PegPayInterface.AddInParameter(command, "@amountTaken", DbType.String, amountTaken);
    //        PegPayInterface.AddInParameter(command, "@tranId", DbType.String, trans.VendorTransactionRef);
    //        PegPayInterface.AddOutParameter(command, "@RecordId", DbType.Int32, 32);
    //        PegPayInterface.ExecuteNonQuery(command);
    //        //Read the Output Parameter Value afte execution
    //        int GeneratedId = Convert.ToInt32(PegPayInterface.GetParameterValue(command, "@RecordId"));
    //        receiptNo = "" + GeneratedId;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}

    //private string FormatDate(string date)
    //{
    //    string[] parts = date.Split('/');
    //    string day = parts[0];
    //    string month = parts[1];
    //    string year = parts[2];
    //    return year + "-" + month + "-" + day;
    //}

    //internal string InsertIntoReceivedTransactions(UmemeTransaction trans, string utilityCode)
    //{
    //    string receiptNo = "";
    //    try
    //    {
    //        string format = "dd/MM/yyyy";
    //        DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
    //        command = PegPayInterface.GetStoredProcCommand("InsertReceivedTransactions", trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, "", "", trans.TransactionAmount, payDate, DateTime.Now, trans.TransactionType, trans.PaymentType, trans.VendorTransactionRef, trans.Narration,
    //            false, trans.VendorCode, trans.Teller, false/*bool.Parse(trans.Reversal)*/, false, false/*bool.Parse(trans.Offline)*/, utilityCode, "", false);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            receiptNo = dt.Rows[0][0].ToString();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return receiptNo;
    //}


    //public BouquetDetails[] GetBouquetDetailsFromDB(string bouquetCode, string PayTvCode)
    //{
    //    List<BouquetDetails> allBouquets = new List<BouquetDetails>();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetBouquetDetails", bouquetCode, PayTvCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];

    //        if (dt.Rows.Count != 0)
    //        {
    //            foreach (DataRow row in dt.Rows)
    //            {
    //                //we are getting details for 1 bouquet
    //                if (!string.IsNullOrEmpty(bouquetCode))
    //                {
    //                    BouquetDetails bd = new BouquetDetails();
    //                    bd.BouquetCode = row["BouquetCode"].ToString();
    //                    bd.BouquetDescription = row["BouquetDescription"].ToString();
    //                    bd.BouquetName = row["BouquetName"].ToString();
    //                    bd.BouquetPrice = row["BouquetPrice"].ToString();
    //                    bd.StatusCode = "0";
    //                    bd.StatusDescription = "SUCCESS";
    //                    if (bd.BouquetCode.ToUpper().Trim() == bouquetCode.ToUpper().Trim())
    //                    {
    //                        allBouquets.Add(bd);
    //                        return allBouquets.ToArray();
    //                    }
    //                }
    //                //we are getting details for all bouquets
    //                else
    //                {
    //                    BouquetDetails bd = new BouquetDetails();
    //                    bd.BouquetCode = row["BouquetCode"].ToString();
    //                    bd.BouquetDescription = row["BouquetDescription"].ToString();
    //                    bd.BouquetName = row["BouquetName"].ToString();
    //                    bd.BouquetPrice = row["BouquetPrice"].ToString();
    //                    bd.StatusCode = "0";
    //                    bd.StatusDescription = "SUCCESS";
    //                    allBouquets.Add(bd);

    //                }
    //            }
    //        }
    //        else
    //        {
    //            allBouquets.Clear();
    //            BouquetDetails bd = new BouquetDetails();
    //            bd.StatusCode = "100";
    //            bd.StatusDescription = "NO BOUQUET DETAILS FOR BOUQUET CODE";
    //            allBouquets.Add(bd);
    //        }
    //        return allBouquets.ToArray();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return allBouquets.ToArray();

    //}

    //public int SaveBouquetDetails(BouquetDetails bd)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveBouquetDetails", bd.BouquetCode, bd.BouquetName, bd.BouquetDescription, bd.BouquetPrice, bd.PayTvCode);
    //        int rows = PegPayInterface.ExecuteNonQuery(command);
    //        return rows;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //}

    //internal int SaveReactivateRequest(string smartCardNumber)
    //{
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("SaveReactivateRequest", smartCardNumber);
    //        int rows = PegPayInterface.ExecuteNonQuery(command);
    //        return rows;
    //    }
    //    catch (Exception ex)
    //    {
    //        return -1;
    //    }
    //}

    //internal string checkCustomerDetails(string custReference)
    //{
    //    string status = "";
    //    DataTable returnTable = new DataTable();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails", custReference);
    //        returnTable = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (returnTable.Rows.Count > 0)
    //        {
    //            status = "true";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return status;// 
    //}

    //internal bool PRNAlreadyExistsInPegPay(string CustomerRef, string utility, string vendorCode)
    //{
    //    bool exists = false;
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("PRNAlreadyExistsInPegPay", CustomerRef, utility, vendorCode);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            exists = true;
    //        }
    //        else
    //        {
    //            exists = false;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    return exists;
    //}

    //internal LookupResp GetDetailsByOffenceCode(string CustomerReference)
    //{
    //    LookupResp resp = new LookupResp();
    //    try
    //    {
    //        command = PegPayInterface.GetStoredProcCommand("GetDetailsByOffenceCode",CustomerReference);
    //        DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
    //        if (dt.Rows.Count > 0)
    //        {
    //            resp.Amount = dt.Rows[0]["Amount"].ToString();
    //            resp.OffenceCode = CustomerReference;
    //            resp.StatusCode = "0";
    //            resp.StatusDescription = "SUCCESS";
    //        }
    //        else 
    //        {
    //            resp.StatusCode = "100";
    //            resp.StatusDescription = "INVALID OFFENCE CODE";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        resp.StatusCode = "100";
    //        resp.StatusDescription = ex.Message;
    //    }
    //    return resp;
    //}

    internal string LogReversalRequest(ReversalRequest req)
    {
        try
        {
            string[] parameters ={ req.VendorCode,
                                   req.OriginalTransactionId,
                                   req.ReversalTransactionId,
                                   req.Reason,
                                   "PENDING",
                                   "",
                                   "",
                                   "" };
            DataTable dt = PegPayInterface.ExecuteDataSet("InsertIntoPrepaidReversalRequests", parameters).Tables[0];
            string PegPayID = dt.Rows[0][0].ToString();
            return PegPayID;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetOriginalPrepaidTransaction(string VendorCode, string VendorTranId)
    {
        try
        {
            string[] parameters ={ VendorCode, VendorTranId };
            DataTable dt = PegPayInterface.ExecuteDataSet("GetOriginalPrepaidTransaction", parameters).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetDuplicateReversalRef(string VendorCode, string ReversalTransactionId)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetDuplicateReversalRef", VendorCode, ReversalTransactionId);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetDuplicateReversalRef", VendorCode, ReversalTransactionId).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable CheckIfReversed(string VendorCode, string OriginalTransactionId)
    {
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("CheckIfReversedAlready", VendorCode, OriginalTransactionId);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("CheckIfReversedAlready", VendorCode, OriginalTransactionId).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string GetSmsCharge(string vendorCode)
    {
        string amount = "50";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetPrepaidSmsCharges", vendorCode);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("GetPrepaidSmsCharges", vendorCode).Tables[0];
            if (returndetails.Rows.Count > 0)
            {
                amount = returndetails.Rows[0]["SmsCharge"].ToString();
            }
            
        }
        catch (Exception ex)
        {
            
        }
        return amount;
    }

    internal DataTable GetDataBundles(string Network, string Duration)
    {
        DataTable returndetails = PegPayInterface.ExecuteDataSet("DataBundles_SelectRow", Network, Duration).Tables[0];
        return returndetails;
    }
    internal string GetDataCharge(string BundleCode, string NetworkCode)
    {
        string amount = "";
        try
        {
            //command = PegPayInterface.GetStoredProcCommand("GetPrepaidSmsCharges", vendorCode);
            DataTable returndetails = PegPayInterface.ExecuteDataSet("DataBundles_SelectBundleCode", NetworkCode, BundleCode).Tables[0];
            if (returndetails.Rows.Count > 0)
            {
                amount = returndetails.Rows[0]["BundleAmount"].ToString();
            }

        }
        catch (Exception ex)
        {

        }
        return amount;
    }

    internal string GetCustomerType(string type)
    {
        string typeName = "";
        try
        {

            string[] parameters = { type };
            DataTable customerType = PegPayInterface.ExecuteDataSet("GetCustomerTypes", parameters).Tables[0];
            if (customerType.Rows.Count > 0)
            {
                typeName = customerType.Rows[0]["TypeCode"].ToString();

            }
            else
            {
                typeName = type;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return typeName;
    }
}
