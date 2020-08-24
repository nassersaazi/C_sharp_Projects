using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;

/// <summary>
/// Summary description for SolarHandler
/// </summary>
public class SolarHandler
{
    public SolarHandler()
    {
        
    }

    public SolarCustomer ValidateMeterNumber(string meternumber)
    {
        SolarCustomer customer = new SolarCustomer();
        try
        {
            DatabaseHandler dh = new DatabaseHandler();
            DataTable table = dh.GetRechargeUsers(meternumber);
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                string isactive = row["active"].ToString();
                if (isactive == "1")
                {
                    customer.CustomerName = row["firstName"].ToString();
                    customer.CustomerRef = row["meternumber"].ToString();
                    customer.Usergroup = row["usergroup"].ToString();
                    customer.TokenLimit = row["tokenLimit"].ToString();
                    customer.TIN = row["gender"].ToString();
                    customer.StatusCode = "0";
                    customer.StatusDescription = dh.GetStatusDescr(customer.StatusCode);
                }
                else
                {
                    customer.StatusCode = "99";
                    customer.StatusDescription = "Meternumber has been deactivated".ToUpper();
                }
            }
            else
            {
                customer.StatusCode = "99";
                customer.StatusDescription = "User not not found. Access denied use".ToUpper();
            }

        }
        catch (Exception ee)
        {
            
            throw ee;
        }
        return customer; 
    }
    public TransactionResponse PostSolarPayment(Transaction tran)
    {
        TransactionResponse resp = new TransactionResponse();
        try
        {
            //DatabaseHandler handler = new DatabaseHandler();
            //string format = "dd/MM/yyyy";
            //DateTime payDate = DateTime.ParseExact(tran.PaymentDate, format, CultureInfo.InvariantCulture);
            //tran.PaymentDate = payDate + "";
            //string exists = handler.CheckTransaction(tran.VendorTransactionRef, tran.VendorCode);
            //if (string.IsNullOrEmpty(exists))
            //{
            //    string receipt = handler.PostTransactionObject(tran, "SOLAR");
            //    if (string.IsNullOrEmpty(receipt))
            //    {

            //        resp.ErrorCode = "100";
            //        resp.ErrorDescription = "Insertion failed";
            //    }
            //    else
            //    {
            //        resp.ErrorCode = "0";
            //        resp.ErrorDescription = "SUCCESS";
            //        resp.ReceiptNumber = receipt;
            //    }
            //}
            //else
            //{
            //    resp.ErrorCode = "20";
            //    resp.ErrorDescription = "Duplicate transaction";
            //    resp.ReceiptNumber = exists;
            //}
        }
        catch (Exception ee)
        {
            throw ee;
        }
        return resp;
    }
}
