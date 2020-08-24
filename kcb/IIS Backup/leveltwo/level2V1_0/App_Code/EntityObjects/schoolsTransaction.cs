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
/// Summary description for KCCATransaction
/// </summary>
public class schoolsTransaction:UmemeTransaction
{
    private string utilityCode;

    public string UtilityCode
    {
        get
        {
            return utilityCode;
        }
        set
        {
            utilityCode = value;
        }
    }
}
