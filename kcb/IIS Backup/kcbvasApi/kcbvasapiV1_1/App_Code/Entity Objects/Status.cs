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
/// Summary description for Status
/// </summary>
public class Status
{
    private string statusCode;
    private string description;

    public string Description
    {
        get { return description; }
        set { description = value; }
    }


    public string StatusCode
    {
        get { return statusCode; }
        set { statusCode = value; }
    }

	public Status()
	{
        this.statusCode = "";
        this.description = "";
	}
}
