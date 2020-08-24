using KCBNotifierLibrary.EntityObjects;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCBNotifierLibrary.ControlObjects
{
    public class DatabaseHandler
    {
        private Database ThirdPartyDB;
        private DbCommand mycommand;

        public DatabaseHandler()
        {
            try
            {
                ThirdPartyDB = DatabaseFactory.CreateDatabase("TestPegPay");
            }
            catch (Exception up)
            {
                throw up;
            }
        }
        
        internal Transaction[] GetPocessedTransactions()
        {
            List<Transaction> processedTrans = new List<Transaction>();
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("GetProcessedKcbVasTransactions");
                DataTable dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    Transaction tr = new Transaction();
                    try
                    {
                        //tr.RecordId = dr["Id"].ToString();
                        tr.VendorTranId = dr["VendorTranId"].ToString();
                        tr.CustRef = dr["CustomerRef"].ToString();

                        tr.StatusCode = dr["Status"].ToString().ToUpper().Contains("SUCCESS") ? "0" : "100";
                        tr.StatusDescription = dr["Status"].ToString();

                    }
                    catch (Exception ex)
                    {
                        tr.StatusCode = "100";
                        tr.StatusDescription = ex.Message;
                    }
                    processedTrans.Add(tr);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return processedTrans.ToArray();
        }


        internal void LogError(string error, string vendorCode, DateTime now, string AgentCode)
        {
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("LogError", error, vendorCode, now, AgentCode);
                ThirdPartyDB.ExecuteNonQuery(mycommand);

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
                DbCommand command = ThirdPartyDB.GetStoredProcCommand("InsertIntoUtilityResponseLogs", vendorCode, vendorTranId, respCode, respDesc, OtherData);
                ThirdPartyDB.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateTransactionStatus(string TranId, string status, string utilityTranRef)
        {
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("UpdateKcbReceivedTxnStatus", TranId, status, utilityTranRef);
                ThirdPartyDB.ExecuteNonQuery(mycommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void LogErrorKCB(string reference, string name, DateTime now, string requestType, string phoneNumber, string error, string TranId)
        {
            try
            {
                DbCommand command = ThirdPartyDB.GetStoredProcCommand("LogErrorKCB", reference, name, now, requestType, phoneNumber, error, TranId);
                ThirdPartyDB.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void UpdateTransactionStatus(string vendorTranId)
        {
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("UpdateKcbReceivedTxnStatus2", vendorTranId);
                ThirdPartyDB.ExecuteNonQuery(mycommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
