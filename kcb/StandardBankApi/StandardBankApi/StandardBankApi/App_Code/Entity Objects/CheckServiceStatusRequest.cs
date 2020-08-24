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
/// Summary description for CheckServiceStatusRequest
/// </summary>
public class CheckServiceStatusRequest:Request
{
    private Credentials credentials;
    private CheckServiceStatusRequestPayLoad payload;

    public override string ToString()
    {
        string whatToLog = "CheckServiceStatusRequest"+Environment.NewLine+
                           "Credentials.Username = "+credentials.Username+
                           "Credentials.Password = "+credentials.Password+
                           "Country ID = "+payload.CountryID+
                           "Network ID = "+payload.NetworkID+
                           "Service ID = "+payload.ServiceID;
        return whatToLog;
    }


    public Credentials Credentials
    {
        get { return credentials; }
        set { credentials = value; }
    }

    public CheckServiceStatusRequestPayLoad Payload
    {
        get { return payload; }
        set { payload = value; }
    }

    

    public CheckServiceStatusRequest()
    {
        credentials = new Credentials();
        payload = new CheckServiceStatusRequestPayLoad();
        
    }

    public class CheckServiceStatusRequestPayLoad
    {
        private string countryID;
        private string networkID;
        private string serviceID;
        public string ServiceID
        {
            get { return serviceID; }
            set { serviceID = value; }
        }

        public string NetworkID
        {
            get { return networkID; }
            set { networkID = value; }
        }


        public string CountryID
        {
            get { return countryID; }
            set { countryID = value; }
        }

        public CheckServiceStatusRequestPayLoad()
        {
            this.countryID = "";
            this.networkID = "";
            this.serviceID = "";
        }
    }

    public override bool IsValidRequest()
    {
        return true;
    }
}
