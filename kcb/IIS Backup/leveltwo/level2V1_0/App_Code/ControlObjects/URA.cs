using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for URA
/// </summary>
public class URA
{
    
        private string path = "E://URACerts/";

    public void makePaymentUra(URATransaction transaction, UtilityCredentials creds)
        {
            //URATransaction trans = new URATransaction();
            //trans.Tin = pay.Tin;
            //trans.BillAmount = int.Parse(pay.Amount);
            //trans.CustName = pay.TaxpayerName;
            //trans.CustomerTel = "0781123654";
            //trans.CustRef = pay.Prn;
            //trans.Narration = "CASH";
            //trans.PaymentDate = DateTime.Now;
            //trans.VendorTransactionRef = "123559stn90";
            //trans.TransactionType = "CASH";
            //trans.ChequeNumber = "";
            //trans.Status = "C";
            //trans.BranchCode = "040047";

            ////send to URA
            //string response = SendToURA(trans);
        }


    public string[] makePaymentUraNewImplementation(URATransaction transaction, UtilityCredentials creds)
        {
            try
            {
                BusinessLogic bll = new BusinessLogic();
                UtilityReferences.URA.UraPmtService clt = new UtilityReferences.URA.UraPmtService();
                string username = creds.UtilityCode; //"STN";
                string Passwd = bll.DecryptString(creds.UtilityPassword); // "C1bn@t5#739";
                string CryptPass = EncryptBankString(Passwd);
                //-- one single payment transaction entity
                UtilityReferences.URA.TransactionEntity av = new UtilityReferences.URA.TransactionEntity();
                string tranAmt = Convert.ToString(int.Parse(transaction.TransactionAmount));
                av.Amount = EncryptBankString(tranAmt);
                av.Prn = EncryptBankString(transaction.CustRef);
                av.Tin = EncryptBankString(transaction.TIN);
                av.Reason = transaction.Narration;
                av.Bank_branch_cd = creds.BankCode; // transaction.BranchCode;
                av.Bank_cd = username;
                av.Bank_tr_no = transaction.VendorTransactionRef;
                av.Chq_no = transaction.ChequeNumber;
                av.Paid_dt = DateTime.Now.ToString("dd/MM/yyyy");
                av.Status = "C"; //transaction.Status;
 
                av.Value_dt = DateTime.Now.ToString("dd/MM/yyyy");
                
                av.Signature = GetSignature(av.Bank_cd + av.Prn + av.Tin + av.Amount + av.Paid_dt + av.Value_dt + av.Status +
                               av.Bank_branch_cd + av.Bank_tr_no + av.Chq_no + av.Reason);

                UtilityReferences.URA.TransactionEntity[] arr = { av };
                string[] RetStr;
                try
                {
                    RetStr = clt.NotifyUraPayment(username, CryptPass, arr);
                }
                catch (Exception exe)
                {
                    string res = "000,OFFLINE";
                    string[] arr_str = res.Split(',');
                    RetStr = arr_str;
                }
                return RetStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] GetSignature(string Tosign)
        {
            string certificate = @"E:\Certificates\CELL\test.pfx";
            X509Certificate2 cert = new X509Certificate2(certificate, "", X509KeyStorageFlags.UserKeySet);

            RSACryptoServiceProvider RSAcp = (RSACryptoServiceProvider)cert.PrivateKey;

            if (RSAcp == null)
            {
                throw new Exception("VALID CERTIFICATE NOT FOUND");
            }

            byte[] data = new UnicodeEncoding().GetBytes(Tosign);
            byte[] hash = new SHA1Managed().ComputeHash(data);

            // Sign the hash
            return RSAcp.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
        }

        public string EncryptBankString(string EncrStr)
        {
            try
            {
                X509Certificate2 CertV = GetURACert(); //GetBankCert();
                byte[] cipherbytes = ASCIIEncoding.ASCII.GetBytes(EncrStr);
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)CertV.PublicKey.Key;// .PrivateKey;//to verify/confirm
                byte[] ciph = rsa.Encrypt(cipherbytes, false);
                string CryptPass = Convert.ToBase64String(ciph);
                return CryptPass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private X509Certificate2 GetURACert()
        {
            try
            {
                //string fileName = System.Windows.Forms.Application.StartupPath + "\\certs\\" + "URAPayment.cer";
                string fileName = path + "URAioPmtCert1.cer";

                if (fileName.Trim().Length > 0)
                {
                    X509Certificate2 cert = new X509Certificate2(fileName);
                    return cert;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }

