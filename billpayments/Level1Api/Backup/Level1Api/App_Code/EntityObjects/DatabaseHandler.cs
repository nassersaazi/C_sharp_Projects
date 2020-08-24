using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Collections.Generic;

/// <summary>
/// Summary description for DatabaseHandler
/// </summary>
public class DatabaseHandler
{
    private Database PegPay_DB;
    private DbCommand procommand;
    public DatabaseHandler()
    {
        try
        {
            string constring = "LivePegPay";
            //PegPay_DB = DatabaseFactory.CreateDatabase(constring);

            PegPay_DB = DbLayer.CreateDatabase(constring, DbLayer.DB2);

        }
        catch (Exception ex)
        {
           
        }
    }

    public void LogIp(string vendorCode, string ipaddress)
    {
        try
        {
            procommand = PegPay_DB.GetStoredProcCommand("SaveIpAddress", vendorCode, ipaddress);
            PegPay_DB.ExecuteNonQuery(procommand);
        }
        catch (Exception ee)
        {
            
        }
    }

    public DataSet ExecuteDataSet(string procedure, params object[] parameters)
    {
        try
        {
            procommand = PegPay_DB.GetStoredProcCommand(procedure, parameters);
            return PegPay_DB.ExecuteDataSet(procommand);
        }
        catch (Exception ee)
        {
            throw ee;
        }
    }

    internal List<String> GetUtilities(string category)
    {
        List<String> utilities = new List<String>();
        DataTable dt = ExecuteDataSet("GetUtilitiesByCategory", category).Tables[0];
        foreach (DataRow dr in dt.Rows)
        {
            string utility = dr["UtilityCode"].ToString();
            utilities.Add(utility);
        }
        return utilities;
    }
}
