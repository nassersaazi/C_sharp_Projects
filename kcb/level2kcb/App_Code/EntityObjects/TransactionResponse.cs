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
/// Summary description for TransactionResponse
/// </summary>
public class TransactionResponse
{
    string errorCode, errorDescription, receiptNumber;

    public string ErrorCode
    {
        get { return errorCode; }
        set { errorCode = value; }
    }

    public string ErrorDescription
    {
        get { return errorDescription; }
        set { errorDescription = value; }
    }

    public string ReceiptNumber
    {
        get { return receiptNumber; }
        set { receiptNumber = value; }
    }

    public TransactionResponse()
    {
        
        
    }
}
