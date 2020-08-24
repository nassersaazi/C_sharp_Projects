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
/// Summary description for PostResponse
/// </summary>
public class Response
{
    private string statusCode, statusDescription;

    public string StatusCode
    {
        get
        {
            return statusCode;
        }
        set
        {
            statusCode = value;
        }
    }
    public string StatusDescription
    {
        get
        {
            return statusDescription;
        }
        set
        {
            statusDescription = value;
        }
    }
}
