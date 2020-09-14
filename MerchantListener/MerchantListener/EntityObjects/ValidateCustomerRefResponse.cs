using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantListener.EntityObjects
{
    class ValidateCustomerRefResponse
    {
        private string merchantName, merchantCode, customerName, oustandingBalance, minimumBalance, statusCode, statusDescription;

        private bool end, log;

        public string MerchantName
        {
            get
            {
                return merchantName;
            }
            set
            {
                merchantName = value;
            }
        }

        public string MerchantCode
        {
            get
            {
                return merchantCode;
            }
            set
            {
                merchantCode = value;
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
        public string OutstandingBalance
        {
            get
            {
                return oustandingBalance;
            }
            set
            {
                oustandingBalance = value;
            }
        }
        public string MinimumBalance
        {
            get
            {
                return minimumBalance;
            }
            set
            {
                minimumBalance = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }


        }

        public string StatusDescription
        {
            get
            {
                return statusDescription;
            }
            set
            {
                statusDescription = value;
            }


        }
    }
    
}
