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
/// Summary description for UmemePostResponse
/// </summary>
public class UmemePostResponse:PostResponse
{
    private string receiptNumber;

    public string ReceiptNumber
    {
        get
        {
            return receiptNumber;
        }
        set
        {
            receiptNumber = value;
        }
    }
}
