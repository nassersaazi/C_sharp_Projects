using KCBNotifierLibrary.ControlObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBNotifierLibrary.EntityObjects
{
    public class Transaction
    {
        private string chequeNumber, narration, custRef, custName, customerTel, vendorTransactionRef,
                       transactionType, vendorCode, password, companyCode, utilityCompany, email, teller,
                       reversal, paymentType, offline, custType, area, DigitalSignature;
        private string paymentDate;
        private string transactionAmount, recordId;
        public string StatusCode = "";
        public string StatusDescription = "";
        public string TranIdToReverse = "";
        public string ChequeNumber = "";
        public string MerchantID = "";
        DateTime recordDate;

        public string digitalSignature
        {
            get
            {
                return DigitalSignature;
            }
            set
            {
                DigitalSignature = value;
            }
        }


        public DateTime RecordDate
        {
            get
            {
                return recordDate;
            }
            set
            {
                recordDate = value;
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

        public string CustType
        {
            get
            {
                return custType;
            }
            set
            {
                custType = value;
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
        public string UtilityCompany
        {
            get
            {
                return utilityCompany;
            }
            set
            {
                utilityCompany = value;
            }
        }
        public string CompanyCode
        {
            get
            {
                return companyCode;
            }
            set
            {
                companyCode = value;
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
        public string VendorTranId
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
        public string RecordId
        {
            get
            {
                return recordId;
            }
            set
            {
                recordId = value;
            }
        }


        internal bool PassesValidation()
        {
            DatabaseHandler dh = new DatabaseHandler();
            string utility = UtilityCompany.ToUpper();



            StatusCode = "0";
            StatusDescription = "SUCCESS";
            return true;
        }
    }
}
