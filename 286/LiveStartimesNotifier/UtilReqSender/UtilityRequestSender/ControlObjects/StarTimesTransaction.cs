using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilReqSender.ControlObjects
{
    public class StarTimesTransaction
    {
        private string custRef, custName, customerTel, vendorTransactionRef, transactionType, vendorCode, password, teller, reversal, reversedTrans, feeSpecified, utilityCode,
            paymentDate, transactionAmount, transactionID, bouquet, narration;

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

    
