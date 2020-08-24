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
/// Summary description for KCCAQueryResponse
/// </summary>
public class SchoolsQueryResponse:UmemeQueryResponse
{
    string level, amount, school, accountNumber, customerBalance, minimumPayment, bankCode, bank, tranCharge, allowPartialPayment, className;
    public string ClassName
    {
        get { return className; }
        set { className = value; }
    }
    public string MinimumPayment
    {
        get { return minimumPayment; }
        set { minimumPayment = value; }
    }
    public string AllowPartialPayment
    {
        get { return allowPartialPayment; }
        set { allowPartialPayment = value; }
    }
    public string BankCode
    {
        get { return bankCode; }
        set { bankCode = value; }
    }

    public string Bank
    {
        get { return bank; }
        set { bank = value; }
    }
    public string TranCharge
    {
        get { return tranCharge; }
        set { tranCharge = value; }
    }


    public string CustomerBalance
    {
        get { return customerBalance; }
        set { customerBalance = value; }
    }

    public string AccountNumber
    {
        get { return accountNumber; }
        set { accountNumber = value; }
    }

    public string School
    {
        get { return school; }
        set { school = value; }
    }

    public string Level
    {
        get { return level; }
        set { level = value; }
    }

    public string Amount
    {
        get { return amount; }
        set { amount = value; }
    }
    public SchoolsQueryResponse()
    {

        //
        // TODO: Add constructor logic here
        //
    }
}
