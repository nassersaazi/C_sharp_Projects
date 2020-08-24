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
/// Summary description for ValidateCustomerRequest
/// </summary>
public class ValidateCustomerRequest : Request
{
    private Credentials credentials;
    private ValidateCustomerRequestPayload payload;


    public override string ToString()
    {
        string whatToLog = "ValidateCustomerRequest" + Environment.NewLine +
                           "Credentials.Username = " + credentials.Username + Environment.NewLine +
                           "Credentials.Password = " + credentials.Password + Environment.NewLine +
                           "CustRef = " + payload.CustomerRef + Environment.NewLine +
                           "Area = " + payload.Area + Environment.NewLine +
                           "ServiceCode = " + payload.ServiceCode + Environment.NewLine;
        return whatToLog;
    }

    public Credentials Credentials
    {
        get { return credentials; }
        set { credentials = value; }
    }

    public ValidateCustomerRequestPayload PayLoad
    {
        get { return payload; }
        set { payload = value; }
    }



    public ValidateCustomerRequest()
    {
        this.Credentials = new Credentials();
        this.PayLoad = new ValidateCustomerRequestPayload();
    }


    public class ValidateCustomerRequestPayload
    {
        private string customerRef;
        private string serviceCode;
        private string area;

        public string Area
        {
            get { return area; }
            set { area = value; }
        }

        public string ServiceCode
        {
            get { return serviceCode; }
            set { serviceCode = value; }
        }

        public string CustomerRef
        {
            get { return customerRef; }
            set { customerRef = value; }
        }


        public ValidateCustomerRequestPayload()
        {
            this.area = "";
            this.customerRef = "";
            this.serviceCode = "";
        }

    }

    public override bool IsValidRequest()
    {
        BussinessLogic bll=new BussinessLogic();
        if (!bll.IsValidCredentials(this.credentials)) 
        {
            this.PegPayStatusCode = "100";
            this.PegPayStatusDescription = "INVALID PEGPAY VENDOR CREDENTIALS";
            return false;
        }
        return true;
    }
}
