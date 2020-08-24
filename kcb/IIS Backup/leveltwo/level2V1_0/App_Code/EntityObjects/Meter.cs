using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Meter
/// </summary>
public class Meter : REAResponse
{

    private string sn, metertype, msno, krn, ti, ken, sgc, lastvending, lastfeevending, ordno,
                   currentmonthunits, currentmonthmoney, username, identityid, customertype,
                   firstname, lastname, address, telNo, fname,lname, creditAmount,fullName,energy;
    public Meter()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public string OrderNo
    {
        get
        {
            return ordno;
        }
        set
        {
            ordno = value;
        }
    }
    public string Energy
    {
        get
        {
            return energy;
        }
        set
        {
            energy = value;
        }
    }
    public string CurrentMonthUnits
    {
        get
        {
            return currentmonthunits;
        }
        set
        {
            currentmonthunits = value;
        }
    }
    
    public string Customertype
    {
        get
        {
            return customertype;
        }
        set
        {
            customertype = value;
        }
    }
    public string FullName
    {
        get
        {
            return fullName;
        }
        set
        {
            fullName = value;
        }
    }
    public string Lname
    {
        get
        {
            return lname;
        }
        set
        {
            lname = value;
        }
    }
    public string Fname
    {
        get
        {
            return fname;
        }
        set
        {
            fname = value;
        }
    }
    public string CreditAmount
    {
        get
        {
            return creditAmount;
        }
        set
        {
            creditAmount = value;
        }
    }

    public string TelNo
    {
        get
        {
            return telNo;
        }
        set
        {
            telNo = value;
        }
    }

    public string Lastfeevending
    {
        get
        {
            return lastfeevending;
        }
        set
        {
            lastfeevending = value;
        }
    }

    public string SN
    {
        get
        {
            return sn;
        }
        set
        {
            sn = value;
        }
    }

    public string FirstName
    {
        get
        {
            return firstname;
        }
        set
        {
            firstname = value;
        }
    }

    public string LastName
    {
        get
        {
            return lastname;
        }
        set
        {
            lastname = value;
        }
    }

    public string MeterType
    {
        get
        {
            return metertype;
        }
        set
        {
            metertype = value;
        }
    }

    public string Address
    {
        get
        {
            return address;
        }
        set
        {
            address = value;
        }
    }

    public string MeterNo
    {
        get
        {
            return msno;
        }
        set
        {
            msno = value;
        }
    }

    public string Currentmonthmoney
    {
        get
        {
            return currentmonthmoney;
        }
        set
        {
            currentmonthmoney = value;
        }
    }
}