using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;


public class PhoneValidator
{
    private string _okNumber = "";
    private ArrayList _validNumbers, _invalidNumbers;
    private DbAccess _db;

    public bool PhoneNumbersOk(string numbers)
    {
        var phonesok = false;
        if (!numbers.Trim().Equals(""))
        {
            string[] stringSeparators = new string[] { ",", "\r\n" };
            string[] phones = numbers.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            _validNumbers = new ArrayList();
            _invalidNumbers = new ArrayList();
            foreach (string number in phones)
            {
                if (!number.Trim().Equals(""))
                {
                    if (NumberFormatIsValid(number.Trim()))
                    {
                        //Console.WriteLine(number.Trim() + "'s format is ok");
                        if (!NumberContainsLetters(_okNumber.Trim()))
                        {
                            if (NumberFormatIsValid(_okNumber))
                            {
                                _validNumbers.Add(_okNumber.Trim());

                            }
                            else
                            {
                                _invalidNumbers.Add(number.Trim());
                            }
                        }
                        else
                        {
                            //Console.WriteLine(okNumber + " Contains Letters");
                            _invalidNumbers.Add(number.Trim());
                        }
                    }
                    else
                    {
                        //Console.WriteLine(number + " has an invalid number format");
                        _invalidNumbers.Add(number.Trim());
                    }
                    if (_invalidNumbers.Count > 0)
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
            _db = new DbAccess();
            ArrayList blacklistedNumbers = _db.GetBlackListedNumbers();
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
        return _invalidNumbers;
    }
    public ArrayList GetValidNumbers()
    {
        return _validNumbers;
    }

    private bool NumberContainsLetters(string number)
    {
        bool containsLetters = false;
        var digits = new ArrayList();
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

    public bool NumberFormatIsValid(string number)
    {
        bool isValid = false;
        _okNumber = "";
        if (number.Trim().StartsWith("000256") && number.Length == 15)
        {
            _okNumber = number.Remove(0, 6);
            isValid = true;
        }
        else if (number.Trim().StartsWith("00256") && number.Length == 14)
        {
            _okNumber = number.Remove(0, 5);
            isValid = true;
        }
        else if ((number.Trim().StartsWith("256") && number.Length == 12))
        {
            _okNumber = number.Remove(0, 3);
            isValid = true;
        }
        else if ((number.Trim().StartsWith("0") && number.Length == 10))
        {
            _okNumber = number.Remove(0, 1);
            isValid = true;
        }
        else if ((number.Trim().StartsWith("7") && number.Length == 9))
        {
            _okNumber = number;
            isValid = true;
        }
        else if ((number.Trim().StartsWith("+") && number.Length == 13))
        {
            _okNumber = number.Remove(0, 4);
            isValid = true;
        }
        else
        {
            _okNumber = number;
            isValid = false;
        }
        return isValid;
    }
    public string Format(string number)
    {
        _okNumber = "";
        if (number.Trim().StartsWith("000256") && number.Length == 15)
        {
            _okNumber = number.Remove(0, 3);
        }
        else if (number.Trim().StartsWith("00256") && number.Length == 14)
        {
            _okNumber = number.Remove(0, 2);
        }
        else if ((number.Trim().StartsWith("256") && number.Length == 12))
        {
            _okNumber = number;
        }
        else if ((number.Trim().StartsWith("0") && number.Length == 10))
        {
            _okNumber = number.Remove(0, 1);
            _okNumber = "256" + _okNumber;
        }
        else if ((number.Trim().StartsWith("7") && number.Length == 9))
        {
            _okNumber = "256" + number;
        }
        else if ((number.Trim().StartsWith("+") && number.Length == 13))
        {
            _okNumber = number.Remove(0, 1);
        }
        else
        {
            _okNumber = number;
        }
        return _okNumber;
    }

}

