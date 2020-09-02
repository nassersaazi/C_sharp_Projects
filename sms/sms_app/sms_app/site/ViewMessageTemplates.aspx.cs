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
using Encryption;

public partial class ViewMessageTemplates : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    PhoneValidator phone_validity = new PhoneValidator();
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
                MultiView2.ActiveViewIndex = 0;
                //LoadMessageTemplateTitles();
                //LoadUsers();
                LoadListDetails();
               
            }

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }



    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            LoadListDetails();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadListDetails()
    {

        MultiView2.ActiveViewIndex = 0;

        string title = txtTitle.Text;
        string user = txtUser.Text;

        data_table = Process_file.GetMessageTemplates(title, user);
        DataGrid1.DataSource = data_table;
        DataGrid1.CurrentPageIndex = 0;
        DataGrid1.DataBind();
        ShowMessage(".", true);


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
                //string phone_code = e.Item.Cells[1].Text;
                string id = encrypt.EncryptString(e.Item.Cells[1].Text, "25011Pegsms2322");

                Response.Redirect("./MessageTemplate.aspx?msgid=" + id, false);
                
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    //private void LoadPhoneControl(string phone_code, string phone, string name, string status)
    //{
    //    MultiView2.ActiveViewIndex = 1;
    //    lblPhoneCode.Text = phone_code;
    //    //txtName.Text = name;
    //    //txtPhoneNumber.Text = phone;
    //    bool isactive = true;
    //    if (status.Equals("NO"))
    //    {
    //        isactive = false;
    //    }
    //    //chkActive.Checked = isactive;
    //    //txtPhoneNumber.Enabled = false;
    //}
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        try
        {
            string VendorCode = "";
            string list_code = ""; //ddllists.SelectedValue.ToString();
            string phone = ""; // txtPhone.Text.Trim();
            string name = ""; // txtSearch.Text.Trim();
            data_table = Process_file.GetListDetails(list_code);
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
            string phone_code = lblPhoneCode.Text.Trim();
            string phone = ""; // txtPhoneNumber.Text.Trim();
            string phone_name = ""; // txtName.Text.Trim();
            bool isactive = true; // chkActive.Checked;
            Process_file.UpdatePhoneDetails(phone_code, phone, phone_name, isactive);
            ShowMessage("Phone Details Updated Successfully", false);
            LoadListDetails();
        }
        catch (Exception eX)
        {
            ShowMessage(eX.Message, true);
        }
    }
}
