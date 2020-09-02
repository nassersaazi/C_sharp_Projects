using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;

public partial class _Default : System.Web.UI.Page 
{
    Processfile process_file = new Processfile();
    DataTable data_table = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                //DisableBtnsOnClick();
                MultiView1.ActiveViewIndex = 0;
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    private void ShowMessage(string Message, bool Error)
    {
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
    private void DisableBtnsOnClick()
    {
        string strProcessScript = "this.value='Working...';this.disabled=true;";
        btnlogin.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnlogin, "").ToString());  
        btnchange.Attributes.Add("onclick", strProcessScript + ClientScript.GetPostBackEventReference(btnchange, "").ToString());         
    }
    protected void Btnlogin_Click(object sender, EventArgs e)
    {
        try
        {
            string userId = txtUsername.Text.Trim();
            string passwd = txtpassword.Text.Trim();
            if (userId.Equals(""))
            {
                ShowMessage("UserName Required", true);
                txtUsername.Focus();
            }
            else if (passwd.Equals(""))
            {
                ShowMessage("Password Required", true);
                txtpassword.Focus();
            }
            else
            {
                System_login(userId, passwd);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }

    private void System_login(string userId, string passwd)
    {
        data_table = process_file.LoginDetails(userId, passwd);

       

        if (data_table.Rows.Count > 0)
        {
            int user_Id = int.Parse(data_table.Rows[0]["UserId"].ToString());
            bool isactive = bool.Parse(data_table.Rows[0]["Active"].ToString());
            bool reset = bool.Parse(data_table.Rows[0]["Reset"].ToString());
            if (isactive)
            {
                if (reset)
                {
                    CallReset(user_Id);
                }
                else
                {
                    CallLogin(data_table);
                }
            }
            else
            {
                ShowMessage("System Logins disabled", true);
            }
        }
        else
        {
            ShowMessage("System Logon denied", true);
        }
    }

    private void CallLogin(DataTable data_table)
    {
        Session["UserId"] = data_table.Rows[0]["UserId"].ToString();
        Session["Username"] = data_table.Rows[0]["Username"].ToString();
        Session["FullName"] = data_table.Rows[0]["FullName"].ToString();
        Session["RoleCode"] = data_table.Rows[0]["RoleCode"].ToString();
        Session["UserRole"] = data_table.Rows[0]["UserRole"].ToString();
        Session["Vendor"] = data_table.Rows[0]["Vendor"].ToString();
        //Session["SenderId"] = data_table.Rows[0]["SenderId"].ToString();
       // Session["SmsCredit"] = data_table.Rows[0]["SmsCredit"].ToString();
        Session["Phone"] = data_table.Rows[0]["Phone"].ToString();
       // Session["Mask"] = data_table.Rows[0]["Mask"].ToString();
        Session["VendorCode"] = data_table.Rows[0]["VendorCode"].ToString();
        Session["VendorName"] = data_table.Rows[0]["VendorName"].ToString();
        Session["Email"] = data_table.Rows[0]["Email"].ToString();

        if (Session["Username"] != null)
        {
            process_file.LogActivity("Login", Session["Username"].ToString(), Session["VendorCode"].ToString(), "Successfull Login", "", process_file.GetLocalIPAddress().ToString());
            Response.Redirect("Admin.aspx");
            
        }
        
    }

    private void CallReset(int user_Id)
    {
         
 
        ShowMessage("System Password Reset Required, Please Reset Password and Continue", true);
        process_file.LogActivity("Login", txtUsername.Text.Trim(), "", "Failed Login using password "+txtpassword.Text.Trim(), "Password Reset", process_file.GetLocalIPAddress().ToString());
        lblUsercode.Text = user_Id.ToString();
        MultiView1.ActiveViewIndex = 1;
    }
   
 
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 0;
        ShowMessage(".", true);
    }
    protected void btnchange_Click(object sender, EventArgs e)
    {
        try
        {
            //ShowMessage("Testing", true);
            string user_code = lblUsercode.Text.Trim();
            string username = txtUsername.Text;
            string password = txtResetPasswd.Text.Trim();
            string confirm = txtResetConfirm.Text.Trim();
            bool Alphanumeric = process_file.IsAlphaNumeric(confirm);
            //if (Alphanumeric == false)
            //{
            //    ShowMessage("New Password Should be Alpha Numeric", true);
            //    txtResetConfirm.Focus();
            //}else
            if (confirm.Length < 8) {
                ShowMessage("New Password Should be minimum 8 Characters", true);
                txtResetConfirm.Focus();
            }else
            if (password.Equals(""))
            {
                ShowMessage("New Password Required", true);
                txtResetPasswd.Focus();
            }
            else if (confirm.Equals(""))
            {
                ShowMessage("Confirm Password Provide", true);
                txtResetConfirm.Focus();
            }
            else
            {
                if (!user_code.Equals("0"))
                {
                    if (password == confirm)
                    {
                        string reset_status = process_file.Reset_Passwd(username, password, false);
                        MultiView1.ActiveViewIndex = 0;
                        ShowMessage("RESET DONE SUCCESSFULLY, NOW LOGIN WITH YOUR NEW PASSWORD", false);
                        process_file.LogActivity("PasswordReset", username, "", "Success Password Reset", "Password Reset", process_file.GetLocalIPAddress());
                    }
                    else
                    {
                        process_file.LogActivity("PasswordReset", username, "", "Failed Password Reset", "Password Reset", process_file.GetLocalIPAddress());
                        ShowMessage("Passwords dont match", true);
                        txtResetPasswd.Focus();
                    }
                }
                else
                {
                    ShowMessage("System failed to alocate User Id", true);
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }
    protected void btnlogin_Click(object sender, EventArgs e)
    {
        try
        {
            
            //string pass = Encryption.encrypt.DecryptString("jXU+xuRePS4KqcaRDwPW4Q==", "25011Pegsms2322");
            string userId = txtUsername.Text.Trim();
            string passwd = txtpassword.Text.Trim();
            if (userId.Equals(""))
            {
                ShowMessage("UserName Required", true);
                txtUsername.Focus();
            }
            else if (passwd.Equals(""))
            {
                ShowMessage("Password Required", true);
                txtpassword.Focus();
            }
            else
            {
                System_login(userId, passwd);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message, true);
        }
    }


    protected void btnlogin_Forgot(object sender, EventArgs e)
    {
        MultiView1.ActiveViewIndex = 2;
    }


    protected void btnpassword_Reset(object sender, EventArgs e)
    {

        string email = txtEmail.Text;
            if (email == "")
            {
                ShowMessage("Please Enter Your Email", true);
                txtEmail.Focus();
            }
            else
            {

                data_table = process_file.ValidateResetPassword( email);

                if (data_table.Rows.Count == 1)
                {

                    foreach (DataRow row in data_table.Rows)
                    {
                        string userName = row["Username"].ToString();
                        string passwd = process_file.RandomString();
                        string fname = row["FirstName"].ToString();
                        string lname = row["LastName"].ToString();
                        string vendorCode = row["Vendor"].ToString();
                        bool reset = true;
                        string passreset = process_file.ResetPassword(userName, passwd, reset);
                        if (passreset == "0")
                        {
                            SendMail.ResetUserCredentials(userName, passwd, fname, lname, email, vendorCode);
                            ShowMessage("Password Was Successfully Reset, Check your email for the new password", false);
                            MultiView1.ActiveViewIndex = 0;
                        }
                        else
                        {
                            process_file.LogActivity("PasswordReset", userName, "", "Failed Password Reset using username " + userName + " and Email " + email, "Password Reset", process_file.GetLocalIPAddress());
                            ShowMessage("Password Rest Failed, Please try again", true);
                        }
                    }

                }
                else
                {
                    process_file.LogActivity("PasswordReset", email, "", "Failed Password Reset using Email " + email, "Password Reset", process_file.GetLocalIPAddress());
                    ShowMessage("Password Rest Failed, Invalid details supplied", true);

                }
            }
    }



}
