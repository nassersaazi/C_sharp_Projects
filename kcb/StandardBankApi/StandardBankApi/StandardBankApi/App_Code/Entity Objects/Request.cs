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
/// Summary description for Request
/// </summary>
public abstract class Request
{
    private string pegPayStatusCode = "";

    internal string PegPayStatusCode
    {
        get { return pegPayStatusCode; }
        set { pegPayStatusCode = value; }
    }
    private string pegPayStatusDescription = "";

    internal string PegPayStatusDescription
    {
        get { return pegPayStatusDescription; }
        set { pegPayStatusDescription = value; }
    }

    public Request()
    {
        pegPayStatusCode = "";
        pegPayStatusDescription = "";
    }

    public abstract bool IsValidRequest();
    
}
