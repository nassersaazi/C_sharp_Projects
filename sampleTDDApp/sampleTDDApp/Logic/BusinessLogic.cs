using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Encryption;
using System.Threading.Tasks;

namespace sampleTDDApp.Logic
{
    public class BusinessLogic
    {
        public bool IsNumeric(string amount)
        {

            if (amount.Equals("0"))
            {
                return false;
            }
            else
            {
                double amt = double.Parse(amount);
                amount = amt.ToString();
                float Result;
                return float.TryParse(amount, out Result);
            }
        }

        public bool IsValidDate(string paymentDate)
        {
            DateTime date;
            string format = "dd/MM/yyyy";
            return DateTime.TryParseExact(paymentDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            
        }

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
