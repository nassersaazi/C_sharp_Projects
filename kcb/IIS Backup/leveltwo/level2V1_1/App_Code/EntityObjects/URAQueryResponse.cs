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
/// Summary description for URAQueryResponse
/// </summary>
public class URAQueryResponse : QueryResponse
{
    private string customerName, tin, outstandingBalance;
    public string paymentRegistrationDate,prnStatus;

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
    public string TIN
    {
        get
        {
            return tin;
        }
        set
        {
            tin = value;
        }
    }
    public string PaymentRegistrationDate
    {
        get
        {
            return paymentRegistrationDate;
        }
        set
        {
            paymentRegistrationDate = value;
        }
    }
    public string PrnStatus
    {
        get
        {
            return prnStatus;
        }
        set
        {
            prnStatus = value;
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
