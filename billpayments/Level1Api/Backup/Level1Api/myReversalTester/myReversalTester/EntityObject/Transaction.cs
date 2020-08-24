using System;
using System.Collections.Generic;
using System.Text;

namespace myReversalTester.EntityObject
{
    public class Transaction
    {
        private string chequeNumber, narration, custRef, custName, customerTel, vendorTransactionRef, transactionType, vendorCode, password, teller, reversal, reversedTrans, offline, paymentDate, transactionAmount, digitalSignature, telephone, email, customerType, transNo, utilityTranRef, area, status;
        int tranId, sentTovendor;
        private string utilityCode, paymentType, bankID;

        public string BankID
        {
            get
            {
                return bankID;
            }
            set
            {
                bankID = value;
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

        public int SentTovendor
        {
            get
            {
                return sentTovendor;
            }
            set
            {
                sentTovendor = value;
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


        public string CustomerType
        {
            get
            {
                return customerType;
            }
            set
            {
                customerType = value;
            }
        }

        public int TranId
        {
            get
            {
                return tranId;
            }
            set
            {
                tranId = value;
            }
        }


        public string TransNo
        {
            get
            {
                return transNo;
            }
            set
            {
                transNo = value;
            }
        }


        public string UtilityTranRef
        {
            get
            {
                return utilityTranRef;
            }
            set
            {
                utilityTranRef = value;
            }
        }

        public string Area
        {
            get
            {
                return area;
            }
            set
            {
                area = value;
            }
        }

    }
}
