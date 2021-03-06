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
/// Summary description for KCCAQueryResponse
/// </summary>
public class KCCAQueryResponse : UmemeQueryResponse
{
    private string success, sessionKey, refCheck, errorCode, errorDescription, customerID, customerName, customerPhone,
           paymentReference, paymentType, paymentGroup, paymentDescription, paymentAmount, paymentCurrency, transactionID,
           paymentDate, trackingID, coin, status, prnDate, expiryDate, secretKey,key;


        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public string SecretKey
        {
            get
            {
                return secretKey;
            }
            set
            {
                secretKey = value;
            }
        }
        public string TrackingID
        {
            get
            {
                return trackingID;
            }
            set
            {
                trackingID = value;
            }
        }
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
        public string PrnDate
        {
            get
            {
                return prnDate;
            }
            set
            {
                prnDate = value;
            }
        }
        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }
        public string Coin
        {
            get
            {
                return coin;
            }
            set
            {
                coin = value;
            }
        }
        public string PaymentDate
        {
            get
            {
                return paymentDate;
            }
            set
            {
                paymentDate = value;
            }
        }
        public string TransactionID
        {
            get
            {
                return transactionID;
            }
            set
            {
                transactionID = value;
            }
        }
        public string PaymentCurrency
        {
            get
            {
                return paymentCurrency;
            }
            set
            {
                paymentCurrency = value;
            }
        }
        public string PaymentAmount
        {
            get
            {
                return paymentAmount;
            }
            set
            {
                paymentAmount = value;
            }
        }
        public string PaymentDescription
        {
            get
            {
                return paymentDescription;
            }
            set
            {
                paymentDescription = value;
            }
        }
        public string PaymentGroup
        {
            get
            {
                return paymentGroup;
            }
            set
            {
                paymentGroup = value;
            }
        }
        public string PaymentType
        {
            get
            {
                return paymentType;
            }
            set
            {
                paymentType = value;
            }
        }
        public string PaymentReference
        {
            get
            {
                return paymentReference;
            }
            set
            {
                paymentReference = value;
            }
        }
        public string CustomerPhone
        {
            get
            {
                return customerPhone;
            }
            set
            {
                customerPhone = value;
            }
        }
        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                customerName = value;
            }
        }
        //public string CustomerID
        //{
        //    get
        //    {
        //        return customerID;
        //    }
        //    set
        //    {
        //        customerID = value;
        //    }
        //}
        public string ErrorDescription
        {
            get
            {
                return errorDescription;
            }
            set
            {
                errorDescription = value;
            }
        }
        public string ErrorCode
        {
            get
            {
                return errorCode;
            }
            set
            {
                errorCode = value;
            }
        }
        public string RefCheck
        {
            get
            {
                return refCheck;
            }
            set
            {
                refCheck = value;
            }
        }
        public string SessionKey
        {
            get
            {
                return sessionKey;
            }
            set
            {
                sessionKey = value;
            }
        }
        public string Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
            }
        }

    }

