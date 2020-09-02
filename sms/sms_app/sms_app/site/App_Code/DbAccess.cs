using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using InterConnect.PegPay;
using Microsoft.Practices.EnterpriseLibrary.Data;

/// <summary>
///     Summary description for DbAccess
/// </summary>
public class DbAccess
{
    private readonly Database _smsDb, SmsClientDb;
    private DbCommand _command;

    public DbAccess()
    {
        try
        {
            _smsDb = DatabaseFactory.CreateDatabase(ReturnConString());
            SmsClientDb = DatabaseFactory.CreateDatabase(SmsClientConString());
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static string ReturnConString()
    {
        const string conString = "TestPegPay";
        return conString;
    }

    private static string SmsClientConString()
    {
        const string conString = "SmsClientconnect";
        return conString;
    }

    internal void LogActivity(string procedure, object[] data)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand(procedure, data);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ee)
        {
            throw ee;
        }
    }

    internal DataTable GetUserAccessibility(string username, string password)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Login", username, password);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal void ResetPassword(string username, string password,  bool reset)
    {
        try
        {
           
            _command = _smsDb.GetStoredProcCommand("ResetPassword", username, password, reset);
             _smsDb.ExecuteDataSet(_command);
          
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal DataTable ValidateResetPassword(string email)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_UserDetails", email);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable GetLists(string VendorCode, string listName)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetLists", VendorCode, listName);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveList(int listId, string listName, bool isActive, string areaCode, string user)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("Save_list", listId, listName, isActive, areaCode, user);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable Get_list(int listId)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetList", listId);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetActiveLists(string listName, string areaCode)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetActiveListsByName", listName, areaCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable GetActiveLists(string areaCode)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetActiveLists", areaCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetActiveList(string listId)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetList", listId);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetAllListsByArea(string areaCode)
    {
        try
        {
           // var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetListsByArea1", areaCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetAllLists(string areaCode)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("GetContactList", areaCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal ArrayList GetBlackListedNumbers()
    {
        throw new Exception("The method or operation is not implemented.");
    }

    internal void SavePhoneToList(string phone, string name, int listId, string areaCode, string user)
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("SavePhoneToList", phone, name, listId, user);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetListDetails(string list_id)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetListDetails", list_id);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetListContent(string VendorCode, string list_code, string phone, string name)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetListContent", VendorCode, list_code, phone, name);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
   
    internal DataTable GetTemplate(string id)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetMessageTemplateById", id);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetSystemRoles()
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_SystemRoles");
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetAreas()
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetAllVendors");
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable GetMaskTypes()
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetMaskTypes");
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetArea(string VendorCode)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVendor", VendorCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    public DataTable GetCurrentCredit(string Vendor, string user)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetSMSCredit", Vendor, user);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetUserDetails(int userid)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetUserById", userid);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetUserDetailsByUserName(string userName)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetUserDetailsByUserName", userName);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetUsers(string vendorcode, string rolecode, string name)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_Users", name, vendorcode, rolecode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetUsersActivity(string vendorcode, string name, string FromDate, string ToDate)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_UsersActivity", name, vendorcode, FromDate, ToDate);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetUsersByArea(string area_id)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetUserByVendor", area_id);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetNumbersInFile(string sourceId)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetNumbersInUploadedFile", sourceId);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetActiveListNumbers(int list_id)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetActiveListNumbers", list_id);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetAreaslist(string Vendor)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetSmsVendors", Vendor);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetVendor(string VendorCode)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVendor", VendorCode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetMasks()
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetMasks");
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public  string GetVariableType(string VariableType)
    {
        string variable = "";
        DataTable table = new DataTable();
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVariableType", VariableType);
            table = _smsDb.ExecuteDataSet(_command).Tables[0];
            variable = table.Rows[0]["Variable"].ToString();

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return variable;
    }

    public DataTable GetPresetSms(string vendorcode)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetPresetSms", vendorcode);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetNonPresetSms(string vendorcode, string mask, string status, string IsPreset, string startDate, string EndDate)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetNonPresetSms", vendorcode, mask, status, IsPreset, startDate, EndDate);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetVendorMasks(string name)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVendorMasks", name);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetVendorCredentials(string name)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVendorCredentials", name);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetActiveMasks( string MaskType)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetActiveMasks", MaskType);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetVendorActiveMasks(string VendorCode, string MaskType)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetVendorActiveMasks", VendorCode, MaskType);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal DataTable GetSmslogs(string listID, string vendorCode, string user, DateTime from_date, DateTime end_date)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetSmslogs", listID, vendorCode, user, from_date, end_date);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetSmsSent(string area_id, string user, string sent, DateTime from_date, DateTime end_date)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_SmsSent", area_id, user, sent, from_date, end_date);
            _command.CommandTimeout = 0;
            //data_table = Sms_DB.ExecuteDataSet(procommand).Tables[0];
            //return data_table;
            using (var data_table1 = _smsDb.ExecuteDataSet(_command).Tables[0])
            {
                return data_table1;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCreditHistory(string area_id, string user,  string from_date, string end_date)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_CreditHistory", area_id, user, from_date, end_date);
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetSmsSentPR(string area_id, string user, string sent, DateTime from_date, DateTime end_date)
    {
        try
        {

            _command = _smsDb.GetStoredProcCommand("Get_SmsSentPR", area_id, user, sent, from_date, end_date);
            //data_table = Sms_DB.ExecuteDataSet(procommand).Tables[0];
            //return data_table;
            using (var data_table1 = _smsDb.ExecuteDataSet(_command).Tables[0])
            {
                return data_table1;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal int GetListTotalCost(int list_id)
    {
        var total = 0;
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetListTotalCost", list_id);
            var dataTable = _smsDb.ExecuteDataSet(_command).Tables[0];
            if (dataTable.Rows.Count > 0) total = int.Parse(dataTable.Rows[0]["TotalCost"].ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return total;
    }

    internal void ChangePhoneStatus(int phoneId, bool active)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("ChangePhoneStatus", phoneId, active);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void UpdatePhoneDetails(int phone_id, string phone, string phone_name, bool active)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("UpdatePhoneDetails", phone_id, phone, phone_name, active);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void Save_user(int userId, string userName, string passwd, string fname, string lname, string phone,
        string email, string Vendor, string RoleCode, bool active, bool reset, string createdBy)
    {

        try
        {
            _command = _smsDb.GetStoredProcCommand("Save_user", userId, userName, fname, lname, passwd, phone, email,
                Vendor, RoleCode, active, createdBy);
            _smsDb.ExecuteNonQuery(_command);
            if (reset) ResetPassword(userName, passwd, true);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal void AddCredit(string user, int creditToadd, string CreditedBy, string VendorCode)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("AddCredit", user, creditToadd, CreditedBy, VendorCode);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void InsertSmsToSend(string phone, string message, string mask, string user, string areaID)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("PortalInsertSmsToSend", phone, message, mask, user, areaID);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal void InsertUploadedSMSFile(string ListId, string filePath, string message, string mask, string user, string vendorCode, string senderId, string processType, bool IsScheduled, DateTime SheduledTime)
    {
        try
        {

            object[] paramaters = new object[] { ListId, filePath, message, mask, user, vendorCode, senderId, processType };
            _command = _smsDb.GetStoredProcCommand("PortalInsertUploadedSmsFile", ListId, filePath, message, mask, user, vendorCode, senderId, processType, IsScheduled, SheduledTime);
            _smsDb.ExecuteNonQuery(_command);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal static string Generate11UniqueDigits()
    {
        var locker = new object();
        lock (locker)
        {
            Thread.Sleep(100);
            return DateTime.Now.ToString("MMddHHmmssff");
        }
    }


    public static string SignCertificate(string text)
    {
        // retrieve private key
        var certificate =
            @"E:\AirtelMoneyCerts\Pegpay-AirtelMoney.pfx"; //@"C:\PegPayCertificates\CRAFTSILICON\Client.pfx";
        var cert = new X509Certificate2(certificate, "Tingate710", X509KeyStorageFlags.UserKeySet);
        var rsa = (RSACryptoServiceProvider) cert.PrivateKey;

        // Hash the data
        var sha1 = new SHA1Managed();
        var encoding = new ASCIIEncoding();
        var data = encoding.GetBytes(text);
        var hash = sha1.ComputeHash(data);

        // Sign the hash
        var digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        var strDigCert = Convert.ToBase64String(digitalCert);
        return strDigCert;
    }

    internal void reduce_credit(int newBalance, string user)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Reduce_credit", newBalance, user);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal void Save_Mask(string Mask, string MaskName, string MaskType, string RecordedBy, bool IsActive, string Id)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Save_Mask", Mask, MaskName, MaskType, RecordedBy, IsActive, Id);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void Save_Vendor_Mask(string Mask, string Vendor, string RecordedBy)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Save_Vendor_Mask", Mask, Vendor, RecordedBy);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void Save_Vendor_Credentials(string Vendorcode, string secreteKey, string password, string RecordedBy)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Save_Vendor_Credentials", Vendorcode, secreteKey, password);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void Save_Area(string VendorName, string VendorCode, string VendorContact, string VendorEmail, bool Isactive, string VendorType, string CreatedBy)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("SaveVendorDetails", VendorName, VendorCode, VendorContact, VendorEmail, Isactive, VendorType, CreatedBy);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal void Save_PresetSms(string ShortCode, string identifier, string Smsvariables, string vendorCode, bool Active, string user, bool match, string RecordId)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Save_PresetSmsSettings", ShortCode, identifier, Smsvariables, vendorCode, Active, user, match, RecordId);
            _smsDb.ExecuteNonQuery(_command);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string GetSystemParameter(int valueCode)
    {
        var ret = "";
        try
        {
            _command = _smsDb.GetStoredProcCommand("GetSys_Parameter", valueCode);
            var dataTable = _smsDb.ExecuteDataSet(_command).Tables[0];
            if (dataTable.Rows.Count > 0) ret = dataTable.Rows[0]["ParameterValue"].ToString();
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return ret;
    }


    internal DataTable GetTotalSmsSentNew(string areaCode, string fromDate, string endDate)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_SmsSentPR", areaCode,  fromDate, endDate);
            _command.CommandTimeout = 120;
            return _smsDb.ExecuteDataSet(_command).Tables[0];
            
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    internal DataTable GetFileReport(string VendorCode, string user, string report, string from, string end)
    {
        try
        {
            _command = _smsDb.GetStoredProcCommand("Get_FileReport", VendorCode, user, report, from, end);
            _command.CommandTimeout = 120;
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetMessageTemplates(string vendorCode, string user = "", string templateTitle = "")
    {
        try
        {
            //var areaId = int.Parse(areaCode);
            _command = _smsDb.GetStoredProcCommand("PortalGetMessageTemplates1", vendorCode, templateTitle, user);
            return _smsDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal void SaveMessageTemplate(string filepath, string title, string message, string VendorCode, string createdBy, string mask)
    {

        try
        {
            _command = _smsDb.GetStoredProcCommand("PortalInsertMessageTemplate", filepath, title, message, createdBy, VendorCode, mask);
            //_command.CommandTimeout = 120;
            _smsDb.ExecuteNonQuery(_command);

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    internal void SaveVendorCredentialsV1(string Vendorcode, string secreteKey, string certPass, string password, string RecordedBy)
    {

        try
        {

            _command = _smsDb.GetStoredProcCommand("AssignCredentialsToAVendor", Vendorcode, secreteKey, certPass, password);
            _smsDb.ExecuteNonQuery(_command);

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    public DataTable GetSmsReportFromSMSClientDb(string VendorCode, string PhoneNumber, string status, DateTime StartDate, DateTime EndDate)
    {

        try
        {

            _command = SmsClientDb.GetStoredProcCommand("GetSmsByVendorCode", VendorCode, PhoneNumber, status, StartDate, EndDate);
            _command.CommandTimeout = 720;
            return SmsClientDb.ExecuteDataSet(_command).Tables[0];

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


}