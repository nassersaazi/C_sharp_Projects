using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BrightLifeResponse
/// </summary>
public class BrightLifeResponse:Response
{
    public BrightLifeResponse()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private string client_id,customerReference, client_name, client_surname,fullname, client_registration_date, client_termination_date, client_planned_termination_date, client_birthdate, client_device, client_next_payment_due;
    public string Client_id
    {
        get
        {
            return client_id;
        }
        set
        {
            client_id = value;
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
    public string Client_name
    {
        get
        {
            return client_name;
        }
        set
        {
            client_name = value;
        }
    }

    public string Client_surname
    {
        get
        {
            return client_surname;
        }
        set
        {
            client_surname = value;
        }
    }
    public string Fullname
    {
        get
        {
            return fullname;
        }
        set
        {
            fullname = value;
        }
    }

    public string Client_registration_date
    {
        get
        {
            return client_registration_date;
        }
        set
        {
            client_registration_date = value;
        }
    }

    public string Client_termination_date
    {
        get
        {
            return client_termination_date;
        }
        set
        {
            client_termination_date = value;
        }
    }

    public string Client_planned_termination_date
    {
        get
        {
            return client_planned_termination_date;
        }
        set
        {
            client_planned_termination_date = value;
        }
    }

    public string Client_birthdate
    {
        get
        {
            return client_birthdate;
        }
        set
        {
            client_birthdate = value;
        }
    }

    public string Client_device
    {
        get
        {
            return client_device;
        }
        set
        {
            client_device = value;
        }
    }

    public string Client_next_payment_due
    {
        get
        {
            return client_next_payment_due;
        }
        set
        {
            client_next_payment_due = value;
        }
    }
}