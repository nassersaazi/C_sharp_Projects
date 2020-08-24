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
/// Summary description for QueryTransactionStatusRequest
/// </summary>
public class QueryTransactionStatusRequest:Request
{
    private Credentials credentials;

    public Credentials Credentials
    {
        get { return credentials; }
        set { credentials = value; }
    }

    private QueryTransactionStatusRequestPayload payload;

    public QueryTransactionStatusRequestPayload PayLoad
    {
        get { return payload; }
        set { payload = value; }
    }

    public QueryTransactionStatusRequest()
    {
        this.credentials = new Credentials();
        this.payload = new QueryTransactionStatusRequestPayload();

    }

    public class QueryTransactionStatusRequestPayload
    {
        private string c360UniqueId;
        private string referenceID;
        private string merchantID;

        public string MerchantID
        {
            get { return merchantID; }
            set { merchantID = value; }
        }

        public string ReferenceID
        {
            get { return referenceID; }
            set { referenceID = value; }
        }

        public string C360UniqueId
        {
            get { return c360UniqueId; }
            set { c360UniqueId = value; }
        }


        public QueryTransactionStatusRequestPayload()
        {
            this.c360UniqueId = "";
            this.merchantID = "";
            this.referenceID = "";
        }

    }

    public override bool IsValidRequest()
    {
        
        if (string.IsNullOrEmpty(this.payload.MerchantID)) 
        {
            this.PegPayStatusCode = "100";
            this.PegPayStatusDescription = "PLEASE SUPPLY A VALID MERCHANT ID";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.ReferenceID))
        {
            this.PegPayStatusCode = "100";
            this.PegPayStatusDescription = "PLEASE SUPPLY A VALID REFERENCE ID";
            return false;
        }
        return true;
    }
    private string getIp()
    {
        string _custIP = null;
        try
        {

            _custIP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];

            if (String.IsNullOrEmpty(_custIP))
            {
                _custIP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            }
            if (String.IsNullOrEmpty(_custIP))
            {
                _custIP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }
        catch (Exception ex)
        {
            _custIP = "";
        }
        return _custIP;

    }
}
