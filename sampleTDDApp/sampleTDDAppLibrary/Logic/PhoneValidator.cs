using System;
using System.Collections;
using System.Data;

namespace sampleTDDAppLibrary.Logic
{
    public class PhoneValidator
    {
        DatabaseHandler dp = new DatabaseHandler();
        private string okNumber = "";
        public string NetworkCode;
        private ArrayList validNumbers, invalidNumbers;

        public bool PhoneNumbersOk(string numbers)
        {
            if (numbers.Trim().Equals(""))
            {
                return false;
            }
            else
            {
                string[] stringSeparators = new string[] { ",", "\r\n" };
                string[] phones = numbers.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                validNumbers = new ArrayList();
                invalidNumbers = new ArrayList();
                foreach (string number in phones)
                {
                    if (number.Trim().Equals("") || (!NumberFormatIsValid(number.Trim())) || (NumberContainsLetters(okNumber.Trim())) || (NumberIsBlacklisted(okNumber)) || (!NetworkCodeOk(okNumber)))
                    {
                        invalidNumbers.Add(number.Trim());
                    }
                    else
                    {
                        validNumbers.Add(number.Trim());
                    }
                }
                return (invalidNumbers.Count > 0) ? false : true;
            }
        }

        private bool NumberIsBlacklisted(string okNumber)
        {
            try
            {
                ArrayList blacklistedNumbers = dp.GetBlackListedNumbers();
                return blacklistedNumbers.Contains(okNumber) ? true : false ;
                
            }
            catch (Exception ex)
            {   //Log exception
                return true ;
            }
            
        }

        public ArrayList GetInvalidNumbers()
        {
            return invalidNumbers;
        }
        public ArrayList GetValidNumbers()
        {
            return validNumbers;
        }

        public bool NumberContainsLetters(string number)
        {
            bool containsLetters = false;
            ArrayList digits = new ArrayList();
            digits.Add('0');
            digits.Add('1');
            digits.Add('2');
            digits.Add('3');
            digits.Add('4');
            digits.Add('5');
            digits.Add('6');
            digits.Add('7');
            digits.Add('8');
            digits.Add('9');
            char[] chars = number.ToCharArray();
            foreach (char c in chars)
            {
                if (!digits.Contains(c))
                {
                    containsLetters = true;
                    break;
                }
            }
            return containsLetters;

        }

        private bool NumberFormatIsValid(string number)
        {
            bool isValid = false;
            okNumber = "";
            if (number.Trim().StartsWith("000256") && number.Length == 15)
            {
                okNumber = number.Remove(0, 6);
                isValid = true;
            }
            else if (number.Trim().StartsWith("00256") && number.Length == 14)
            {
                okNumber = number.Remove(0, 5);
                isValid = true;
            }
            else if ((number.Trim().StartsWith("256") && number.Length == 12))
            {
                okNumber = number.Remove(0, 3);
                isValid = true;
            }
            else if ((number.Trim().StartsWith("0") && number.Length == 10))
            {
                okNumber = number.Remove(0, 1);
                isValid = true;
            }
            else if ((number.Trim().StartsWith("7") && number.Length == 9))
            {
                okNumber = number;
                isValid = true;
            }
            else if ((number.Trim().StartsWith("+") && number.Length == 13))
            {
                okNumber = number.Remove(0, 4);
                isValid = true;
            }
            else
            {
                okNumber = number;
                isValid = false;
            }
            return isValid;
        }
        private bool NetworkCodeOk(string okNumber)
        {
            bool ok = false;
            string code = okNumber.Substring(0, 3);
            Hashtable networkCodes;
            networkCodes = dp.GetNetworkCodes();
            ArrayList codes = new ArrayList(networkCodes.Keys);
            if (codes.Contains(code))
            {
                NetworkCode = networkCodes[code].ToString();
                ok = true;
            }
            else
            {
                ok = false;
            }
            return ok;
        }
        
        
        
    }
}