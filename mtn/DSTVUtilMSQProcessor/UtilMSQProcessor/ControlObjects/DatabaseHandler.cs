using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Globalization;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using UtilMSQProcessor.EntityObjects;

namespace UtilMSQProcessor.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayInterface;
        private DbCommand command;
        public string queue;
        public string stkQueue;
        public string SmsQueue;
        //public string ConnectionString = "TestPegPayConnectionString";
        public string ConnectionString = "PegPayConnectionString";

        public DatabaseHandler()
        {
            try
            {
                //PegPayInterface = DatabaseFactory.CreateDatabase(ConnectionString);
                PegPayInterface = DbLayer.CreateDatabase("PegPayConnectionString", DbLayer.DB2);
                if (ConnectionString.Equals("PegPayConnectionString"))
                {
                    queue = @".\private$\MtnDstvQueue";
                    stkQueue = @".\private$\MtnStkQueue";
                }
                else
                {
                    queue = @".\private$\testmtnDSTVQueue";
                    stkQueue = @".\private$\testmtnSTKQueue";
                }
                SmsQueue = @".\private$\smsQueue";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal void SavePostLog(Transaction trans, string utilityCode, string sourceIp, string sourcePort)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("InsertPostLog", trans.CustomerRef, trans.CustomerName, trans.CustomerType, trans.CustomerTel, "", "", trans.TranAmount, trans.PaymentDate, trans.TranType, trans.PaymentType, trans.VendorTranId, trans.TranNarration, trans.VendorCode, trans.Teller, trans.Reversal, "", trans.Offline, utilityCode, "", "", sourceIp, sourcePort, trans.DigitalSignature);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal void LogErrors(string exception)
        {
            try
            {
                //@Msg varchar(MAX),
                //@Code varchar(50),
                //@Date datetime,
                //@Utility varchar(50)
                command = PegPayInterface.GetStoredProcCommand("LogError", exception, "MTN", DateTime.Now, "DSTV");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {

            }
        }

        internal void InsertDeleteTransation(Transaction trans,string utilityCode,string reason)
        {
            try
            {
                trans.CustomerName = "";
                command = PegPayInterface.GetStoredProcCommand("InsertDeleteTransation", 0, "", trans.CustomerRef, trans.CustomerName, trans.CustomerType, trans.CustomerTel, trans.Area, "", double.Parse(trans.TranAmount.Trim()), DateTime.Now, DateTime.Now, trans.TranType, trans.PaymentType, trans.VendorTranId, "", "", false, trans.VendorCode, trans.CustomerTel, false, false, false, utilityCode, "", false, "", "", "", false, reason, "");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable CrossCheckVendorRef(string VendorTranId)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("CrossCheckVendorRef", VendorTranId);
                DataTable results = PegPayInterface.ExecuteDataSet(command).Tables[0];
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal Customer GetCustDetails(string custref, string UtilityCode, string Bouquet)
        {
            Customer cust = new Customer();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetDstvsCustomerDetailsMosesNew", custref, Bouquet, UtilityCode);//GetDstvsCustomerDetailsMoses
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                    cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
                    cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                    if (cust.Balance == "")
                    {
                        cust.Balance = "0";
                    }
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

        internal Customer GetCustDetailsV1(string custref, string UtilityCode)
        {
            Customer cust = new Customer();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetDstvsCustomerDetails", custref, "", UtilityCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                    cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
                    cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                    if (cust.Balance == "") 
                    {
                        cust.Balance = "0";
                    }
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

        internal bool CheckIfItWasSuccessfullAtECW(Transaction tr)
        {
            DataTable datatable = new DataTable();
            try
            {
                command = PegPayInterface.GetStoredProcCommand("CheckIfWasSuccessfullAtECW", tr.VendorTranId);
                datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (datatable.Rows.Count > 0)
                {
                    //UpdateSentToVendor(tr.VendorTransactionRef, 1);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on checking if was ever successful at MTN");
                LogError("ProcessDstvQueue7: " + tr.VendorTranId + " " + "Exception on checking if was ever successful at MTN");
                throw ex;
            }
        }

        internal void UpdateSentToVendor(string vendorTranId, int sentToVendor)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateSentToVendor", vendorTranId, sentToVendor);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal void UpdateSentToVendor1(string vendorTranId, int sentToVendor,DateTime queuetime)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("UpdateSentToVendor1", vendorTranId, sentToVendor,queuetime);
                PegPayInterface.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string GetSystemParameter(int GroupCode, int ValueCode)
        {
            string ret = "";
            try
            {
                command = PegPayInterface.GetStoredProcCommand("GetSystemSetting", GroupCode, ValueCode);
                DataTable datatable = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (datatable.Rows.Count > 0)
                {
                    ret = datatable.Rows[0]["ValueVarriable"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ret;
        }

        public void InsertIntoVendorResponseLogs(string vendorTranId, string errorMessage, string xmlResp, string status)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("InsertIntoVendorResponseLogs", vendorTranId, errorMessage, xmlResp, "" + DateTime.Now, status);
                PegPayInterface.ExecuteNonQuery(command);
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

        public void LogSMS(string CustomerTel, string VendorTransactionRef, string Msg, string Mask, string Service)
        {
            try
            {
                command = PegPayInterface.GetStoredProcCommand("InsertSmsToSend1", CustomerTel, Msg, Mask, Service, VendorTransactionRef);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal bool ExistsInPegPay(string vendorTranId, string utilityCode, string venorCode)
        {
            bool exists = false;
            try
            {
                command = PegPayInterface.GetStoredProcCommand("ExistsInPegPay", vendorTranId,utilityCode,venorCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on checking if exists in PegPay");
                LogError("ProcessDstvQueue9: " + vendorTranId + " " + "Exception on rchecking if exists in PegPay");
                throw ex;
            }
            return exists;
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

        internal bool ExistsInDeletedTable(string vendorTranId, string utilityCode, string venorCode)
        {
            bool exists = false;
            try
            {
                command = PegPayInterface.GetStoredProcCommand("ExistsInDeletedTable", vendorTranId, utilityCode, venorCode);
                DataTable dt = PegPayInterface.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    exists = true;
                }
                else
                {
                    exists = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on checking deleted");
                LogError("ProcessDstvQueue6: " + vendorTranId + " " + "Error on checking deleted");
                throw ex;
            }
            return exists;
        }

        internal void LogError(string exception)
        {
            try
            {
                //@Msg varchar(MAX),
                //@Code varchar(50),
                //@Date datetime,
                //@Utility varchar(50)
                command = PegPayInterface.GetStoredProcCommand("LogError", exception, "MTN", DateTime.Now, "DSTV");
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
               
            }
        }
        public void LogRequest(string utilitycode, string tranId, string request, string response)
        {
            try
            {
                //LogXmlRequestResponse
                command = PegPayInterface.GetStoredProcCommand("LogXmlRequestResponseNew", utilitycode, tranId, request, response);
                PegPayInterface.ExecuteNonQuery(command);
            }
            catch (Exception ee)
            {

            }
        }
    }
}
