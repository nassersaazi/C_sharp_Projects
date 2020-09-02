using System;
using System.IO;
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

public partial class PhoneNumber : System.Web.UI.Page
{
    private readonly Processfile _processFile = new Processfile();
    DbAccess data_file = new DbAccess();
    Processfile Process_file = new Processfile();
    DataTable data_table = new DataTable();
    PhoneValidator phone_validity = new PhoneValidator();
    DataFile df;
    private ArrayList fileContents;
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
                MultiView2.ActiveViewIndex = 0;
                
                if (Request.QueryString["transfereid"] != null)
                {
                    //string list_code = Encryption.encrypt.DecryptString(Request.QueryString["transfereid"].ToString(), "25011Pegsms2322");
                    string list_code = Encryption.encrypt.DecryptString(Request.QueryString["transfereid"].ToString(), "PegasusSms2020");
                    //string list_code = Request.QueryString["transfereid"].ToString();
                    data_table = Process_file.GetActiveList(list_code);
                    string txtvalue = "";
                    string txtname = "";
                    foreach (DataRow row in data_table.Rows)
                    {
                        txtname = row["ListName"].ToString();
                        txtvalue = row["ListID"].ToString();
                    }
                    ddllists.Items.Insert(0, new ListItem(txtname, list_code));

                    ddllists.Enabled = false;
                }
                else
                {
                    LoadLists();
                    ddllists.Enabled = true;
                    
                }
               
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
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
    private void LoadLists()
    {
        data_table = Process_file.GetActiveLists();
        ddllists.DataSource = data_table;
        ddllists.DataValueField = "ListID";
        ddllists.DataTextField = "ListName";
        ddllists.DataBind();
    }
    protected void ddllists_DataBound(object sender, EventArgs e)
    {
        ddllists.Items.Insert(0, new ListItem(" Select  group ", "0"));
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            string receipient = ddlReceipient.SelectedValue;
            string list_code = ddllists.SelectedValue.ToString();
            string phone = txtPhoneNumber.Text.Trim();
            if (list_code.Equals("0"))
            {
                ShowMessage("Select List To add Numbers to", true);
            }else
            if(receipient.Equals("0") && phone==""){
                ShowMessage("Please Enter Phone Number", true);
            }
            else
            if (receipient.Equals("0") && !phone_validity.NumberFormatIsValid(phone))
            {
                ShowMessage("Please Provide a valid Phone number", true);
            }
            else if (receipient.Equals("1") && FileUpload1.FileName.Trim().Equals(""))
            {
                ShowMessage("Browse file to Upload or Enter Phone Number", true);
            }
            else
            {
                UploadNumbers(phone);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void UploadNumbers(string phone)
    {
        if (!FileUpload1.FileName.Trim().Equals(""))
        {
            ReadFile();
        }
        else
        {
            string name = txtName.Text.Trim();
            string list_code = ddllists.SelectedValue.ToString();
            if (phone_validity.NumberFormatIsValid(phone))
            {
                string phoneNum = phone_validity.Format(phone);
                Process_file.SavePhoneNumber(phoneNum, name, list_code);

                txtName.Text = "";
                txtPhoneNumber.Text = "";
                ShowMessage(phone + " Add to " + ddllists.SelectedItem.ToString() + "  Successfully", false);
            }
            else
            {
                ShowMessage(phone + " is not a valid Phone Number", true);
            }
        }
    }
    private void ReadFile()
    {
        HttpFileCollection uploads;
        uploads = HttpContext.Current.Request.Files;
        string c = FileUpload1.FileName;
        string file_ext = Path.GetExtension(c);
        string cNoSpace = c.Replace(" ", "-");
        string User = Session["Username"].ToString().Replace(" ", "-");
        string Date = DateTime.Now.ToString().Replace("/", "-");
        Date = Date.Replace(":", "-");
        string c1 = User + "_" + Date + "_" + cNoSpace;
        c1 = c1.Replace(" ", "");
        string PathFrom = @"E:\SMSUploads\NumbersToAdd";
        Process_file.CheckPath(PathFrom);
        string FullPath = (PathFrom + "" + c1);
        FileUpload1.PostedFile.SaveAs(FullPath);

        if (file_ext == ".csv" || file_ext == ".txt")
        {
            
            int position = 0;
            df = new DataFile();
            fileContents = df.ReadFile(FullPath);
            int count = fileContents.Count;
            if (fileContents.Count <= 1000)
            {
                for (int i = 0; i < fileContents.Count; i++)
                {
                    position = i + 1;
                    string line = fileContents[i].ToString();
                    string[] sLine = line.Split(',');
                    //line = line.Replace("", "");
                    if (sLine.Length == 1 || sLine.Length == 2)
                    {
                        string phone = sLine[0].ToString();
                        if (phone_validity.NumberFormatIsValid(phone))
                        {
                            count = i + 1;
                        }
                        else
                        {
                            throw new Exception("Invalid Phone Number at line " + position);
                        }
                    }
                    else
                    {
                        throw new Exception("File Format is not OK, Columns must be 1 or 2..");
                    }

                }
                lblPath.Text = FullPath;
                Toggle(count, true);
            }
            else
            {
                lblPath.Text = FullPath;
                Toggle(count, true);
            }

        }
        else
        {
            Process_file.RemoveFile(FullPath);
            ShowMessage("File format " + file_ext + " is not supported", true);
        }
    }


    private void Toggle(int count, bool Check)
    {
        MultiView1.ActiveViewIndex = 1;
        ddllists.Enabled = false;
        lblQn.Text = "Are you sure you want to upload a file of " + count + " Number(s) to " + ddllists.SelectedItem.ToString();
    }


    protected void btnYes_Click(object sender, EventArgs e)
    {
        try
        {

            int count =0;
            var resLog = "";
            string list_code = ddllists.SelectedValue.ToString();
            string list_name = ddllists.SelectedItem.ToString();
            string FullPath = lblPath.Text.Trim();
            string VendorCode = Session["VendorCode"].ToString();
            string user = Session["Username"].ToString();
            string mask = "";// Session["Mask"].ToString();
            string SenderId = ""; //Session["Mask"].ToString();
            bool IsScheduled = false;
            DateTime today = DateTime.Now;

            df = new DataFile();
            fileContents = df.ReadFile(FullPath);
            if (fileContents.Count > 1000)
            {
                resLog = _processFile.LogSMSFileUpload(list_code, FullPath, "", VendorCode, user, mask, SenderId, "ContactUpload", IsScheduled, today);
                string msg = fileContents.Count + " Phone Number(s) have been Logged to be added (" + list_name + ")";
                MultiView1.ActiveViewIndex = 0;
                ShowMessage(msg, false);
               
            }
            else
            {
                for (int i = 0; i < fileContents.Count; i++)
                {
                    count++;
                    string phone = "";
                    string name = "";

                    string line = fileContents[i].ToString();
                    string[] sLine = line.Split(',');
                    string[] StrArray = line.Split(Convert.ToChar(","));
                    phone = StrArray[0].ToString();
                    if (sLine.Length == 2)
                    {
                        name = StrArray[1].ToString().ToUpper();
                    }
                    
                        Process_file.SavePhoneNumber(phone, name, list_code);
                    
                    MultiView1.ActiveViewIndex = 0;
                }
                string msg = count + " Phone Number(s) have been add to list(" + list_name + ")";
                ShowMessage(msg, false);
               
            }
            
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void btnNo_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
    }

    protected void ddlReceipient_SelectedIndexChanged(object sender, EventArgs e)
    {
        var prefix = ddlReceipient.SelectedValue;
        if (prefix.Equals("0"))
        {
            MultiView2.ActiveViewIndex = 0;
        }
        else if (prefix.Equals("1"))
        {
            MultiView2.ActiveViewIndex = 1;
        }
    }
}
