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
/// Summary description for BETWAYTransaction
/// </summary>
public class BETWAYTransaction : UmemeTransaction
{

    private string sessionKey, key, secretKey, coin, branchCode;

    public string BranchCode
    {
        get
        {
            return branchCode;
        }
        set
        {
            branchCode = value;
        }
    }
    public string Coin
    {
        get
        {
            return coin;
        }
        set
        {
            coin = value;
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
    public string SessonKey
    {
        get
        {
            return sessionKey;
        }
        set
        {
            sessionKey = value;
        }
    }

}
