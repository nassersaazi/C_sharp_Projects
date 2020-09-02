using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class MasterMain : System.Web.UI.MasterPage
{
    Processfile process_file = new Processfile();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if ((Session["Username"] != null))
            {


                FullNames.Text = Session["FullName"].ToString();
                TogglePermissions();
            }
            else
            {
                Response.Redirect("Default.aspx", false);
            }
        }

        catch (NullReferenceException exe)
        {
            Response.Redirect("Default.aspx", false);

        }
        catch (Exception ex)
        {
            Response.Redirect("Default.aspx", false);
        }
    }

    private void TogglePermissions()
    {
        string role_code = Session["Username"].ToString();
        if (role_code.Equals("001"))
        {
            //lblsmsPanel.Visible = true;
            //lblReporting.Visible = true;
            //lblSetup.Visible = true;
            //lbtnSetting.Visible = true;
        }
        else
        {
            //lblsmsPanel.Visible = true;
            //lblReporting.Visible = true;
            //lblSetup.Visible = true;
            //lbtnSetting.Visible = false;
        }

        lblmsg.Text = "";
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        process_file.LogActivity("Logout", Session["Username"].ToString(), Session["VendorCode"].ToString(), "Logged out", "", process_file.GetLocalIPAddress().ToString());
        Session.Clear();
        Session.Abandon();
        Response.Redirect("./Default.aspx");
    }
}
