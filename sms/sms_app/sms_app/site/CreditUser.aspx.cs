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
using Encryption;

public partial class CreditUser : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    DataTable data_table = new DataTable();
    DataTable d_table = new DataTable();
    private readonly DbAccess _db = new DbAccess();
    private readonly Processfile _processFile = new Processfile();
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
                LoadUsers();
               
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadUsers()
    {
        string area_code = Session["VendorCode"].ToString();
        d_table = data_file.GetUsersByArea(area_code);
        ddlUsers.DataSource = d_table;
        ddlUsers.DataValueField = "Username";
        ddlUsers.DataTextField = "FullName";
        ddlUsers.DataBind();
        ddlUsers.Items.Insert(0, new ListItem(" All users ", "0"));
    }



    protected void Button1_Click(object sender, EventArgs e)
    {
        string User = ddlUsers.SelectedValue;
        
        string credit = txtCredit.Text.Trim();
        string CreditedBy = HttpContext.Current.Session["Username"].ToString();
        string VendorCode = HttpContext.Current.Session["VendorCode"].ToString();
        if (credit.Equals(""))
        {
            ShowMessage("Please Enter Credit to add", true);
            txtCredit.Focus();
        }
        else
        {
            string res = Process_file.AddCredit(User, credit, CreditedBy, VendorCode);
            if (res.Contains("SUCCESSFULLY"))
            {
                ddlUsers.SelectedValue = "0";
                txtCredit.Text = "";
                MultiView1.ActiveViewIndex = 0;
                ShowMessage(res, false);
                //LoadUsers();
            }
            else
            {
                ShowMessage(res, true);
            }

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

    protected void ddlusers_SelectedIndexChanged(object sender, EventArgs e)
    {
        string VendorCode = Session["VendorCode"].ToString();
        string user = ddlUsers.SelectedValue;
        int bal = Process_file.GetUserCredit(VendorCode, user);
        txtBal.Text = bal.ToString();
    }

   
}