using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TotalCardValidationResponse
/// </summary>
public class TotalCardValidationResponse
{
    private string status_code, status_description, holder_name;

    public string StatusCode
    {
        get { return status_code; }
        set { status_code = value; }
    }
    public string StatusDescription
    {
        get { return status_description; }
        set { status_description = value; }
    }
    public string HolderName
    {
        get { return holder_name; }
        set { holder_name = value; }
    }
}