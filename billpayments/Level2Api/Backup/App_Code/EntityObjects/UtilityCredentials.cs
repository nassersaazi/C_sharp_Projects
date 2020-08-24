using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for UtilityCredentials
/// </summary>
public class UtilityCredentials
{
    private string utilityCode, password, utility, bankCode, secretKey, key;

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
}
