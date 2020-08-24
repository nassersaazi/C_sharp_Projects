using LiveVasUtilitySenderLibrary.EntityObjects;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LiveVasUtilitySenderLibrary.ControlObjects
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
                DbCommand mycommand = ThirdPartyDB.GetStoredProcCommand("GetInsertedKcbReceivedTransactions");
                DataTable dt = ThirdPartyDB.ExecuteDataSet(mycommand).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    Transaction tr = new Transaction();
                    try
                    {
                        //tr.RecordId = dr["Id"].ToString();
                        tr.VendorTranId = dr["TranId"].ToString();
                        tr.CustRef = dr["CustomerRef"].ToString();
                        string phone = dr["CustomerTel"].ToString();
                        phone = phone.Replace(" ", String.Empty);
                        phone = phone.Replace("-", String.Empty);
                        tr.CustomerTel = phone;
                        string name = dr["CustomerName"].ToString();

                        // Formatting the name so that it does not fuck up the digital signature 
                      
                        tr.CustName = Regex.Replace(name, @"\s+", " ");

                        tr.Area = dr["Area"].ToString();
                        tr.TransactionAmount = Convert.ToInt32(dr["TranAmount"]).ToString();
                  //      tr.MerchantID = dr["MechantID"].ToString();
                        tr.PaymentDate = DateTime.Parse(dr["PaymentDate"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        tr.RecordDate = DateTime.Parse(dr["PaymentDate"].ToString());

                        tr.UtilityCompany = dr["UtilityCode"].ToString();
                        tr.Narration = dr["TranNarration"].ToString();
                        tr.Teller = tr.CustRef;
                        tr.VendorCode = "STANBIC_VAS";
                        tr.Password = "53P48KU262";//live //"62S44HN420";//test
                        tr.Offline = "0";
                        tr.Reversal = "0";
                        tr.TransactionType = "CASH";
                        tr.PaymentType = "2";
                        //tr.TransactionType = "CASH";

                        tr.StatusCode = "0";
                        tr.StatusDescription = "SUCCESS";

                        //string merchantID = dr["MechantID"].ToString();
                        //ServiceDetails details = GetServiceDetails(merchantID);

                       // tr.UtilityCompany = details.ServiceCode;




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
    }
}
