using System;
using System.Data;
using System.Configuration;
using System.Web;
using ConsoleApplication1.EntityObjects;

/// <summary>
/// Summary description for KCCATransaction
/// </summary>
public class SchoolsTransaction : UmemeTransaction
{
    private string utilityCode;

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
}
