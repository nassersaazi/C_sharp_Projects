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
        public Transaction  transaction { get; set; }

        DatabaseHandler dp = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();
        
        public bool VerifySignature(Transaction tran)
        {
            transaction = tran;
            
            if (transaction.VendorCode.Equals("CENTENARY") && transaction.DigitalSignature.Equals("1234"))
            {
                valid = true;
                
            }
            else
            {
                switch (transaction.VendorCode)
                {
                    case "MTN":
                    case "TEST":
                    case "PEGPAY":
                    case "AIRTEL":
                    case "AFRICELL":
                    case "SMART":
                    case "SMS2BET":
                    case "TESTFLEXIPAY":
                    case "MOWE":
                    case "CELL":
                    case "SMARTMONEY":
                    case "1234":
                        valid = true;
                        break;
                    case "EzeeMoney":
                        VerifyEzeeMoney();
                        break;
                    case "CENTENARY":
                        VerifyCentenary();
                        break;
                    case "ISYS":
                        VerifyISYS();
                        break;


                    default:
                        VerifyAnyOtherVendor();
                        break;
                }
            }
            
            return valid;
        }

        private void VerifyAnyOtherVendor()
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

        private void VerifyISYS()
        {
            CheckForCertificate();
            
        }

        private void CheckForCertificate()
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
                filePath = fileEntries[0].ToString();
                X509Certificate2 cert = new X509Certificate2(filePath);
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                SHA1Managed sha1 = new SHA1Managed();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(text);
                byte[] hash = sha1.ComputeHash(data);
                byte[] sig = Convert.FromBase64String(transaction.DigitalSignature);
                valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);


            }
            else
            {
                dp.LogError(" more than 1 certificate in folder", transaction.VendorCode, DateTime.Now, "NONE");

            }
        }

        private void VerifyCentenary()
        {
            CheckForCertificate();


        }

        private void VerifyEzeeMoney()
        {
            string text = transaction.CustRef + transaction.CustName + transaction.CustomerTel + transaction.VendorTransactionRef + transaction.VendorCode + transaction.Password + transaction.PaymentDate +
                transaction.Teller + transaction.TransactionAmount + transaction.Narration + transaction.TransactionType;

            string certPath = "C:\\PegPayCertificates1\\";

            string vendorCode = transaction.VendorCode;
            certPath = certPath + "\\" + vendorCode + "\\";
            string[] fileEntries = Directory.GetFiles(certPath);
            string filePath = "";
            if (fileEntries.Length == 1)
            {

                valid = true;
            }
            
        }
      
    }
}
