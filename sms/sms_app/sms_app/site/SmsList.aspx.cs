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

public partial class SmsList : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    DataTable data_table = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string Id = Request.QueryString["ListID"];

            if (!string.IsNullOrEmpty(Id))
            {
                LoadContrls(Id);
            }
            else if (IsPostBack == false)
            {
                MultiView1.ActiveViewIndex = 0;
                if ((Session["Username"] == null))
                {
                    Response.Redirect("Default.aspx");
                }

                LoadLists();
               
            }

        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadLists()
    {
        MultiView2.ActiveViewIndex = 0;
        string VendorCode="";
        if (Session["RoleCode"].ToString() == "001")
        {
            VendorCode = "";
        }
        else {
            VendorCode = Session["VendorCode"].ToString();
        }
        string list_name = txtSearch.Text.Trim();
        data_table = data_file.GetLists(VendorCode, list_name);
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
                string listCode = e.Item.Cells[0].Text;
                LoadContrls(listCode);
                ShowMessage(".", true);
            }
            else if (e.CommandName == "btnAdd")
            {
                string listCode = e.Item.Cells[0].Text;
                string listName = e.Item.Cells[3].Text;
                string active = e.Item.Cells[4].Text;
                if (active.Equals("YES"))
                {
                    //string Code = encrypt.EncryptString(listCode, "25011Pegsms2322");
                    string Code = encrypt.EncryptString(listCode, "PegasusSms2020");
                    Response.Redirect("./PhoneNumber.aspx?transfereid=" + Code, false);
                }
                else
                {
                    ShowMessage("List "+listName+" is not active to add numbers on", true);
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void LoadContrls(string listCode)
    {
        int list_Id = int.Parse(listCode);
        data_table = data_file.Get_list(list_Id);
        if (data_table.Rows.Count > 0)
        {
            lbllistCode.Text = data_table.Rows[0]["ListID"].ToString();
            txtListName.Text = data_table.Rows[0]["ListName"].ToString();
            bool is_active = bool.Parse(data_table.Rows[0]["Active"].ToString());
            chkActive.Checked = is_active;
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string list_code = lbllistCode.Text.Trim();
            string list_name = txtListName.Text.Trim();
            if (list_name.Equals(""))
            {
                ShowMessage("Group Name Required", true);
                txtListName.Focus();
            }
            else
            {
                bool is_active = chkActive.Checked;
                string res_listSaving = Process_file.SaveList(list_code, list_name, is_active);
                if (res_listSaving.Equals("SAVED"))
                {
                    ShowMessage("Contact Group  (" + list_name + ") Created Successfully", false);
                }
                else
                {
                    ShowMessage("Contact Group  (" + list_name + ") Edited Successfully", false);
                }
                txtListName.Text = "";
                lbllistCode.Text = "0";
                txtSearch.Text = "";
                LoadLists();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        try
        {
            LoadLists();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {

    }
   
}
