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
/// Summary description for Vendor
/// </summary>
public class Vendor
{
    private string vendorCode, billsyscode, vendor, email, contact, passwd, user, subject, message, status;
    private int vendorid, invalidCount;
    private bool active, sendemail, reset;
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
    public string Subject
    {
        get
        {
            return subject;
        }
        set
        {
            subject = value;
        }
    }
    public string Message
    {
        get
        {
            return message;
        }
        set
        {
            message = value;
        }
    }
    public string Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }
    public string BillSysCode
    {
        get
        {
            return billsyscode;
        }
        set
        {
            billsyscode = value;
        }
    }
    public string VendorName
    {
        get
        {
            return vendor;
        }
        set
        {
            vendor = value;
        }
    }
    public string Email
    {
        get
        {
            return email;
        }
        set
        {
            email = value;
        }
    }
    public string Contact
    {
        get
        {
            return contact;
        }
        set
        {
            contact = value;
        }
    }
    public string Passwd
    {
        get
        {
            return passwd;
        }
        set
        {
            passwd = value;
        }
    }
    public string User
    {
        get
        {
            return user;
        }
        set
        {
            user = value;
        }
    }
    public int Vendorid
    {
        get
        {
            return vendorid;
        }
        set
        {
            vendorid = value;
        }
    }
    public int InvalidCount
    {
        get
        {
            return invalidCount;
        }
        set
        {
            invalidCount = value;
        }
    }
    public bool Active
    {
        get
        {
            return active; /// 0701047275 Irene
        }
        set
        {
            active = value;
        }
    }
    public bool Sendemail
    {
        get
        {
            return sendemail; /// 0701047275 Irene
        }
        set
        {
            sendemail = value;
        }
    }
    public bool Reset
    {
        get
        {
            return reset; /// 0701047275 Irene
        }
        set
        {
            reset = value;
        }
    }
}
