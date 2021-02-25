using System;
using System.Collections.Generic;
using System.Text;

namespace DSTVListener.EntityObjects
{
    public class ConfirmPaymentRequest : Request
    {
        public string BouquetCode;
        public string TraceUniqueID;
        public string ServiceId;
        public string VendorTranId;
        public string SenderID;
        public string CustomerRef;
        public string TransactionAmount;
        public string PaymentRef;
        public string ThirdPartyTransactionID;
        public string CustomerTel;
        public string CustName;
        public string TXNType;
        public string OpCoID;
        public string Utility;

        public bool IsValidRequest() 
        {
            StatusCode = "0";
            StatusDescription = "SUCCESS";
            return true;
        }
    }
}
