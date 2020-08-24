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
/// Summary description for ValidateTransactions
/// </summary>
public class ValidateTransactions
{
    public ValidateTransactions()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string ValidateTransactionObject(Transaction transaction)
    {
        string value = "OK";
        int amount, number = 0;
        if (string.IsNullOrEmpty(transaction.CustRef))
        {
            value = "Provide a valid customer reference".ToUpper();
        }
        else if (string.IsNullOrEmpty(transaction.TransactionType))
        {
            value = "provide a payment type, (cash or cheque)".ToUpper();
        }
        else if (transaction.TransactionType == "cheque" && !Int32.TryParse(transaction.ChequeNumber, out number))
        {
            value = "Please provide a numeric cheque number".ToUpper();
        }
       
        else if (!Int32.TryParse(transaction.TransactionAmount, out amount))
        {
            value = "Please provide a transaction amount".ToUpper();
        }
        else if (!ValidateDate(transaction.PaymentDate))
        {
            value = "invalid transaction date".ToUpper();
        }
        else if (amount < 500)
        {
            value = "Payments below 500 UGX are not allowed";
        }
        else
        {
            string stat = CustomerReferenceExists(transaction.CustRef);
            if (stat != "ok")
            {
                value = stat;
            }
        }

        return value;

    }

    private static string CustomerReferenceExists(string meternumber)
    {
        DataTable table = new DatabaseHandler().GetRechargeUsers(meternumber);
        string value = "ok";
        if (table.Rows.Count > 0)
        {
            DataRow row = table.Rows[0];
            string isactive = row["active"].ToString();
            if (isactive == "1")
            {
                value = "ok";
            }
            else
            {
                value = "customer is not active. please contact administrators";
            }

        }
        else
            value = "unknown customer refence";

        return value;
    }
    private static bool ValidateDate(string datetime)
    {
        string[] formats = { "dd/MM/yyyy" };
        //, "M/d/yyyy", "M/dd/yyyy", "MM/d/yyyy" 
        DateTime expectedDate;
        if (!DateTime.TryParseExact(datetime, formats, new CultureInfo("en-US"), DateTimeStyles.None, out expectedDate))
        {
            return false;// Console.Write("Thank you Mario, but the DateTime is in another format.");
        }
        return true;
    }
}
