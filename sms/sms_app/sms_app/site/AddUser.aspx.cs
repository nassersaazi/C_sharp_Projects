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
public partial class AddUser : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile process_file = new Processfile();
    DataTable data_table = new DataTable();
    DataTable d_table = new DataTable();
    PhoneValidator phone_validity = new PhoneValidator();

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
                LoadAreas();
                LoadRoles();
                MultiView2.ActiveViewIndex = -1;
                if (Request.QueryString["transferid"] != null)
                {
                    string UserCode = Request.QueryString["transferid"].ToString(); 
                    LoadControls(UserCode);
                }
              
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    

    private void LoadControls(string user_code)
    {
        int user_id = int.Parse(user_code);
        data_table = data_file.GetUserDetails(user_id);
        if (data_table.Rows.Count > 0)
        {
            lblCode.Text = data_table.Rows[0]["UserId"].ToString();
            txtlname.Text = data_table.Rows[0]["FirstName"].ToString();
            txtfname.Text = data_table.Rows[0]["LastName"].ToString();
            txtphone.Text = data_table.Rows[0]["Phone"].ToString();
            txtemail.Text = data_table.Rows[0]["Email"].ToString();
            txtUserName.Text = data_table.Rows[0]["Username"].ToString();
            string area_code = data_table.Rows[0]["Vendor"].ToString();
            string type_code = data_table.Rows[0]["UserRole"].ToString();

            bool isactive = bool.Parse(data_table.Rows[0]["Active"].ToString());
            ddlAreas.SelectedIndex = ddlAreas.Items.IndexOf(ddlAreas.Items.FindByValue(area_code));
            ddlUserType.SelectedIndex = ddlUserType.Items.IndexOf(ddlUserType.Items.FindByValue(type_code));
            chkActive.Checked = isactive;

            MultiView2.ActiveViewIndex = 1;
        }
    }

    private void LoadRoles()
    {
        string Role = Session["RoleCode"].ToString();
        if (Role == "001")
        {
            d_table = data_file.GetSystemRoles();
            ddlUserType.DataSource = d_table;
            ddlUserType.DataValueField = "RoleCode";
            ddlUserType.DataTextField = "UserRole";
            ddlUserType.DataBind();
        }
        else
        {
            ddlUserType.Items.Insert(0, new ListItem("System User", "003"));
            ddlUserType.Items.Insert(0, new ListItem("Branch Administrator", "002"));
            ddlUserType.Items.Insert(0, new ListItem("Select Role", ""));
        }
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
    protected void ddlUserType_DataBound(object sender, EventArgs e)
    {
        ddlUserType.Items.Insert(0, new ListItem(" Select User Role ", "0"));
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            
            string uname = txtUserName.Text.Trim();
            string user_code = lblCode.Text.Trim();
            string fname = txtfname.Text.Trim();
            string lname = txtlname.Text.Trim();
            string phone = txtphone.Text.Trim();
            string email = txtemail.Text.Trim();
            string vendor_code = ddlAreas.SelectedValue.ToString();
            string Role_code = ddlUserType.SelectedValue.ToString();
            bool is_active = chkActive.Checked;
            bool reset = CheckBox1.Checked;


            string check_status = validate_input(fname, lname, phone, vendor_code, Role_code);

            if (!check_status.Equals("OK"))
            {
                ShowMessage(check_status, true);
            }
            else
            {
                //string username = "";
                //if (uname == "")
                //{
                //    username = process_file.GetUserName(fname, lname, uname);
                //    bool UsernameExists = process_file.UserNameExists(username);

                //    if (UsernameExists == true)
                //    {
                //        username = username + process_file.RandomNumber();
                //    }
                //}
                //else {
                //    username = uname;
                //}
                string res_save = process_file.SaveUser(user_code, email, fname, lname, phone, email, vendor_code, Role_code, is_active, reset);
               
                if (res_save.Contains("USERNAME EXISTS"))
                {
                    MultiView2.ActiveViewIndex = 0;
                    txtUserName.Focus();
                    ShowMessage(res_save, true);
                }
                else
                {
                    if (res_save.Contains("Successfully"))
                    {
                        ShowMessage(res_save, false);
                        Clear_contrls();
                    }
                    else
                    {
                        ShowMessage(res_save, true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void Clear_contrls()
    {
        lblCode.Text = "0";
        txtUserName.Text = "";
        txtphone.Text = "";
        txtlname.Text = "";
        txtfname.Text = "";
        txtemail.Text = "";
        ddlAreas.SelectedIndex = ddlAreas.Items.IndexOf(ddlAreas.Items.FindByValue("0"));
        ddlUserType.SelectedIndex = ddlUserType.Items.IndexOf(ddlUserType.Items.FindByValue("0"));
        MultiView2.ActiveViewIndex = -1;
    }

    private string validate_input(string fname, string lname, string phone, string vendor, string role)
    {
        string output = "";
        if (fname.Equals(""))
        {
            output = "First Name Required";
            txtfname.Focus();
        }
        else if (lname.Equals(""))
        {
            output = "Last Name Required";
            txtlname.Focus();
        }
        else if (phone.Equals(""))
        {
            output = "Mobile Phone Number Required";
            txtphone.Focus();
        }
        else if (!phone_validity.NumberFormatIsValid(phone))
        {
            output = "Enter Valid Mobile Phone Number Required";
            txtphone.Focus();
        }
        else if (vendor.Equals("0"))
        {
            output = "Select a vendor";
        }
        else if (role.Equals("0"))
        {
            output = "Select User Role";
        }
        else
        {
            output = "OK";
        }
        return output;
    }
    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" Select Vendor ", "0"));
    }
}
