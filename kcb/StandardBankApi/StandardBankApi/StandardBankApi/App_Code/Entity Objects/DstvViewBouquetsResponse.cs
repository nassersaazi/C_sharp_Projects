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
/// Summary description for DstvViewBouquetsResponse
/// </summary>
public class DstvViewBouquetsResponse
{
    public Status Status;
    public string BouquetCode;
    public string PayTvCode;
    public string BouquetName;
    public string BouquetPrice;
    public string BouquetDescritpion;

    public DstvViewBouquetsResponse()
    {
        //
        // TODO: Add constructor logic here
        //
    }
}
