using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using InterConnect.SMSConfigurationApi;
using System.Net;
using System.IO;

public partial class PresetSms : System.Web.UI.Page
{
    DbAccess data_file = new DbAccess();
    private readonly Processfile _processFile = new Processfile();
    DataTable data_table = new DataTable();
    public string variableChar = "";
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
                else {
                    LoadPresetSms();
                    loadVendorMasks();
                    variableChar = data_file.GetVariableType("SMS_TEMPLATE");
                }

            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void loadVendorMasks()
    {
        var VendorCode = Session["VendorCode"].ToString();
        var dataTable = _processFile.GetVendorMasks(VendorCode, "INBOUND");
        ddlMasks.DataSource = dataTable;
        ddlMasks.DataValueField = "Mask";
        ddlMasks.DataTextField = "MaskName";
        ddlMasks.DataBind();
        ddlMasks.Items.Insert(0, new ListItem(" All Codes ", ""));
    }
    private void LoadPresetSms() {
        MultiView2.ActiveViewIndex = 0;
        var VendorCode = Session["VendorCode"].ToString();
        data_table = data_file.GetPresetSms(VendorCode);
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

    protected void btnOK_Click(object sender, EventArgs e)
    {
        var identifier = txtPreset.Text;
        var ShortCode = ddlMasks.SelectedValue;
        var response = txtMessage.Text;
        var VendorCode = Session["VendorCode"].ToString();
        bool IsActive = chkIsActive.Checked;
        bool match = chkMatch.Checked;
        var user = Session["Username"].ToString();
        var RecordId = txtId.Text;
        var precedence = "";
        if(identifier==""){
            ShowMessage("Please Enter an identifier for a preset message", true);
        }else
        if (ShortCode=="")
        {
            ShowMessage("Please Select a short code to attach this Preset rule to", true);
        }else
            if (response == "")
        {
            ShowMessage("Please Enter a preset Rulee", true);
        }
        else {
            InterConnect.SMSConfigurationApi.SmsConfigWebApi myapi = new InterConnect.SMSConfigurationApi.SmsConfigWebApi();

            PresetRule PresetSms = new PresetRule();
            PresetSms.Id =Int32.Parse(_processFile.RandomNumber());
            PresetSms.ShortCode = ShortCode;
            PresetSms.VendorCode = VendorCode;
            PresetSms.MessageFormat = response;
            PresetSms.VariableIdentifier =  data_file.GetVariableType("SMS_TEMPLATE");
            PresetSms.MsgLengthMustMatchRuleLength = match;
            PresetSms.PresetRuleID = _processFile.RandomString();
            OpResult PresetSend = new OpResult();
            string result = "";
           
            if(RecordId!=""){
                result = _processFile.Save_PresetSms(ShortCode, identifier, response, VendorCode, IsActive, user, match, RecordId);

            }else{
                PresetSend = myapi.SavePresetRule(PresetSms);
            if (PresetSend.StatusCode == "0")
            {
                result = _processFile.Save_PresetSms(ShortCode, identifier, response, VendorCode, IsActive, user,match, RecordId);
            }
            else
            {
                ShowMessage("Failed:  "+PresetSend.StatusDesc,  true);
            }
        }
            if (result == "SAVED")
            {
                ShowMessage("Preset SMS Saved Successfully", false);
                txtPreset.Text = "";
                txtMessage.Text = "";
                txtId.Text = "";
                chkIsActive.Checked = false;
                txtId.Text = "";
                LoadPresetSms();
            }
            else
            {
                ShowMessage("Error: " + result, true);
            }

        }
    }

    protected void DataGrid1_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
    {
        LoadPresetSms();

    }

    protected void DataGrid1_ItemCommand(object source, DataGridCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "btnEdit")
            {
                string RecordId = e.Item.Cells[1].Text;
                string Identifier = e.Item.Cells[2].Text;
                string response = e.Item.Cells[3].Text;
                string shortcode = e.Item.Cells[4].Text;
                string active = e.Item.Cells[5].Text;
                string match = e.Item.Cells[5].Text;
                bool IsActive = active.Equals("YES") ? true : false;
                bool IsMatch = match.Equals("YES") ? true : false;

                txtId.Text = RecordId;
                txtMessage.Text = response;
                ddlMasks.SelectedValue = shortcode;
                txtPreset.Text = Identifier;
                chkIsActive.Checked = IsActive;
                chkMatch.Checked = IsMatch;
                btnOK.Text = "Edit Preset";
                ShowMessage("EDITING PRESET SMS <b>" + Identifier + "</b>", false);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }  
}