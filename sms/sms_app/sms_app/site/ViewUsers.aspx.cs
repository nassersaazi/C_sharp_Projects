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

public partial class ViewUsers : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    DataTable data_table = new DataTable();
    DataTable d_table = new DataTable();
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
                MultiView2.ActiveViewIndex = 0;
                LoadAreas();
                LoadRoles();
                string urole = Session["RoleCode"].ToString();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    private void LoadRoles()
    {
        d_table = data_file.GetSystemRoles();
        ddlUserType.DataSource = d_table;
        ddlUserType.DataValueField = "RoleCode";
        ddlUserType.DataTextField = "UserRole";
        ddlUserType.DataBind();
        ddlUserType.Items.Insert(0, new ListItem(" All Roles ", "0"));
    }
    private void LoadAreas()
    {
        string Role = Session["RoleCode"].ToString();
        if (Role == "001")
        {
            data_table = data_file.GetAreas();
            ddlAreas.DataSource = data_table;
            ddlAreas.DataValueField = "VendorCode";
            ddlAreas.DataTextField = "VendorName";
            ddlAreas.DataBind();
        }
        else {
            ddlAreas.Items.Insert(0, new ListItem(Session["VendorName"].ToString(), Session["VendorCode"].ToString()));
            ddlAreas.Enabled = false;
        }
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadUsers();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadUsers()
    {
        string vendor_code = ddlAreas.SelectedValue.ToString();
        string role_code = ddlUserType.SelectedValue.ToString();
        string name = txtSearch.Text.Trim();
        data_table = Process_file.GetUsers(vendor_code, role_code, name);
        if (data_table.Rows.Count > 0)
        {
            ShowMessage(".", true);
        }
        else
        {
            ShowMessage("No Results Returned", true);
        }
        DataGrid1.DataSource = data_table;
        DataGrid1.CurrentPageIndex = 0;
        DataGrid1.DataBind();
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
     protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnEdit")
            {

                string user_code = e.Item.Cells[0].Text; //encrypt.EncryptString(e.Item.Cells[0].Text, "25011Pegsms2322"); 
                Response.Redirect("./AddUser.aspx?transferid=" + user_code, false);                
            }
            else if (e.CommandName == "btnCredit")
            {
                string user_code = e.Item.Cells[0].Text;
                string username = e.Item.Cells[1].Text;
                string name = e.Item.Cells[5].Text;
                LoadCreditControl(user_code,username,name);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadCreditControl(string user_code, string username, string name)
    {
        MultiView2.ActiveViewIndex = 1;
        lblPhoneCode.Text = user_code;
        txtUserName.Text = username;
        txtName.Text = name;
        lblCredit.Text = GetCredit(username);
    }

    private string GetCredit(string username)
    {
        string response = "";
        try
        {
            //int money1 = 0;
            //DataTable dt = data_file.GetCurrentCredit(username);
            //if (dt.Rows.Count > 0)
            //{
            //    money1 = int.Parse(dt.Rows[0]["Credit"].ToString());
            //}
            //response = "CURRENT CREDIT IS: " + money1.ToString("#,##0");
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return response;
    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            string vendor_code = ddlAreas.SelectedValue.ToString();
            string user_role = ddlUserType.SelectedValue.ToString();
            string name = txtSearch.Text.Trim();
            data_table = Process_file.GetUsers(vendor_code, user_role, name);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataGrid1.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message,true);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        string username = txtUserName.Text.Trim();
        string name = txtName.Text.Trim();
        string credit = txtCredit.Text.Trim();
        string VendorCode = Session["VendorCode"].ToString();
        if (credit.Equals(""))
        {
            ShowMessage("Please Enter Credit to add", true);
            txtCredit.Focus();
        }
        else
        {
            string res = Process_file.AddCredit(username, credit, name, VendorCode);
            if (res.Contains("SUCCESSFULLY"))
            {
                MultiView2.ActiveViewIndex = 0;
                ShowMessage(res, false);
            }
            else
            {
                ShowMessage(res, true);
            }
            
        }
    }

    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }
}
