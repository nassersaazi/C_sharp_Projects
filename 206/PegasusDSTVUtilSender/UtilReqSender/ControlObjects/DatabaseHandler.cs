using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using UtilReqSender.EntityObjects;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace UtilReqSender.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayInterface;
        private DbCommand command;
        public string SmsQueuePath = "";
        public static string LogDirectoryPath = "";
        public string ConnectionString = "TestPegPayConnectionString";
        //public string ConnectionString = "PegPayConnectionString";

        public DatabaseHandler()
        {
            try
            {
                PegPayInterface = DatabaseFactory.CreateDatabase(ConnectionString);
                //PegPayInterface = DbLayer.CreateDatabase("PegPayConnectionString", DbLayer.DB2);
                if (ConnectionString.Equals("PegPayConnectionString"))
                {
                    LogDirectoryPath = @"E:\Logs\DstvLogs\LiveDstvPostLogs";
                }
                else
                {
                    LogDirectoryPath = @"E:\Logs\DstvLogs\TestDstvPostLogs";
                }
                SmsQueuePath = @".\private$\smsQueue";//
            }
            catch (Exception ex)
            {
               // throw ex;
            }
        }

        internal DataTable GetPegasusDstvTransactionsToSend()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetPegasusDstvTransactionsToSend");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                return GetPegasusDstvTransactionsToSend();
            }
            return datatable;
        }

        internal DataTable GetPegasusDstvTransactionsToSend_1()
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetPegasusDstvTransactionsToSend_1");
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "PEGASUS", DateTime.Now, "DSTV");
                return GetPegasusDstvTransactionsToSend();
            }
            return datatable;
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
                command = PegPayInterface.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode,GetServerIpIpValue());
                PegPayInterface.ExecuteNonQuery(command);
                ret = "YES";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        internal int TransferFailedTransaction(int PegPayId, string reason)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("TransferFailedTransaction", PegPayId, reason);
                int rows = PegPayInterface.ExecuteNonQuery(command);
                return rows;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void deleteTransaction2(string vendorTranId, string reason)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("DeleteTransation2", vendorTranId, reason);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetServerIpIpValue()
        {
            string ipaddress = null;
            try
            {
                object addresslist = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                foreach (IPAddress ip in (Dns.GetHostEntry(Dns.GetHostName()).AddressList))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipaddress = ip.ToString();
                    }
                    else
                    {
                        ipaddress = ip.ToString();
                    }
                }

                if (String.IsNullOrEmpty(ipaddress))
                {
                    ipaddress = "localhostip";
                }
            }
            catch (Exception ex)
            {

            }

            return ipaddress;
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

        public Customer[] GetAllDstvCustomers()
        {
            List<Customer> allCustomers = new List<Customer>();

            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetAllCustomers", "DSTV");
                DataTable results = PegPayInterface.ExecuteDataSet(command).Tables[0];

                foreach (DataRow row in results.Rows)
                {
                    Customer cust = new Customer();
                    cust.CustomerName = row["CustomerName"].ToString();
                    cust.CustomerType = row["CustomerType"].ToString();

                    //the customer number is always stored in the meterNo field
                    cust.CustomerRef = row["MeterNo"].ToString();
                    cust.Balance = row["AccountBal"].ToString();
                    cust.StatusCode = "0";
                    cust.StatusDescription = "SUCCESS";
                    allCustomers.Add(cust);
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return allCustomers.ToArray();
        }


        public Customer GetCustomerDetails(string customerRef, string AgentCode)
        {
            Customer cust = new Customer();

            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetCustomerDetails1", customerRef, "", AgentCode);
                DataTable results = PegPayInterface.ExecuteDataSet(command).Tables[0];

                if (results.Rows.Count > 0)
                {
                    DataRow row = results.Rows[0];

                    cust.CustomerName = row["CustomerName"].ToString();
                    cust.CustomerType = row["CustomerType"].ToString();
                    //the customer number is always stored in the meterNo field
                    cust.CustomerRef = row["MeterNo"].ToString();
                    cust.Balance = row["AccountBal"].ToString();
                    cust.StatusCode = "0";
                    cust.StatusDescription = "SUCCESS";
                    return cust;
                }
                else
                {
                    cust.StatusCode = "100";
                    cust.StatusDescription = "CUSTOMER NOT FOUND";
                    return cust;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        internal void UpdateBouquetPrice(BouquetDetails bouquetDetails)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateBouquetPrice", bouquetDetails.BouquetPrice, bouquetDetails.BouquetCode, bouquetDetails.PayTvCode);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateCustomerBalance(Customer customer)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateCustomerBalance", customer.Balance, customer.CustomerRef, "DSTV");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateReactivateRequestStatus(string smartCardNumber, string status, string sentFlag)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateReactivateRequestStatus", smartCardNumber, status, sentFlag);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
            }
        }

        internal DataTable GetAllPendingReactivateRequests()
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetAllPendingReactivateRequests");
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                return dt;
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

        internal void UpdateCustomerName(string CustName, int TranId)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateCustomerName", CustName, TranId);
                int rows = PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable GetBouquetByBouquetCode(string bouquetCode)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetBouquetByBouquetCode", bouquetCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateBouquetCode(int TranId, string vendorTranId, string vendorCode, string bouuqetCode)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateBouquetCode", TranId, vendorTranId, vendorCode, bouuqetCode);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateSentTransactionById3(int TranId, string utilityReceipt, string status)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateTransactionStatusByTranId3", TranId, utilityReceipt, status);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw new Exception("Error At Update");
            }
        }

        public void SetPendingTransactionStatusToInserted()
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("SetPendingTransactionStatusToInserted");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
