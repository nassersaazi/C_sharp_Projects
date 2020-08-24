using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class ReversalRequest
{
    public string OriginalTransactionId = "";
    public string ReversalTransactionId = "";
    public string Reason = "";
    public string VendorCode = "";
    public string Password = "";
    public string DigitalSignature = "";
    public string StatusCode = "";
    public string StatusDesc = "";

    public ReversalRequest()
    {

    }

    internal bool IsValid()
    {
        BusinessLogic bll = new BusinessLogic();

        if (string.IsNullOrEmpty(this.OriginalTransactionId))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY THE ORIGINAL TRANSACTION ID";
            return false;
        }
        if (string.IsNullOrEmpty(this.ReversalTransactionId))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY THE REVERSAL TRANSACTION ID";
            return false;
        }
        if (string.IsNullOrEmpty(this.Password))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY A VENDOR PASSWORD";
            return false;
        }
        if (string.IsNullOrEmpty(this.VendorCode))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY A VENDOR CODE";
            return false;
        }
        if (string.IsNullOrEmpty(this.Reason))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY A REASON FOR THE REVERSAL";
            return false;
        }
        if (string.IsNullOrEmpty(this.DigitalSignature))
        {
            StatusCode = "100";
            StatusDesc = "PLEASE SUPPLY A DIGITAL SIGNATURE";
            return false;
        }
        if (this.OriginalTransactionId == this.ReversalTransactionId)
        {
            StatusCode = "100";
            StatusDesc = "ORIGINAL TRANSACTION ID AND REVERSAL ID CANNOT BE THE SAME";
            return false;
        }
        if (IsDuplicateReversalId(this))
        {
            StatusCode = "100";
            StatusDesc = "THIS REVERSAL ID ALREADY EXISTS";
            return false;
        }
        if (IsAlreadyReversed(this))
        {
            return false;
        }
        if (!bll.IsValidVendorCrednetials(this.VendorCode, this.Password))
        {
            StatusCode = "100";
            StatusDesc = "INVALID PEGPAY VENDOR CREDENTIALS";
            return false;
        }
        if (!bll.IsValidReversalDigitalSignature(this))
        {
            StatusCode = "100";
            StatusDesc = "INVALID DIGITAL SIGNATURE AT PEGPAY";
            return false;
        }
        if (!OriginalTransactionExistsAndAcceptsReversal())
        {
            return false;
        }
        return true;
    }

    private bool IsDuplicateReversalId(ReversalRequest tran)
    {
        bool ret = false;
        DatabaseHandler dp = new DatabaseHandler();
        DataTable dt = dp.GetDuplicateReversalRef(tran.VendorCode, tran.ReversalTransactionId);
        if (dt.Rows.Count > 0)
        {
            ret = true;
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    private bool IsAlreadyReversed(ReversalRequest tran)
    {
        bool ret = false;
        DatabaseHandler dp = new DatabaseHandler();
        DataTable dt = dp.CheckIfReversed(tran.VendorCode, tran.OriginalTransactionId);
        if (dt.Rows.Count > 0)
        {
            ret = true;
            string status = dt.Rows[0]["Status"].ToString();
            if (status == "SUCCESS")
            {
                ReversalTransactionId = dt.Rows[0]["ReversalTransactionId"].ToString();
                StatusCode = "0";
                StatusDesc = "THE TRANSACTION HAS ALREADY BEEN REVERSED";
            }
            else if (status == "PENDING")
            {
                ReversalTransactionId = dt.Rows[0]["ReversalTransactionId"].ToString();
                StatusCode = "100";
                StatusDesc = "A REVERSAL REQUEST HAS ALREADY BEEN LOGGED FOR SPECIFIED ORIGINAL TRANSACTION ID";
            }
        }
        else
        {
            ret = false;
        }
        return ret;
    }

    private bool OriginalTransactionExistsAndAcceptsReversal()
    {
        DatabaseHandler dh = new DatabaseHandler();
        DataTable dt = dh.GetOriginalPrepaidTransaction(this.VendorCode, this.OriginalTransactionId);

        //if original transaction is found
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            bool result = UtilityAcceptsReversal(dr);
            return result;
        }
        //original transaction not found
        else
        {
            StatusCode = "100";
            StatusDesc = "ORIGINAL TRANSACTION NOT FOUND";
            return false;
        }
    }

    private bool UtilityAcceptsReversal(DataRow dr)
    {
        string UtilityCode = dr["UtilityCode"].ToString().ToUpper();
        string CustomerType = dr["CustomerType"].ToString().ToUpper();

        //if its a post paid Umeme transaction
        if (UtilityCode == "UMEME" && CustomerType == "POSTPAID")
        {
            return true;
        }
        //if its a Nwsc transaction
        else if (UtilityCode == "NWSC")
        {
            return true;
        }
        //some other utility
        else
        {
            StatusCode = "100";
            StatusDesc = "UTILITY DOESNT SUPPORT AUTOMATED REVERSALS";
            return false;
        }
    }
}
