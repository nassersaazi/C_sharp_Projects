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
/// Summary description for PingResponse
/// </summary>
public class PingResponse
{
    public string StatusCode;
    public string StatusDescription;
    public string LastTimeUp;
    public string LastTimeDown;
    public string LastWentOff;
    public string ServiceType;
    public string Reason;

    public PingResponse()
    {
       
    }
}
