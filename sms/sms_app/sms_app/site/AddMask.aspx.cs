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

public partial class AddMask : System.Web.UI.Page
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
                LoadMasks();
                LoadMaskTypes();
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    private void LoadMaskTypes()
    {

            data_table = data_file.GetMaskTypes();
            ddlMaskType.DataSource = data_table;
            ddlMaskType.DataValueField = "MaskType";
            ddlMaskType.DataTextField = "MaskType";
            ddlMaskType.DataBind();
            ddlMaskType.Items.Insert(0, new ListItem(" Select Mask Type ", ""));
    }

    private void LoadMasks()
    {
        MultiView2.ActiveViewIndex = 0;
        data_table = data_file.GetMasks();
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
                string recordid = e.Item.Cells[1].Text;
                string Mask = e.Item.Cells[2].Text;
                string MaskName = e.Item.Cells[3].Text;
                string MaskType = e.Item.Cells[4].Text;
                string active = e.Item.Cells[5].Text;
                bool IsActive = active.Equals("YES") ? true : false; 
                txtMask.Text = Mask;
                txtMaskName.Text = MaskName;
                chkActive.Checked = IsActive;
                ddlMaskType.SelectedValue = MaskType;
                txtId.Text = recordid;
                ShowMessage("EDITING MASK FOR <b>" + Mask + "</b>", false);
            }
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
            string Mask = txtMask.Text.Trim();
            string MaskName = txtMaskName.Text.Trim();
            string MaskType = ddlMaskType.SelectedValue;
            bool IsActive = chkActive.Checked;
            string Id = txtId.Text;

            if (MaskType.Equals(""))
            {
                ShowMessage("Please Select a Mask Type", true);
                txtMaskName.Focus();
            }
            else
            if (Mask.Equals(""))
            {
                ShowMessage("Please Enter Mask ", true);
                txtMask.Focus();
            }
            else if (MaskName.Equals(""))
            {
                ShowMessage("Please Enter Mask Name", true);
                txtMaskName.Focus();
            }
            else 
            {
                string User = Session["Username"].ToString();
                string res_mask = Process_file.Save_Mask(Mask, MaskName, MaskType, User, IsActive, Id);
                if (res_mask == "SAVED")
                {
                    ShowMessage("MASK SAVED SUCCESSFULLY", false);
                    txtMask.Text = "";
                    txtMaskName.Text = "";
                    
                }
                else {
                    ShowMessage("MASK NOT SAVED", true);
                }
                LoadMasks(); 
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        MultiView2.ActiveViewIndex = 0;
        data_table = data_file.GetMasks();
        DataGrid1.DataSource = data_table;
        DataGrid1.DataBind();

    }

   
}
