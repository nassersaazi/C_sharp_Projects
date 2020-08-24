using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for UtilityQuery
/// </summary>
public class QueryRequest
{
    private string queryField1, queryField2, queryField3, queryField4, queryField5, queryField6, queryField7, queryField8, queryField9, queryField10;

    public override string ToString()
    {
        string whatToLog = "request.queryField1(CustRef)=" + queryField1 + Environment.NewLine +
                           "request.queryField2(Area)=" + queryField2 + Environment.NewLine +
                           "request.queryField3=" + queryField3 + Environment.NewLine +
                           "request.queryField4(UtilityCode)=" + queryField4 + Environment.NewLine +
                           "request.queryField5(VendorCode)=" + queryField5 + Environment.NewLine +
                           "request.queryField6(Password)=" + queryField6 + Environment.NewLine +
                           "request.queryField7=" + queryField7 + Environment.NewLine +
                           "request.queryField8=" + queryField8 + Environment.NewLine +
                           "request.queryField9=" + queryField9 + Environment.NewLine +
                           "request.queryField10=" + queryField10 + Environment.NewLine;
        return whatToLog;
    }

    public string QueryField1
    {
        get
        {
            return queryField1;
        }
        set
        {
            queryField1 = value;
        }
    }
    public string QueryField2
    {
        get
        {
            return queryField2;
        }
        set
        {
            queryField2 = value;
        }
    }
    public string QueryField3
    {
        get
        {
            return queryField3;
        }
        set
        {
            queryField3 = value;
        }
    }
    public string QueryField4
    {
        get
        {
            return queryField4;
        }
        set
        {
            queryField4 = value;
        }
    }
    public string QueryField5
    {
        get
        {
            return queryField5;
        }
        set
        {
            queryField5 = value;
        }
    }
    public string QueryField6
    {
        get
        {
            return queryField6;
        }
        set
        {
            queryField6 = value;
        }
    }
    public string QueryField7
    {
        get
        {
            return queryField7;
        }
        set
        {
            queryField7 = value;
        }
    }
    public string QueryField8
    {
        get
        {
            return queryField8;
        }
        set
        {
            queryField8 = value;
        }
    }
    public string QueryField9
    {
        get
        {
            return queryField9;
        }
        set
        {
            queryField9 = value;
        }
    }
    public string QueryField10
    {
        get
        {
            return queryField10;
        }
        set
        {
            queryField10 = value;
        }
    }
    
}
