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
                ToggleControls();
                
               // ToggleControls();
               // LoadLists();
               //LoadSentTypes();
                
                
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void ToggleControls()
    {
        string role_code = Session["RoleCode"].ToString();
        if (role_code.Equals("002"))
        {
            string area = Session["VendorCode"].ToString();
            string Name = Session["VendorName"].ToString();
            ddlAreas.Items.Insert(0, new ListItem(Name, area));

            d_table = data_file.GetUsersByArea(area);
            ddlUsers.DataSource = d_table;
            ddlUsers.DataValueField = "Username";
            ddlUsers.DataTextField = "FullName";
            ddlUsers.DataBind();
            ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
            ddlAreas.Enabled = false;
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
        }else
        {
            LoadAreas();
            //LoadUsers();
            ddlAreas.Enabled = true;
            ddlUsers.Enabled = true;
            ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
        }
    }

    private void LoadUsers()
    {
        string area_code =  ddlAreas.SelectedValue.ToString();

        d_table = data_file.GetUsersByArea(area_code);
        ddlUsers.DataSource = d_table;
        ddlUsers.DataValueField = "Username";
        ddlUsers.DataTextField = "FullName";
        ddlUsers.DataBind();
        ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
        
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
    private void LoadSentTypes()
    {
        DataTable data_table1 = new DataTable();
        ddlSent.DataSource = data_table1;
        //ddlAreas.DataValueField = "AreaID";
        //ddlAreas.DataTextField = "Area";
        ddlSent.DataBind();
    }
    private void LoadLists()
    {
        //string area_code = ddlAreas.SelectedValue.ToString();
        //data_table = data_file.GetAllListsByArea(area_code);
        //ddllists.DataSource = data_table;
        //ddllists.DataValueField = "ListID";
        //ddllists.DataTextField = "ListName";
        //ddllists.DataBind();
    }
    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadSmsSent();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadSmsSent()
    {

        MultiView2.ActiveViewIndex = 0;
        //string list_code = ddlUsers.SelectedValue.ToString();
        string Sent = ddlSent.SelectedValue.ToString();
        string area_code = ddlAreas.SelectedValue.ToString();
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
            data_table = Process_file.GetSmsSent(area_code, user,Sent, from, end);
            //CalculateTotal(data_table);
            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = 0;
            DataGrid1.DataBind();
            ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);
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

        string area_code = ddlAreas.SelectedValue.ToString();
        string user = ddlUsers.SelectedValue.ToString();
        string from = txtstartdate.Text.Trim().ToString();
        string Sent = ddlSent.SelectedValue.ToString();
        string end = txtenddate.Text.Trim().ToString();

        DataTable data_table1 = Process_file.GetSmsSent(area_code, user, Sent, from, end);

        BusinessLogic bll = new BusinessLogic();

        if (asExcel)
        {

            bll.ExportToExcel(data_table1, "", Response);

        }
        else {

            bll.ExportToPdf(data_table1, "", Response);

        }


      //  using (DataTable data_table1 = Process_file.GetSmsSentPR(area_code, user, Sent, from, end))
        //using (DataTable data_table1 = Process_file.GetSmsSent(area_code, user, Sent, from, end))
        //{

        //    //data_table = formatTable(data_table);
        //    string appPath, physicalPath, rptName;
        //    appPath = HttpContext.Current.Request.ApplicationPath;
        //    physicalPath = HttpContext.Current.Request.MapPath(appPath);

        //    rptName = physicalPath + "\\Bin\\Reports\\SentSms.rpt";

        //    Rptdoc.Load(rptName);
        //    Rptdoc.SetDataSource(data_table1);
        //    CrystalReportViewer1.ReportSource = Rptdoc;
        //}
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
            string list_code = ddlUsers.SelectedValue.ToString();
            string Sent = ddlSent.SelectedValue.ToString();
            string area_code = ddlAreas.SelectedValue.ToString();
            string user = ddlUsers.SelectedValue.ToString();
            string from = txtstartdate.Text.Trim();
            string end = txtenddate.Text.Trim();
            data_table = Process_file.GetSmsSent(area_code, user, Sent, from, end);
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
            ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void ddlUsers_DataBound(object sender, EventArgs e)
    {
        //ddlUsers.Items.Insert(0, new ListItem(" All Users ", "0"));
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
        try
        {
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }

    }

    private void CalculateTotal(DataTable Table)
    {
        double total = 0;
        int count = Table.Rows.Count;
        foreach (DataRow dr in Table.Rows)
        {
            double amount = double.Parse(dr["Cost"].ToString());
            total += amount;
        }
        lblTotal.Text = "Total Cost of Sms [" + total.ToString("#,##0") + "]";
        lblTotalNo.Text = "Total Sms Sent " + count;
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
