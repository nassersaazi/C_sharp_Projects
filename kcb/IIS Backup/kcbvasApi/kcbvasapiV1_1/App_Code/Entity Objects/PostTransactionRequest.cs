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
/// Summary description for PostTransactionRequest
/// </summary>
public class PostTransactionRequest : Request
{
    private Credentials credentials;
    private PostTransactionRequestPayLoad payload;

    public Credentials Credentials
    {
        get { return credentials; }
        set { credentials = value; }
    }

    public PostTransactionRequestPayLoad PayLoad
    {
        get { return payload; }
        set { payload = value; }
    }

    public PostTransactionRequest()
    {
        this.credentials = new Credentials();
        this.payload = new PostTransactionRequestPayLoad();
    }

    public class PostTransactionRequestPayLoad
    {
        private string mSISDN;
        private string currency;
        private string amount;
        private string referenceID;
        private string merchantID;
        private string narration;
        private string beneficiaryName;
        private string beneficiaryID;
        private string area;
        internal string UtilityCode;
        private string addendum;

        public string Area
        {
            get { return area; }
            set { area = value; }
        }

        public string BeneficiaryID
        {
            get { return beneficiaryID; }
            set { beneficiaryID = value; }
        }

        public string BeneficiaryName
        {
            get { return beneficiaryName; }
            set { beneficiaryName = value; }
        }


        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }


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


        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }


        public string Currency
        {
            get { return currency; }
            set { currency = value; }
        }


        public string MSISDN
        {
            get { return mSISDN; }
            set { mSISDN = value; }
        }

        public string Addendum
        {
            get { return addendum; }
            set { addendum = value; }
        }


        public PostTransactionRequestPayLoad()
        {
            this.area = "";
            this.amount = "";
            this.beneficiaryID = "";
            this.beneficiaryName = "";
            this.currency = "";
            this.merchantID = "";
            this.mSISDN = "";
            this.narration = "";
            this.referenceID = "";
            this.addendum = "";
        }
    }


    public override bool IsValidRequest()
    {
        BussinessLogic bll = new BussinessLogic();
        string SourceIp = getIp();
        if (string.IsNullOrEmpty(this.payload.MerchantID))
        {
            PegPayStatusCode = "111";
            PegPayStatusDescription = "PLEASE SUPPLY A MERCHANT ID";
            return false;
        }
        //else if (!SourceIp.Equals("196.8.208.117"))
        //{
        //    this.PegPayStatusCode = "111";
        //    this.PegPayStatusDescription = "REQUEST HAS BEEN REJECTED";
        //    return false;
        //}
        else if (string.IsNullOrEmpty(this.payload.Amount))
        {
            PegPayStatusCode = "109";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID TRANSACTION AMOUNT";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.BeneficiaryID))
        {
            PegPayStatusCode = "101";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID BENEFICIARY ID";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.BeneficiaryName))
        {
            PegPayStatusCode = "101";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID BENEFICIARY NAME";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.Currency))
        {
            PegPayStatusCode = "102";
            PegPayStatusDescription = "PLEASE SUPPLY A CURRENCY e.g UGX";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.MSISDN))
        {
            PegPayStatusCode = "108";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID MSISDN e.g 256etc";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.Narration))
        {
            PegPayStatusCode = "101";
            PegPayStatusDescription = "PLEASE SUPPLY A NARRATION";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.ReferenceID))
        {
            PegPayStatusCode = "101";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID REFERENCE ID";
            return false;
        }
        else if (string.IsNullOrEmpty(this.payload.ReferenceID))
        {
            PegPayStatusCode = "101";
            PegPayStatusDescription = "PLEASE SUPPLY A VALID REFERENCE ID";
            return false;
        }
        else if (bll.IsDuplicateVendorRef(this.payload.ReferenceID))
        {
            PegPayStatusCode = "110";
            PegPayStatusDescription = "FAILED: DUPLICATE REFERENCE ID";
            return false;
        }
        else if (!IsValidMerchantID(this.payload.MerchantID))
        {
            PegPayStatusCode = "111";
            PegPayStatusDescription = "INVALID MERCHANT ID";
            return false;
        }
       
        else
        {
            PegPayStatusCode = "0";
            PegPayStatusDescription = "SUCCESS";
            return true;
        }
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

    private bool IsValidMerchantID(string p)
    {
        DatabaseHandler dh=new DatabaseHandler();
        this.payload.UtilityCode = dh.GetUtilityToBeAccessed(this.payload.MerchantID);
        if (this.payload.UtilityCode == "UNKNOWN")
        {
            return false;
        }
        else 
        {
            return true;
        }
    }
}
