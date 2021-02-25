using System;
using System.Collections.Generic;
using System.Text;

namespace UtilReqSender.EntityObjects
{
    public class Credentials
    {
        private string utilityCode, password, utility, bankCode, secretKey, key;

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

        public string UtilityPassword
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
        public string Utility
        {
            get
            {
                return utility;
            }
            set
            {
                utility = value;
            }
        }
        public string BankCode
        {
            get
            {
                return bankCode;
            }
            set
            {
                bankCode = value;
            }
        }
        public string SecretKey
        {
            get
            {
                return secretKey;
            }
            set
            {
                secretKey = value;
            }
        }
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }
    }
}
