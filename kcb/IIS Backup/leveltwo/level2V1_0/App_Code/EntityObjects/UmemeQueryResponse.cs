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
/// Summary description for UmemeResponse
/// </summary>
public class UmemeQueryResponse:QueryResponse
{
    private string customerName, customerType, outstandingBalance;

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
    public string CustomerType
    {
        get
        {
            return customerType;
        }
        set
        {
            customerType = value;
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
