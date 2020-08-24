using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using myReversalTester.EntityObject;


namespace myReversalTester
{
    class DatabaseHandler
    {
        private Database PegPayInterface;

        public DatabaseHandler()
        {
            try
            {
                PegPayInterface = DatabaseFactory.CreateDatabase("TestPegPayConnectionString");
               // PegPayInterface = DatabaseFactory.CreateDatabase("LivePegPayConnectionString");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       internal DataTable GetReversePrepaidTransactions()
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("GetReversalPrepaid");
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string LogError(string error, string vendorCode, DateTime now, string AgentCode)
        {
            string ret = "";
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
                PegPayInterface.ExecuteNonQuery(command);
                ret = "YES";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }


        internal UtilityCredentials GetUtilityCreds(string utilityCode, string vendorCode)
        {
            UtilityCredentials creds = new UtilityCredentials();
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("GetUtilityCredentials", vendorCode, utilityCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    creds.UtilityCode = dt.Rows[0]["UtilityUsername"].ToString();
                    creds.UtilityPassword = dt.Rows[0]["UtilityPassword"].ToString();
                    creds.Utility = dt.Rows[0]["UtilityCode"].ToString();
                    creds.BankCode = dt.Rows[0]["BankCode"].ToString();
                    creds.SecretKey = dt.Rows[0]["SecretKey"].ToString();
                    creds.Key = dt.Rows[0]["Key"].ToString();
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

       
        internal void InsertIntoBankResponseLogs(string vendorCode, string vendorTranId, string respCode, string respDesc, string OtherData)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("InsertIntoBankResponseLogs", vendorCode, vendorTranId, respCode, respDesc, OtherData);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal string GetSystemSetting(int GroupCode, int valueCode)
        {
            string value = "";
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("GetSystemSetting", GroupCode, valueCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                value = dt.Rows[0]["ValueVarriable"].ToString().Trim();
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReverseTransactionAndCharges(string VendorTranId, string VendorCode, string Reason, string BankId)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("FailTransactionAndReverseTransactionCharges1", VendorTranId, VendorCode, Reason, BankId);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SuccessfulUtilityReversalRequests(string tranId, string vendorCode, string status, string utilityTranRef, string reason)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidTrasactionStatusWithoutDebit2", tranId, vendorCode, status, utilityTranRef, reason);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal void InsertIntoUtilityResponseLogs(string vendorCode, string vendorTranId, string respCode, string respDesc, string OtherData)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("InsertIntoUtilityResponseLogs", vendorCode, vendorTranId, respCode, respDesc, OtherData);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal void UpdatePrepaidTransactionStatus(string vendorCode, string vendorTranId, string Status)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("UpdatePrepaidTransactionStatus", vendorCode, vendorTranId, Status);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void FailTransactionAndReverseCharges(string VendorTranId, string VendorCode, string Reason, string BankId)
        {
            try
            {
                DbCommand command = PegPayInterface.GetStoredProcCommand("FailTransactionAndReverseTransactionCharges1", VendorTranId, VendorCode, Reason, BankId);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
