using System;
using System.Collections;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web;
using System.IO;
using Encryption;
using System.Data;
using System.Text.RegularExpressions;

public partial class MessageTemplate : Page
{
    private readonly DbAccess _db = new DbAccess();
    FileUpload uploadedFile;

    public static string RECEIPTION_TYPE_PHONE_NO = "0";
    public static string RECEIPTION_TYPE_LIST = "1";
    public static string RECEIPTION_TYPE_FILE = "2";
    public static string msgId = "";


    private readonly Processfile _processFile = new Processfile();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (IsPostBack) return;
            if ((Session["Username"] == null))
            {
                Response.Redirect("Default.aspx");
            }
           
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }



    private void ShowMessage(string Message, bool Error)
    {
        var lblmsg = (Label) Master.FindControl("lblmsg");
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
            var title = txtTitle.Text.Trim();
            var FileUpload = ddlvariable.FileName;
            var message = txtMessage.Text.Trim();

            if ( title.Equals(""))
            {
                ShowMessage("Please input the template message title", true);
            }else
            if (FileUpload.Equals(""))
            {
                ShowMessage("Please upload the file with variables", true);
            }
            else
            if (message.Equals(""))
            {
                ShowMessage("Please input the template message body", true);
            }

            else {

                string createdBy = Session["Username"].ToString();
                string VendorCode = Session["VendorCode"].ToString();
                string mask = VendorCode; // Session["Mask"].ToString();
                string filename = Path.GetFileName(ddlvariable.FileName);
                string extension = Path.GetExtension(filename);
                int credit = _processFile.GetUserCredit(VendorCode, createdBy);

                if (extension == ".csv" || extension == ".txt")
                {
                    string pathToFile = @"E:\SMSUploads\SMSTemplates";
                    DateTime todaydate = DateTime.Now;
                    string datetoday = todaydate.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "-");
                    string filepath = pathToFile + filename + "_" + datetoday + extension;
                    ddlvariable.SaveAs(filepath);
                    var lines = File.ReadAllLines(filepath);
                    var count = lines.Length;
                    if (!Directory.Exists(pathToFile))
                        Directory.CreateDirectory(pathToFile);

                    StreamReader sr = new StreamReader(filepath);
                    string[] col = sr.ReadLine().Split(',');

                    string msg = message;
                    msg = Regex.Replace(message, @"[\n \r , ! ? . # $ % ]", " ");
                    string[] words = msg.Split(' ');
                                   

                    int i = 0;
                    foreach (var word in words)
                    {

                        if (word.StartsWith("@"))
                        {
                           string variable = word.Replace("@", "");
                            string heading = col[(i+1)];
                            if (variable == heading)
                           {
                                i++; 
                            }
                           
                        }

                    }

                    if (col.Length >= i)
                    {
                        if ((count > 1) && (count < ((credit - 1))))
                        {
                            string reduce = _processFile.Reduct_credit(count);
                            if (reduce == "SAVED")
                            {
                                string result = _processFile.SaveMessageTemplate(filepath, title, message, createdBy, VendorCode, mask);

                                if (result == "success")
                                {
                                   
                                    ShowMessage("Message successully saved for processing, An email will be sent on <b>" + Session["Email"].ToString() + "</b> after processing", false);
                                    Clear_contrls();
                                }
                                else
                                {
                                    ShowMessage(result, true);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            ShowMessage("SMS Could not be sent, check your current SMS credit", true);

                        }

                    }
                    else {
                        ShowMessage("Colums in the uploaded file are less than the variables defined in the message", true);
                    }
                }
                else {
                    ShowMessage("Invalid file format, please upload either a csv or txt file", true);
                }
            } 
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private ArrayList GetNumbers(string nums)
    {
        var tels = new ArrayList();

        return tels;
    }

    private void Clear_contrls()
    {
       // ddlists.SelectedIndex = ddlists.Items.IndexOf(ddlists.Items.FindByValue("0"));
        txtMessage.Text = "";
        txtTitle.Text = "";
    }

}