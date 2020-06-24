using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkingLogic.EntityObjects;

namespace WorkingLogic
{
    public class DatabaseHandler
    {
        BusinessLogic bll = new BusinessLogic();
        private Database ThirdPartyDB;
        private DataTable dt = new DataTable();
        private DbCommand command;

        public DatabaseHandler()
        {
            try
            {
                ThirdPartyDB = DatabaseFactory.CreateDatabase("TestBank");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public int InsertIntoClientTable(PostTransactionRequest tran)
        {
            int result = 0;
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("InsertIntoClientTable",
                                                            
                                                            tran.firstName,
                                                            tran.lastName,
                                                            tran.phoneNumber,
                                                            tran.email
                                                           
                                                            );
                return ThirdPartyDB.ExecuteNonQuery(command);
                
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now, ex.Message, "");
                return result;
            }
        }

        public int LogDeposit(Transaction trans)
        {
            string reference = bll.GenerateRandomNNumber();
            int result = 0;
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("DepositFunds",

                                                            trans.AccountNumber,
                                                            trans.Amount,
                                                            reference
                                                            );
                return ThirdPartyDB.ExecuteNonQuery(command);
               
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now, ex.Message, "");
                return result;
            }
        }

        public int LogWithdraw(Transaction trans)
        {
            int result = 0;
            string reference = bll.GenerateRandomNNumber();
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("WithdrawFunds",

                                                            trans.AccountNumber,
                                                            trans.Amount,
                                                            reference
                                                            );
                return ThirdPartyDB.ExecuteNonQuery(command);
                
            }
            catch (Exception ex)
            {
                LogError(DateTime.Now, ex.Message, "");
                return result;
            }
        }


        public DataTable CheckBalance(string AccountNo)
        {
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("CheckBalanceNew", AccountNo);

                dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)  
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetStatement(string AccountNo, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("GetStatement", AccountNo, FromDate, ToDate);

                dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;

        }

        internal string LogRequestAndResponse(string Method, string requestData, DateTime requestTime, string responseData, DateTime responseTime, string CustomerReference, string ReferenceId)
        {
            string ret = "";
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("LogVasXmlRequestResponse", Method, CustomerReference, ReferenceId, requestTime, requestData, responseTime, responseData);
                ThirdPartyDB.ExecuteNonQuery(command);
                ret = "YES";
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }

        public void LogError( DateTime now, string error, string TranId)
        {
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("LogError",  now, error, TranId);
                ThirdPartyDB.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveRequestlog( string message, DateTime now)
        {
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("SaveRequestLogs",message, now);
                command.CommandTimeout = 300000000;
                ThirdPartyDB.ExecuteNonQuery(command);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetAccounts()
        {
            try
            {
                command = ThirdPartyDB.GetStoredProcCommand("GetAccounts");

                dt = ThirdPartyDB.ExecuteDataSet(command).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

    }
}
