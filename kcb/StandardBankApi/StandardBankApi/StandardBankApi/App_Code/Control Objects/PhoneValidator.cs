using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

/// <summary>
/// Summary description for PhoneValidator
/// </summary>
public class PhoneValidator
{
    private string okNumber = "";
    private ArrayList validNumbers, invalidNumbers;
    public bool PhoneNumbersOk(string numbers)
    {
        bool phonesok = false;
        if (!numbers.Trim().Equals(""))
        {
            string[] stringSeparators = new string[] { ",", "\r\n" };
            string[] phones = numbers.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            validNumbers = new ArrayList();
            invalidNumbers = new ArrayList();
            foreach (string number in phones)
            {
                if (!number.Trim().Equals(""))
                {
                    if (NumberFormatIsValid(number.Trim()))
                    {
                        //Console.WriteLine(number.Trim() + "'s format is ok");
                        if (!NumberContainsLetters(okNumber.Trim()))
                        {
                            if (NetworkCodeOk(okNumber))
                            {
                                if (!NumberIsBlacklisted(okNumber))
                                {
                                    //Console.WriteLine(okNumber + " IS OK");
                                    validNumbers.Add(okNumber.Trim());
                                }
                                else
                                {
                                    invalidNumbers.Add(number.Trim());
                                }
                            }

                            else
                            {
                                invalidNumbers.Add(number.Trim());
                            }
                        }
                        else
                        {
                            //Console.WriteLine(okNumber + " Contains Letters");
                            invalidNumbers.Add(number.Trim());
                        }
                    }
                    else
                    {
                        //Console.WriteLine(number + " has an invalid number format");
                        invalidNumbers.Add(number.Trim());
                    }
                    if (invalidNumbers.Count > 0)
                    {
                        phonesok = false;
                    }
                    else
                    {
                        phonesok = true;
                    }
                }
            }
        }
        else
        {
            phonesok = true;
        }
        return phonesok;
    }

    private bool NumberIsBlacklisted(string okNumber)
    {
        bool blacklisted = false;
        try
        {
            DatabaseHandler dp = new DatabaseHandler();
            ArrayList blacklistedNumbers = dp.GetBlackListedNumbers();
            if (blacklistedNumbers.Contains(okNumber))
            {
                blacklisted = true;
            }
            else
            {
                blacklisted = false;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return blacklisted;
    }
    public ArrayList GetInvalidNumbers()
    {
        return invalidNumbers;
    }
    public ArrayList GetValidNumbers()
    {
        return validNumbers;
    }

    private bool NumberContainsLetters(string number)
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
        //else if ((number.Trim().StartsWith("256") && number.Length == 12))
        //{
        //    okNumber = number.Remove(0, 3);
        //    isValid = true;
        //}
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
            if (number.Length == 12)
            {
                okNumber = number.Remove(0, 3);
                string code = number.Substring(0, 3);
                isValid = GetAllowedCountryCodes(code);
                //isValid = true;
            }
            else
            {
                okNumber = number;
                isValid = false;
            }
        }
        return isValid;
    }
    private bool GetAllowedCountryCodes(string countryCode)
    {
        DatabaseHandler handler = new DatabaseHandler();
        bool isAllowed = false;
        try
        {
            DataTable table = handler.GetAllowedCountryCodes(countryCode);
            if (table.Rows.Count > 0)
            {

                isAllowed = true;
            }
        }
        catch (Exception ee)
        {

        }
        return isAllowed;
    }

    private bool NetworkCodeOk(string okNumber)
    {
        bool ok = false;
        string code = okNumber.Substring(0, 3);
        DatabaseHandler dp = new DatabaseHandler();
        Hashtable networkCodes;
        networkCodes = dp.GetNetworkCodes();
        ArrayList codes = new ArrayList(networkCodes.Keys);
        if (codes.Contains(code))
        {
            ok = true;
        }
        else
        {
            ok = false;
        }
        return ok;
    }
   
    public string FormatTelephone(string number)
    {
        okNumber = "";
        if (number.Trim().StartsWith("000256") && number.Length == 15)
        {
            okNumber = number.Remove(0, 6);
        }
        else if (number.Trim().StartsWith("00256") && number.Length == 14)
        {
            okNumber = number.Remove(0, 5);
        }
        else if ((number.Trim().StartsWith("256") && number.Length == 12))
        {
            okNumber = number.Remove(0, 3);
        }
        else if ((number.Trim().StartsWith("0") && number.Length == 10))
        {
            okNumber = number.Remove(0, 1);
        }
        else if ((number.Trim().StartsWith("7") && number.Length == 9))
        {
            okNumber = number;
        }
        else if ((number.Trim().StartsWith("+") && number.Length == 13))
        {
            okNumber = number.Remove(0, 4);
        }
        else
        {
            okNumber = number;
        }
        return okNumber;
    }
}
