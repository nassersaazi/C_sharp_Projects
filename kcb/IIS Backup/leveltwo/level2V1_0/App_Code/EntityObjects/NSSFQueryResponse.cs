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
/// Summary description for NSSFQueryResponse
/// </summary>
public class NSSFQueryResponse : UmemeQueryResponse
{
    public string customerReference, employerNo, employer, period, paymentMethod;
    public string vendorCode;
    public string password;
    public  new string CustomerReference
    {
        get
        {
            return customerReference;
        }
        set
        {
            customerReference = value;
        }
    }
    public new string EmployerNo
    {
        get
        {
            return employerNo;
        }
        set
        {
            employerNo = value;
        }
    }
    public new string Employer
    {
        get
        {
            return employer;
        }
        set
        {
            employer = value;
        }
    }
    public new string Period
    {
        get
        {
            return period;
        }
        set
        {
            period = value;
        }
    }
    public new string PaymentMethod
    {
        get
        {
            return paymentMethod;
        }
        set
        {
            paymentMethod = value;
        }
    }
    public string VendorCode
    {
        get
        {
            return vendorCode;
        }
        set
        {
            vendorCode = value;
        }
    }
    public string Password
    {
        get
        {
            return password;
        }
        set
        {
            password = value;
        }
    }
}
