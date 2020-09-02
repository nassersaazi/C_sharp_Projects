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

public partial class Sms_logs : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    PhoneValidator phone_validity = new PhoneValidator();
    DataTable data_table = new DataTable();
    DataTable d_table = new DataTable();
    DataTable dtable = new DataTable();
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
                LoadVendors();
                //ToggleControls();
                LoadLists();
                //LoadUsers();
               
                
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadUsers()
    {
        string vendor_code = ddlAreas.SelectedValue.ToString();
       // int area_id = int.Parse(area_code);
        d_table = data_file.GetUsersByArea(vendor_code);
        ddlUsers.DataSource = d_table;
        ddlUsers.DataValueField = "Username";
        ddlUsers.DataTextField = "FullName";
        ddlUsers.DataBind();
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
        if (role_code.Equals("003"))
        {
            string area = Session["VendorCode"].ToString();
            string Name = Session["VendorName"].ToString();
            ddlAreas.Items.Insert(0, new ListItem(Name, area));
            ddlUsers.Items.Insert(0, new ListItem(Session["FullName"].ToString(), Session["Username"].ToString()));
            ddlAreas.Enabled = false;
            ddlUsers.Enabled = false;
        }
        else
        {
            ddlAreas.Items.Insert(0, new ListItem(Session["VendorName"].ToString(), Session["VendorCode"].ToString()));
        
            LoadUsers();
            LoadLists();
            ddlAreas.Enabled = false;
        }
    }
    private void LoadLists()
    {
        string area_code = ddlAreas.SelectedValue.ToString();
        data_table = data_file.GetAllListsByArea(area_code);
        ddllists.DataSource = data_table;
        ddllists.DataValueField = "ListID";
        ddllists.DataTextField = "ListName";
        ddllists.DataBind();
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadSmslogs();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadSmslogs()
    {

        MultiView2.ActiveViewIndex = 0;
        string listID = ddllists.SelectedValue.ToString();
        string vendorCode = ddlAreas.SelectedValue.ToString();
        string user = ddlUsers.SelectedValue.ToString();
        string from = txtstartdate.Text.Trim();
        string end = txtenddate.Text.Trim();

        if (from.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
            txtstartdate.Focus();
        }
        else
        {
            data_table = Process_file.GetSmslogs(listID, vendorCode, user, from, end);
            DataGrid1.DataSource = data_table;
            DataGrid1.DataBind();
            ShowMessage(".",true);
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
        ddllists.Items.Insert(0, new ListItem(" All Groups ", "0"));
    }
    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnMessage")
            {
                string Listcode = e.Item.Cells[1].Text;
                string message = e.Item.Cells[6].Text;
                string mask = e.Item.Cells[7].Text;
                LoadMessage(message, mask);
            }
            else if (e.CommandName == "btnView")
            {
                string Listcode = e.Item.Cells[1].Text;
                loadNumber(Listcode);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void loadNumber(string list_code)
    {
        MultiView2.ActiveViewIndex = 2;
        int list_id = int.Parse(list_code);
        data_table = data_file.GetActiveListNumbers(list_id);
        DataGrid2.DataSource = data_table;
        DataGrid2.DataBind();
    }

    private void LoadMessage(string message, string mask)
    {
        MultiView2.ActiveViewIndex = 1;
        txtMessage.Text = message;
        txtMask.Text = mask;
    }

    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
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
                     
        }
        catch (Exception eX)
        {
            ShowMessage(eX.Message, true);
        }
    }
    protected void ddlAreas_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            LoadUsers();
            LoadLists();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void ddlUsers_DataBound(object sender, EventArgs e)
    {
        ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
    }
    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }
    protected void Button1_Click1(object sender, EventArgs e)
    {
        MultiView2.ActiveViewIndex = 0;
    }
    protected void DataGrid1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
