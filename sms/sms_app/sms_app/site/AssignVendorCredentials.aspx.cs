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
using System.IO;

public partial class AssignVendorCredentials : System.Web.UI.Page
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
                
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    private void LoadVendors()
    {

        data_table = data_file.GetAreas();
        ddlVendors.DataSource = data_table;
        ddlVendors.DataValueField = "VendorCode";
        ddlVendors.DataTextField = "VendorName";
        ddlVendors.DataBind();

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

            //to hold the vendor credentials
            SmsVendorCredentials smsVendorCredentials = new SmsVendorCredentials();

            //general vendor credentials
            string vendorCode = ddlVendors.SelectedValue;
            string vendorPassword = txtVendorPassword.Text;

            //for postpaid vendors
            string postpaidSecreteKey = txtKey.Text;

            //for prepaid vendors
            string prepaidCertificateFile = FileUpload1.FileName;         
            string prepaidCertificatePassword = txtCertPassword.Text;
            
            //user who is doing the assignment
            string assignedBy = Session["Username"].ToString();
                     
            //get the vendor type based on vendor code
            data_table = data_file.GetArea(vendorCode);

            //we failed to get the type of vendor
            if (data_table == null || data_table.Rows.Count == 0) {
                ShowMessage("Failed to get vendor type for vendor: "+vendorCode, true);
                return;
            }

            //get the vendor type
            string vendorType = data_table.Rows[0]["VendorType"].ToString().ToUpper();

            if (vendorCode.Equals(""))
            {
                ShowMessage("Please Select a vendor ", true);
                ddlVendors.Focus();
            }
            else if (vendorPassword.Equals(""))
            {
                ShowMessage("Please Enter a password", true);
                txtVendorPassword.Focus();
            }
            else if (vendorType == SmsVendorCredentials.VENDOR_TYPE_POSTPAID && postpaidSecreteKey.Equals(""))
            {
                ShowMessage("Please Enter a secrete key", true);
                txtKey.Focus();
            }
            else if (vendorType == SmsVendorCredentials.VENDOR_TYPE_PREPAID && prepaidCertificateFile.Equals(""))
            {
                ShowMessage("Please Upload a certficate file", true);
                FileUpload1.Focus();
            }
            else if (vendorType == SmsVendorCredentials.VENDOR_TYPE_PREPAID && prepaidCertificatePassword.Equals(""))
            {
                ShowMessage("Please Enter the Certficate Password", true);
                txtCertPassword.Focus();
            }
            else
            {
                //assign the vendor credentials
                smsVendorCredentials.vendorCode = vendorCode;
                smsVendorCredentials.vendorPassword = vendorPassword;
                smsVendorCredentials.vendorType = vendorType;
                smsVendorCredentials.assignedBy = assignedBy;


                if (vendorType == SmsVendorCredentials.VENDOR_TYPE_POSTPAID)
                {
                    //if the vendor is postpaid, we just need their secret key
                    smsVendorCredentials.vendorType = SmsVendorCredentials.VENDOR_TYPE_POSTPAID;
                    smsVendorCredentials.postpaidSecretKey = postpaidSecreteKey;
                }
                else if (vendorType == SmsVendorCredentials.VENDOR_TYPE_PREPAID)
                {

                    string filename = Path.GetFileName(FileUpload1.FileName);
                    string extension = Path.GetExtension(filename);

                    string pathToFile = @"E:\SMSUploads\Certificates\";
                    DateTime todaydate = DateTime.Now;
                    string datetoday = todaydate.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "-");
                    string filepath = pathToFile + filename + "_" + datetoday + extension;
                    if (!Directory.Exists(pathToFile))
                        Directory.CreateDirectory(pathToFile);

                    FileUpload1.SaveAs(filepath);

                    //if the vendor is prepaid, we need the certificate and the certificate password
                    //assign the certificate details
                    smsVendorCredentials.vendorType = SmsVendorCredentials.VENDOR_TYPE_PREPAID;
                    smsVendorCredentials.prepaidCertificatePath = filepath;
                    smsVendorCredentials.prepaidCertificatePassword = prepaidCertificatePassword;

                }                

                //save the SMS Vendor Credentials
                string res_mask = Process_file.SaveSmsVendorCredentials(smsVendorCredentials);

                //check if we managed to save the credentials
                if (res_mask == "SAVED")
                {
                    ShowMessage("VENDOR CREDENTIALS ASSIGNED SUCCESSFULLY", false);
                    txtName.Text = "";
                    LoadVendorCredentials();
                }
                else
                {
                    ShowMessage("VENDOR CREDENTIALS NOT ASSIGNED", true);
                }

            }

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }

    }

    protected void Button1_Ok(object sender, EventArgs e)
    {
        LoadVendorCredentials();

    }

    private void LoadVendorCredentials() {
        string name = txtName.Text;
        data_table = data_file.GetVendorCredentials(name);
        DataGrid1.DataSource = data_table;
        DataGrid1.DataBind();
        MultiView2.ActiveViewIndex = 0;
        ddlVendors.Enabled = true;
    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        string name = txtName.Text;
        data_table = data_file.GetVendorCredentials(name);
        DataGrid1.DataSource = data_table;
        DataGrid1.DataBind();
        MultiView2.ActiveViewIndex = 0;
    }

    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnEdit")
            {
                string VendorCode = e.Item.Cells[2].Text;
               

                data_table = data_file.GetVendor(VendorCode);
                if (data_table.Rows.Count > 0)
                {
                     string vendorname = data_table.Rows[0]["VendorName"].ToString();
                string vendorcode = data_table.Rows[0]["VendorCode"].ToString();
                ddlVendors.SelectedValue = vendorcode;
                    ddlVendors.Enabled = false;

                    txtKey.Text = data_table.Rows[0]["SecretKey"].ToString();
                    txtVendorPassword.Text = data_table.Rows[0]["Password"].ToString();

                    
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void ddlAreas_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string vendorcode = ddlVendors.SelectedValue;

            data_table = data_file.GetArea(vendorcode);
            if (data_table.Rows[0]["VendorType"].ToString()=="POSTPAID") {
                MultiView3.ActiveViewIndex = 0;
               
            }else
            if (data_table.Rows[0]["VendorType"].ToString() == "PREPAID") {
                MultiView3.ActiveViewIndex = 1;
            }


        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void ddlAreas_DataBound(object sender, EventArgs e)
    {
        ddlVendors.Items.Insert(0, new ListItem(" Select Vendor ", ""));
    }
}