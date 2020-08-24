using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveVasUtilityTranProcessorLibrary.EntityObjects
{
    public class Customer
    {
        private string statusDescription, statusCode, customerRef, customerName, customerType, tin, balance;

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

        public string TIN
        {
            get
            {
                return tin;
            }
            set
            {
                tin = value;
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
        public string CustomerRef
        {
            get
            {
                return customerRef;
            }
            set
            {
                customerRef = value;
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
        public string Balance
        {
            get
            {
                return balance;
            }
            set
            {
                balance = value;
            }
        }
    }
}
