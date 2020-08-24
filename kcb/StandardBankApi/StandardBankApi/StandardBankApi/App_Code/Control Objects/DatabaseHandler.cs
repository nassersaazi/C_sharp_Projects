using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using PegasusEnviroment;
using System.Collections;

/// <summary>
/// Summary description for DatabaseHandler
/// </summary>
public class DatabaseHandler
{
    private Database ThirdPartyDB;
    private DataTable dt = new DataTable();
    private DbCommand command;

    public DatabaseHandler()
    {
        try
        {
            //Connection con = new Connection(PegpayConfigs.SERVER_DBSERVER2, PegpayConfigs.ENV_LIVE, "LivePegPay");
            //con = con.GetConnection();

           // ThirdPartyDB = con.Database;// DatabaseFactory.CreateDatabase("LivePegPay");
            ThirdPartyDB = DatabaseFactory.CreateDatabase("LivePegPay");
            //MobileMoney = DatabaseFactory.CreateDatabase("LiveMobileMoney");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void LogXmlRequestResponse(string vendorId, string request, string response)
    {
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("LogXmlRequestResponse", vendorId, request, response);
            ThirdPartyDB.ExecuteNonQuery(command);
        }
        catch (Exception ee)
        {

        }
    }

    internal string LogError(string error, string vendorCode, DateTime now, string AgentCode)
    {
        string ret = "";
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
            ThirdPartyDB.ExecuteNonQuery(command);
            ret = "YES";
        }
        catch (Exception ex)
        {
            ret = ex.Message;
        }
        return ret;
    }

    internal void LogErrorKCB(string reference, string name, DateTime now, string requestType, string phoneNumber, string error, string TranId)
    {
        // "" ,"" , DateTime.Now, "" ,"",e.Message,""

        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("LogErrorKCB", reference, name, now, requestType, phoneNumber, error, TranId);
            ThirdPartyDB.ExecuteNonQuery(command);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public string InsertIntoStanbicRecieved(PostTransactionRequest tran,string UtilityCode,string Status)
    {
        try
        {
            string Reason = "";
            if (string.IsNullOrEmpty(tran.PayLoad.Addendum))
            {
                tran.PayLoad.Addendum = "";
            }
            command = ThirdPartyDB.GetStoredProcCommand("InsertVasTransactionRecieved",
                                                        tran.PayLoad.MSISDN,
                                                        tran.PayLoad.BeneficiaryID,
                                                         
                                                        tran.PayLoad.BeneficiaryName,
                                                        tran.PayLoad.Area,
                                                        tran.PayLoad.Amount,
                                                        tran.PayLoad.Currency,
                                                        tran.PayLoad.ReferenceID,
                                                        UtilityCode,
                                                        tran.PayLoad.Narration,
                                                        Status,
                                                        tran.PayLoad.MerchantID,
                                                        Reason,
                                                        tran.PayLoad.Addendum
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt.Rows[0][0].ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string GetUtilityToBeAccessed(string merchantID)
    {
        string Utility = "UNKNOWN";
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetServiceCode",
                                                        merchantID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt.Rows[0]["ServiceID"].ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return Utility;
    }

    internal DataTable GetTransactionStatus(QueryTransactionStatusRequest requestData)
    {
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetTransactionStatus",
                                                        requestData.PayLoad.ReferenceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveRequestlog(string CustRef, string CustName, string TranAmount, string password, string LogType, DateTime now)
    {
        try
        {
            string[] parameters = { CustRef, CustName, TranAmount, password, LogType };
            command = ThirdPartyDB.GetStoredProcCommand("SaveRequestLogsKCB", CustRef, CustName, TranAmount, password, LogType, now);
            command.CommandTimeout = 300000000;
            ThirdPartyDB.ExecuteNonQuery(command);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal string GetServiceNameFromDB(string serviceID)
    {
        string Utility = "UNKNOWN";
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetServiceCode",
                                                        serviceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["ServiceCode"].ToString();
            }
            else 
            {
                return Utility;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        } 
    }

    internal string GetServerIpAddress(string serviceID)
    {
        string Utility = "UNKNOWN";
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetServiceCode",
                                                        serviceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt.Rows[0]["IpAddress"].ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    public string GetServiceType(string serviceID)
    {
        string Utility ="UTILITY";
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetServiceCode",
                                                        serviceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt.Rows[0]["IpAddress"].ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        
    }

    internal DataTable GetDuplicateTranByReferenceID(string referenceID)
    {
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetDuplicateTranByReferenceID",
                                                        referenceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetServiceDetailsFromDB(string serviceID)
    {
        try
        {
            command = ThirdPartyDB.GetStoredProcCommand("GetServiceCode",
                                                        serviceID
                                                        );
            DataTable dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            return dt;
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
            command = ThirdPartyDB.GetStoredProcCommand("GetBlacklistedNumbers");
            DataSet ds = ThirdPartyDB.ExecuteDataSet(command);
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
            command = ThirdPartyDB.GetStoredProcCommand("GetNetworkCodes");
            DataSet ds = ThirdPartyDB.ExecuteDataSet(command);
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

   
    internal DataTable GetAllowedCountryCodes(string countryCode)
    {
        DataTable table = new DataTable();
        try
        {
            // table = PegPayInterface.ExecuteDataSet("GetAllowedCountryCodes", countryCode).Tables[0];
            command = ThirdPartyDB.GetStoredProcCommand("GetAllowedCountryCodes", countryCode);
            table = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
        }
        catch (Exception ee)
        {

        }
        return table;
    }
}
