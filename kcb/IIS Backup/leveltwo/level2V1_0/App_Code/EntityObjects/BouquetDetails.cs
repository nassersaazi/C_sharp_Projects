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
/// Summary description for BouquetDetails
/// </summary>
public class BouquetDetails
{
    public string BouquetCode;
    public string BouquetName;
    public string BouquetPrice;
    public string BouquetDescription;
    public string PayTvCode;
    public string StatusCode;
    public string StatusDescription;

    public BouquetDetails()
    {
    }

    public override string ToString()
    {
        string whatToLog = "BouquetCode = " + BouquetCode + Environment.NewLine +
                           "BouquetName = " + BouquetName + Environment.NewLine +
                           "BouquetPrice = " + BouquetPrice + Environment.NewLine +
                           "BouquetDescription = " + BouquetDescription + Environment.NewLine +
                           "PayTvCode = " + PayTvCode + Environment.NewLine +
                           "StatusCode = " + StatusCode + Environment.NewLine +
                           "StatusDescription = " + StatusDescription + Environment.NewLine;
        return whatToLog;
    }


}
