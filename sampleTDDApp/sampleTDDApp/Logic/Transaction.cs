using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDApp.Logic
{
    public class Transaction
    {
        private string chequeNumber, narration, custRef, custName, customerTel, vendorTransactionRef,
            transactionType, vendorCode, password, teller, reversal, reversedTrans, offline, utilityCode,
            paymentDate, transactionAmount, digitalSignature, telephone, email, TransactionID, tin;

        public string Tin
        {
            get { return tin; }
            set { tin = value; }
        }
        public string CustomerType;
        public string PaymentType = "";
        public string Area = "";
        public string chargeType;
        private double pegpayCharge;

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
        public string transactionID
        {
            get
            {
                return TransactionID;
            }
            set
            {
                TransactionID = value;
            }
        }
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }
        public string Telephone
        {
            get
            {
                return telephone;
            }
            set
            {
                telephone = value;
            }
        }
        public string Offline
        {
            get
            {
                return offline;
            }
            set
            {
                offline = value;
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
        public string ChequeNumber
        {
            get
            {
                return chequeNumber;
            }
            set
            {
                chequeNumber = value;
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
        public string DigitalSignature
        {
            get
            {
                return digitalSignature;
            }
            set
            {
                digitalSignature = value;
            }
        }
        public double PegpayCharge
        {
            get
            {
                return pegpayCharge; /// 0701047275 Irene
            }
            set
            {
                pegpayCharge = value;
            }
        }

        public string ChargeType
        {
            get
            {
                return chargeType; /// 0701047275 Irene
            }
            set
            {
                chargeType = value;
            }
        }
    }
}
