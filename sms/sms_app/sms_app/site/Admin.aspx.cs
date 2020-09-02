using System;
using System.Collections;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin : System.Web.UI.Page
{

    private readonly Processfile _processFile = new Processfile();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (IsPostBack) return;
            if ((Session["Username"] == null))
            {
                Response.Redirect("Default.aspx");
            }
            lbluserid.Text = Session["Username"].ToString();
            string VendorCode = Session["VendorCode"].ToString();
            string user = Session["Username"].ToString();
            var credit = _processFile.GetUserCredit(VendorCode, user);
           lblCredit.Text =  credit.ToString();
            
            AreaRole.Text = Session["UserRole"].ToString() + " - " + Session["VendorName"].ToString();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    private void ShowMessage(string Message, bool Error)
    {
        var lblmsg = (Label)Master.FindControl("lblmsg");
        if (Error)
        {
            lblmsg.ForeColor = Color.Red;
            lblmsg.Font.Bold = false;
        }
        else
        {
            lblmsg.ForeColor = Color.Green;
            lblmsg.Font.Bold = true;
        }

        if (Message == ".")
            lblmsg.Text = ".";
        else
            lblmsg.Text = "MESSAGE: " + Message.ToUpper();
    }

    private ArrayList GetNumbers(string nums)
    {
        var tels = new ArrayList();

        return tels;
    }


}
