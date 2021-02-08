using System;
using System.Globalization;

namespace sampleTDDAppLibrary.Logic
{
    public class BusinessLogic
    {
        

        

        public string EncryptString(string ClearText)
        {
            string ret = "";
            ret = Encryption.encrypt.EncryptString(ClearText, "Umeme2501PegPay");
            return ret;
        }

        public string DecryptString(string Encrypted)
        {
            string ret = "";
            string pword = EncryptString("stan_counter");
            ret = Encryption.encrypt.DecryptString(Encrypted, "Umeme2501PegPay");
            return ret;
        }


    }
}