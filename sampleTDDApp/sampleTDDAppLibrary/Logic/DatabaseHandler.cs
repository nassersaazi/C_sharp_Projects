using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace sampleTDDAppLibrary.Logic
{
    public class DatabaseHandler
    {

        private DbCommand command;
        private const String constring = "TestPegPay";
        private Database pegpaydbase;

        public DatabaseHandler()
        {
            try
            {
                pegpaydbase = DatabaseFactory.CreateDatabase(constring);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal string GetStatusDescr(string statusCode)
        {
            string descr = "";
            try
            {
                string[] parameters = { statusCode };
                DataTable dt = pegpaydbase.ExecuteDataSet("GetStatusDescr", parameters).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    descr = dt.Rows[0]["StatusDescription"].ToString();
                }
                else
                {
                    descr = "GENERAL ERROR";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return descr;
        }

        internal void SaveRequestlog(string VendorCode, string Utility, string LogType, string CustRef, string password)
        {
            try
            {
                string[] parameters = { VendorCode, Utility, LogType, CustRef, password };
                command = pegpaydbase.GetStoredProcCommand("SaveRequestlog", parameters);
                command.CommandTimeout = 300000000;
                pegpaydbase.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetVendorDetails(string vendorCode)
        {
            try
            {
                string[] parameters = { vendorCode };
                command = pegpaydbase.GetStoredProcCommand("GetVendorDetails", parameters);
                DataTable dt = pegpaydbase.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal UtilityCredentials GetUtilityCreds(string utilityCode, string vendorCode)
        {
            UtilityCredentials creds = new UtilityCredentials();
            try
            {
                string[] parameters = { vendorCode, utilityCode };
                DataTable dt = pegpaydbase.ExecuteDataSet("GetUtilityCredentials", parameters).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    creds.UtilityCode = dt.Rows[0]["UtilityUsername"].ToString();
                    creds.UtilityPassword = dt.Rows[0]["UtilityPassword"].ToString();
                    creds.Utility = dt.Rows[0]["UtilityCode"].ToString();
                    creds.BankCode = dt.Rows[0]["BankCode"].ToString();
                    creds.SecretKey = dt.Rows[0]["SecretKey"].ToString();
                    creds.Key = dt.Rows[0]["Key"].ToString();
                    creds.UtilityIsOffline = dt.Rows[0]["IsOffline"].ToString().ToUpper();
                }
                else
                {
                    creds.UtilityCode = "";
                    creds.UtilityPassword = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return creds;
        }


        internal string PostTransaction(NWSCTransaction trans, string utilityCode)
        {
            string receiptNo = "";
            try
            {
                string format = "dd/MM/yyyy";
                DateTime payDate = DateTime.ParseExact(trans.PaymentDate, format, CultureInfo.InvariantCulture);
                string[] parameters ={trans.CustRef, trans.CustName, trans.CustomerType, trans.CustomerTel, trans.Area, "", trans.TransactionAmount, ""+payDate, ""+DateTime.Now, trans.TransactionType, "", trans.VendorTransactionRef, trans.Narration,
                ""+false, trans.VendorCode, trans.Teller, ""+false/*bool.Parse(trans.Reversal)*/, ""+false, ""+false/*bool.Parse(trans.Offline)*/, utilityCode, "", ""+false};
                DataTable dt = pegpaydbase.ExecuteDataSet("InsertReceivedTransactions", parameters).Tables[0];

                if (dt.Rows.Count != 0)
                {
                    receiptNo = dt.Rows[0][0].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return receiptNo;
        }

        internal void UpdateVendorInvalidLoginCount(string vendorCode, int loginCount, string ip)
        {
            try
            {
                string[] p = { vendorCode, "" + loginCount };
                pegpaydbase.ExecuteNonQuery("UpdateVendorInvalidLoginCount", p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void DeactivateVendor(string vendorCode, string ip_address)
        {
            try
            {
                string[] p = { vendorCode, ip_address };
                pegpaydbase.ExecuteNonQuery("DeactivateVendor", p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal string LogError(string error, string vendorCode, DateTime now, string AgentCode)
        {
            string ret = "";
            try
            {
                string[] p = { error, vendorCode, "" + now, AgentCode };
                pegpaydbase.ExecuteNonQuery("LogError", p);
                ret = "YES";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        internal void deleteTransaction(string vendorTranId, string reason)
        {
            try
            {
                string[] p = { vendorTranId, reason };
                pegpaydbase.ExecuteNonQuery("DeleteTransation2", p);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetOriginalVendorRef(Transaction trans)
        {
            try
            {
                string[] p = { trans.VendorCode, trans.TranIdToReverse };
                DataTable returndetails = pegpaydbase.ExecuteDataSet("GetDuplicateVendorRef", p).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        internal DataTable CheckBlacklist(string customerRef)
        {
            try
            {
                string[] p = { customerRef };
                DataTable dt = pegpaydbase.ExecuteDataSet("GetCustBlacklistStatus", p).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetSystemSettings(string Valuecode, string ValueGroupcode)
        {
            try
            {

                string[] parameters = { Valuecode, ValueGroupcode };
                DataTable returndetails = pegpaydbase.ExecuteDataSet("GetSystemSettings", parameters).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal ArrayList GetBlackListedNumbers()
        {
            ArrayList blacklisted = new ArrayList();
            try
            {
                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetBlacklistedNumbers", parameters);
                //DataSet ds = PegPayInterface.ExecuteDataSet(command);
                int recorcCount = ds.Tables[0].Rows.Count;
                for (int i = 0; i < recorcCount; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string number = dr["Phone"].ToString();
                    blacklisted.Add(number);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return blacklisted;
        }

        internal bool IsChequeBlacklisted(Transaction trans)
        {

            if (trans.TransactionType.ToUpper().Contains("CHEQUE"))
            {

                DataTable dt = CheckBlacklist(trans.CustRef);
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

        internal Hashtable GetNetworkCodes()
        {
            Hashtable networkCodes = new Hashtable();
            try
            {

                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetNetworkCodes", parameters);
                int recordCount = ds.Tables[0].Rows.Count;
                if (recordCount != 0)
                {
                    for (int i = 0; i < recordCount; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        string network = dr["Network"].ToString();
                        string code = dr["Code"].ToString();
                        networkCodes.Add(code, network);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return networkCodes;
        }

        internal Hashtable GetNetworkRates()
        {
            Hashtable networkRates = new Hashtable();
            try
            {

                string[] parameters = { };
                DataSet ds = pegpaydbase.ExecuteDataSet("GetNetworkRates", parameters);
                int recorcCount = ds.Tables[0].Rows.Count;
                for (int i = 0; i < recorcCount; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string network = dr["Network"].ToString();
                    int rate = int.Parse(dr["Rate(UShs.)"].ToString());
                    networkRates.Add(network, rate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return networkRates;
        }

        internal DataTable GetDuplicateVendorRef(Transaction trans)
        {
            try
            {
                string[] parameters = { trans.VendorCode, trans.VendorTransactionRef };
                command = pegpaydbase.GetStoredProcCommand("GetDuplicateVendorRef", parameters);
                DataTable returndetails = pegpaydbase.ExecuteDataSet(command).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetDuplicateCustPayment(string vendorCode, string custRef, double amount, DateTime postDate)
        {
            try
            {
                //command = PegPayInterface.GetStoredProcCommand("GetDuplicateCustPayment", vendorCode, custRef, amount, postDate);
                string[] p = { vendorCode, custRef, "" + amount, "" + postDate };
                DataTable returndetails = pegpaydbase.ExecuteDataSet("GetDuplicateCustPayment", p).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}