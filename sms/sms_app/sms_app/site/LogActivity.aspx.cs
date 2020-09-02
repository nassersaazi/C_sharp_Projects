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

public partial class LogActivity : System.Web.UI.Page
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
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
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
            ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
        }
        else
        {
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
        string name = txtSearch.Text.Trim();
        string FromDate = TextFromDate.Text;
        string ToDate = TextToDate.Text;
        if(ToDate == ""){
            ToDate = DateTime.Now.ToString();
        }
        if (FromDate == "")
        {
            ShowMessage("Please, Enter a start date", true);
            TextFromDate.Focus();
        }
        else
        {

            data_table = Process_file.GetUsersActivity(vendor_code, name, FromDate, ToDate);
            if (data_table.Rows.Count > 0)
            {
                ShowMessage("Found ["+data_table.Rows.Count+"] Records", false);

            }
            else
            {
                ShowMessage("No Results Returned", true);

            }
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = 0;
            DataGrid1.DataBind();
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

    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            string vendor_code = ddlAreas.SelectedValue.ToString();
            string name = txtSearch.Text.Trim();
            string FromDate = TextFromDate.Text;
            string ToDate = TextToDate.Text;

            data_table = Process_file.GetUsersActivity(vendor_code, name, FromDate, ToDate);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataGrid1.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

}