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

public partial class ViewCreditLog : System.Web.UI.Page
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
                LoadAreas();
                
                //ToggleControls();
                //LoadLists();
                
                
              
                
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    private void LoadUsers()
    {
        string area_code = ddlAreas.SelectedValue.ToString();
        //int area_id = int.Parse(area_code);
        d_table = data_file.GetUsersByArea(area_code);
        ddlUsers.DataSource = d_table;
        ddlUsers.DataValueField = "Username";
        ddlUsers.DataTextField = "FullName";
        ddlUsers.DataBind();
    }
    private void LoadAreas()
    {
        string role_code = Session["RoleCode"].ToString();
        if(role_code == "001"){
        data_table = data_file.GetAreas();
        ddlAreas.DataSource = data_table;
        ddlAreas.DataValueField = "VendorCode";
        ddlAreas.DataTextField = "VendorName";
        ddlAreas.DataBind();
        }else{
            ddlAreas.Enabled = false;
            ddlAreas.Items.Insert(0, new ListItem(Session["VendorName"].ToString(), Session["VendorCode"].ToString()));
            LoadUsers();
          
        }
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadCreditHistory();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadCreditHistory()
    {
        MultiView2.ActiveViewIndex = 0;
        string area_code = ddlAreas.SelectedValue.ToString();
        string user = ddlUsers.SelectedValue.ToString();
        string from = txtstartdate.Text.Trim();
        string end = txtenddate.Text.Trim();
        if (from.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
        }
        else
        {
            data_table = Process_file.GetCreditHistory(area_code, user, from, end);
            //CalculateTotal(data_table); 
            DataGrid1.DataSource = data_table;
            DataGrid1.DataBind();
            ShowMessage("Found ["+data_table.Rows.Count+"] Rows",false);
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
        //ddllists.Items.Insert(0, new ListItem(" All Lists ", "0"));
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
                //Rptdoc.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, "CreditHistory");

            }
            else
            {
                LoadRpt(false);
                //Rptdoc.ExportToHttpResponse(ExportFormatType.ExcelRecord, Response, true, "CreditHistory");

            }
        }
    }

    private void LoadRpt(bool asExcel = true)
    {
        string area_code = ddlAreas.SelectedValue.ToString();
        string user = ddlUsers.SelectedValue.ToString();
        string from = txtstartdate.Text.Trim().ToString();
        string end = txtenddate.Text.Trim().ToString();
        data_table = Process_file.GetCreditHistory(area_code, user, from, end);

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
    private DataTable CreditLogs() 
    {
        DataTable dtCreditLogs = new DataTable("CreditLogs");

        dtCreditLogs.Columns.Add("vendorRef");
        dtCreditLogs.Columns.Add("str1");
        dtCreditLogs.Columns.Add("Date2");
        dtCreditLogs.Columns.Add("str2");
        dtCreditLogs.Columns.Add("printedby");
 
        return dtCreditLogs;       
    }

    private DataTable GetGridRpt()
    {
        DataTable dtable = CreditLogs();

        foreach (DataGridItem Items in DataGrid1.Items)
        {
            DataRow dr = dtable.NewRow();

                string vendorRef = Items.Cells[2].Text;
                string payDate = Items.Cells[3].Text;
                string Date2 = Items.Cells[4].Text;
                string str2 = Items.Cells[5].Text;
                string printedby = Session["FullName"].ToString();

                dr["vendorRef"] = vendorRef;
                dr["str1"] = payDate;
                dr["Date2"] = Date2;
                dr["str2"] = str2;
                dr["printedby"] = printedby;
                dtable.Rows.Add(dr);
                dtable.AcceptChanges();          
        }
        return dtable;
    
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
           // LoadLists();
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

    protected void ddlSent_DataBound(object sender, EventArgs e)
    {

    }

    private void CalculateTotal(DataTable Table)
    {
        double total = 0;
        foreach (DataRow dr in Table.Rows)
        {
            double amount = double.Parse(dr["Credit"].ToString());
            total += amount;
        }
        lblTotal.Text = "Total Allocated Credit: [" + total.ToString("#,##0") + "]";
    }
    //protected void btnConvert_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        ConvertToFile();
    //    }
    //    catch (Exception ex)
    //    {
    //        ShowMessage(ex.Message, true);
    //    }

    //}

    protected void rdbtnpdf_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnExcel.Checked = false;
    }
    protected void rdbtnExcel_CheckedChanged(object sender, EventArgs e)
    {
        rdbtnpdf.Checked = false;
    }
}
