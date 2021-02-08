using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VerifySignature
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var p = new Program();
            Transaction txn = p.GetTransaction();
            bool validity = p.Verify(txn);
        }
        public bool Verify(Transaction trans)
        {
            string text = trans.CustRef + trans.CustName + trans.CustomerTel + trans.VendorTransactionRef + trans.VendorCode + trans.Password + trans.PaymentDate + trans.Teller + trans.TransactionAmount + trans.Narration + trans.TransactionType;

            DatabaseHandler dp = new DatabaseHandler();
            DataTable dt2 = dp.GetSystemSettings("1", "6");
            string certPath = dt2.Rows[0]["ValueVarriable"].ToString();
            string vendorCode = trans.VendorCode;
            certPath = certPath + "\\" + vendorCode + "\\";
            string[] fileEntries = Directory.GetFiles(certPath);
            string filePath = "";
            if (fileEntries.Length == 1)
            {
                //filePath = @"E:\nasser\test_driven_development\EllypayCert.cer";
                filePath = fileEntries[0].ToString();
                X509Certificate2 cert = new X509Certificate2(filePath);
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)cert.PublicKey.Key;
                SHA1Managed sha1 = new SHA1Managed();
                //UnicodeEncoding encoding = new UnicodeEncoding();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(text);
                byte[] hash = sha1.ComputeHash(data);
                byte[] sig = Convert.FromBase64String(trans.DigitalSignature);
                bool valid = csp.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), sig);
                return valid;
            }
            return false;
        }

        private string GetSchoolTransactionSignature(string Tosign)
        {
            string signature = "";
            try
            {
                string certificate = @"E:\nasser\output.pfx";
                X509Certificate2 cert = new X509Certificate2(certificate, "", X509KeyStorageFlags.UserKeySet);
                //_certificate.Import(certificatePath, "mypasswordusedtocreatethecertificate", X509KeyStorageFlags.MachineKeySet);

                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

                // Hash the data
                SHA1Managed sha1 = new SHA1Managed();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] data = encoding.GetBytes(Tosign);
                byte[] hash = sha1.ComputeHash(data);
                string hashString = Convert.ToBase64String(hash);
                // Sign the hash
                byte[] digitalCert = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
                string strDigCert = Convert.ToBase64String(digitalCert);
                return strDigCert;
            }
            catch (Exception ee)
            {
                throw ee;
            }
           // return signature;
        }
        //public void Sign()
        //{
        //    string dataToSign = req.PostField1 + req.PostField2 + req.PostField11 + req.PostField20
        //        + req.PostField9 + req.PostField10 +
        //    req.PostField5 + req.PostField14 + req.PostField7 + req.PostField18 + req.PostField8;

        //    string pathToCertificate = sms.PathToCertificateFile;
        //    string passwordTocertificate = sms.PasswordToCertificateFile;

        //    //string serialNo = "499e396f0bc59392492e354f19641d91";

        //    //X509Certificate2 cert = GetCertificate(serialNo);

        //    X509Certificate2 cert = new X509Certificate2(pathToCertificate, passwordTocertificate,
        //    X509KeyStorageFlags.UserKeySet);
        //    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;

        //    //Encode the data
        //    ASCIIEncoding encoding = new ASCIIEncoding(); byte[] data =
        //   encoding.GetBytes(dataToSign);

        //    // Hash the data
        //    SHA1Managed sha1 = new SHA1Managed(); byte[] hash =
        //   sha1.ComputeHash(data);
        //    // Sign the hash
        //    byte[] digitalSign = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")); string strDigSign =
        //   Convert.ToBase64String(digitalSign); return strDigSign;
        //}



        public Transaction GetTransaction()
        {
            Transaction transaction = new Transaction();
            transaction.CustRef = "11111";
            transaction.CustName = "KAHANGIRE STEPHEN";
            transaction.CustomerTel = "256777044237";
            transaction.Narration = "NWSC PAYMENT FOR 11111";
            transaction.Password = "Elia@1234";
            transaction.PaymentDate = "12/09/2020";
            transaction.TransactionAmount = "5000";
            transaction.TransactionType = "CASH";
            transaction.VendorCode = "ELLYPAY";
           
           
            transaction.Reversal = "0";
            transaction.Teller = "NONE";
            transaction.Offline = "0";
            transaction.VendorTransactionRef = "ELPTX1PXXQ41CCWKIH38DTJ";
            transaction.UtilityCode = "NWSC";
            string text = transaction.CustRef + transaction.CustName + transaction.CustomerTel + transaction.VendorTransactionRef +
                transaction.VendorCode + transaction.Password + transaction.PaymentDate + transaction.Teller + transaction.TransactionAmount +
                transaction.Narration + transaction.TransactionType;
           //string forSigning = "04237341583MUKASA NSUBUGA256777044237ELPTX1PXXQ41MX4KIHEBJJBELLYPAYElia@123412/09/2020NONE5000UMEME PAYMENT FOR 04237341583CASH";

            transaction.DigitalSignature = GetSchoolTransactionSignature(text);

            return transaction;
        }

       }
}

