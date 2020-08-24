using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

/// <summary>
/// Summary description for Response
/// </summary>
public class QueryResponse:Response
{
    private string customerReference, pegpayQueryId, customerName, customerBalance;

    private string lifeline, servicefee, payaccount, debtrecovery, receiptnumber, tokenvalue, inflation, fx, tax, fuel, noOfUnits, issuingEntity;


    private List<DataBundle> bundle;

    public List<DataBundle> Bundles
    {
        get { return bundle; }
        set { bundle = value; }
    }

    public string CustomerBalance
    {
        get { return customerBalance; }
        set { customerBalance = value; }
    }
    public string CustomerName
    {
        get { return customerName; }
        set { customerName = value; }
    }

    public string PegPayQueryId
    {
        get
        {
            return pegpayQueryId;
        }
        set
        {
            pegpayQueryId = value;
        }
    }
    public string CustomerReference
    {
        get
        {
            return customerReference;
        }
        set
        {
            customerReference = value;
        }
    }

    public string Lifeline
    {
        get
        {
            return lifeline;
        }
        set
        {
            lifeline = value;
        }
    }
    public string ServiceFee
    {
        get
        {
            return servicefee;
        }
        set
        {
            servicefee = value;
        }
    }

    public string PayAccount
    {
        get
        {
            return payaccount;
        }
        set
        {
            payaccount = value;
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
            return receiptnumber;
        }
        set
        {
            receiptnumber = value;
        }
    }
    public string TokenValue
    {
        get
        {
            return tokenvalue;
        }
        set
        {
            tokenvalue = value;
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
    public string Forex
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
    public string VAT
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
    public string NoOfUnits
    {
        get
        {
            return noOfUnits;
        }
        set
        {
            noOfUnits = value;
        }
    }

    public string IssuingEntity
    {
        get
        {
            return issuingEntity;
        }
        set
        {
            issuingEntity = value;
        }
    }
}
