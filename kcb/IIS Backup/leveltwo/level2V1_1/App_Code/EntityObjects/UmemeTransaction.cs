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
/// Summary description for UmemeTransaction
/// </summary>
public class UmemeTransaction:Transaction
{
    private string paymentType;

    public string PaymentType
    {
        get
        {
            return paymentType;
        }
        set
        {
            paymentType = value;
        }
    }

    //public string CustomerType
    //{
    //    get
    //    {
    //        return customerType;
    //    }
    //    set
    //    {
    //        customerType = value;
    //    }
    //}
}
