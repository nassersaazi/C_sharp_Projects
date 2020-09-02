using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using Encryption;
using InterConnect.PegPay;
using InterConnect.SMSService;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

/// <summary>
///     Summary description for Processfile
/// </summary>
public class Processfile
{
    private readonly DbAccess _db = new DbAccess();
    private readonly DataFile _df = new DataFile();
    private ArrayList _fileContent;
    private PhoneValidator _phoneValidator;
    private SMSService _smsApi;

    public void LogActivity(string action, string Username, string VendorCode, string Message, string Resource,string ipAddress)
    {
        object[] data = { action, Username, VendorCode, Message, Resource, ipAddress };
        _db.LogActivity("LogActivity", data);
       
    }

    public DataTable LoginDetails(string username, string password)
    {
       
        password = HashPassword(password);
        var dataTable = _db.GetUserAccessibility(username, password);
        return dataTable;
    }

    public DataTable ValidateResetPassword(string email)
    {
        // password = EncryptString(password);
        var dataTable = _db.ValidateResetPassword(email);
        return dataTable;
    }

    public string ResetPassword(string username, string password, bool reset)
    {
        string resett = "";
        try
        {
            object[] data = { "Reset Password", username, "", "Reset Password Attempt", "", GetLocalIPAddress() };
            _db.LogActivity("LogActivity", data);
            password = HashPassword(password);
            _db.ResetPassword(username, password, reset);
            resett = "0";
        }
        catch(Exception ex) {
            throw ex;
             
        }

        return resett;
    }

    public string GetLocalIPAddress()
    {
        string hostName = Dns.GetHostName(); // Retrive the Name of HOST  

        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

        return myIP;
    }

    public string EncryptString(string clearText)
    {
        try
        {
            var ret = "";
            ret = encrypt.EncryptString(clearText, "25011Pegsms2322");
            return ret;
        }catch(Exception ex){
            throw ex;
            
        }
    }

    public string DecryptString(string hashedText)
    {
        try
        {
            var ret = "";
            ret = Encryption.encrypt.DecryptString(hashedText, "25011Pegsms2322");//encrypt.EncryptString(clearText, "25011Pegsms2322");
            return ret;
        }catch(Exception ex){
            throw ex;
            
        }
    }

    public string Reset_Passwd(string userCode, string password, bool reset)
    {
        var userId = userCode;
        object[] data = { "Reset Password", userCode, "", "Reset Passeord attempt", "", GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);
        password = HashPassword(password);
        _db.ResetPassword(userId, password, reset);
        return "RESET";
    }


    public string RandomString()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[8];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }

    public int GetUserCredit(string Vendor, string user)
    {
        var credit = 0;
        var dataTable = _db.GetCurrentCredit(Vendor, user);
        if (dataTable.Rows.Count > 0) credit = int.Parse(dataTable.Rows[0]["SMSBalance"].ToString());
        return credit;
    }

    public string SaveList(string listCode, string listName, bool isActive)
    {
        var listId = int.Parse(listCode);
        var areaCode = HttpContext.Current.Session["VendorCode"].ToString();
        var user = HttpContext.Current.Session["Username"].ToString();
        object[] data = { "Save Contact Group", user, areaCode, "Created or Edited a contact group of Group Id"+listId, listName, GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);
        _db.SaveList(listId, listName, isActive, areaCode, user);
        return listId.Equals(0) ? "SAVED" : "EDITED";
    }

    public DataTable GetVendorMasks( string Vendorcode, string MaskType)
    {
        var dataTable = _db.GetVendorActiveMasks(Vendorcode, MaskType);
        return dataTable;
    }

    public DataTable GetActiveLists()
    {
        var areaCode = HttpContext.Current.Session["VendorCode"].ToString();
        var dataTable = _db.GetActiveLists(areaCode);
        return dataTable;
    }

    public DataTable GetActiveLists(string listname, string vendor)
    {
        var dataTable = _db.GetActiveLists(listname, vendor);
        return dataTable;
    }

    public DataTable GetActiveList(string listId)
    {
        //var areaCode = HttpContext.Current.Session["VendorCode"].ToString();
        var dataTable = _db.GetActiveList(listId);
        return dataTable;
    }
    public DataTable GetAllLists()
    {
        var areaCode = "";
        if (HttpContext.Current.Session["RoleCode"].ToString() == "001")
        {
            areaCode = "";
        }
        else {
            areaCode = HttpContext.Current.Session["VendorCode"].ToString();
        }
        var dataTable = _db.GetAllLists(areaCode);
        return dataTable;
    }

    public DataTable GetVendorLists(string VendorCode)
    {
        var dataTable = _db.GetAllLists(VendorCode);
        return dataTable;
    }

    public void CheckPath(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    public void RemoveFile(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }

    public void SavePhoneNumber(string phone, string name, string list_code)
    {
        try
        {
            var areaCode = HttpContext.Current.Session["VendorCode"].ToString();
            var user = HttpContext.Current.Session["Username"].ToString();
            var listId = int.Parse(list_code);
            phone = FormatPhone(phone);
            _db.SavePhoneToList(phone, name.ToUpper(), listId, areaCode, user);
            object[] data = { "Save Phone Number", user, areaCode, "Saved Phone Number "+phone+" to listId "+listId+" and ListName "+name, "", GetLocalIPAddress() };
            _db.LogActivity("LogActivity", data);
        }
        catch(Exception ex) {
            throw ex;
        }

    }

    public DataTable GetListDetails(string listID)
    {
       // var listId = int.Parse(listCode);
        var dataTable = _db.GetListDetails(listID);
        return dataTable;
    }

    public DataTable GetListContent(string VendorCode, string list_code, string phone, string name)
    {
        // var listId = int.Parse(listCode);
        var dataTable = _db.GetListContent(VendorCode, list_code, phone, name);
        return dataTable;
    }
    public DataTable GetMessageTemplate(string id)
    {
        var dataTable = _db.GetTemplate(id);
        return dataTable;
    }
    public void ChangePhoneStatus(string phoneCode, string status)
    {
        var phoneId = int.Parse(phoneCode);
        var active = status.Equals("NO");
        _db.ChangePhoneStatus(phoneId, active);
    }

    public void UpdatePhoneDetails(string phoneCode, string phone, string phoneName, bool active)
    {
        var phoneId = int.Parse(phoneCode);
        _db.UpdatePhoneDetails(phoneId, phone, phoneName, active);
    }

    public string SaveUser(string userCode, string userName, string fname, string lname, string phone, string email, string vendorCode, string userRole, bool active, bool reset)
    {

        var output = "";
        var userId = int.Parse(userCode);
        var createdBy = HttpContext.Current.Session["Username"].ToString();
        var areaCode = HttpContext.Current.Session["VendorCode"].ToString();
        //userName = GetUserName(fname, lname, userName);

        if ((userId>0) && (reset == false))
        {
            _db.Save_user(userId, userName, "", fname, lname, phone, email, vendorCode, userRole, active, reset, createdBy);
            output =  "User details Updated Successfully" ;
           
        }else
        if ((userId > 0) && (reset == true)) {
            var passwd = RandomString();
            _db.Save_user(userId, userName, HashPassword(passwd), fname, lname, phone, email, vendorCode, userRole, active, reset, createdBy);
            SendMail.sendUserCredentialsToTheirEmail(userName, passwd, fname, lname, email, vendorCode);
            output = "User Credentials have Successfully been reset";
        }
        else
        {
            var passwd = RandomString();
            _db.Save_user(userId, userName, HashPassword(passwd), fname, lname, phone, email, vendorCode, userRole, active, reset, createdBy);

           
                //Send the user credentials to their email
                SendMail.sendUserCredentialsToTheirEmail(email, passwd, fname, lname, email, vendorCode);
            

            output = userCode.Equals("0") ? "USER SAVED SUCCESSFULLY" : "USER DETAILS UPDATED SUCCESSFULLY";
            
            object[] data = { "Reset User Credentials", createdBy, areaCode, "Reset user's Credentials User=" + userName + " IsReset=" + reset, "", GetLocalIPAddress() };
            _db.LogActivity("LogActivity", data);
        }

        return output;
    }


    public bool UserNameExists(string userName)
    {
        try
        {
            var dataTable = _db.GetUserDetailsByUserName(userName);
            if (dataTable.Rows.Count > 0)
            {
                return true;
            }
            else {
                return false;
            }
        }
        catch (Exception ex){
            throw ex;

        }
    }

    public string RandomNumber() {
        Random rnd = new Random();
        return rnd.Next(1, 9).ToString();
    }


    public string GetUserName(string fname, string sname, string userName)
    {
        var ret = "";
        if (userName.Equals(""))
        {
            var initial = fname.Substring(0, 1);
            ret = initial + "." + sname;
            ret = ret.ToLower();
        }
        else
        {
            ret = userName;
        }

        return ret;
    }

    public DataTable GetUsers(string vendor_code, string role_code, string name)
    {
        try
        {
            var dataTable = _db.GetUsers(vendor_code, role_code, name);
            return dataTable;
        }catch(Exception ex){
            throw ex;
            
        }
    }

    public DataTable GetUsersActivity(string vendor_code, string name, string FromDate, string ToDate)
    {
        try
        {
            var dataTable = _db.GetUsersActivity(vendor_code, name, FromDate, ToDate);
            return dataTable;
        }catch(Exception ex){
            throw ex;
        }
    }

    public string AddCredit(string user, string credit, string CreditedBy, string VendorCode)
    {
        var output = "";
        try
        {
           
            var creditToadd = int.Parse(credit);
            if (creditToadd.Equals(0))
            {
                output = "Credit to add cannot be zero";
            }
            else
            {

                _db.AddCredit(user, creditToadd, CreditedBy, VendorCode);
                output = creditToadd + " SMS  HAVE BEEN CREDITED SUCCESSFULLY TO USER " + user;
            }
        }catch(Exception ex){
            throw ex;
            output = "Failed "+ex ;
        }

        return output;
    }

    public bool SufficientCredit(string listCode)
    {
        var listId = int.Parse(listCode);
        var listTotal = _db.GetListTotalCost(listId);
       // var credit = GetUserCredit();
        //if (credit >= listTotal)
            return true;
        //return false;
    }

    public string LogSMSCommaSeparatedList( string[] phonesArr, string message, string VendorCode, string mask)
    {

        var output = "";
        _phoneValidator = new PhoneValidator();

        var arrayPhones = phonesArr;

        //List has phone numbers
        if (arrayPhones.Length > 0)
        {
            var user = HttpContext.Current.Session["Username"].ToString();
            //var mask = HttpContext.Current.Session["Mask"].ToString();
            var mail = HttpContext.Current.Session["Email"].ToString();
            int smscredit = GetUserCredit(VendorCode, user);
            if (arrayPhones.Length < smscredit)
            {
                string reduce = Reduct_credit(arrayPhones.Length);
                if (reduce == "SAVED")
                {
                    
                    for (var i = 0; i < arrayPhones.Length; i++)
                    {

                        if (_phoneValidator.NumberFormatIsValid(arrayPhones[i].Trim()))
                        {
                            var phone = _phoneValidator.Format(arrayPhones[i].Trim());
                            _db.InsertSmsToSend(phone, message, mask, user, VendorCode);
                            output += "SMS to Phone Number" + arrayPhones[i].Trim() + " logged Successfully <br/>";
                        }
                        else {
                            output += "Invalid Number " + arrayPhones[i].Trim() + " Was Provided and not processed <br/>";
                        }

                    }       
                   
                }
            }
            else
            {
                output = "Dear customer, Your Current SMS Limit is too low to send these SMS(s)";
            }
        }
        else
        {
            output = "No Active Phone number(s) Entered";
        }

        return output;
    }

    //public string LogSMSContactGroup(string listCode, string otherPhones, string message, string VendorCode)
    //{
    //    var output = "";
    //    _phoneValidator = new PhoneValidator();
    //    var listId = int.Parse(listCode);

    //    var arrayPhones = otherPhones.Split(',');
    //    var dt = _db.GetActiveListNumbers(listId);

    //    if (dt.Rows.Count > 0 || arrayPhones.Length > 0)
    //    {
    //        var user = HttpContext.Current.Session["Username"].ToString();
    //        var mask = HttpContext.Current.Session["Mask"].ToString();
    //        var i = 0;
    //        var count = 0;
    //        foreach (DataRow dr in dt.Rows)
    //        {
    //            var phone = _phoneValidator.Format(dr["PhoneNumber"].ToString().Trim());
    //            var name = dr["PhoneName"].ToString();

    //            count++;

    //            //Reduct_credit(listId, message, mask, user, phone);
    //        }

    //        if (count > 0) i = 1;

    //        for (; i < arrayPhones.Length; i++, count++)
    //        {
    //            var phone = _phoneValidator.Format(arrayPhones[i].Trim());
                

    //            //Reduct_creditforOtherPhones(arrayPhones, message, mask, user, "YES");
    //        }

    //        output = "A list of " + count + " has been logged Successfully";
    //    }
    //    else
    //    {
    //        output = "No Active Phone number(s) on list";
    //    }

    //    return output;
    //}

    public string LogSMSFileUpload(string listId, string pathToFIle, string message, string VendorCode, string user,string mask, string senderId, string processType, bool IsScheduled, DateTime SheduledTime)
    {

        var output = ""; 

        try
        {

            new DbAccess().InsertUploadedSMSFile(listId, pathToFIle, message, mask, user, VendorCode, senderId, processType, IsScheduled, SheduledTime);

           output = "SMS Records saved Successfully for processing";

        }
        catch (Exception ex)
        {
            output = "Failed to save SMS records with error "+ex.Message;
        }
           

        return output;

    }

    public string Reduct_credit(int total)
    {
        try
        {
            var user = HttpContext.Current.Session["Username"].ToString();
            var vendor = HttpContext.Current.Session["VendorCode"].ToString();
            var credit = GetUserCredit(vendor, user);
            int newBalance = credit - total;
            _db.reduce_credit(newBalance, user);
            return "SAVED";
        }
        catch (Exception ex) {
            throw ex;
        }
    }


    public string FormatPhone(string phone)
    {
        if (phone.Trim().StartsWith("256") && phone.Trim().Length == 12)
        {
            phone = phone.Remove(0, 3);
            phone = "0" + phone;
        }
        else if (phone.Trim().StartsWith("+256") && phone.Trim().Length == 13)
        {
            phone = phone.Remove(0, 4);
            phone = "0" + phone;
        }
        else if ((phone.Trim().StartsWith("7") || phone.Trim().StartsWith("3")) && phone.Trim().Length == 9)
        {
            phone = "0" + phone;
        }
        else if ((phone.StartsWith("07") || phone.StartsWith("03")) && phone.Trim().Length == 10)
        {
            phone = phone.Remove(0, 1);
            phone = "256" + phone;
        }

        return phone;
    }

    public string Change_Passwd(string oldPasswd, string newPasswd, string confirm)
    {
        var output = "";
        var userId = HttpContext.Current.Session["Username"].ToString();
        var vendor = HttpContext.Current.Session["VendorCode"].ToString();
        if (newPasswd == confirm)
        {
            
            //oldPasswd = EncryptString(oldPasswd);
            var dataTable = LoginDetails(userId, oldPasswd);
            if (dataTable.Rows.Count > 0)
            {
                newPasswd = HashPassword(newPasswd);
                var usercode = HttpContext.Current.Session["UserID"].ToString();
                var user_id = int.Parse(usercode);
                _db.ResetPassword(userId, newPasswd, false);
                output = "Password Changed Successfully";
            }
            else
            {
                output = "Invalid Old Password";
            }
        }
        else
        {
            output = "Passwords do not match";
        }

        object[] data = { "Change Password", userId, vendor, "Change Users Passord and Response=" + output + " IsReset=", "", GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);
        return output;
    }

    public string Save_Mask(string Mask, string MaskName, string MaskType, string RecordedBy, bool IsActive, string Id)
    {
        string result = "";
        var vendor = HttpContext.Current.Session["VendorCode"].ToString();
        try
        {
            _db.Save_Mask(Mask, MaskName, MaskType, RecordedBy, IsActive, Id);
            result = "SAVED";
        }catch(Exception ex){
            throw ex;
            result = "Failed: " + ex;
        }

        object[] data = { "Save Mask", RecordedBy, vendor, "Created or edited New Mask with Mask Code=" + Mask + " Mask Name="+MaskName+" Mask type="+MaskType+" RecordId="+Id+" Result =", "", GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);

        return result;
    }

    public string Save_Vendor_Mask(string Mask, string Vendor, string RecordedBy)
    {
        string result = "";
        try
        {
            _db.Save_Vendor_Mask(Mask, Vendor, RecordedBy);
            result= "SAVED";
        }
        catch (Exception ex)
        {
            throw ex;
            result = "Failed: "+ex;
        }
        return result;
    }

    public string Save_Vendor_Credentials(string Vendorcode, string secreteKey, string password, string RecordedBy)
    {
        try
        {
            _db.Save_Vendor_Credentials(Vendorcode, secreteKey, password, RecordedBy);
            object[] data = { "Save_Vendor_Credentials", RecordedBy, Vendorcode, "Assigned Vendor Credentials to Vendor="+Vendorcode, "", GetLocalIPAddress() };
            _db.LogActivity("LogActivity", data);
            return "SAVED";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public string Save_Area(string VendorName, string VendorCode, string VendorContact, string VendorEmail, bool Isactive, string VendorType, string CreatedBy)
    {
        try
        {
            _db.Save_Area(VendorName, VendorCode, VendorContact, VendorEmail,  Isactive, VendorType, CreatedBy);
            return "SAVED";
        }catch(Exception ex){
            throw ex;
            return ex.ToString();
        }
    }

    public DateTime ReturnDate(string date, int type)
    {
        DateTime dates;

        if (type == 1)
        {
            if (date == "")
                dates = DateTime.Parse("January 1, 2012");
            else
                dates = DateTime.Parse(date);
        }
        else
        {
            if (date == "" || date == " ")
                dates = DateTime.Now;
            else
                dates = DateTime.Parse(date);
        }

        return dates;
    }

    public DataTable GetSmslogs(string listID, string vendorCode, string user, string from, string end)
    {
        var from_date = ReturnDate(from, 1);
        var end_date = ReturnDate(end, 2);
        var dataTable = _db.GetSmslogs(listID, vendorCode, user, from_date, end_date);
        return dataTable;

    }

    public DataTable GetSmsSent(string area_code, string user, string sent, string from, string end)
    {
        var from_date = ReturnDate(from, 1);
        var end_date = ReturnDate(end, 2)  ;
        using (var data_table1 = _db.GetSmsSent(area_code, user, sent, from_date, end_date))
        {
            return data_table1;
        }
    }

    public DataTable GetCreditHistory(string area_code, string user, string from_date, string end_date)
    {
        var dataTable = _db.GetCreditHistory(area_code, user, from_date, end_date);
        return dataTable;
    }

    public DataTable GetSmsSentPR(string area_code, string user, string sent, string from, string end)
    {
        var from_date = ReturnDate(from, 1);
        var end_date = ReturnDate(end, 2);
        using (var data_table1 = _db.GetSmsSentPR(area_code, user, sent, from_date, end_date))
        {
            return data_table1;
        }
    }


    public void UpdateUserCredit(string username, int newCredit)
    {
        try
        {
            _smsApi = new SMSService();
            _smsApi.UpdateUserCredit(username, newCredit);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public string Save_PresetSms(string ShortCode, string identifier, string response, string vendorCode, bool Active, string user, bool match, string RecordId)
    {
        string result = "";
        try
        {
            _db.Save_PresetSms(ShortCode, identifier, response, vendorCode, Active, user,match, RecordId);
            result= "SAVED";
        }catch(Exception ex){
            result= "Failed: " + ex;
        }

        object[] data = { "Save Preset SMS", user, vendorCode, "Added or Edited a new Preset Rule on ShortCode=" + ShortCode, "", GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);

        return result;
    }

    public string SaveMessageTemplate(string filepath, string title, string message, string createdBy, string VendorCode, string mask)
    {
        string result = "";
        try
        {

            _db.SaveMessageTemplate(filepath, title, message, VendorCode, createdBy, mask);
            result = "SMS Template File Uploaded Successfully, You will be notified when processing is done.";
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        object[] data = { "Save Message Template", createdBy, VendorCode, "Created new SMS Template for  Mask=" + mask, "", GetLocalIPAddress() };
        _db.LogActivity("LogActivity", data);

        return result;
    }

    public DataTable GetMessageTemplates()
    {
        var vendorCode = HttpContext.Current.Session["VendorCode"].ToString();
        var dataTable = _db.GetMessageTemplates(vendorCode);
        return dataTable;
    }


    public DataTable GetMessageTemplates(string templateTitle, string user)
    {
        var vendorCode = HttpContext.Current.Session["VendorCode"].ToString();
        var dataTable = _db.GetMessageTemplates(vendorCode, user, templateTitle);
        return dataTable;
    }

    public DataTable GetTotalSmsSentNew(string areaCode, string from, string end)
    {
        var fromDate = ReturnDate(from, 1);
        var endDate = ReturnDate(end, 2);
        var fromDate1 = formatDate(fromDate);
        var endDate1 = formatDate(endDate);
        var dataTable = _db.GetTotalSmsSentNew(areaCode, fromDate1, endDate1);
        return dataTable;
    }

    public DataTable GetFileReport(string VendorCode, string user, string report, string from, string end)
    {
        var fromDate = ReturnDate(from, 1).ToString(); ;
        var endDate = ReturnDate(end, 2).ToString(); ;

        var dataTable = _db.GetFileReport(VendorCode, user, report, fromDate, endDate);
        return dataTable;
    }
    public string formatDate(DateTime Date)
    {
        var newDate = Date.ToString().Split(' ');
        var dateOnly = newDate[0];
        var dateParts = dateOnly.Split('/');
        var month = dateParts[0];
        var day = dateParts[1];
        var year = dateParts[2];
        var fulldate = year + '-' + month + '-' + day;
        return fulldate;
    }

    public bool IsAlphaNumeric(string strToCheck)
    {
        bool status = false;
        string[] array = { strToCheck };
        StringBuilder sb = new StringBuilder();
        foreach (string str in array)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^[a-zA-Z0-9]+$"))
                status = true;
            else
                status = false;
        }
        return status;
        //Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
        //return !objAlphaNumericPattern.IsMatch(strToCheck);
    }

    public string SaveSmsVendorCredentials(SmsVendorCredentials credentials)
    {

        try
        {

            string certPathOrSecretKey = credentials.vendorType == SmsVendorCredentials.VENDOR_TYPE_PREPAID ?
                                         credentials.prepaidCertificatePath : credentials.postpaidSecretKey;

            _db.SaveVendorCredentialsV1(credentials.vendorCode, certPathOrSecretKey, credentials.prepaidCertificatePassword, credentials.vendorPassword, credentials.assignedBy);


            object[] data = { "Save_Vendor_Credentials", credentials.assignedBy, credentials.vendorCode, "Assigned Vendor Credentials to Vendor=" + credentials.vendorCode, "", GetLocalIPAddress() };
            _db.LogActivity("LogActivity", data);
            return "SAVED";

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
    
    public string HashPassword(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("X2", CultureInfo.CurrentCulture));
            return sb.ToString();
        }
    }

    public string DecryptPass(string opassword)
    {
        string pp = encrypt.DecryptString(opassword, "25011Pegsms2322");
        return pp;
    }

}