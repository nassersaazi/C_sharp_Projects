using System;
using System.Collections.Generic;
using System.Text;


    public class AirtimeResponseObj
    {
        private string responseId, statusCode, code, balance, statusDescription;

        public string StatusDescription
        {
            get { return statusDescription; }
            set { statusDescription = value; }
        }
        public string ResponseId
        {
            get
            {
                return responseId;
            }
            set
            {
                responseId = value;
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
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
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
