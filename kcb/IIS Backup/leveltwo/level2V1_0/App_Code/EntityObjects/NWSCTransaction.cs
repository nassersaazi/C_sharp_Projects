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
/// Summary description for NWSCTransaction
/// </summary>
public class NWSCTransaction:Transaction
{
    public string area,UtilityCompany;

    public string utilityCompany
    {
        get
        {
            return UtilityCompany;
        }
        set
        {
            UtilityCompany = value;
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
}
