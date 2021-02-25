using System;
using System.Collections.Generic;
using System.Text;

namespace UtilReqSender.EntityObjects
{
    public class UmemeTransaction : Transaction
    {
        private string paymentType, customerType;

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
    }
}
