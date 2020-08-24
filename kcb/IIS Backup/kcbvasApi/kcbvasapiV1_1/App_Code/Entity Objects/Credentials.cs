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
/// Summary description for Credentials
/// </summary>
public class Credentials
{
    private string username,password;

    public string Username
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
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

    public Credentials() 
    {
        this.username = "";
        this.password = "";
    }
}
