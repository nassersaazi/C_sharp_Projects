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
public class PostResponse:Response
{
    private string pegpayPostId;
    private string token;
    private string units;

    public string PegPayPostId
    {
        get
        {
            return pegpayPostId;
        }
        set
        {
            pegpayPostId = value;
        }
    }
    public string Token
    {
        get
        {
            return token;
        }
        set
        {
            token = value;
        }
    }
    public string Units
    {
        get
        {
            return units;
        }
        set
        {
            units = value;
        }
    }
}
