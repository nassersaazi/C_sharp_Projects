using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class SmsVendorCredentials
{

    public static string VENDOR_TYPE_PREPAID = "PREPAID";
    public static string VENDOR_TYPE_POSTPAID = "POSTPAID";

    public string vendorType = "";
    public string vendorCode = "";
    public string vendorPassword = "";
    public string postpaidSecretKey = "";
    public string prepaidCertificatePath = "";
    public string prepaidCertificatePassword = "";
    public string assignedBy = "";
    
}