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
/// Summary description for RechargeCustomer
/// </summary>
public class SolarCustomer:Customer
{
    public SolarCustomer()
    {
    
    }

    string tokenLimit, usergroup;

    public string TokenLimit
    {
        get { return tokenLimit; }
        set { tokenLimit = value; }
    }

    public string Usergroup
    {
        get { return usergroup; }
        set { usergroup = value; }
    }
    bool isRegistered;

    public bool IsRegistered
    {
        get { return isRegistered; }
        set { isRegistered = value; }
    }

    
}
