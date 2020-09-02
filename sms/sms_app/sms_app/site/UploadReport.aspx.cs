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

public partial class UploadReport : System.Web.UI.Page
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
                MultiView1.ActiveViewIndex = 0;
                LoadVendors();
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
            //LoadLists();
            ddlAreas.Enabled = false;
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

    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
           LoadFileReport();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadFileReport()
    {
        string VendorCode = ddlAreas.SelectedValue;
        string user = ddlUsers.SelectedValue;
        string report = ddlReportType.SelectedValue;
        string from = txtstartdate.Text.Trim();
        string end = txtenddate.Text.Trim();
        if (from.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
        }else
        if(end==""){
            end = DateTime.Now.ToString();
        }
        
            data_table = Process_file.GetFileReport(VendorCode, user,report, from,  end);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = 0;
            DataGrid1.DataBind();
            ShowMessage("Found ["+data_table.Rows.Count+"] Records", false);
        
    }

    protected void ddlAreas_SelectedIndexChanged(object sender, EventArgs e)
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

    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }

    protected void ddlUsers_DataBound(object sender, EventArgs e)
    {
        ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
    }

    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnEdit")
            {
                MultiView1.ActiveViewIndex = 1;
                string SourceId = e.Item.Cells[1].Text;
                d_table = data_file.GetNumbersInFile(SourceId);
                DataGrid2.DataSource = d_table;
                DataGrid2.DataBind();
                ShowMessage("Found [" + d_table.Rows.Count + "] Records", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void loadNumber(string SourceId)
    {
       
       
    }

    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            string VendorCode = ddlAreas.SelectedValue;
            string user = ddlUsers.SelectedValue;
            string report = ddlReportType.SelectedValue;
            string from = txtstartdate.Text.Trim();
            string end = txtenddate.Text.Trim();
            if (end == "")
            {
                end = DateTime.Now.ToString();
            }

            data_table = Process_file.GetFileReport(VendorCode, user, report, from, end);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataGrid1.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    protected void btnPhone_Click(object sender, EventArgs e)
    {
        try
        {
            MultiView1.ActiveViewIndex = 1;
            string num = txtPhone.Text;
            d_table = data_file.GetNumbersInFile(num);
            DataGrid2.DataSource = d_table;
            DataGrid2.DataBind();
            ShowMessage("Found [" + d_table.Rows.Count + "] Records", false);
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void DataGrid2_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            MultiView1.ActiveViewIndex = 1;
            string num = txtPhone.Text;
            d_table = data_file.GetNumbersInFile(num);
            DataGrid2.DataSource = d_table;
            DataGrid2.CurrentPageIndex = e.NewPageIndex;
            DataGrid2.DataBind();

           
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void btnConvert_Click(object sender, EventArgs e)
    {
        try
        {
            ConvertToFile();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }

    }

    private void ConvertToFile()
    {
        if (rdbtnExcel.Checked.Equals(false) && rdbtnpdf.Checked.Equals(false))
        {
            ShowMessage("Please Check file format to Convert to", true);
        }
        else
        {

            if (rdbtnpdf.Checked.Equals(true))
            {
                LoadRpt(false);
                // Rptdoc.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, "SMS");

            }
            else
            {
                LoadRpt(true);
                // Rptdoc.ExportToHttpResponse(ExportFormatType.ExcelRecord, Response, true, "SMS");

            }

        }
    }

    private void LoadRpt(bool asExcel = true)
    {
        MultiView1.ActiveViewIndex = 1;
        string num = txtPhone.Text;

        data_table = data_file.GetNumbersInFile(num);

        BusinessLogic bll = new BusinessLogic();
        if (asExcel)
        {
            bll.ExportToExcel(data_table, "", Response);
        }
        else
        {
            bll.ExportToPdf(data_table, "", Response);
        }

    }

}