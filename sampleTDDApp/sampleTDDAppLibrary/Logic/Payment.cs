using sampleTDDAppLibrary.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace sampleTDDAppLibrary.Logic
{
    public abstract class Payment : IValidity
    {
        public PostResponse resp = new PostResponse();
        protected DatabaseHandler dp = new DatabaseHandler();
        protected BusinessLogic bll = new BusinessLogic();
        protected PhoneValidator pv = new PhoneValidator();

        protected void  CheckForEmptyProperties(NWSCTransaction trans)
        {
            if (trans.CustName == null || trans.CustName.Trim().Equals(""))
            {
                HandleResponse(trans, resp, "13", "");
            }

            else if (trans.Area == null || trans.Area.Trim().Equals(""))
            {
                HandleResponse(trans, resp, "35", "");
            }

            else if (trans.TransactionType == null || trans.TransactionType.Trim().Equals(""))
            {
                HandleResponse(trans, resp, "14", "");
            }

            else if (trans.VendorTransactionRef == null || trans.VendorTransactionRef.Trim().Equals(""))
            {
                HandleResponse(trans, resp, "16", "");
            }

            else if (trans.Teller == null || trans.Teller.Trim().Equals(""))
            {
                HandleResponse(trans, resp, "17", "");
            }

            else if (trans.DigitalSignature == null || trans.DigitalSignature.Length == 0)
            {
                HandleResponse(trans, resp, "19", "");
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse == null)
            {
                HandleResponse(trans, resp, "22", "");
            }
            else if (trans.Reversal == "1" && trans.TranIdToReverse.Equals(""))
            {
                HandleResponse(trans, resp, "22", "");
            }
            else if ((trans.Reversal == "1" && trans.Narration == null) || (trans.Reversal == "1" && trans.Narration.Equals("")))
            {
                HandleResponse(trans, resp, "23", "");
            }
        }
        protected void CheckTranValidity(Transaction tran)
        {

        }
        public abstract PostResponse pay(NWSCTransaction tran);
        public bool isSignatureValid(Transaction trans)
        {
            bool valid = false;
            try
            {
                DatabaseHandler dp = new DatabaseHandler();
                BusinessLogic bll = new BusinessLogic();
                if ((trans.VendorCode.Equals("MTN")) || (trans.VendorCode.Equals("TEST") || trans.VendorCode.ToUpper().Equals("PEGPAY") ||
                    trans.VendorCode.ToUpper().Equals("AIRTEL")) || (trans.VendorCode.Equals("AFRICELL")) || trans.VendorCode.Equals("SMART") || trans.VendorCode.Equals("SMS2BET") ||
                    (trans.VendorCode.Equals("TESTFLEXIPAY") && trans.UtilityCode.Equals("MOWE")) || trans.VendorCode.ToUpper().Equals("CELL") || trans.VendorCode.ToUpper().Equals("SMARTMONEY") ||
                    (trans.VendorCode.Equals("CENTENARY") && trans.DigitalSignature.Equals("1234")))
                {
                    return true;
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


        public bool isActiveVendor(string vendorCode, DataTable vendorData)
        {
            bool active = false;
            try
            {
                bool activeVendor = bool.Parse(vendorData.Rows[0]["Active"].ToString());
                return activeVendor == true ? true : false;
                
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        public bool isValidVendorCredentials(string vendorCode, string password, DataTable vendorData)
        {
            bool valid = false;
            try
            {
                BusinessLogic bll = new BusinessLogic();
                if (vendorData.Rows.Count != 0)
                {
                    string vendor = vendorData.Rows[0]["VendorCode"].ToString();
                    string encVendorPassword = vendorData.Rows[0]["VendorPassword"].ToString();
                    valid = (vendor.Trim().Equals(vendorCode.Trim()) && encVendorPassword.Trim().Equals(password.Trim())) ? true : false;
                    
                }
                else
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                return valid;
            }
            return valid;
        }

       

        public bool isValidVendorTraficIpAccess()
        {
            throw new NotImplementedException();
        }

        public bool isValidVendorUtilityMapping()
        {
            throw new NotImplementedException();
        }

        public bool IsduplicateVendorRef(Transaction trans)
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

        public bool IsValidReversalStatus(Transaction trans)
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

        public void HandleResponse(NWSCTransaction trans, PostResponse resp, string status, string statusDescription)
        {
            
            if (status == "0")
            {
                resp.PegPayPostId = dp.PostTransaction(trans, "NWSC");
                resp.StatusCode = "0";
                resp.StatusDescription = dp.GetStatusDescr(resp.StatusCode);
            }
            else
            {
                resp.PegPayPostId = "";
                resp.StatusCode = status;
                resp.StatusDescription = string.IsNullOrEmpty(statusDescription) ? dp.GetStatusDescr(resp.StatusCode) : statusDescription;
            }
        }

        

        protected bool ReverseAmountsMatch(Transaction trans)
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
                    return total.Equals(0) ? true : false;
                   
                }
                else
                {
                    return false;
                }
            }
        }

        protected bool HasOriginalEntry(Transaction trans)
        {
            DatabaseHandler dp = new DatabaseHandler();
            if (trans.Reversal.Equals("0"))
            {
                return true;
            }
            else
            {
                DataTable dt = dp.GetOriginalVendorRef(trans);
                return (dt.Rows.Count > 0) ? true : false;
                
            }
        }

        protected string GetReversalState(Transaction trans)
        {
            string res = "";
            if (trans.Reversal != null)
            {
                double amt = double.Parse(trans.TransactionAmount);
                string amountstr = amt.ToString();
                int amount = int.Parse(amountstr);
                res = (amount > 0) ? "0" : "1";
                
            }
            return res;
        }

        public string Serialize(object dataToSerialize)
        {
            if (dataToSerialize == null) return null;

            using (StringWriter stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(dataToSerialize.GetType());
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
        }

        protected bool IsduplicateCustPayment(Transaction trans)
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
                ret = (tmdiff < 10) ? true : false;
                
            }
            else
            {
                ret = false;
            }
            return ret;
        }
    }
}
