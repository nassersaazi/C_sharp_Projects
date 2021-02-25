using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using DSTVListener.EntityObjects;

namespace DSTVListener.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayDB;
        private DataTable dt = new DataTable();
        private DbCommand command;
        public static string QueueName = "";
        private string conString = "LivePegPay";
        // private string conString = "TestPegPay";
        public DatabaseHandler()
        {
            try
            {
                //PegPayDB = DatabaseFactory.CreateDatabase(conString);
                PegPayDB = DbLayer.CreateDatabase("LivePegPay", DbLayer.DB2);
                if (conString.Equals("LivePegPay"))
                {
                    QueueName = @".\private$\MtnDstvQueue";
                }
                else
                {
                    QueueName = @".\private$\testMtnDstvQueue";
                }
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
                command = PegPayDB.GetStoredProcCommand("GetStatusDescr", statusCode);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    descr = dt.Rows[0]["StatusDescription"].ToString();
                }
                else
                {
                    descr = "GENERAL ERROR AT PEGASUS failed to get status description";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return descr;
        }
        //internal string LogConfirmationRequest(ConfrimPaymentRequest ConfirmationRequest)
        //{
        //    string status = "";
        //    try
        //    {
        //        command = MobileMoney.GetStoredProcCommand("LogConfirmationRequest", ConfirmationRequest.ProcessingNumber.Trim(), ConfirmationRequest.SenderID.Trim(), ConfirmationRequest.AcctRef.Trim(),
        //            ConfirmationRequest.RequestAmount.Trim(), ConfirmationRequest.PaymentRef.Trim(), ConfirmationRequest.ThirdPartyTransactionID.Trim(), ConfirmationRequest.MOMAcctNum.Trim(), ConfirmationRequest.CustName.Trim()
        //            , ConfirmationRequest.TXNType.Trim(), ConfirmationRequest.StatusCode.Trim(), ConfirmationRequest.OpCoID.Trim());
        //        //MobileMoney.ExecuteNonQuery(command);
        //        DataTable dt = MobileMoney.ExecuteDataSet(command).Tables[0];

        //        if (dt.Rows.Count != 0)
        //        {
        //            //status = dt.Rows[0][0].ToString();
        //        }
        //        status = "SUCCESS";
        //        return status;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        internal DataTable GetValidSystemSetting(int ValueGroupCode, string valueVariable)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetValidSystemSetting", ValueGroupCode, valueVariable);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
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
                command = PegPayDB.GetStoredProcCommand("GetSystemSetting", GroupCode, valueCode);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                value = dt.Rows[0]["ValueVarriable"].ToString().Trim();
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //internal DataTable GetValidTransactionDetail(RequestConfirmation ConfirmationRequest)
        //{
        //    try
        //    {
        //        command = MobileMoney.GetStoredProcCommand("GetValidTransactionDetail", ConfirmationRequest.ThirdPartyTransactionID);
        //        dt = MobileMoney.ExecuteDataSet(command).Tables[0];
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //internal string LogQueryThirdPartyAccountRequest(QueryAccountDetailsRequest query)
        //{
        //    string queryId = "";
        //    try
        //    {
        //        command = MobileMoney.GetStoredProcCommand("LogQueryThirdPartyAccountRequest", query.TraceUniqueID, query.ServiceId, query.ProcessingNumber.Trim(), query.SenderID.Trim(), query.AcctRef.Trim(),
        //               query.PrefLang.Trim(), query.OpCoID.Trim(), query.Utility);
        //        DataTable dt = MobileMoney.ExecuteDataSet(command).Tables[0];
        //        if (dt.Rows.Count > 0)
        //        {
        //            queryId = dt.Rows[0]["QueryId"].ToString();
        //        }
        //        return queryId;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        internal DataTable GetPaymentByThirdPartyTranId(string ThirdPartyTranId)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetPaymentByProcNo", ThirdPartyTranId);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void logSMS(string Phone, string message, string Mask, string sender)
        {
            try
            {

                command = PegPayDB.GetStoredProcCommand("InsertSmsToSend", Phone, message, Mask, sender);
                PegPayDB.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void LogError(string exception)
        {
            try
            {
                //@Msg varchar(MAX),
                //@Code varchar(50),
                //@Date datetime,
                //@Utility varchar(50)
                command = PegPayDB.GetStoredProcCommand("LogError",exception, "MTN", DateTime.Now, "DSTV");
                PegPayDB.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LogRequest(string utilitycode, string tranId, string request,string response)
        {
            try
            {
                //LogXmlRequestResponse
                command = PegPayDB.GetStoredProcCommand("LogXmlRequestResponseNew", utilitycode, tranId, request, response);
                PegPayDB.ExecuteNonQuery(command);
            }
            catch (Exception ee)
            {
                
            }
        }

        internal DataTable GetDuplicateVendorRef(string VendorTranId)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetDuplicateVendorRef2", VendorTranId);
                DataTable returndetails = PegPayDB.ExecuteDataSet(command).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int SaveReactivationRequest(string smartCardNumber)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("SaveReactivateRequest", smartCardNumber);
                int rows = PegPayDB.ExecuteNonQuery(command);
                return rows;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        internal Customer GetCustomerDetails(string customerReference, string area, string UtilityCode)
        {
            Customer cust = new Customer();
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetCustomerDetails1", customerReference, area, UtilityCode);
                DataTable dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    cust.CustomerRef = dt.Rows[0]["CustomerRef"].ToString();
                    cust.CustomerName = dt.Rows[0]["CustomerName"].ToString();
                    cust.CustomerType = dt.Rows[0]["CustomerType"].ToString();
                    cust.Area = dt.Rows[0]["Area"].ToString();
                    cust.AgentCode = dt.Rows[0]["AgentCode"].ToString();
                    cust.Balance = dt.Rows[0]["AccountBal"].ToString();
                    cust.StatusCode = "0";
                    cust.StatusDescription = "SUCCESS";
                }
                else
                {
                    cust.StatusCode = "1";
                    cust.StatusDescription = "CUSTOMER DETAILS DON'T EXIST";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cust;
        }


        public int[] CheckDBForVendorTranId(string VendorTranId)
        {
            int[] rowsReturned = new int[2];
            try
            {
                command = PegPayDB.GetStoredProcCommand("CheckDBForVendorTranId", VendorTranId, "DSTV", "MTN");
                rowsReturned[0] = PegPayDB.ExecuteDataSet(command).Tables[0].Rows.Count;
                rowsReturned[1] = PegPayDB.ExecuteDataSet(command).Tables[0].Rows.Count;
                return rowsReturned;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataSet ExecuteDataSet(string procedure, params object[] parameters)
        {
            try
            {
                return PegPayDB.ExecuteDataSet(procedure, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
