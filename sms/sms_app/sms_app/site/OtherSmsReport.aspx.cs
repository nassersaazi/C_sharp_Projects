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

public partial class OtherSmsReport : System.Web.UI.Page
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

                //load SMS vendors
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
        /*
        else if (role_code.Equals("003"))
        {
            string area = Session["VendorCode"].ToString();
            string Name = Session["VendorName"].ToString();
            ddlAreas.Items.Insert(0, new ListItem(Name, area));
            ddlAreas.Enabled = false;
        }
        */
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
        string vendorCode = ddlAreas.SelectedValue.ToString();
        string phone =phone_validity.Format(ddlPhone.Text);
        string status = ddlStatus.SelectedValue;
        DateTime from = Process_file.ReturnDate(txtstartdate.Text.Trim(), 1);
        DateTime end = Process_file.ReturnDate(txtenddate.Text.Trim(), 2);

        if (phone != "" && !phone_validity.NumberFormatIsValid(phone))
        {
            ShowMessage("Please Enter A valid Phone Number", true);
            txtstartdate.Focus();
        }
        else if (txtstartdate.Text.Equals(""))
        {
            ShowMessage("Please Enter Start Date for your Search", true);
            txtstartdate.Focus();
        }
        else
        {

            //get the SMS Report from SMS Client
            data_table = data_file.GetSmsReportFromSMSClientDb(vendorCode, phone, status, from, end);
            DataGrid1.DataSource = data_table;
            DataGrid1.DataBind();

            Session["ResultTable"] = data_table;

            ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);


            //old logic for getting vendor SMSs
            
            /*
            data_table = data_file.GetArea(vendorCode);
            if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "POSTPAID")
            {
                data_table = data_file.GetPostPaidSMS(vendorCode, phone,status, from, end);
                DataGrid1.DataSource = data_table;
                DataGrid1.DataBind();
                ShowMessage("Found ["+data_table.Rows.Count+"] Records", false);
            }
            else if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "PREPAID")
            {
                data_table = data_file.GetPrePaidSMS(vendorCode, phone,status, from, end);
                DataGrid1.DataSource = data_table;
                DataGrid1.DataBind();
                ShowMessage("Please Enter Start Date for your Search", true);
                ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);
            }
            else
            {
                ShowMessage("Vendor Details Not Found", true);
            }
            */

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

    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnView")
            {
                string Listcode = e.Item.Cells[1].Text;
                string message = e.Item.Cells[6].Text;
                string mask = e.Item.Cells[7].Text;
                LoadMessage(message, mask);
            }
           
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
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

            data_table = Session["ResultTable"] as DataTable;

            DataGrid1.DataSource = data_table;
            DataGrid1.CurrentPageIndex = e.NewPageIndex;
            DataGrid1.DataBind();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
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
    
    /*
    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlAreas.Items.Insert(0, new ListItem(" All Vendors ", "0"));
    }
     */

    protected void Button1_Click1(object sender, EventArgs e)
    {
        MultiView2.ActiveViewIndex = 0;
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

            }
            else
            {
                LoadRpt(true);

            }
        }
    }

    private void LoadRpt(bool asExcel = true)
    {
            DataTable data_table1 = new DataTable();

            string vendorCode = ddlAreas.SelectedValue.ToString();
            string phone = phone_validity.Format(ddlPhone.Text);
            string status = ddlStatus.SelectedValue; 
            DateTime from = Process_file.ReturnDate(txtstartdate.Text.Trim(), 0);
            DateTime end = Process_file.ReturnDate(txtenddate.Text.Trim(), 0);


            data_table1 = data_file.GetSmsReportFromSMSClientDb(vendorCode, phone, status, from, end);

            /*
            data_table = data_file.GetArea(vendorCode);
            if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "POSTPAID")
            {
                data_table1 = data_file.GetPostPaidSMS(vendorCode, phone,status, from, end);
                    
            }
            else
            if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "PREPAID")
            {
                data_table1 = data_file.GetPrePaidSMS(vendorCode, phone, status, from, end);
                        
            }
            */

        BusinessLogic bll = new BusinessLogic();

        if (asExcel)
        {

            bll.ExportToExcel(data_table1, "", Response);

        }
        else
        {

            bll.ExportToPdf(data_table1, "", Response);

        }


    }

    protected void DataGrid1_SelectedIndexChanged(object sender, DataGridPageChangedEventArgs e)
    {
        MultiView2.ActiveViewIndex = 0;
        string vendorCode = ddlAreas.SelectedValue.ToString();
        string phone = phone_validity.Format(ddlPhone.Text);
        string status = ddlStatus.SelectedValue;
        DateTime from = Process_file.ReturnDate(txtstartdate.Text.Trim(), 0);
        DateTime end = Process_file.ReturnDate(txtenddate.Text.Trim(), 0);

        if (phone != "" && !phone_validity.NumberFormatIsValid(phone))
        {
            ShowMessage("Please Enter A valid Phone Number", true);
            txtstartdate.Focus();
        }
        else
            if (txtstartdate.Text.Equals(""))
            {
                ShowMessage("Please Enter Start Date for your Search", true);
                txtstartdate.Focus();
            }
            else
            {


                data_table = data_file.GetSmsReportFromSMSClientDb(vendorCode, phone, status, from, end);
                DataGrid1.DataSource = data_table;
                DataGrid1.CurrentPageIndex = e.NewPageIndex;
                DataGrid1.DataBind();
                ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);

                /*
                data_table = data_file.GetArea(vendorCode);

                if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "POSTPAID")
                {
                    data_table = data_file.GetPostPaidSMS(vendorCode, phone, status, from, end);
                    DataGrid1.DataSource = data_table;
                    DataGrid1.CurrentPageIndex = e.NewPageIndex;
                    DataGrid1.DataBind();
                    ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);
                }
                else if (data_table.Rows[0]["VendorType"].ToString().ToUpper() == "PREPAID")
                {
                    data_table = data_file.GetPrePaidSMS(vendorCode, phone, status, from, end);
                    DataGrid1.DataSource = data_table;
                    DataGrid1.CurrentPageIndex = e.NewPageIndex;
                    DataGrid1.DataBind();
                    ShowMessage("Please Enter Start Date for your Search", true);
                    ShowMessage("Found [" + data_table.Rows.Count + "] Records", false);
                }
                else
                {

                    ShowMessage("Vendor Details Not Found", true);
                }
                */

            }

    }

    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {

        string roleCode = Session["RoleCode"].ToString();

        //for admin role code, we give the option to get report of all vendors
        if (roleCode.Equals("001"))
        {
            ddlAreas.Items.Insert(0, new ListItem("All Vendors", "0"));
        }

    }

}