using MerchantListener.EntityObjects;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerchantListener.Logic
{
    class DatabaseHandler
    {
        private Database MerchantDb;
        private DbCommand com;
        private string constr = "Listen";
        ListenerResponse resp = new ListenerResponse();

        public DatabaseHandler()
        {
            try
            {
                MerchantDb = DatabaseFactory.CreateDatabase(constr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        public ListenerResponse LogRequestResponse(string request, string response,string requestid, string method, string vendorcode,string sourceip, string serverip)
        {
            object[] paras = { request, response,  requestid, method, vendorcode,  sourceip,  serverip };
            string result = null;
            try
            {
                com = MerchantDb.GetStoredProcCommand("InsertRequestResponseLogs", paras);
                int q = MerchantDb.ExecuteNonQuery(com);
                if (q > 0)
                {
                    result = "Request Logged Successfully for Processing";
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                }
                else
                {
                    resp.StatusCode = "2";
                    resp.StatusDescription = "FAILED";
                }
                Console.WriteLine(result);
               // Console.ReadLine();
            }
            catch (Exception ex)
            {
                throw ex;
                resp.StatusCode = "200";
                resp.StatusDescription = ex.Message ;
            }

            return resp;
        }


        public DataTable GetVendorDetails(string vendorcode, string vendorpassword)
        {
            object[] paras = { vendorcode, vendorpassword };
            try
            {
                com = MerchantDb.GetStoredProcCommand("GetEXTVendorDetails", paras);
                DataTable dt = MerchantDb.ExecuteDataSet(com).Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ListenerResponse LogError(string VendorCode, string Method,string MerchantCode, string ErrorType, string Message, string RequestId)
        {
            object[] paras = { VendorCode, Method, MerchantCode, ErrorType, Message, RequestId };
            string result = null;
            try
            {
                com = MerchantDb.GetStoredProcCommand("InsertErrorLogs", paras);
                int q = MerchantDb.ExecuteNonQuery(com);
                if (q > 0)
                {
                    result = "Error Logged Successfully";
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                }
                else
                {
                    resp.StatusCode = "2";
                    resp.StatusDescription = "FAILED";
                }
            }
            catch (Exception ex)
            {
                throw ex;
                resp.StatusCode = "200";
                resp.StatusDescription = ex.Message;
            }

            return resp;
        }

    }
}
