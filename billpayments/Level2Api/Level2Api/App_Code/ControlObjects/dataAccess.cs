using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Security.Cryptography;
using System.Data.Common;
using System.Collections;
using System.Data.Sql;
using System.Net;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.ObjectBuilder;



public class dataAccess
{
    private Database ThirdParty_DB;
    private DbCommand mycommand;
    private DataTable returndetails;
    private DataSet returndetailsds;

    public dataAccess()
    {
        try
        {
            ThirdParty_DB = DatabaseFactory.CreateDatabase("TestPegPay");

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string calculateMD5(string input)
    {
        try
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    public DataTable GetGlobalValues(string Valuecode, string ValueGroupcode)
    {
        try
        {
            mycommand = ThirdParty_DB.GetStoredProcCommand("GetGlobalValues", Valuecode, ValueGroupcode);
            returndetails = ThirdParty_DB.ExecuteDataSet(mycommand).Tables[0];
            return returndetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetNWSCCustomer(string customerReference, string area, string vendorCode) 
    {
        returndetails = new DataTable();
        try
        {
            mycommand = ThirdParty_DB.GetStoredProcCommand("");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return returndetails;
    }

    internal string GetKCCASession(string custref, string vendorCode)
    {
        string sessionKey = "";
        try
        {
            mycommand = ThirdParty_DB.GetStoredProcCommand("GetKCCASession", custref, vendorCode);
            returndetails = ThirdParty_DB.ExecuteDataSet(mycommand).Tables[0];
            if (returndetails.Rows.Count > 0)
            {
                sessionKey = returndetails.Rows[0]["Area"].ToString();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return sessionKey;
    }
    internal string GetMtnTransactionId(string customerReference)
    {
        string TransactionID = "";
        try
        {
            mycommand = ThirdParty_DB.GetStoredProcCommand("GetTransactionId", customerReference);
            DataTable dt = ThirdParty_DB.ExecuteDataSet(mycommand).Tables[0];
            return dt.Rows[0]["VendorTranId"].ToString();
        }
        catch (Exception ex)
        {
            return TransactionID;
        }
    }
}



