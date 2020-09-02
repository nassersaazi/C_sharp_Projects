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

public partial class Vendors : System.Web.UI.Page
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
                else if (Request.QueryString["Vendor"] != null)
                {
                    string VendorCode = Process_file.DecryptString(Request.QueryString["Vendor"].ToString());
                        
                        //Encryption.encrypt.DecryptString(Request.QueryString["Vendor"].ToString(), "25011Pegsms2322");
                   
                }
                else
                {

                    LoadData();
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadData()
    {
        MultiView1.ActiveViewIndex = 0;
        IsActive.Checked=true;
        
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
            string VendorName = txtVendorName.Text;
            string VendorCode = txtVendorCode.Text.ToUpper();
            string VendorContact = txtVendorContact.Text;
            string VendorEmail = txtEmail.Text;
            
            string vendType = ddlUsers.SelectedValue;
            string CreatedBy = Session["Username"].ToString();

            
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
            if (vendType.Equals("0"))
            {
                txtVendorName.Focus();
            }else
            {
                //data_table = data_file.GetArea(VendorCode);

                //if(data_table.Rows.Count==0){
                    string SaveVendor = Process_file.Save_Area(VendorName, VendorCode, VendorContact, VendorEmail, IsActive.Checked, vendType, CreatedBy);
                    if (SaveVendor == "SAVED")
                    {
                        ShowMessage("VENDOR DETAILS SAVED SUCCESSFULLY", false);
                        txtVendorName.Text = "";
                        txtVendorCode.Text = "";
                        
                        txtVendorContact.Text = "";
                        txtEmail.Text = "";
                        
                        IsActive.Checked = false;
                        //IsPrepaid.Checked = false;
                    }
                    else {
                        ShowMessage("VENDOR DETAILS COULD NOT BE SAVED", true);
                    }
                //}else {
                //        ShowMessage("VENDOR CODE ALREADY REGISTERED", true);
                //    }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

   
}
