using Encryption;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MultipleContactListsSend : System.Web.UI.Page
{
    public const string SELECTED_INDEX = "SelectedIndex";
    private readonly Processfile _processFile = new Processfile();
    DataTable dtable = new DataTable();
    DataTable data_table = new DataTable();
    DbAccess data_file = new DbAccess();
    //public string variableChar = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        if ((Session["Username"] == null))
        {
            Response.Redirect("Default.aspx");
        }

        loadVendorMasks();
        loadLists("");

        MultiView2.ActiveViewIndex = 0;
        MultiView4.SetActiveView(View10);
        RePopulateCheckBoxes();
    }
    private void loadLists(string name)
    {
        var VendorCode = Session["VendorCode"].ToString();
        DataTable dt =_processFile.GetActiveLists(name, VendorCode);
        dataGridResults.DataSource = dt;
        dataGridResults.DataBind();
    }

    private void RePopulateCheckBoxes()
    {
        foreach (GridViewRow row in dataGridResults.Rows)
        {
            var chkBox = row.FindControl("CheckBox") as CheckBox;

            IDataItemContainer container = (IDataItemContainer)chkBox.NamingContainer;

            if (SelectedCustomersIndex != null)
            {
                if (SelectedCustomersIndex.Exists(i => i == container.DataItemIndex))
                {
                    chkBox.Checked = true;
                }
            }
        }
    }

    private List<Int32> SelectedCustomersIndex
    {
        get
        {
            if (ViewState[SELECTED_INDEX] == null)
            {
                ViewState[SELECTED_INDEX] = new List<Int32>();
            }

            return (List<Int32>)ViewState[SELECTED_INDEX];
        }
    }

    private void PersistRowIndex(int index)
    {
        if (!SelectedCustomersIndex.Exists(i => i == index))
        {
            SelectedCustomersIndex.Add(index);
        }
    }
    protected void DataGrid1_PageIndexChanged(object source, GridViewPageEventArgs e)
    {
        try
        {
            string listName = TextBox1.Text.Trim().ToUpper();
            var VendorCode = Session["VendorCode"].ToString();
            
            foreach (GridViewRow row in dataGridResults.Rows)
            {
                //for each row get the checkbox attached
                CheckBox ChkBox = (CheckBox)row.FindControl("CheckBox");
                IDataItemContainer container = (IDataItemContainer)ChkBox.NamingContainer;

                //has user ticked the box
                if (ChkBox.Checked)
                {
                    //if this row is not the header row
                    if (row.RowType != DataControlRowType.Header)
                    {
                        try
                        {
                            PersistRowIndex(container.DataItemIndex);

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        RemoveRowIndex(container.DataItemIndex);

                    }
                }
            }

            DataTable dt = _processFile.GetActiveLists(listName, VendorCode);
            dataGridResults.DataSource = dt;
            dataGridResults.PageIndex = e.NewPageIndex;
            dataGridResults.DataBind();
            RePopulateCheckBoxes();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void RemoveRowIndex(int index)
    {
        SelectedCustomersIndex.Remove(index);
    }
    private void loadVendorMasks()
    {
        var VendorCode = Session["VendorCode"].ToString();
        var dataTable = _processFile.GetVendorMasks(VendorCode, "OUTBOUND");
        ddlMasks.DataSource = dataTable;
        ddlMasks.DataValueField = "Mask";
        ddlMasks.DataTextField = "MaskName";
        ddlMasks.DataBind();
        //variableChar = data_file.GetVariableType("SMS_TEMPLATE");
        MultiView4.ActiveViewIndex = 0;
    }

    protected void dataGridResults_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            CheckBox ChkBoxHeader = (CheckBox)dataGridResults.HeaderRow.FindControl("chkboxSelectAll");
            foreach (GridViewRow row in dataGridResults.Rows)
            {
                CheckBox ChkBoxRows = (CheckBox)row.FindControl("CheckBox");
                if (ChkBoxHeader.Checked == true)
                {
                    ChkBoxRows.Checked = true;
                }
                else
                {
                    ChkBoxRows.Checked = false;
                }
            }
        }
        catch (Exception ex)
        {
            string msg = "FAILED: " + ex.Message;
            ShowMessage( msg, true);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            var message = txtViewMessage.Text.Trim();
            var msk = ddlMasks.SelectedValue;
            bool IsScheduled = chkSchedule.Checked;
            string schedule = txtSendDate.Text + " " + txtSendTime.Text;
            DateTime scheduledDateTime = _processFile.ReturnDate(schedule, 0);
            var VendorCode = Session["VendorCode"].ToString();
            string user = Session["Username"].ToString();
            int credit = _processFile.GetUserCredit(VendorCode, user);

             if (txtMessage.Equals(""))
            {
                ShowMessage("Please type a message to send", true);
                MultiView1.ActiveViewIndex = 0;
            }
            else
            {
                var resLog = "";
                var returnLog = "";
                // id from the loop
                string mask = ddlMasks.SelectedValue;
                string SenderId = ddlMasks.SelectedValue;
                string filepath = "";
                int countSuccess = 0;
                int countFailed = 0;
                foreach (GridViewRow row in dataGridResults.Rows)
                {
                    //for each row get the checkbox attached
                    CheckBox ChkBox = (CheckBox)row.FindControl("CheckBox");

                    //has user ticked the box
                    if (ChkBox.Checked)
                    {
                        //if this row is not the header row
                        if (row.RowType != DataControlRowType.Header)
                        {
                            try
                            {
                                string listId = row.Cells[2].Text;
                                string listName = row.Cells[3].Text.ToUpper();
                                //send reversal request
                                dtable = _processFile.GetListDetails(listId);
                                if (dtable.Rows.Count < 1)
                                {
                                    resLog = resLog + listName + ":HAS NO NUMBERS, ";
                                    countSuccess++;
                                    continue;
                                }
                                else
                                if ((dtable.Rows.Count > 0) && (dtable.Rows.Count < credit))
                                {
                                    string reduce = _processFile.Reduct_credit(dtable.Rows.Count);
                                    if (reduce == "SAVED")
                                    {
                                        resLog = _processFile.LogSMSFileUpload(listId, filepath, message, VendorCode, user, mask, SenderId, "ListSMS", IsScheduled, scheduledDateTime);
                                        if (resLog.Contains("Successfully"))
                                        {
                                            countSuccess++;
                                            //ShowMessage(resLog, false);
                                            returnLog = returnLog + listName + ":SUCCESS, ";
                                        }
                                        else
                                        {
                                            countFailed++;
                                            //ShowMessage(resLog, true);
                                            returnLog = returnLog + listName + ":FAILED, ";
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                countFailed++;
                                resLog = returnLog + ex.Message + ":EXCEPTION, ";
                                //string msg = "FAILED: " + ex.Message;
                                //ShowMessage(msg, true);
                            }
                        }
                    }
                }
                if (countFailed > 0)
                {
                    // some failed
                    ShowMessage(returnLog, true);
                }
                else
                {
                    if (countSuccess > 0)
                    {
                        // all passed
                        ShowMessage(returnLog, false);
                    }
                    else
                    {
                        // most likely none passed
                        ShowMessage("NO LISTS SELECTED TO SEND MESSAGE TO", true);
                    }
                    
                }

                MultiView1.ActiveViewIndex = 0;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        ShowMessage(".", true);
    }

    protected void btnOK_Click(object sender, EventArgs e)
    {
        try
        {
            var message = txtMessage.Text.Trim();
            var mask = ddlMasks.SelectedValue;

            bool IsScheduled = chkSchedule.Checked;
            string schedule = txtSendDate.Text + " " + txtSendTime.Text; ;
            DateTime scheduledDateTime = _processFile.ReturnDate(schedule, 0);
            var today = DateTime.Now;

            if (mask.Equals("0") || mask.Equals(""))
            {
                ShowMessage("Please Select a mask to use", true);
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

        if (txtMessage.Equals(""))
        {
            ShowMessage("Please type a message to send", true);
            MultiView1.ActiveViewIndex = 0;
            return;
        }

        string contactsToDisplay = "";
        var listName = "";
        foreach (GridViewRow row in dataGridResults.Rows)
        {
            //for each row get the checkbox attached
            CheckBox ChkBox = (CheckBox)row.FindControl("CheckBox");

            //has user ticked the box
            if (ChkBox.Checked)
            {
                //if this row is not the header row
                if (row.RowType != DataControlRowType.Header)
                {
                    try
                    {

                        //string listName = row.Cells[2].Text.ToUpper();
                        //send reversal request
                        listName = listName + row.Cells[3].Text.ToUpper() + ", ";
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

        //User selected a list to send to
        if (string.IsNullOrEmpty(listName))
        {
            ShowMessage("NO SELECTED CONTACT GROUPS TO SEND MESSAGE TO", true);
            MultiView1.ActiveViewIndex = 0;
            return;
        }

        // all seems good load the confirmation view
        MultiView1.ActiveViewIndex = 2;
        contactsToDisplay = listName;
        txtviewlistname.Text = contactsToDisplay;
        //txtviewprefix.Text = prefix;
        txtViewMessage.Text = txtMessage.Text;
        Label1.Text = "Please Confirm Details Below";
        ShowMessage("Please Confirm and Continue", false);

    }

    protected void ChckedChanged(object sender, EventArgs e)
    {
        bool ischecked = chkSchedule.Checked;
        if (ischecked == true)
        {
            MultiView3.Visible = true;
            MultiView3.ActiveViewIndex = 0;
        }
        else
        {
            MultiView3.Visible = false;
        }
        //variableChar = data_file.GetVariableType("SMS_TEMPLATE");
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
    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {
        string listName = TextBox1.Text.Trim().ToUpper();
        loadLists(listName);
        RePopulateCheckBoxes();
        // Search the db for list with name for vendor
    }

    private void LoadContrls(string listCode)
    {
        string Code = encrypt.EncryptString(listCode, "PegasusSms2020");
        Response.Redirect("./PhoneNumber.aspx?transfereid=" + Code, false);
        //Response.Redirect("PhoneNumber.aspx?transfereid=" + listCode);
    }

    protected void dataGridResults_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            if (e.CommandName == "btnEdit")
            {
                string listCode = dataGridResults.Rows[index].Cells[2].Text;
                LoadContrls(listCode);
                ShowMessage(".", true);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
}