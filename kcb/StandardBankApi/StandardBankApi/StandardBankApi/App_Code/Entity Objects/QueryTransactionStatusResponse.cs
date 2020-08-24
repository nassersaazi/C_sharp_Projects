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
/// Summary description for QueryTransactionStatusResponse
/// </summary>
public class QueryTransactionStatusResponse
{
    private Status status;

    public Status Status
    {
        get { return status; }
        set { status = value; }
    }

    private QueryTransactionStatusResponsePayload payload;

    public QueryTransactionStatusResponsePayload PayLoad
    {
        get { return payload; }
        set { payload = value; }
    }

    public QueryTransactionStatusResponse()
    {
        this.payload = new QueryTransactionStatusResponsePayload();
        this.status = new Status();
    }

    public class QueryTransactionStatusResponsePayload
    {
        private string referenceID;

        public string ReferenceID
        {
            get { return referenceID; }
            set { referenceID = value; }
        }
        private string c360UniqueID;

        public string C360UniqueID
        {
            get { return c360UniqueID; }
            set { c360UniqueID = value; }
        }
        private string statusCode;

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public QueryTransactionStatusResponsePayload()
        {
            this.c360UniqueID = "";
            this.description = "";
            this.referenceID = "";
            this.statusCode = "";
        }


    }

}
