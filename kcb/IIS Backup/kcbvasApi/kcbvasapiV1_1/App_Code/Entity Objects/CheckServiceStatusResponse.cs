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
/// Summary description for CheckServiceStatusResponse
/// </summary>
public class CheckServiceStatusResponse
{
    private Status status;

    public Status Status
    {
        get { return status; }
        set { status = value; }
    }

    private CheckServiceStatusResponsePayLoad payload;

    public CheckServiceStatusResponsePayLoad Payload
    {
        get { return payload; }
        set { payload = value; }
    }

    public class CheckServiceStatusResponsePayLoad 
    {
        private string statusCode;

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }
        private string reason;

        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }
        private string serviceType;

        public string ServiceType
        {
            get { return serviceType; }
            set { serviceType = value; }
        }
        private string serviceID;

        public string ServiceID
        {
            get { return serviceID; }
            set { serviceID = value; }
        }
        private string countryID;

        public string CountryID
        {
            get { return countryID; }
            set { countryID = value; }
        }
        private string networkID;

        public string NetworkID
        {
            get { return networkID; }
            set { networkID = value; }
        }
        private string lastTimeUp;

        public string LastTimeUp
        {
            get { return lastTimeUp; }
            set { lastTimeUp = value; }
        }
        private string lastWentoff;

        public string LastWentoff
        {
            get { return lastWentoff; }
            set { lastWentoff = value; }
        }

        public CheckServiceStatusResponsePayLoad() 
        {
            this.statusCode = "";
            this.reason = "";
            this.serviceType = "";
            this.serviceID = "";
            this.countryID = "";
            this.networkID = "";
            this.lastTimeUp = "";
            this.lastWentoff = "";
        }
    }


    public CheckServiceStatusResponse()
    {
        this.status = new Status();
        this.payload = new CheckServiceStatusResponsePayLoad();
        
    }

    public override string ToString()
    {
        string whatToLog = "";
        return whatToLog;
    }
}
