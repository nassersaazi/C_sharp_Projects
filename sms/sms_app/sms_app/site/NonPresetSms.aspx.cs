using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class NonPresetSms : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    private readonly Processfile _processFile = new Processfile();
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
                else {
                    //LoadNonPresetSms();
                    ToggleControls();
                    loadVendorMasks();
                }

            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void loadVendorMasks()
    {
        var VendorCode = Session["VendorCode"].ToString();
        var dataTable = _processFile.GetVendorMasks(VendorCode, "INBOUND");
        ddlMasks.DataSource = dataTable;
        ddlMasks.DataValueField = "Mask";
        ddlMasks.DataTextField = "MaskName";
        ddlMasks.DataBind();
        ddlMasks.Items.Insert(0, new ListItem(" Masks ", "0"));
    }

    private void LoadNonPresetSms()
    {
        MultiView1.ActiveViewIndex = 0;
        var VendorCode = ddlAreas.SelectedValue;
        var Mask = ddlMasks.SelectedValue;
        var status = ddlStatus.SelectedValue;
        var IsPreset = ddlSmsType.SelectedValue;
        var startDate = txtstartdate.Text;
        var EndDate = txtenddate.Text;
        if (startDate == "")
        {
            ShowMessage("Please Select A  Start date", true);
        }
        else
        if(EndDate==""){
            EndDate = DateTime.Now.ToString();
        }
        
            data_table = data_file.GetNonPresetSms(VendorCode, Mask, status, IsPreset, startDate, EndDate);
            DataGrid1.DataSource = data_table;
            DataGrid1.DataBind();
            ShowMessage("Found ["+data_table.Rows.Count+"] Records", false);       
    }


    private void ToggleControls()
    {
        string role_code = Session["RoleCode"].ToString();
        if (role_code.Equals("002") || role_code.Equals("003"))
        {
            string area = Session["VendorCode"].ToString();
            string Name = Session["VendorName"].ToString();
            ddlAreas.Items.Insert(0, new ListItem(Name, area));
           
            ddlAreas.Enabled = false;
        }
        else
        {
            LoadAreas();
            //LoadUsers();
            ddlAreas.Enabled = true;
            
        }
    }

    private void LoadAreas()
    {
        data_table = data_file.GetAreas();
        ddlAreas.DataSource = data_table;
        ddlAreas.DataValueField = "VendorCode";
        ddlAreas.DataTextField = "VendorName";
        ddlAreas.DataBind();
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
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

    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadNonPresetSms();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        LoadNonPresetSms();

    }

    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }

    protected void rdbtnpdf_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnExcel.Checked = false;
    }
    protected void rdbtnExcel_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnpdf.Checked = false;
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
            // LoadRpt();
            if (rdbtnpdf.Checked.Equals(true))
            {
                LoadRpt(false);
                // Rptdoc.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, "SMS");                

            }
            else
            {
                LoadRpt(true);
                //  Rptdoc.ExportToHttpResponse(ExportFormatType.ExcelRecord, Response, true, "SMS");

            }
        }
    }

    private void LoadRpt(bool asExcel = true)
    {

        var VendorCode = ddlAreas.SelectedValue;
        var mask = ddlMasks.SelectedValue;
        var status = ddlStatus.SelectedValue;
        var IsPreset = ddlSmsType.SelectedValue;
        var startDate = txtstartdate.Text;
        var EndDate = txtenddate.Text;

        if (EndDate == "")
        {
            EndDate = DateTime.Now.ToString();
        }

        data_table = data_file.GetNonPresetSms(VendorCode, mask, status,IsPreset, startDate, EndDate);

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
            if (e.CommandName == "btnEdit")
            {
                string num =e.Item.Cells[2].Text; //encrypt.EncryptString(e.Item.Cells[0].Text, "25011Pegsms2322"); 
                Response.Redirect("./SmsSending.aspx?number=" + num, true);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}