using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using PegPayUtilityApi.PegPayApi;

/// <summary>
/// Summary description for Response
/// </summary>
public class Response
{
    private string responseField1, responseField2, responseField3, responseField4, responseField5, responseField6,
        responseField7, responseField8, responseField9, responseField10, responseField11, responseField12, responseField13, responseField14;


    private string responseField15, responseField16, responseField17, responseField18, responseField19, responseField20, responseField21, responseField22, responseField23, responseField24, responseField25, responseField26;
    
    private List<Transaction> tranList;

    public List<Transaction> TranList
    {
        get { return tranList; }
        set { tranList = value; }
    }
    public override string ToString()
    {
        string whatToLog = "responseField1(custRef)=" + responseField1 + Environment.NewLine +
                           "responseField2=" + responseField2 + Environment.NewLine +
                           "responseField3(Area/Tin)=" + responseField3 + Environment.NewLine +
                           "responseField4=" + responseField4 + Environment.NewLine +
                           "responseField5(CustType)=" + responseField5 + Environment.NewLine +
                           "responseField6(StatusCode)=" + responseField6 + Environment.NewLine +
                           "responseField7(StatusDesc))=" + responseField7 + Environment.NewLine +
                           "responseField8=" + responseField8 + Environment.NewLine +
                           "responseField9=" + responseField9 + Environment.NewLine +
                           "responseField10=" + responseField10 + Environment.NewLine +
                           "responseField11=" + responseField11 + Environment.NewLine +
                           "responseField12=" + responseField12 + Environment.NewLine +
                           "responseField13=" + responseField13 + Environment.NewLine;
        return whatToLog;
    }

    
    public string ResponseField1
    {
        get
        {
            return responseField1;
        }
        set
        {
            responseField1 = value;
        }
    }
    public string ResponseField2
    {
        get
        {
            return responseField2;
        }
        set
        {
            responseField2 = value;
        }
    }
    public string ResponseField3
    {
        get
        {
            return responseField3;
        }
        set
        {
            responseField3 = value;
        }
    }
    public string ResponseField4
    {
        get
        {
            return responseField4;
        }
        set
        {
            responseField4 = value;
        }
    }
    public string ResponseField5
    {
        get
        {
            return responseField5;
        }
        set
        {
            responseField5 = value;
        }
    }
    public string ResponseField6
    {
        get
        {
            return responseField6;
        }
        set
        {
            responseField6 = value;
        }
    }
    public string ResponseField7
    {
        get
        {
            return responseField7;
        }
        set
        {
            responseField7 = value;
        }
    }
    public string ResponseField8
    {
        get
        {
            return responseField8;
        }
        set
        {
            responseField8 = value;
        }
    }
    public string ResponseField9
    {
        get
        {
            return responseField9;
        }
        set
        {
            responseField9 = value;
        }
    }
    public string ResponseField10
    {
        get
        {
            return responseField10;
        }
        set
        {
            responseField10 = value;
        }
    }
    public string ResponseField11
    {
        get
        {
            return responseField11;
        }
        set
        {
            responseField11 = value;
        }
    }
    public string ResponseField12
    {
        get
        {
            return responseField12;
        }
        set
        {
            responseField12 = value;
        }
    }

    public string ResponseField14
    {
        get
        {
            return responseField14;
        }
        set
        {
            responseField14 = value;
        }
    }

    public string ResponseField13
    {
        get
        {
            return responseField13;
        }
        set
        {
            responseField13 = value;
        }
    }



    public string ResponseField15
    {
        get
        {
            return responseField15;
        }
        set
        {
            responseField15 = value;
        }
    }

    public string ResponseField16
    {
        get
        {
            return responseField16;
        }
        set
        {
            responseField16 = value;
        }
    }

    public string ResponseField17
    {
        get
        {
            return responseField17;
        }
        set
        {
            responseField17 = value;
        }
    }

    public string ResponseField18
    {
        get
        {
            return responseField18;
        }
        set
        {
            responseField18 = value;
        }
    }

    public string ResponseField19
    {
        get
        {
            return responseField19;
        }
        set
        {
            responseField19 = value;
        }
    }

    public string ResponseField20
    {
        get
        {
            return responseField20;
        }
        set
        {
            responseField20 = value;
        }
    }

    public string ResponseField21
    {
        get
        {
            return responseField21;
        }
        set
        {
            responseField21 = value;
        }
    }

    public string ResponseField22
    {
        get
        {
            return responseField22;
        }
        set
        {
            responseField22 = value;
        }
    }

    public string ResponseField23
    {
        get
        {
            return responseField23;
        }
        set
        {
            responseField23 = value;
        }
    }

    public string ResponseField24
    {
        get
        {
            return responseField24;
        }
        set
        {
            responseField24 = value;
        }
    }

    public string ResponseField25
    {
        get
        {
            return responseField25;
        }
        set
        {
            responseField25 = value;
        }
    }

    public string ResponseField26
    {
        get
        {
            return responseField26;
        }
        set
        {
            responseField26 = value;
        }
    }
}
