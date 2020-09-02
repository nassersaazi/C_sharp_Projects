using System;
using System.Collections;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;
using CrystalDecisions.Shared;
using System.Text.RegularExpressions;

public partial class SmsSending : Page
{

    FileUpload uploadedFile;
    DataTable dtable = new DataTable();
    private readonly Processfile _processFile = new Processfile();
    private PhoneValidator phone = new PhoneValidator();
    DbAccess data_file = new DbAccess();
    public static string RECEIPTION_TYPE_PHONE_NO = "0";
    public static string RECEIPTION_TYPE_LIST = "1";
    public static string RECEIPTION_TYPE_FILE = "2";
    public static string RECEIPTION_TYPE_TEMPLATE = "3";
    public  string variableChar="";



    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (IsPostBack) return;
            if ((Session["Username"] == null))
            {
                Response.Redirect("Default.aspx");
            }
            MultiView2.ActiveViewIndex = 0;

            if (Request.QueryString["number"] != null)
            {
                string num = Request.QueryString["number"].ToString(); ;
                ddlReceipient.SelectedValue = "0";
                ddlReceipient.Enabled = false;
                MultiView2.ActiveViewIndex = 0;
                
                txtPhones.Text = num;
                txtPhones.Enabled = false;
            }

            LoadActivelists();
            LoadMessageTemplates();
            loadVendorMasks();
            variableChar = data_file.GetVariableType("SMS_TEMPLATE");
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void loadVendorMasks()
    {
        var VendorCode = Session["VendorCode"].ToString();
        var dataTable = _processFile.GetVendorMasks(VendorCode, "OUTBOUND");
        ddlMasks.DataSource = dataTable;
        ddlMasks.DataValueField = "Mask";
        ddlMasks.DataTextField = "MaskName";
        ddlMasks.DataBind();
        variableChar = data_file.GetVariableType("SMS_TEMPLATE");
        MultiView4.ActiveViewIndex = 0;
    }

    private void LoadActivelists()
    {
        var dataTable = _processFile.GetActiveLists();
        ddlists.DataSource = dataTable;
        ddlists.DataValueField = "ListID";
        ddlists.DataTextField = "ListName";
        ddlists.DataBind();
        variableChar = data_file.GetVariableType("SMS_TEMPLATE");
    }

    private void LoadMessageTemplates()
    {
        //ddlMessageTemplates.DataSource = null;
        //ddlMessageTemplates.Items.Add(new ListItem("Select template", ""));

        //var dataTable = _processFile.GetMessageTemplates();
        //ddlMessageTemplates.DataSource = dataTable;
        //ddlMessageTemplates.DataValueField = "Message";
        //ddlMessageTemplates.DataTextField = "Title";
        //ddlMessageTemplates.DataBind();
    }

    private void ShowMessage(string Message, bool Error)
    {
        var lblmsg = (Label)Master.FindControl("lblmsg");
        if (Error)
        {
            lblmsg.ForeColor = Color.Red;
            lblmsg.Font.Bold = false;
        }
        else
        {
            lblmsg.ForeColor = Color.Green;
            lblmsg.Font.Bold = true;
        }

        if (Message == ".")
            lblmsg.Text = ".";
        else
            lblmsg.Text = "MESSAGE: " + Message.ToUpper();
    }

    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            var ContactGroup = ddlists.SelectedValue;
            var receipientType = ddlReceipient.SelectedValue;
            var FileUpload = FileUpload1.FileName;
            var TemplateFile = ddlvariable.FileName;
            var enteredNumbers = txtPhones.Text.Trim();
            var message = txtMessage.Text.Trim();
            var mask = ddlMasks.SelectedValue;

            bool IsScheduled = chkSchedule.Checked;
            string schedule = txtSendDate.Text + " " + txtSendTime.Text; ;
            DateTime scheduledDateTime = _processFile.ReturnDate(schedule, 0);
            var today = DateTime.Now;

            //if (IsScheduled == true)
            //{
            //    schedule=txtSendDate.Text + " " + txtSendTime.Text;
            //    scheduledDateTime=_processFile.ReturnDate(schedule, 0);
            //}
            //DateTime scheduledTime = _processFile.ReturnDate(txtSendTime.Text, 0);

            string filename1 = Path.GetFileName(FileUpload1.FileName);
            string extension1 = Path.GetExtension(filename1);

            string filename2 = Path.GetFileName(ddlvariable.FileName);
            string extension2 = Path.GetExtension(filename2);
            if (mask.Equals("0") || mask.Equals(""))
            {
                ShowMessage("Please Select a mask to use", true);
            }else
            if (receipientType.Equals("0") && enteredNumbers.Equals(""))
            {
                ShowMessage("Please Enter Number(s) or Select contact group to Send Message to", true);
            }
            else if (receipientType.Equals("1") && ContactGroup.Equals("0"))
            {
                ShowMessage("Please Select a contact group to Send Message to", true);
            }
            else if (receipientType.Equals("2") && FileUpload.Equals(""))
            {
                ShowMessage("Please upload a CSV contact file to Send Message to", true);
            }else
            if (receipientType.Equals("2") && (!extension1.Equals(".csv")))
            {
                ShowMessage("Invalid file format, please upload either a csv or txt file in the foemat of the provided Template", true);
            }
            else
            
            if (receipientType.Equals("3") && TemplateFile.Equals(""))
            {
                ShowMessage("Please upload a CSV Template file to Send Message to", true);
            }
            else
            if (receipientType.Equals("3") && (!extension2.Equals(".csv")))
            {
                ShowMessage("Invalid file format, please upload either a csv or txt file in the foemat of the provided Template", true);
            }
            else
            if ((IsScheduled == true) && (txtSendDate.Text == ""))
            {
                ShowMessage("Please Enter Scheduled date for the message", true);
            }
            else
            if ((IsScheduled == true) && (txtSendTime.Text == ""))
            {
                ShowMessage("Please Set Scheduled time for the message", true);
            }
            else
            if ((IsScheduled == true) && (scheduledDateTime < today))
            {
                ShowMessage("Scheduled Date and time can not be less than the current date and time", true);
            }
            else if (message.Trim().Equals(""))
            {
                ShowMessage("Please Enter Message to send", true);
                txtMessage.Focus();
            }
            else
            {
                Toggle();
            }


        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    private void Toggle()
    {
        MultiView1.ActiveViewIndex = 2;

        string contactsToDisplay = "";
        var listName = ddlists.SelectedItem.ToString();
        var enteredNumber = txtPhones.Text.Trim();

        var receipientType = ddlReceipient.SelectedValue;

        //User entered a list of numbers comma separated
        if ((receipientType == RECEIPTION_TYPE_PHONE_NO))
        {
            contactsToDisplay = txtPhones.Text.Trim();
        }

        //User selected a list to send to
        else if (receipientType == RECEIPTION_TYPE_LIST)
        {
            contactsToDisplay = listName;
        }

        //User uploaded a contact file
        else if (receipientType == RECEIPTION_TYPE_FILE)
        {
           
            contactsToDisplay = FileUpload1.FileName;
            uploadedFile = FileUpload1;
            Session["uploadedFile"] = uploadedFile;
           
        }

            //User uploaded a template file
        else if (receipientType == RECEIPTION_TYPE_TEMPLATE)
        {
            contactsToDisplay = ddlvariable.FileName;
            uploadedFile = ddlvariable;
            Session["uploadedFile"] = uploadedFile;

        }

        txtviewlistname.Text = contactsToDisplay;
        //txtviewprefix.Text = prefix;
        txtViewMessage.Text = txtMessage.Text;
        Label1.Text = "Please Confirm Details Below";
        ShowMessage("Please Confirm and Continue", false);

    }

    protected void ddlists_DataBound(object sender, EventArgs e)
    {
        ddlists.Items.Insert(0, new ListItem(" Select contact List ", "0"));
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            var Receipient = ddlReceipient.SelectedItem.ToString();
            var ReceipientType = ddlReceipient.SelectedValue;
            
            var Phones = txtPhones.Text;
            var ContactGroup = ddlists.SelectedValue;
            var FileUpload = FileUpload1.FileName;
            var message = txtViewMessage.Text.Trim();
            var msk = ddlMasks.SelectedValue;
            bool IsScheduled = chkSchedule.Checked;
            string schedule = txtSendDate.Text + " " + txtSendTime.Text;
            DateTime scheduledDateTime = _processFile.ReturnDate(schedule, 0);
            var VendorCode = Session["VendorCode"].ToString();
            string user = Session["Username"].ToString();
            int credit = _processFile.GetUserCredit(VendorCode, user);
            
            if (Receipient.Equals(""))
            {
                ShowMessage("Please Select a contact group To Send to", true);
                MultiView1.ActiveViewIndex = 0;
            }
            else
                if (txtMessage.Equals(""))
                {
                    ShowMessage("Please type a message to send", true);
                    MultiView1.ActiveViewIndex = 0;
                }
                else
                {
                    var resLog = "";

                    if (ReceipientType.Equals(RECEIPTION_TYPE_PHONE_NO) && (txtPhones.Text != null || txtPhones.Text != ""))
                    {

                        String[] phoneNumberArr = Phones.Split(',');
                        resLog = _processFile.LogSMSCommaSeparatedList(phoneNumberArr, message, VendorCode, msk);

                        if (resLog.Contains("Successfully"))
                        {
                            ShowMessage(resLog, false);
                            Clear_contrls();
                        }
                        else {
                            ShowMessage(resLog, true);
                        }
                    }

                    else if (ReceipientType.Equals(RECEIPTION_TYPE_LIST) && (ddlists.SelectedValue.ToString() != null || ddlists.SelectedValue.ToString() != ""))
                    {
                        string listId = ddlists.SelectedValue.ToString();
                        string mask = ddlMasks.SelectedValue;
                        string SenderId = ddlMasks.SelectedValue;
                        string filepath = "";
                        dtable = _processFile.GetListDetails(listId);
                        if (dtable.Rows.Count < 1) {
                            resLog = "The selected List is empty or has no active contacts";
                        }else
                        if ((dtable.Rows.Count > 0) && (dtable.Rows.Count < credit))
                        {
                            string reduce = _processFile.Reduct_credit(dtable.Rows.Count);
                            if (reduce == "SAVED")
                            {
                                resLog = _processFile.LogSMSFileUpload(listId, filepath, message, VendorCode, user, mask, SenderId, "ListSMS", IsScheduled, scheduledDateTime);
                                if (resLog.Contains("Successfully"))
                                {
                                    ShowMessage(resLog, false);
                                    Clear_contrls();
                                }
                                else {
                                    ShowMessage(resLog, true);
                                }
                            }
                        }
                        else {
                           resLog = "SMS Could not be sent, check your current SMS credit";
                        ShowMessage(resLog, true);
                    }
                    }

                    else if (ReceipientType.Equals(RECEIPTION_TYPE_FILE))
                    {

                        uploadedFile = Session["uploadedFile"] as FileUpload;
                        //string filename = Path.GetFileName(FileUpload1.FileName);
                        string filename = Path.GetFileName(uploadedFile.FileName);
                        string extension = Path.GetExtension(filename);

                        string pathToFile = @"E:\SMSUploads\FilessToSend\";
                        DateTime todaydate = DateTime.Now;
                        string datetoday = todaydate.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "-");
                        string filepath = pathToFile + filename + "_" + datetoday + extension;
                        if (!Directory.Exists(pathToFile))
                            Directory.CreateDirectory(pathToFile);

                        uploadedFile.SaveAs(filepath);
                        var mask = ddlMasks.SelectedValue;
                        var SenderId = ddlMasks.SelectedValue;
                        var listId = "";
                        var lines = File.ReadAllLines(filepath);
                        var count = lines.Length;
                        if (count < 1)
                        {
                            resLog = "The selected File is empty or has no active contacts";
                            ShowMessage(resLog, true);
                        }
                        else
                        if ((count > 0) && (count < credit))
                        {
                            string reduce = _processFile.Reduct_credit(count);
                            if (reduce == "SAVED")
                            {
                                resLog = _processFile.LogSMSFileUpload(listId, filepath, message, VendorCode, user, mask, SenderId, "FileSMS", IsScheduled, scheduledDateTime);
                                ShowMessage(resLog, false);
                                Clear_contrls();
                            }
                            else
                            {
                                resLog = "Failed to save file to the server";
                                ShowMessage(resLog, true);
                            }
                        }
                        else
                        {
                            resLog = "SMS Could not be sent, check your current SMS credit";
                            ShowMessage(resLog, true);
                        }
                    }
                    else if (ReceipientType.Equals(RECEIPTION_TYPE_TEMPLATE))
                    {
                        uploadedFile = Session["uploadedFile"] as FileUpload;
                        string filename = Path.GetFileName(uploadedFile.FileName);
                        string extension = Path.GetExtension(filename);
                        var mask = ddlMasks.SelectedValue;
                        var SenderId = ddlMasks.SelectedValue;
                        var listId = "";
                        string pathToFile = @"E:\SMSUploads\SMS_TEMPLATES\";
                        DateTime todaydate = DateTime.Now;
                        string datetoday = todaydate.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "-");
                        string filepath = pathToFile + filename + "_" + datetoday + extension;
                        if (!Directory.Exists(pathToFile))
                            Directory.CreateDirectory(pathToFile);
                        uploadedFile.SaveAs(filepath);
                        var lines = File.ReadAllLines(filepath);
                        var count = lines.Length;
                        var title = "SMS_Template" + datetoday;
                        variableChar = data_file.GetVariableType("SMS_TEMPLATE");

                        StreamReader sr = new StreamReader(filepath);
                        string[] col = sr.ReadLine().Split(',');

                        string msg = message;
                        msg = Regex.Replace(message, @"[\n \r ]", " ");
                        string[] words = msg.Split(' ');

                        int i = 0;
                        int j = 0;
                        foreach (var word in words)
                        {
                            if (word.StartsWith(variableChar))
                            {
                                string variable = word.Replace(variableChar, "");
                                string heading = col[(i + 1)];
                                if (variable == heading)
                                {
                                    i++;
                                }
                                else {
                                    j++;
                                }
                            }
                        }

                        //if (j > 0) {
                        //    ShowMessage("Some defined Variables do not match the Column titles in the file", true);
                        //}else
                        if (i <= col.Length)
                        {
                            if ((count > 1) && (count < (credit - 1)))
                            {
                                string reduce = _processFile.Reduct_credit(count);
                                if (reduce == "SAVED")
                                {
                                    //save template to general upload sms table
                                    resLog = _processFile.LogSMSFileUpload(listId, filepath, message, VendorCode, user, mask, SenderId, "SMSTEMPLATE", IsScheduled, scheduledDateTime);

                                    //Save SMS Template for processing...........
                                    resLog = _processFile.SaveMessageTemplate(filepath, title, message, user, VendorCode, mask);
                                    ShowMessage(resLog, false);
                                    Clear_contrls();
                                }
                                else
                                {
                                    ShowMessage("SMS Could not be sent, System Failed to calculate SMS Balance", true);
                                }
                            }
                            else
                            {
                                ShowMessage("SMS Could not be sent, check your current SMS credit", true);
                            }
                        }
                        else
                        {
                            ShowMessage("Columns in the uploaded file are less than the variables defined in the message", true);
                        }
                    }
                    else {
                        ShowMessage("Failed to validate Recepient Type", true);
                    }

                   
                    MultiView1.ActiveViewIndex = 0;
                }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void Clear_contrls()
    {
        ddlists.SelectedIndex = ddlists.Items.IndexOf(ddlists.Items.FindByValue("0"));
        ddlReceipient.SelectedIndex = ddlReceipient.Items.IndexOf(ddlReceipient.Items.FindByValue("0"));
        txtMessage.Text = "";
        txtPhones.Text = "";
        txtPhones.Enabled = true;
        ddlReceipient.Enabled = true;
        chkSchedule.Checked = false;
        txtSendDate.Text = "";
        txtSendTime.Text = "";
        MultiView3.Visible = false;
        variableChar = data_file.GetVariableType("SMS_TEMPLATE");
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        ShowMessage(".", true);
    }



    protected void ddlReceipient_SelectedIndexChanged(object sender, EventArgs e)
    {
        var prefix = ddlReceipient.SelectedValue;
        if (prefix.Equals("0"))
        {
            MultiView2.ActiveViewIndex = 0;
            MultiView4.ActiveViewIndex = 0;
        }
        else if (prefix.Equals("1"))
        {
            //lblMessageLength.Text = "SMS MESSAGE LENGTH : 155";
            LoadActivelists();
            MultiView2.ActiveViewIndex = 1;
            MultiView4.ActiveViewIndex = 1;
        }
        else if (prefix.Equals("2"))
        {
            //lblMessageLength.Text = "SMS MESSAGE LENGTH : 154";
            MultiView2.ActiveViewIndex = 2;
            MultiView4.ActiveViewIndex = 2;
        }
        else if (prefix.Equals("3"))
        {
            //lblMessageLength.Text = "SMS MESSAGE LENGTH : 154";
            MultiView2.ActiveViewIndex = 3;
            MultiView4.ActiveViewIndex = 3;
        }

        //lblMessageLength.Text = "SMS MESSAGE LENGTH : 160";
        variableChar = data_file.GetVariableType("SMS_TEMPLATE");
    }


    protected void ChckedChanged(object sender, EventArgs e)
    {
        bool ischecked = chkSchedule.Checked;
        if (ischecked == true)
        {
            MultiView3.Visible = true;
            MultiView3.ActiveViewIndex = 0;
        }
        else {
            MultiView3.Visible = false;
        }
        variableChar = data_file.GetVariableType("SMS_TEMPLATE");
    }

    protected void ddlMessageTemplates_SelectedIndexChanged(object sender, EventArgs e)
    {

        //var messageTemplate = ddlMessageTemplates.SelectedValue;
        //txtMessage.Text = messageTemplate;

    }

}