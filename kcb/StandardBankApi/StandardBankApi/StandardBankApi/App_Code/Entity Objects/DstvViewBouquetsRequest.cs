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
/// Summary description for DstvViewBouquetsRequest
/// </summary>
public class DstvViewBouquetsRequest:Request
{
    public string BouquetCode = "";
    public string PayTvCode = "";
    public Credentials Credentials;

    public DstvViewBouquetsRequest()
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
