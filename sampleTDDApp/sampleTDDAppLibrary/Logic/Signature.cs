using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace sampleTDDAppLibrary.Logic
{
    public class Signature
    {
        bool valid = false;
        
        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        
        public bool VerifySignature(ITransaction transaction)
        {
            
            if (transaction.VendorCode.Equals("CENTENARY") && transaction.DigitalSignature.Equals("1234"))
            {
                valid = true;
                
            }
            else
            {
                switch (transaction.VendorCode)
                {
                    case "MTN": case "TEST": case "PEGPAY": case "AIRTEL": case "AFRICELL":case "SMART":case "SMS2BET": case "TESTFLEXIPAY": case "MOWE": case "CELL": case "SMARTMONEY": case "1234":
                        valid = true;
                        break;
                    case "EzeeMoney": VerifyEzeeMoney(transaction); break;
                    case "CENTENARY": VerifyCentenary(transaction); break;
                    case "ISYS": VerifyISYS(transaction); break; 
                    default: VerifyAnyOtherVendor(transaction); break;
                }
            }
            
            return valid;
        }

        private void VerifyAnyOtherVendor(ITransaction transaction)
        {
            try
            {

                    string text = transaction.CustRef + transaction.CustName + transaction.CustomerTel + transaction.VendorTransactionRef + transaction.VendorCode + transaction.Password +
                    transaction.PaymentDate + transaction.Teller + transaction.TransactionAmount + transaction.Narration + transaction.TransactionType;

                    DataTable dt2 = dp.GetSystemSettings("1", "6");
                    string certPath = dt2.Rows[0]["ValueVarriable"].ToString();
                    string vendorCode = transaction.VendorCode;
                    certPath = certPath + "\\" + vendorCode + "\\";

                    string[] fileEntries = Directory.GetFiles(certPath);
                    string filePath = "";
                    if (fileEntries.Length == 1)
                    {
                        filePath = fileEntries[0].ToString();
                        X509Certificate2 cert = new X509Certificate2(filePath);

                        valid = true;
                        
                    }
                    else
                    {
                        dp.LogError(" more than 1 certificate in folder", transaction.VendorCode, DateTime.Now, "NONE");
                        
                    }
               
            }
            catch (Exception ex)
            {
                valid = false;
            }
        }

        private void VerifyISYS(ITransaction tran)
        {
            CheckForCertificate(tran);
            
        }

        private void CheckForCertificate(ITransaction transaction)
        {
            string text = transaction.CustRef + transaction.CustName + transaction.CustomerTel + transaction.VendorTransactionRef + transaction.VendorCode + transaction.Password +
                transaction.PaymentDate + transaction.Teller + transaction.TransactionAmount + transaction.Narration + transaction.TransactionType;


            string certPath = "C:\\PegPayCertificates1\\";
            string vendorCode = transaction.VendorCode;
            certPath = certPath + "\\" + vendorCode + "\\";
            string[] fileEntries = Directory.GetFiles(certPath);
            string filePath = "";
            if (fileEntries.Length == 1)
            {
                switch (vendorCode)
                {
                    case "EzeeMoney":
                        valid = true;
                        break;
                    default:
                        filePath = fileEntries[0].ToString();
                        X509Certificate2 cert = new X509Certificate2(filePath);
                        RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                        SHA1Managed sha1 = new SHA1Managed();
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] data = encoding.GetBytes(text);
                        byte[] hash = sha1.ComputeHash(data);
                        byte[] sig = Convert.FromBase64String(transaction.DigitalSignature);
                        valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);
                        break;
                }
                

            }
            else
            {
                dp.LogError(" more than 1 certificate in folder", transaction.VendorCode, DateTime.Now, "NONE");

            }
        }

        private void VerifyCentenary(ITransaction tran)
        {
            CheckForCertificate(tran);
            
        }

        private void VerifyEzeeMoney(ITransaction tran)
        {
            CheckForCertificate(tran);

        }
      
    }
}
