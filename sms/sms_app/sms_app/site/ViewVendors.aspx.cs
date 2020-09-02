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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Encryption;

public partial class ViewVendors : System.Web.UI.Page
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
        MultiView1.ActiveViewIndex = 0;
        string Vendor = txtVendor.Text;
        data_table = data_file.GetAreaslist(Vendor);
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

        protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "btnEdit")
                {
                    string VendorCode = e.Item.Cells[1].Text;
                   
                    MultiView1.ActiveViewIndex = 1;
                    
                    data_table = data_file.GetVendor(VendorCode);
                    if (data_table.Rows.Count > 0)
                    {
                        txtVendorName.Text = data_table.Rows[0]["VendorName"].ToString();
                        txtVendorCode.Text = data_table.Rows[0]["VendorCode"].ToString();
                        
                        txtVendorContact.Text = data_table.Rows[0]["Contact"].ToString();
                        txtEmail.Text = data_table.Rows[0]["VendorEmail"].ToString();
                        
                        string check = data_table.Rows[0]["Active"].ToString();
                        string VendorType = data_table.Rows[0]["VendorType"].ToString();
                        IsActive.Checked = Convert.ToBoolean(check);
                        //ddlUsers.Items.Insert(0, new ListItem(VendorType, VendorType));
                        //IsPrepaid.Checked = Convert.ToBoolean(prepaid);
                        txtVendorCode.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, true);
            }
        }

        protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            try
            {
                string Vendor = txtVendor.Text;
                data_table = data_file.GetAreaslist(Vendor);
                DataGrid1.DataSource = data_table;
                DataGrid1.CurrentPageIndex = e.NewPageIndex;
                DataGrid1.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, true);
            }
        }
        protected void btnOK_Click(object sender, EventArgs e)
        {
            LoadVendors();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string VendorName = txtVendorName.Text;
                string VendorCode = txtVendorCode.Text;
                string VendorContact = txtVendorContact.Text;
                string VendorEmail = txtEmail.Text;
                string CreatedBy = Session["Username"].ToString();
                string VendorType = ddlUsers.SelectedValue;
                if (VendorName.Equals(""))
                {
                    ShowMessage("Please Enter Vendor Name", true);
                    txtVendorName.Focus();
                }
                else if (VendorCode.Equals(""))
                {
                    ShowMessage("Please Enter Vendor Code", true);
                    txtVendorCode.Focus();
                }
                else
                {
                    string SaveVendor = Process_file.Save_Area(VendorName, VendorCode, VendorContact, VendorEmail, IsActive.Checked, VendorType, CreatedBy);
                    if (SaveVendor == "SAVED")
                    {
                        ShowMessage("VENDOR DETAILS SAVED SUCCESSFULLY", false);
                        LoadVendors();
                    }
                    else
                    {
                        ShowMessage("VENDOR DETAILS COULD NOT BE SAVED", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, true);
            }
        }
}