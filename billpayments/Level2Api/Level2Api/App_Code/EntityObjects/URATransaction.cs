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
/// Summary description for URATransaction
/// </summary>
public class URATransaction : Transaction
{
    private string tin, branchCode, bankCode,status;

    public string TIN
    {
        get
        {
            return tin;
        }
        set
        {
            tin = value;
        }
    }

    public string Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    public string BranchCode
    {
        get
        {
            return branchCode;
        }
        set
        {
            branchCode = value;
        }
    }

    public string BankCode
    {
        get
        {
            return bankCode;
        }
        set
        {
            bankCode = value;
        }
    }
}
