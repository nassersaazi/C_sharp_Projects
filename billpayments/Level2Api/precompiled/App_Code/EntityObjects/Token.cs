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
/// Summary description for Token
/// </summary>
public class Token
{
    private string payAccount, debtrecovery, receiptNumber, token, totalAmount, tax, inflation, fuel, fx, tokenValue, units, meterNumber, statusCode, statusDescription, pegPayPostId, lifeLine;

    public string LifeLine
    {
        get { return lifeLine; }
        set { lifeLine = value; }
    }

    public string PegPayPostId
    {
        get
        {
            return pegPayPostId;
        }
        set
        {
            pegPayPostId = value;
        }
    }
    public string PayAccount
    {
        get
        {
            return payAccount;
        }
        set
        {
            payAccount = value;
        }
    }
    public string DebtRecovery
    {
        get
        {
            return debtrecovery;
        }
        set
        {
            debtrecovery = value;
        }
    }
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
    public string StatusDescription
    {
        get
        {
            return statusDescription;
        }
        set
        {
            statusDescription = value;
        }
    }
    public string StatusCode
    {
        get
        {
            return statusCode;
        }
        set
        {
            statusCode = value;
        }
    }
    public string MeterNumber
    {
        get
        {
            return meterNumber;
        }
        set
        {
            meterNumber = value;
        }
    }
    public string Units
    {
        get
        {
            return units;
        }
        set
        {
            units = value;
        }
    }
    public string TokenValue
    {
        get
        {
            return tokenValue;
        }
        set
        {
            tokenValue = value;
        }
    }
    public string Inflation
    {
        get
        {
            return inflation;
        }
        set
        {
            inflation = value;
        }
    }
    public string Tax
    {
        get
        {
            return tax;
        }
        set
        {
            tax = value;
        }
    }
    public string Fx
    {
        get
        {
            return fx;
        }
        set
        {
            fx = value;
        }
    }
    public string Fuel
    {
        get
        {
            return fuel;
        }
        set
        {
            fuel = value;
        }
    }
    public string TotalAmount
    {
        get
        {
            return totalAmount;
        }
        set
        {
            totalAmount = value;
        }
    }
    public string PrepaidToken
    {
        get
        {
            return token;
        }
        set
        {
            token = value;
        }
    }
}
