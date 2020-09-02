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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using CrystalDecisions.Shared;

public partial class ViewListSmsSent : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    PhoneValidator phone_validity = new PhoneValidator();
    DataTable data_table = new DataTable();
    DataTable d_table = new DataTable();
    DataTable dtable = new DataTable();
    private ReportDocument Rptdoc = new ReportDocument();
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

            ddlAreas.Enabled = false;
        }
    }



    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadSmsSentTotal();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadSmsSentTotal()
    {
        MultiView2.ActiveViewIndex = 0;

        string Vendor = ddlAreas.SelectedValue;
        string from = txtstartdate.Text.Trim();
        string end = txtenddate.Text.Trim();
        if (from.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
        }else
        if (end=="") {
            end = DateTime.Now.ToString();
        }
        
            data_table = Process_file.GetTotalSmsSentNew(Vendor, from, end);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = 0;
            DataGrid1.DataBind();
            ShowMessage("Found ["+data_table.Rows.Count+"] Records",false);
        
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
        MultiView2.ActiveViewIndex = 1;
        string Vendor = ddlAreas.SelectedValue;
        string from = txtstartdate.Text.Trim();
        string end = txtenddate.Text.Trim();

        if (from.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
        }
        else
            if (end == "")
            {
                end = DateTime.Now.ToString();
            }

        data_table = Process_file.GetTotalSmsSentNew(Vendor, from, end);

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

            string Vendor = ddlAreas.SelectedValue;
            string from = txtstartdate.Text.Trim();
            string end = txtenddate.Text.Trim();
            data_table = Process_file.GetTotalSmsSentNew(Vendor, from, end);
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

    protected void rdbtnpdf_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnExcel.Checked = false;
    }
    protected void rdbtnExcel_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnpdf.Checked = false;
    }

}
