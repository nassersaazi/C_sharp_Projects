using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using UtilityReferences.DataApi;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

/// <summary>
/// Summary description for DbAccess
/// </summary>
public class DbAccess
{
    DataAccess access = new DataAccess();
    private Database pegpaydbase;
    private DbCommand command;
    private const String constring = "TestPegPay";

    public DbAccess()
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


    internal DataSet ExecuteDataSet(string procedure, params object[] parameters)
    {
        command = pegpaydbase.GetStoredProcCommand(procedure, parameters);
        return pegpaydbase.ExecuteDataSet(command);
        //return access.ExecuteDataSet("5", "GenericPegPayApi", procedure, parameters);
    }

    internal Result ExecuteNonQuery(string procedure, params object[] parameters)
    {
        command = pegpaydbase.GetStoredProcCommand(procedure, parameters);
        pegpaydbase.ExecuteNonQuery(command);
        return new Result();
        //return access.ExecuteNonQuery("5", "GenericPegPayApi", procedure, parameters);
    }
}
