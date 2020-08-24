using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for REATransaction
/// </summary>
public class REATransaction : Transaction
{
    private string meterNo, utilityCompany;
    public REATransaction()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string MeterNo
    {
        get
        {
            return meterNo;
        }
        set
        {
            meterNo = value;
        }
    }

    public string UtilityCompany
    {
        get
        {
            return utilityCompany;
        }
        set
        {
            utilityCompany = value;
        }
    }

}