using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for REAResponse
/// </summary>
public class REAResponse
{

    private string errorCode, errorMsg, sessionID, user_ID, userName, latesAction, logintime, ip, cduName, token;
    private float cduAmount;

    public string ErrorCode
    {
        get
        {
            return errorCode;
        }
        set
        {
            errorCode = value;
        }
    }

    public string ErrorMsg
    {
        get
        {
            return errorMsg;
        }
        set
        {
            errorMsg = value;
        }
    }

    public string Ip
    {
        get
        {
            return ip;
        }
        set
        {
            ip = value;
        }
    }

    public string SessionID
    {
        get
        {
            return sessionID;
        }
        set
        {
            sessionID = value;
        }
    }

    public string Token
    {
        get
        {
            return token;
        }
        set
        {
            token = value;
        }
    }

    public string User_ID
    {
        get
        {
            return user_ID;
        }
        set
        {
            user_ID = value;
        }
    }

    public string UserName
    {
        get
        {
            return userName;
        }
        set
        {
            userName = value;
        }
    }

    public string LatesAction
    {
        get
        {
            return latesAction;
        }
        set
        {
            latesAction = value;
        }
    }

    public string Logintime
    {
        get
        {
            return logintime;
        }
        set
        {
            logintime = value;
        }
    }

    public string CduName
    {
        get
        {
            return cduName;
        }
        set
        {
            cduName = value;
        }
    }

    public float CduAmount
    {
        get
        {
            return cduAmount;
        }
        set
        {
            cduAmount = value;
        }
    }
}
