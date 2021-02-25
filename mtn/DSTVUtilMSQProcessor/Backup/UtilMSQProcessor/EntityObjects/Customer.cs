using System;
using System.Collections.Generic;
using System.Text;

namespace UtilMSQProcessor.EntityObjects
{
    public class Customer
    {
        private string statusDescription, statusCode, customerRef, customerName, customerType, balance;

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
