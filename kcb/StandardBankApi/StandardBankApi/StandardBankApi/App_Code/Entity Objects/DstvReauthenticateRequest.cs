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
/// Summary description for DstvReauthenticateRequest
/// </summary>
public class DstvReauthenticateRequest:Request
{
    public string SmartCardNumber;
    public Credentials credentials;
    public DstvReauthenticateRequest()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public override bool IsValidRequest()
    {
        return true;
    }
}
