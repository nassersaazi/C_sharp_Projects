using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

namespace StartimesProcessor
{
    public class DataLogic
    {
        private Database PegasusUssddb;
        private DbCommand command;

        public DataLogic()
        {
            PegasusUssddb = DatabaseFactory.CreateDatabase("LivePegasusUssddbConnection");
        }

        public DataSet ExecuteDataSet(string procedure, params object[] parameters)
        {
            try
            {
                command = PegasusUssddb.GetStoredProcCommand(procedure, parameters);
                return PegasusUssddb.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ExecuteNonQuery(string procedure, params object[] parameters)
        {
            try
            {
                command = PegasusUssddb.GetStoredProcCommand(procedure, parameters);
                PegasusUssddb.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        internal Request[] GetTransactionRequests()
        {
            DataTable dt = ExecuteDataSet("GetTransactionRequests", "").Tables[0];
            List<Request> requests = new List<Request>();
            foreach (DataRow dr in dt.Rows)
            {
                Request request = new Request();
                request.TranId = dr["TransNo"].ToString();
                request.CustRef = dr["CustomerRef"].ToString();
                request.CustName = dr["CustomerName"].ToString();
                request.CustTel = dr["CustomerTel"].ToString();
                request.Bouquet = dr["CustomerType"].ToString();
                request.Amount = dr["TranAmount"].ToString();
                request.AccountFrom = dr["FromAccount"].ToString();
                request.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                request.PayType = dr["PaymentType"].ToString();
                request.TranType = dr["TranType"].ToString();
                request.Teller = dr["SessionID"].ToString();
                request.Area = dr["Area"].ToString();
                request.Narration = dr["Narration"].ToString();//change bouquet or payment
                request.SessionId = request.Teller;
                requests.Add(request);
            }
            return requests.ToArray();
        }
        internal Request[] GetTransactionRequests2()
        {
            DataTable dt = ExecuteDataSet("GetTransactionRequests_2", "").Tables[0];
            List<Request> requests = new List<Request>();
            foreach (DataRow dr in dt.Rows)
            {
                Request request = new Request();
                request.TranId = dr["TransNo"].ToString();
                request.CustRef = dr["CustomerRef"].ToString();
                request.CustName = dr["CustomerName"].ToString();
                request.CustTel = dr["CustomerTel"].ToString();
                request.Bouquet = dr["CustomerType"].ToString();
                request.Amount = dr["TranAmount"].ToString();
                request.AccountFrom = dr["FromAccount"].ToString();
                request.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                request.PayType = dr["PaymentType"].ToString();
                request.TranType = dr["TranType"].ToString();
                request.Teller = dr["SessionID"].ToString();
                request.Area = dr["Area"].ToString();
                request.Narration = dr["Narration"].ToString();//change bouquet or payment
                request.SessionId = request.Teller;
                requests.Add(request);
            }
            return requests.ToArray();
        }
        internal Request[] GetTransactionRequests3()
        {
            DataTable dt = ExecuteDataSet("GetTransactionRequests_3", "").Tables[0];
            List<Request> requests = new List<Request>();
            foreach (DataRow dr in dt.Rows)
            {
                Request request = new Request();
                request.TranId = dr["TransNo"].ToString();
                request.CustRef = dr["CustomerRef"].ToString();
                request.CustName = dr["CustomerName"].ToString();
                request.CustTel = dr["CustomerTel"].ToString();
                request.Bouquet = dr["CustomerType"].ToString();
                request.Amount = dr["TranAmount"].ToString();
                request.AccountFrom = dr["FromAccount"].ToString();
                request.PaymentDate = DateTime.Now.ToString("dd/MM/yyyy");
                request.PayType = dr["PaymentType"].ToString();
                request.TranType = dr["TranType"].ToString();
                request.Teller = dr["SessionID"].ToString();
                request.Area = dr["Area"].ToString();
                request.Narration = dr["Narration"].ToString();//change bouquet or payment
                request.SessionId = request.Teller;
                requests.Add(request);
            }
            return requests.ToArray();
        }

        internal void UpdateTransactionStatus(string transactionId, string postingId, string reason, string status, string idType)
        {
            try
            {
                object[] data = { transactionId, postingId, reason, status, idType };
                ExecuteNonQuery("UpdateTransactionStatus", data);
            }
            catch (Exception ee)
            {

            }
        }

        internal void LogError(Request request, object p, object p_3)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        internal DataTable GetVendorCredentials(string vendorCode)
        {
            try
            {
                DataTable dt = ExecuteDataSet("GetVendorCreds", vendorCode).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal Request[] GetPayments()
        {
            DataTable dt = ExecuteDataSet("GetPaymentsToNotify", "").Tables[0];
            List<Request> requests = new List<Request>();
            foreach (DataRow dr in dt.Rows)
            {
                Request request = new Request();
                request.TranId = dr["TransNo"].ToString();
                request.PostId = dr["PaymentReference"].ToString();
                request.CustRef = dr["CustomerRef"].ToString();
                request.CustName = dr["CustomerName"].ToString();
                request.CustTel = dr["CustomerTel"].ToString();
                request.Bouquet = dr["CustomerType"].ToString();
                request.Amount = dr["TranAmount"].ToString();
                request.AccountFrom = dr["FromAccount"].ToString();
                request.PaymentDate = DateTime.Parse(dr["PaymentDate"].ToString()).ToString("dd/MM/yyyy");
                request.PayType = dr["PaymentType"].ToString();
                request.TranType = dr["TranType"].ToString();
                request.Teller = dr["SessionID"].ToString();
                request.Area = dr["Area"].ToString();//holds bouquet no for change bouquet request
                request.Narration = dr["Narration"].ToString();
                requests.Add(request);
            }
            return requests.ToArray();
        }
    }
}
