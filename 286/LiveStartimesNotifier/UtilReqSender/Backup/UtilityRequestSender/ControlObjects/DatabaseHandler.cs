using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using UtilReqSender.EntityObjects;

namespace UtilReqSender.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayInterface;
        private DbCommand command;
        public string SmsQueuePath = "";
        public DatabaseHandler()
        {
            try
            {
                //PegPayInterface = DatabaseFactory.CreateDatabase("TestPegPayConnectionString");
                PegPayInterface = DatabaseFactory.CreateDatabase("PegPayConnectionString");
                SmsQueuePath = @".\private$\smsQueue";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetMTNYakaTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GeStarTimesTransactionsToSend");//GetMTNYakaTransactionsToSend
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
               return GetMTNYakaTransactionsToSend();
            }
            return datatable;
        }

        internal Credentials GetUtilityCreds(string utilityCode, string vendorCode)
        {
            Credentials creds = new Credentials();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetUtilityCredentials", vendorCode, utilityCode);
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

        internal void UpdateSentTransactionById(int TranId, string utilityReceipt, string status)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateSentTransactionByTranId2", TranId, utilityReceipt, status);
                PegPayInterface.ExecuteNonQuery(command);
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
                command = PegPayInterface.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
                PegPayInterface.ExecuteNonQuery(command);
                ret = "YES";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        internal void TransferFailedTransaction(int PegPayId, string reason)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("TransferFailedTransaction", PegPayId, reason);
                PegPayInterface.ExecuteNonQuery(command);
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
                command = PegPayInterface.GetStoredProcCommand("GetStatusDescr", statusCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
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

        internal void LogSMS(string CustomerTel, string VendorTransactionRef, string Msg, string Mask, string Service)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("InsertSmsToSend1", CustomerTel, Msg, Mask, Service, VendorTransactionRef);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
            }
        }
        
        internal void ProcessFailedTransactions()
        {
           try
            {
                command = PegPayInterface.GetStoredProcCommand("GetFailedMTNYakaTransactions");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
