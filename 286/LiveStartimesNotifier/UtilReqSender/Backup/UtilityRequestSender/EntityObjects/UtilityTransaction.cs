using System;
using System.Data;
using System.Configuration;
using System.Web;


/// <summary>
/// Summary description for Transaction
/// </summary>
/// 
namespace UtilReqSender.EntityObjects
{
    public class UtilityTransaction
    {
        private string custRef, custName, customerTel, vendorTransactionRef, transactionType, vendorCode, password, teller, reversal, reversedTrans, feeSpecified, utilityCode,
            paymentDate, transactionAmount, transactionID, bouquet, narration, customerType, paymentType, status, statusDescription, transactionId, utilityRef
            ;
        int sentToUtilty, sentToVendor;

        public int SentToUtilty
        {
            get { return sentToUtilty; }
            set { sentToUtilty = value; }
        }


        public int SentToVendor
        {
            get { return sentToVendor; }
            set { sentToVendor = value; }
        }
        public string TransactionId
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        public string UtilityRef
        {
            get { return utilityRef; }
            set { utilityRef = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string StatusDescription
        {
            get { return statusDescription; }
            set { statusDescription = value; }
        }

        public string PaymentType
        {
            get { return paymentType; }
            set { paymentType = value; }
        }

        public string CustomerType
        {
            get { return customerType; }
            set { customerType = value; }
        }

        public string Bouquet
        {
            get { return bouquet; }
            set { bouquet = value; }
        }


        public string UtilityCode
        {
            get
            {
                return utilityCode;
            }
            set
            {
                utilityCode = value;
            }
        }


        public string FeeSpecified
        {
            get
            {
                return feeSpecified;
            }
            set
            {
                feeSpecified = value;
            }
        }
        public string CustRef
        {
            get
            {
                return custRef;
            }
            set
            {
                custRef = value;
            }
        }
        public string Reversal
        {
            get
            {
                return reversal;
            }
            set
            {
                reversal = value;
            }
        }
        public string TranIdToReverse
        {
            get
            {
                return reversedTrans;
            }
            set
            {
                reversedTrans = value;
            }
        }

        public string Teller
        {
            get
            {
                return teller;
            }
            set
            {
                teller = value;
            }
        }
        public string Narration
        {
            get
            {
                return narration;
            }
            set
            {
                narration = value;
            }
        }

        public string CustName
        {
            get
            {
                return custName;
            }
            set
            {
                custName = value;
            }
        }
        public string CustomerTel
        {
            get
            {
                return customerTel;
            }
            set
            {
                customerTel = value;
            }
        }
        public string VendorTransactionRef
        {
            get
            {
                return vendorTransactionRef;
            }
            set
            {
                vendorTransactionRef = value;
            }
        }
        public string TransactionType
        {
            get
            {
                return transactionType;
            }
            set
            {
                transactionType = value;
            }
        }
        public string VendorCode
        {
            get
            {
                return vendorCode;
            }
            set
            {
                vendorCode = value;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
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
        public string TransactionAmount
        {
            get
            {
                return transactionAmount;
            }
            set
            {
                transactionAmount = value;
            }
        }


    }
}