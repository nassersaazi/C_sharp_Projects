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

public partial class AssignMask : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
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
                MultiView1.ActiveViewIndex = 0;
                LoadVendors();
                LoadMasks();
                LoadVendorMasks();

            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadMasks()
    {
        data_table = data_file.GetActiveMasks("");
        dllMask.DataSource = data_table;
        dllMask.DataValueField = "Mask";
        dllMask.DataTextField = "MaskName";
        dllMask.DataBind();
        dllMask.Items.Insert(0, new ListItem("Select Mask ", ""));
    }
    private void LoadVendors()
    {
        data_table = data_file.GetAreas();
        ddlVendors.DataSource = data_table;
        ddlVendors.DataValueField = "VendorCode";
        ddlVendors.DataTextField = "VendorName";
        ddlVendors.DataBind();
        ddlVendors.Items.Insert(0, new ListItem("Select Vendor ", ""));
    }

    private void LoadVendorMasks()
    {
        MultiView2.ActiveViewIndex = 0;
        data_table = data_file.GetVendorMasks("");
        DataGrid1.DataSource = data_table;
        DataGrid1.DataBind();
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

    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string Mask = dllMask.SelectedValue;
            string Vendor = ddlVendors.SelectedValue;

            if (Mask.Equals(""))
            {
                ShowMessage("Please Select Mask ", true);
                dllMask.Focus();
            }
            else if (Vendor.Equals(""))
            {
                ShowMessage("Please Select Vendor", true);
                ddlVendors.Focus();
            }
            else
            {
                string User = Session["Username"].ToString();
                string res_mask = Process_file.Save_Vendor_Mask(Mask, Vendor, User);
                if (res_mask == "SAVED")
                {
                    ShowMessage("MASK ASSIGNED SUCCESSFULLY", false);
                    txtName.Text = "";

                }
                else
                {
                    ShowMessage("VENDOR MASK NOT ASSIGNED", true);
                }
                LoadVendorMasks();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void Button1_Ok(object sender, EventArgs e)
    {
        string name = txtName.Text;
        data_table = data_file.GetVendorMasks(name);
        DataGrid1.DataSource = data_table;
        DataGrid1.DataBind();

    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {

    }
   
}