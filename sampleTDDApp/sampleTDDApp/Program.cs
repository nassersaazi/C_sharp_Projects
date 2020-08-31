using sampleTDDApp.Logic;
using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Cryptography;

namespace sampleTDDApp
{
    public class Program
    {
        class Payment
        {
           
            public PostResponse MakeNWSCPayment(NWSCTransaction trans)
            {
                PostResponse resp = new PostResponse();
                DatabaseHandler dp = new DatabaseHandler();
                BusinessLogic bll = new BusinessLogic();
                PhoneValidator pv = new PhoneValidator();
                if (trans.CustomerTel == null)
                {
                    trans.CustomerTel = "";
                }
                if (trans.Email == null)
                {
                    trans.Email = "";
                }
                string vendorCode = trans.VendorCode;
                try
                {
                    dp.SaveRequestlog(trans.VendorCode, "NWSC", "POSTING", trans.CustRef, trans.Password);
                    if (trans.CustName == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "13";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.CustName.Trim().Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "13";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Area == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "35";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Area.Trim().Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "35";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.TransactionType == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "14";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.TransactionType.Trim().Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "14";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.VendorTransactionRef == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "16";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.VendorTransactionRef.Trim().Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "16";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Teller == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "17";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Teller.Trim().Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "17";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.DigitalSignature == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "19";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.DigitalSignature.Length == 0)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "19";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (!IsValidReversalStatus(trans))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "25";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "22";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "22";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Reversal == "1" && trans.Narration == null)
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "23";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else if (trans.Reversal == "1" && trans.Narration.Equals(""))
                    {
                        resp.PegPayPostId = "";
                        resp.StatusCode = "23";
                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    }
                    else
                    {
                        if (bll.IsNumeric(trans.TransactionAmount))
                        {
                            if (bll.IsValidDate(trans.PaymentDate))
                            {
                                DataTable vendaData = dp.GetVendorDetails(trans.VendorCode);
                                if (isValidVendorCredentials(trans.VendorCode, trans.Password, vendaData))
                                {
                                    if (isActiveVendor(trans.VendorCode, vendaData))
                                    {
                                        if (isSignatureValid(trans))
                                        {
                                            if (pv.PhoneNumbersOk(trans.CustomerTel))
                                            {
                                                if (!IsduplicateVendorRef(trans))
                                                {
                                                    if (!IsduplicateCustPayment(trans))
                                                    {
                                                        trans.Reversal = GetReversalState(trans);
                                                        if (HasOriginalEntry(trans))
                                                        {
                                                            if (ReverseAmountsMatch(trans))
                                                            {
                                                                if (!IsChequeBlacklisted(trans))
                                                                {

                                                                    string vendorType = vendaData.Rows[0]["VendorType"].ToString();
                                                                    if (!(vendorType.Equals("PREPAID")))
                                                                    {
                                                                        UtilityCredentials creds = dp.GetUtilityCreds("NWSC", trans.VendorCode);
                                                                        if (!creds.UtilityCode.Equals(""))
                                                                        {
                                                                            if (string.IsNullOrEmpty(trans.CustomerType))
                                                                            {
                                                                                trans.CustomerType = "";
                                                                            }
                                                                            resp.PegPayPostId = dp.PostTransaction(trans, "NWSC");
                                                                            resp.StatusCode = "0";
                                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);

                                                                        }
                                                                        else
                                                                        {
                                                                            resp.PegPayPostId = "";
                                                                            resp.StatusCode = "29";
                                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        resp.PegPayPostId = "";
                                                                        resp.StatusCode = "29";
                                                                        resp.StatusDescription = "NOT ENABLED FOR PREPAID VENDORS";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    resp.PegPayPostId = "";
                                                                    resp.StatusCode = "29";
                                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                resp.PegPayPostId = "";
                                                                resp.StatusCode = "26";
                                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            resp.PegPayPostId = "";
                                                            resp.StatusCode = "24";
                                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                        }

                                                    }
                                                    else
                                                    {
                                                        resp.PegPayPostId = "";
                                                        resp.StatusCode = "21";
                                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                    }
                                                }
                                                else
                                                {
                                                    resp.PegPayPostId = "";
                                                    resp.StatusCode = "20";
                                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                                }
                                            }
                                            else
                                            {
                                                resp.PegPayPostId = "";
                                                resp.StatusCode = "12";
                                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                            }
                                        }
                                        else
                                        {
                                            resp.PegPayPostId = "";
                                            resp.StatusCode = "18";
                                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                        }
                                    }
                                    else
                                    {
                                        resp.PegPayPostId = "";
                                        resp.StatusCode = "11";
                                        resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                    }
                                }
                                else
                                {
                                    resp.PegPayPostId = "";
                                    resp.StatusCode = "2";
                                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                                }
                            }
                            else
                            {
                                resp.PegPayPostId = "";
                                resp.StatusCode = "4";
                                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                            }
                        }
                        else
                        {
                            resp.PegPayPostId = "";
                            resp.StatusCode = "3";
                            resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                        }
                    }
                    if (resp.StatusCode.Equals("2"))
                    {
                        DataTable dt = dp.GetVendorDetails(vendorCode);
                        if (dt.Rows.Count != 0)
                        {
                            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                            string strLoginCount = dt.Rows[0]["InvalidLoginCount"].ToString();
                            int loginCount = int.Parse(strLoginCount);
                            loginCount = loginCount + 1;
                            if (loginCount == 3)
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                                dp.DeactivateVendor(vendorCode, ipAddress);
                            }
                            {
                                dp.UpdateVendorInvalidLoginCount(vendorCode, loginCount, ipAddress);
                            }
                        }
                    }
                }
                catch (System.Net.WebException wex)
                {

                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                    dp.LogError(wex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                }
                catch (SqlException sqlex)
                {
                    resp.StatusCode = "31";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                }
                catch (Exception ex)
                {
                    resp.StatusCode = "32";
                    resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
                    dp.deleteTransaction(resp.PegPayPostId, resp.StatusDescription);
                    resp.PegPayPostId = "";
                    dp.LogError(ex.Message, trans.VendorCode, DateTime.Now, "NWSC");
                }
                return resp;
            }

            private bool IsChequeBlacklisted(Transaction trans)
            {

                if (trans.TransactionType.ToUpper().Contains("CHEQUE"))
                {
                    DatabaseHandler dp = new DatabaseHandler();
                    DataTable dt = dp.CheckBlacklist(trans.CustRef);
                    if (dt.Rows.Count > 0)
                    {
                        string status = dt.Rows[0]["ChequeBlackListed"].ToString();
                        if (status.Equals("1"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            private bool ReverseAmountsMatch(Transaction trans)
            {
                DatabaseHandler dp = new DatabaseHandler();
                if (trans.Reversal.Equals("0"))
                {
                    return true;
                }
                else
                {
                    DataTable dt = dp.GetOriginalVendorRef(trans);
                    if (dt.Rows.Count > 0)
                    {
                        double amount = double.Parse(trans.TransactionAmount);
                        double amountToreverse = double.Parse(dt.Rows[0]["TranAmount"].ToString());
                        double total = amountToreverse + amount;
                        if (total.Equals(0))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            private bool HasOriginalEntry(Transaction trans)
            {
                DatabaseHandler dp = new DatabaseHandler();
                if (trans.Reversal.Equals("0"))
                {
                    return true;
                }
                else
                {
                    DataTable dt = dp.GetOriginalVendorRef(trans);
                    if (dt.Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            private string GetReversalState(Transaction trans)
            {
                string res = "";
                if (trans.Reversal != null)
                {
                    double amt = double.Parse(trans.TransactionAmount);
                    string amountstr = amt.ToString();
                    int amount = int.Parse(amountstr);
                    if (amount > 0)
                    {
                        res = "0";
                    }
                    else
                    {
                        res = "1";
                    }
                }
                return res;
            }

            private bool IsduplicateCustPayment(Transaction trans)
            {
                if (trans.VendorCode.Trim().ToUpper() == "MTN")
                {
                    return false;
                }
                bool ret = false;
                DatabaseHandler dp = new DatabaseHandler();
                string custRef = trans.CustRef;
                double amount = double.Parse(trans.TransactionAmount);
                DateTime postDate = DateTime.Now;
                DataTable dt = dp.GetDuplicateCustPayment(trans.VendorCode, custRef, amount, postDate);
                if (dt.Rows.Count > 0)
                {
                    DateTime Postdate = DateTime.Parse(dt.Rows[0]["RecordDate"].ToString());
                    TimeSpan t = postDate.Subtract(Postdate);
                    int tmdiff = t.Minutes;
                    if (tmdiff < 10)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    ret = false;
                }
                return ret;
            }

            private bool IsduplicateVendorRef(Transaction trans)
            {
                bool ret = false;
                DatabaseHandler dp = new DatabaseHandler();
                DataTable dt = dp.GetDuplicateVendorRef(trans);
                if (dt.Rows.Count > 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
                return ret;
            }

            private bool isSignatureValid(Transaction trans)
            {
                bool valid = false;
                try
                {
                    DatabaseHandler dp = new DatabaseHandler();
                    BusinessLogic bll = new BusinessLogic();
                    if (trans.VendorCode.Equals("MTN"))
                    {
                        valid = true;
                        return valid;
                    }
                    else if (trans.VendorCode.Equals("AFRICELL"))
                    {
                        valid = true;
                        return valid;
                    }
                    else if (trans.VendorCode.Equals("TESTFLEXIPAY"))
                    {
                        if (trans.UtilityCode.Equals("MOWE"))
                        {
                            valid = true;
                            return valid;
                        }

                    }
                    else if (trans.VendorCode.Equals("EzeeMoney"))
                    {
                        string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                        string certPath = "C:\\PegPayCertificates1\\";

                        string vendorCode = trans.VendorCode;
                        certPath = certPath + "\\" + vendorCode + "\\";
                        string[] fileEntries = Directory.GetFiles(certPath);
                        string filePath = "";
                        if (fileEntries.Length == 1)
                        {

                            valid = true;
                        }
                        else
                        {
                            return valid;
                        }

                    }
                    else if (trans.VendorCode.Equals("CENTENARY") && trans.DigitalSignature.Equals("1234"))
                    {
                        valid = true;
                        return valid;
                    }
                    else if (trans.VendorCode.Equals("CENTENARY"))
                    {
                        string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                        string certPath = "C:\\PegPayCertificates1\\";
                        string vendorCode = trans.VendorCode;
                        certPath = certPath + "\\" + vendorCode + "\\";
                        string[] fileEntries = Directory.GetFiles(certPath);
                        string filePath = "";
                        if (fileEntries.Length == 1)
                        {
                            filePath = fileEntries[0].ToString();
                            X509Certificate2 cert = new X509Certificate2(filePath);
                            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                            SHA1Managed sha1 = new SHA1Managed();
                            ASCIIEncoding encoding = new ASCIIEncoding();
                            byte[] data = encoding.GetBytes(text);
                            byte[] hash = sha1.ComputeHash(data);
                            byte[] sig = Convert.FromBase64String(trans.DigitalSignature);
                            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);

                        }
                        else
                        {
                            return valid;
                        }

                    }
                    else if (trans.VendorCode.Equals("TEST") || trans.VendorCode.ToUpper().Equals("PEGPAY") || trans.VendorCode.ToUpper().Equals("AIRTEL"))
                    {
                        valid = true;
                        return valid;
                    }

                    else if (trans.VendorCode.Equals("SMART") || trans.VendorCode.Equals("SMS2BET"))
                    {
                        valid = true;
                        return valid;
                    }
                    else if (trans.VendorCode.ToUpper().Equals("ISYS"))
                    {
                        string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                        string certPath = "C:\\PegPayCertificates1\\";
                        string vendorCode = trans.VendorCode;
                        certPath = certPath + "\\" + vendorCode + "\\";
                        string[] fileEntries = Directory.GetFiles(certPath);
                        string filePath = "";
                        if (fileEntries.Length == 1)
                        {
                            filePath = fileEntries[0].ToString();
                            X509Certificate2 cert = new X509Certificate2(filePath);
                            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                            SHA1Managed sha1 = new SHA1Managed();
                            ASCIIEncoding encoding = new ASCIIEncoding();
                            byte[] data = encoding.GetBytes(text);
                            byte[] hash = sha1.ComputeHash(data);
                            byte[] sig = Convert.FromBase64String(trans.DigitalSignature);
                            valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);
                            return valid;

                        }
                        else
                        {
                            dp.LogError(" more than 1 certificate in folder", trans.VendorCode, DateTime.Now, "NONE");
                            return false;
                        }
                    }
                    else if (trans.VendorCode.ToUpper().Equals("CELL"))
                    {
                        valid = true;
                        return valid;
                    }
                    else if (trans.VendorCode.ToUpper().Equals("SMARTMONEY"))
                    {
                        valid = true;
                        return valid;
                    }
                    
                    else
                    {

                        string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

                        DataTable dt2 = dp.GetSystemSettings("1", "6");
                        string certPath = dt2.Rows[0]["ValueVarriable"].ToString();
                        string vendorCode = trans.VendorCode;
                        certPath = certPath + "\\" + vendorCode + "\\";

                        string[] fileEntries = Directory.GetFiles(certPath);
                        string filePath = "";
                        if (fileEntries.Length == 1)
                        {
                            filePath = fileEntries[0].ToString();
                            X509Certificate2 cert = new X509Certificate2(filePath);

                            valid = true;
                            return valid;
                        }
                        else
                        {
                            dp.LogError(" more than 1 certificate in folder", trans.VendorCode, DateTime.Now, "NONE");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return false;
            }
            

            private bool isActiveVendor(string vendorCode, DataTable vendorData)
            {
                bool active = false;
                try
                {
                    bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
                    if (activeVendor)
                    {
                        active = true;
                    }
                    else
                    {
                        active = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return active;
            }

            private bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
            {
                bool valid = false;
                try
                {
                    BusinessLogic bll = new BusinessLogic();
                    if (vendorData.Rows.Count != 0)
                    {
                        string vendor = vendorData.Rows[0]["VendorCode"].ToString();
                        string encVendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
                        if (vendor.Trim().Equals(vendorCode.Trim()) && encVendorPassword.Trim().Equals(password.Trim()))
                        {
                            valid = true;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return valid;
            }

            private bool IsValidReversalStatus(Transaction trans)
            {
                if (trans.Reversal == null)
                {
                    return false;
                }
                else
                {
                    if (trans.Reversal.Equals("0") || trans.Reversal.Equals("1"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        

        

        public static void Main(string[] args)
        {
            Payment pay = new Payment();

            
            NWSCTransaction transaction = new NWSCTransaction();
            transaction.CustRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.CustName = "KESI INVESTMENTS LTD";
            transaction.CustomerType = "2";
            transaction.Area = "Kampala";
            transaction.utilityCompany = "NWSC";
            transaction.PaymentDate = "31/08/2020";
            transaction.TransactionAmount = "96158";
            transaction.TransactionType = "CASH";
            transaction.VendorCode = "STANBIC_VAS";
            transaction.Password = "53P48KU262";
            transaction.CustomerTel = "256779248579";
            transaction.Reversal = "0";
            //transaction.TranIdToReverse = trans.PostField13;
            transaction.Teller = "213487670";
            transaction.Offline = "0";
            transaction.DigitalSignature = "gHSIBys0MrRGmx78/WEl5CMnugeQUhBMXdYrZM2A8Frw+I64L38MhIuFBlDDQzTDHZ6XYOE7t/vIGEo55enDIL5DVHLU1ld5UZgH4GvktjSiaYxE5LzhIqhEfalQ/gowONpNMP1/1pG8wosb5p0Uve4i5QSHL+gOUx4969eTx78ISR+W0p/6bTjItjXwodtjejdzM0VlM0u4lPkFiOYeTq0zqCsRlLz32fFwj+dIvk/5UpJ6Ot2th41SVZyn9tqRT2oMCo4uqImYXjegzJnzmknr/5y5N7rqAhnX6Xgc9E39l+pjGa2FEIDCghDCCMmzfDhJE4xSXt69E37Ou/j0QQ==";
            //transaction.ChequeNumber = trans.PostField17;
            transaction.Narration = "CUSTOMER NAME-SHAHZAD KAMALUDDIN UKANI:CUSTOMER ID-SHAHZAD85CONSUMER CODE-21287670:REFERENCE ID-18529084:VAS REQUEST AMOUNTUGX|96158.0";
            //transaction.Email = trans.PostField19;
            transaction.VendorTransactionRef = new Random().Next(1, int.MaxValue).ToString();
            transaction.UtilityCode = "NWSC";

            
            pay.MakeNWSCPayment(transaction);

        }
    }
}
