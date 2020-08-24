using LiveVasUtilityTranProcessorLibrary.EntityObjects;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveVasUtilityTranProcessorLibrary.ControlObjects
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




        public Transaction[] GetPendingTransactions()
        {
            List<Transaction> pendingTrans = new List<Transaction>();
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("GetPendingKcbVasTransactions");
                DataTable dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    Transaction tr = new Transaction();
                    try
                    {
                        tr.RecordId = dr["Id"].ToString();
                        tr.VendorTranId = dr["ReferenceId"].ToString();
                        tr.CustRef = dr["BeneficiaryID"].ToString();
                        string phone = dr["MSISDN"].ToString();
                        phone = phone.Replace(" ", String.Empty);
                        phone = phone.Replace("-", String.Empty);
                        tr.CustomerTel = phone;
                        tr.CustName = dr["BeneficiaryName"].ToString();

                        tr.Area = dr["Area"].ToString();
                        tr.TransactionAmount = dr["Amount"].ToString();
                        tr.MerchantID = dr["MechantID"].ToString();
                        tr.PaymentDate = DateTime.Parse(dr["PaymentDate"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        tr.RecordDate = DateTime.Parse(dr["PaymentDate"].ToString());

                        tr.UtilityCompany = dr["UtilityCode"].ToString();
                        tr.Narration = dr["Narration"].ToString();
                        tr.Teller = tr.CustRef;
                        tr.VendorCode = "STANBIC_VAS";
                        tr.Password = "53P48KU262";//live //"62S44HN420";//test
                        tr.Offline = "0";
                        tr.Reversal = "0";
                        tr.TransactionType = "CASH";
                        tr.PaymentType = "2";
                        tr.TransactionType = "CASH";

                        tr.StatusCode = "0";
                        tr.StatusDescription = "SUCCESS";

                        string merchantID = dr["MechantID"].ToString();
                        ServiceDetails details = GetServiceDetails(merchantID);

                        tr.UtilityCompany = details.ServiceCode;




                    }
                    catch (Exception ex)
                    {
                        tr.StatusCode = "100";
                        tr.StatusDescription = ex.Message;
                    }
                    pendingTrans.Add(tr);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pendingTrans.ToArray();
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
                        tr.RecordId = dr["Id"].ToString();
                        tr.VendorTranId = dr["ReferenceId"].ToString();
                        tr.CustRef = dr["BeneficiaryID"].ToString();
                      
                        tr.StatusCode = dr["Status"].ToString().ToUpper().Contains("SUCCESS")? "0":"100";
                        tr.StatusDescription = dr["Status"].ToString() ;

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


        public ServiceDetails GetServiceDetails(string serviceID)
        {
            ServiceDetails details = new ServiceDetails();
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("GetServiceCode", serviceID);
                DataTable dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    details.MerchantID = dt.Rows[0]["ServiceID"].ToString();
                    details.ServiceName = dt.Rows[0]["ServiceName"].ToString();
                    details.ServiceCode = dt.Rows[0]["ServiceCode"].ToString();
                    details.ServiceType = dt.Rows[0]["ServiceID"].ToString();
                    details.StatusCode = "0";
                    details.StatusDescription = "SUCCESS";
                }
                else
                {
                    details.StatusCode = "100";
                    details.StatusDescription = "NOT FOUND";
                }
                return details;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void UpdateTransactionStatus(string TranId, string status, string utilityTranRef)
        {
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("UpdateKcbVasTransactionStatus", TranId, status, utilityTranRef);
                ThirdPartyDB.ExecuteNonQuery(mycommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable getUtilityCredentials(string utilitycode, string agentcode)
        {
            DataTable dt = new DataTable();
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("GetUtilityCredentials", agentcode, utilitycode);
                dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
            }
            catch (Exception ee)
            {

                throw ee;
            }
            return dt;
        }

        public void LogXmlRequestResponse(string vendorId, string request, string response)
        {
            try
            {
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("LogXmlRequestResponse", vendorId, request, response);
                ThirdPartyDB.ExecuteNonQuery(mycommand);
            }
            catch (Exception ee)
            {

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


        public string GetSystemSetting(string ValueCode, string ValueGroupCode)
        {
            string valueVariable = "";
            try
            {
                mycommand = ThirdPartyDB.GetStoredProcCommand("GetSystemSettings", ValueCode, ValueGroupCode);
                DataTable dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    valueVariable = dt.Rows[0]["ValueVarriable"].ToString();

                }
            }
            catch (Exception ex)
            {
            }
            return valueVariable;
        }


    }
}
