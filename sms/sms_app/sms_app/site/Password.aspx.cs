using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Password : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    DataTable data_table = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (IsPostBack == false)
            {
                if ((Session["Username"] == null))
                {
                    Response.Redirect("Default.aspx");
                }
                MultiView1.ActiveViewIndex = 0;
                LinkButton MenuSms = (LinkButton)Master.FindControl("lblsmsPanel");
                LinkButton MenuReport = (LinkButton)Master.FindControl("lblReporting");
                LinkButton MenuProfile = (LinkButton)Master.FindControl("lblSetup");
                LinkButton MenuSettting = (LinkButton)Master.FindControl("lbtnSetting");
                MenuSms.Font.Italic = false;
                MenuReport.Font.Italic = false;
                MenuSettting.Font.Italic = false;
                MenuProfile.Font.Italic = true;
                string strProcessScript = "this.value='Working...';this.disabled=true;";
                Button1.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(Button1, "").ToString());
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    private void ShowMessage(string Message, bool Error)
    {
        Label lblmsg = (Label)Master.FindControl("lblmsg");
        if (Error) { lblmsg.ForeColor = System.Drawing.Color.Red; lblmsg.Font.Bold = false; }
        else { lblmsg.ForeColor = System.Drawing.Color.Green; lblmsg.Font.Bold = true; }
        if (Message == ".")
        {
            lblmsg.Text = ".";
        }
        else
        {
            lblmsg.Text = "MESSAGE: " + Message.ToUpper();
        }
    } 
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string oldPasswd = txtOldPasswd.Text.Trim();
            string newPasswd = txtNewPasswd.Text.Trim();
            string confirm = txtConfirm.Text.Trim();
            if (oldPasswd.Equals(""))
            {
                ShowMessage("Please Enter your Old Password", true);
                txtOldPasswd.Focus();
            }
            else if (newPasswd.Equals(""))
            {
                ShowMessage("Please Enter your New Password", true);
                txtNewPasswd.Focus(); 
            }
            else if (confirm.Equals(""))
            {
                ShowMessage("Please Confirm your New Password", true);
                txtConfirm.Focus(); 
            }
            else
            {
                string res_passwd = Process_file.Change_Passwd(oldPasswd, newPasswd, confirm);
                if (res_passwd.Contains("Successfully"))
                {
                    ShowMessage(res_passwd, false);
                }
                else
                {
                    ShowMessage(res_passwd, true);
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    } 
}
