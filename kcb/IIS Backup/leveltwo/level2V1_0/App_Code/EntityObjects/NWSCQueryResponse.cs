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
/// Summary description for NWSCResponse
/// </summary>
public class NWSCQueryResponse : QueryResponse
{
    private string customerName, area, outstandingBalance,custType;

    public string OutstandingBalance
    {
        get
        {
            return outstandingBalance;
        }
        set
        {
            outstandingBalance = value;
        }
    }

    public string CustType
    {
        get
        {
            return custType;
        }
        set
        {
            custType = value;
        }
    }
    public string Area
    {
        get
        {
            return area;
        }
        set
        {
            area = value;
        }
    }
    public string CustomerName
    {
        get
        {
            return customerName;
        }
        set
        {
            customerName = value;
        }
    }
}
