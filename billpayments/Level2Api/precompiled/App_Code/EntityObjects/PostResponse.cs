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
}
