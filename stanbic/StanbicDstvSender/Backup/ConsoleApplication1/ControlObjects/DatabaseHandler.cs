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
using ConsoleApplication1.EntityObjects;
using System.Messaging;

namespace ConsoleApplication1.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayInterface;
        private DbCommand command;
        public DatabaseHandler()
        {
            try
            {
                //PegPayInterface = DatabaseFactory.CreateDatabase("TestPegPayConnectionString");
               //// PegPayInterface = DatabaseFactory.CreateDatabase("LivePegPayConnectionString");
                PegPayInterface = DbLayer.CreateDatabase("LivePegPayConnectionString", DbLayer.DB2);
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
                command = PegPayInterface.GetStoredProcCommand("GetSystemSetting", GroupCode, valueCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                value = dt.Rows[0]["ValueVarriable"].ToString().Trim();
                return value;
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

        internal void UpdateSentTransactionById(string VendorTranId, string utilityReceipt, string status)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateSentTransactionStanbic", VendorTranId, utilityReceipt, status);
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

        internal void TransferDeletedTransaction(int PegPayId, string reason)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("TransferDeletedTransaction", PegPayId, reason);
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


        internal DataTable GetStanbicYakaTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicYakaTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

        internal DataTable GetStanbicPostPaidTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicPostPaidTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

        internal DataTable GetNWSCTransactionsToSendApartFromMtn()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicNWSCTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

        internal DataTable GetURATransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicURATransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

        internal DataTable GetStanbicSchoolsTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicSchoolsTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

       
        internal Customer GetCustDetails(string custref, string UtilityCode)
        {
            Customer cust = new Customer();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails1", custref, "", UtilityCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                    cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();

                    //the customer number is always stored in the meterNo field
                    cust.CustomerRef = dt.Rows[0]["MeterNo"].ToString();
                    cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                    cust.StatusCode = "0";
                    cust.StatusDescription = "SUCCESS";
                }
                else
                {
                    cust.StatusCode = "100";
                    cust.StatusDescription = "INVALID CUSTOMER REF.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cust;
        }


        internal DataTable GetStanbicDSTVTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetStanbicDSTVTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return datatable;
        }

        internal BouquetDetails GetBouquetClosestToAmountPaid(int AmountPaid, string PayTvCode)
        {
            BouquetDetails details = new BouquetDetails();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetBouquetClosestToAmountPaid", AmountPaid, PayTvCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    details.BouquetCode = dt.Rows[0]["BouquetCode"].ToString();
                    details.BouquetName = dt.Rows[0]["BouquetName"].ToString();
                    details.BouquetPrice = dt.Rows[0]["BouquetPrice"].ToString();
                    details.PayTvCode = dt.Rows[0]["PayTvCode"].ToString();
                    details.StatusCode = "0";
                    details.StatusDesc = "SUCCESS";
                }
                else
                {
                    details.StatusCode = "100";
                    details.StatusDesc = "No Bouquet Found";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return details;
        }
    }
}
