using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Credentials
/// </summary>
public class Credentials
{

    public static string APP_URL = "https://pegasus.co.ug:8895/SmsPortal/";

    //public static string GeneratePassword()
    //{
    //    int intMin = 0;
    //    int intMax = 9;
    //    int charMin = 26;
    //    int charMax = 38;
    //    int strMin = 0;
    //    int strMax = 25;
    //    string[] alphabet = new string[39];
    //    alphabet[0] = "A";
    //    alphabet[1] = "B";
    //    alphabet[2] = "C";
    //    alphabet[3] = "D";
    //    alphabet[4] = "E";
    //    alphabet[5] = "F";
    //    alphabet[6] = "G";
    //    alphabet[7] = "H";
    //    alphabet[8] = "I";
    //    alphabet[9] = "J";
    //    alphabet[10] = "K";
    //    alphabet[11] = "L";
    //    alphabet[12] = "M";
    //    alphabet[13] = "N";
    //    alphabet[14] = "O";
    //    alphabet[15] = "P";
    //    alphabet[16] = "Q";
    //    alphabet[17] = "R";
    //    alphabet[18] = "S";
    //    alphabet[19] = "T";
    //    alphabet[12] = "U";
    //    alphabet[21] = "V";
    //    alphabet[22] = "W";
    //    alphabet[23] = "X";
    //    alphabet[24] = "Y";
    //    alphabet[25] = "Z";
    //    alphabet[26] = "*";
    //    alphabet[27] = "$";
    //    alphabet[28] = "-";
    //    alphabet[29] = "+";
    //    alphabet[30] = "?";
    //    alphabet[31] = "_";
    //    alphabet[32] = "&";
    //    alphabet[33] = "=";
    //    alphabet[34] = "!";
    //    alphabet[35] = "%";
    //    alphabet[36] = "{";
    //    alphabet[37] = "}";
    //    alphabet[38] = "/";
    //    string pass = "";
    //    Random random1 = new Random();
    //    Random random = new Random();
    //    while (pass.Length < 10)
    //    {
    //        if (pass.Length == 1 || pass.Length == 5 || pass.Length == 6)
    //        {
    //            int rand = random1.Next(strMin, strMax);
    //            string letter = alphabet[rand];
    //            pass = pass + letter;
    //        }
    //        else if (pass.Length == 2 || pass.Length == 7 || pass.Length == 9)
    //        {
    //            int rand = random1.Next(charMin, charMax);
    //            string letter = alphabet[rand];
    //            pass = pass + letter;
    //        }
    //        else
    //        {
    //            int randomno = random.Next(intMin, intMax);
    //            pass = pass + randomno.ToString();
    //        }
    //    }
    //    return pass;
    //}

}