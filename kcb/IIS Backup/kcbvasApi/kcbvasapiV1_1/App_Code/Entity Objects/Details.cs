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
/// Summary description for Details
/// </summary>
public class Details
{
    private Status status;

    public Status Status
    {
        get { return status; }
        set { status = value; }
    }

    private Details.Results results;

    public Details.Results Resultz
    {
        get { return results; }
        set { results = value; }
    }


	public Details()
	{
        this.status = new Status();
        this.Resultz = new Details.Results();
	}

    public class Results
    {

        private string customerReference;

        public string CustomerReference
        {
            get { return customerReference; }
            set { customerReference = value; }
        }
        private string customerName;

        private string area;

        public string Area
        {
            get { return area; }
            set { area = value; }
        }

        private string statusCode;

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }
        private string statusDescription;

        public string StatusDescription
        {
            get { return statusDescription; }
            set { statusDescription = value; }
        }

        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; }
        }

        private string tin;

        public string Tin
        {
            get { return tin; }
            set { tin = value; }
        }

        private string customerType;

        public string CustomerType
        {
            get { return customerType; }
            set { customerType = value; }
        }

        private string outstandingBalance;

        public string OutstandingBalance
        {
            get { return outstandingBalance; }
            set { outstandingBalance = value; }
        }

        private string expiryDate;

        public string ExpiryDate
        {
            get
            {
                return expiryDate;
            }
            set
            {
                expiryDate = value;
            }
        }

        private string registrationDate;

        public string RegistrationDate
        {
            get { return registrationDate; }
            set { registrationDate = value; }
        }

        public Results()
        {
            this.area = "";
            this.customerName = "";
            this.customerReference = "";
            this.outstandingBalance = "";
            this.statusCode = "";
            this.statusDescription = "";
            this.customerType = "";
            this.tin = "";
        }

    }

}
