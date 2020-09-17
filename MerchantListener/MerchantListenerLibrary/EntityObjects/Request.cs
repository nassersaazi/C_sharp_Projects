using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantListenerLibrary.EntityObjects
{
    public class Request
    {
        private string username, password, channel, customertel, accountfrom, merchantcode, customerref, digitalsignature, requestid, transactionid, amount, statusCode, statusDesc, narration, transDate;

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public string CustomerTel
        {
            get { return customertel; }
            set { customertel = value; }
        }

        public string AccountFrom
        {
            get { return accountfrom; }
            set { accountfrom = value; }
        }

        public string MerchantCode
        {
            get { return merchantcode; }
            set { merchantcode = value; }
        }

        public string CustomerRef
        {
            get { return customerref; }
            set { customerref = value; }
        }

        public string DigitalSignature
        {
            get { return digitalsignature; }
            set { digitalsignature = value; }
        }

        public string RequestId
        {
            get { return requestid; }
            set { requestid = value; }
        }

        public string TransactionId
        {
            get { return transactionid; }
            set { transactionid = value; }
        }

        public string Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        public string TransactionDate
        {
            get { return transDate; }
            set { transDate = value; }
        }


        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string StatusDescription
        {
            get { return statusDesc; }
            set { statusDesc = value; }
        }

    }
}
