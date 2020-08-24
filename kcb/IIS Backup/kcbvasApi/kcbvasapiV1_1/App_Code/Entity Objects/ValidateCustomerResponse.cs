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
/// Summary description for ValidateCustomerResponse
/// </summary>
public class ValidateCustomerResponse
{
    Details details = new Details();

    public Details Details
    {
        get { return details; }
        set { details = value; }
    }
    
	public ValidateCustomerResponse()
	{
        this.details = new Details();
	}
}
