using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;
using System.Net.Security;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using ConsoleApplication1.EntityObjects;
using System.Messaging;
using log4net;
using ConsoleApplication1.StanbicApi;

namespace ConsoleApplication1.ControlObjects
{
    public class Procssor
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Procssor));
        BussinessLogic bll = new BussinessLogic();


        public void ProcessStanbicYaka()
        {

            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                DataTable dt = dh.GetStanbicYakaTransactionsToSend();

                foreach (DataRow dr in dt.Rows)
                {
                    ConsoleApplication1.EntityObjects.UmemeTransaction trans = GetUmemeTransObj(dr);
                    bll.SendToUmeme(trans);
                    Console.WriteLine("StanbicYaka Finnished");
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "UMEME");
            }
        }

        public void ProcessStanbicSchoolsTransactions()
        {

            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                DataTable dt = dh.GetStanbicSchoolsTransactionsToSend();

                foreach (DataRow dr in dt.Rows)
                {
                    SchoolsTransaction trans = GetSchoolsTransObj(dr);
                    bll.SendToSchool(trans);
                    Console.WriteLine("Stanbic Schools Finnished");
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "Schools");
            }
        }

        public void ProcessStanbicDSTVTransactions()
        {

            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                DataTable dt = dh.GetStanbicDSTVTransactionsToSend();

                foreach (DataRow dr in dt.Rows)
                {
                    Transaction trans = GetTransObj(dr);
                    bll.SendToDSTV(trans);
                    Console.WriteLine("Stanbic DSTV Finnished");
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "DSTV");
            }
        }

        public void ProcessStanbicUmemePostPaid()
        {

            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                DataTable dt = dh.GetStanbicPostPaidTransactionsToSend();

                foreach (DataRow dr in dt.Rows)
                {
                    ConsoleApplication1.EntityObjects.UmemeTransaction trans = GetUmemeTransObj(dr);
                    bll.SendToUmeme(trans);
                    Console.WriteLine("Stanbic PostPaid Finnished");
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "UMEME");
            }
        }

        public void ProcessStanbicNWSC()
        {

            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                DataTable dt = dh.GetNWSCTransactionsToSendApartFromMtn();

                foreach (DataRow dr in dt.Rows)
                {
                    NWSCTransaction trans = GetNWSCTransObj(dr);
                    bll.SendToNWSC(trans);
                    Console.WriteLine("Stanbic NWSC Finnished");
                }

            }
            catch (Exception ex)
            {

                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "NWSC");
            }
        }

        public void ProcessStanbicURA()
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
              
                DataTable dt = dh.GetURATransactionsToSend();

                foreach (DataRow dr in dt.Rows)
                {
                    URATransaction trans = GetUraObj(dr);
                    bll.SendToURA(trans);
                    Console.WriteLine("Stanbic URA Finnished");
                }

            }
            catch (Exception ex)
            {
                dh.LogError(ex.Message, "STANBIC_VAS", DateTime.Now, "URA");
            }
        }


        private Transaction GetTransObj(DataRow dr)
        {
            Transaction trans = new NWSCTransaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            //trans.chequeNumber="";
            trans.Area = dr["Area"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = 0;// Convert.ToInt32(dr["SentToVendor"].ToString());
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }

       

        private URATransaction GetUraObj(DataRow dr)
        {
            URATransaction trans = new URATransaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            trans.Area = dr["Tin"].ToString();
            trans.TIN = dr["Tin"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = 0;// Convert.ToInt32(dr["SentToVendor"].ToString());
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }

        private NWSCTransaction GetNWSCTransObj(DataRow dr)
        {
            NWSCTransaction trans = new NWSCTransaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            //trans.chequeNumber="";
            trans.Area = dr["Area"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = 0;// Convert.ToInt32(dr["SentToVendor"].ToString());
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }

        private SchoolsTransaction GetSchoolsTransObj(DataRow dr)
        {
            SchoolsTransaction trans = new SchoolsTransaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            //trans.chequeNumber="";
            trans.Area = dr["Area"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.UtilityCode = utility;
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = Convert.ToInt32(dr["SentToVendor"].ToString().Trim());
            trans.PaymentType = dr["PaymentType"].ToString();
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }


        private UmemeTransaction GetUmemeTransObj(DataRow dr)
        {
            UmemeTransaction trans = new UmemeTransaction();
            new ConsoleApplication1.EntityObjects.UmemeTransaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            //trans.chequeNumber="";
            trans.Area = dr["Area"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = Convert.ToInt32(dr["SentToVendor"].ToString().Trim());
            trans.PaymentType = dr["PaymentType"].ToString();
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }

       
        
        
    }
}
