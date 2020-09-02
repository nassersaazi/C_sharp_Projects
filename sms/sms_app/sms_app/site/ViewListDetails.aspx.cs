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

public partial class ViewListDetails : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    PhoneValidator phone_validity = new PhoneValidator();
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
                MultiView2.ActiveViewIndex = 0;
                //LoadLists();
                LoadVendors();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadVendors()
    {
        string role_code = Session["RoleCode"].ToString();
        if (role_code.Equals("001"))
        {
            data_table = data_file.GetAreas();
            ddlAreas.DataSource = data_table;
            ddlAreas.DataValueField = "VendorCode";
            ddlAreas.DataTextField = "VendorName";
            ddlAreas.DataBind();
        }
        else
        {
            ddlAreas.Items.Insert(0, new ListItem(Session["VendorName"].ToString(), Session["VendorCode"].ToString()));
            LoadLists();
            ddlAreas.Enabled = false;
        }
    }

    private void LoadLists()
    {
        data_table = Process_file.GetVendorLists(ddlAreas.Text);
        ddllists.DataSource = data_table;
        ddllists.DataValueField = "ListID";
        ddllists.DataTextField = "ListName";
        ddllists.DataBind();
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadListDetails();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadListDetails()
    {
        MultiView2.ActiveViewIndex = 0;
        string list_code = ddllists.SelectedValue.ToString();
        string phone = txtPhone.Text.Trim();
        string name = txtSearch.Text.Trim();
        string VendorCode = ddlAreas.SelectedValue; ;
            
            if (!phone.Equals(""))
            {
                if (phone_validity.PhoneNumbersOk(phone))
                {
                    data_table = Process_file.GetListContent(VendorCode, list_code, phone, name);
                    DataGrid1.DataSource = data_table;
                    DataGrid1.CurrentPageIndex = 0;
                    DataGrid1.DataBind();
                    ShowMessage(".", true);
                }
                else
                {
                    DataGrid1.DataSource = data_table;
                    DataGrid1.DataBind();
                    ShowMessage("Please Enter valid Phone Number", true);
                    txtPhone.Focus();
                }
            }
            else
            {
                data_table = Process_file.GetListContent(VendorCode, list_code, phone, name);
                DataGrid1.DataSource = data_table;
                DataGrid1.DataBind();
                ShowMessage(".", true);
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
    protected void ddllists_DataBound(object sender, EventArgs e)
    {
        ddllists.Items.Insert(0, new ListItem(" Select group ", "0"));
    }
    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnChange")
            {
                string phone_code = e.Item.Cells[0].Text;
                string status = e.Item.Cells[5].Text;
                Process_file.ChangePhoneStatus(phone_code, status);
                LoadListDetails();
            }
            else if (e.CommandName == "btnEdit")
            {
                string phone_code = e.Item.Cells[0].Text;
                string phone = e.Item.Cells[4].Text;
                string name = e.Item.Cells[5].Text;
                string status = e.Item.Cells[6].Text;
                LoadPhoneControl(phone_code, phone, name, status);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void ddlAreas_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            LoadLists();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }
    private void LoadPhoneControl(string phone_code, string phone, string name, string status)
    {
        MultiView2.ActiveViewIndex = 1;
        lblPhoneCode.Text = phone_code;
        txtName.Text = name;
        txtPhoneNumber.Text = phone;
        bool isactive = true;
        if (status.Equals("NO"))
        {
            isactive = false;
        }
        chkActive.Checked = isactive;
        txtPhoneNumber.Enabled = false;
    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            string list_code = ddllists.SelectedValue.ToString();
            string phone = txtPhone.Text.Trim();
            string name = txtSearch.Text.Trim();
            string VendorCode = "";
            if (Session["VendorCode"].ToString() == "001")
            {
                VendorCode = "";
            }
            else
            {
                VendorCode = Session["VendorCode"].ToString();
            }
            data_table = Process_file.GetListContent(VendorCode, list_code, phone, name);
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
        try
        {
            string phone_code = lblPhoneCode.Text.Trim();
            string phone = txtPhoneNumber.Text.Trim();
            string phone_name = txtName.Text.Trim();
            bool isactive = chkActive.Checked;
            Process_file.UpdatePhoneDetails(phone_code, phone, phone_name, isactive);
            ShowMessage("Phone Details Updated Successfully", false);
            LoadListDetails();
        }
        catch (Exception eX)
        {
            ShowMessage(eX.Message, true);
        }
    }
}
