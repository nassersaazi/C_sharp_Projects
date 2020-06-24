using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Serialization;
using WorkingLogic;
using WorkingLogic.EntityObjects;

namespace pegbankApi
{
    /// <summary>
    /// Summary description for pegbank
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class pegbank : System.Web.Services.WebService
    {
        BusinessLogic bl = new BusinessLogic();
        DatabaseHandler dh = new DatabaseHandler();

        [WebMethod]
        public string RegisterClient(PostTransactionRequest req)
        {
            PostTransactionRequest requestData = new PostTransactionRequest();
            requestData.firstName = req.firstName;
            requestData.lastName = req.lastName;
            requestData.phoneNumber = req.phoneNumber;
            requestData.email= req.email;
            try
            {
                bl.Serialize(requestData);
                return bl.SaveClientToDatabase(requestData) > 0?"success":"insertion error";
            }
            catch (Exception e)
            {
                dh.LogError(DateTime.Now, e.Message, "");
                return "insertion error";
            }
            
        }

        [WebMethod]
        public string DepositFunds(Transaction req )
        {
            try
            {
                Transaction tran = new Transaction();
                tran.AccountNumber = req.AccountNumber;
                tran.Amount = req.Amount;

                bl.Serialize(tran);
                return dh.LogDeposit(tran) > 0 ?"success":null;
            }
            catch (Exception e)
            {
                dh.LogError(DateTime.Now, e.Message, "");
                return null;
            }
        }

        [WebMethod]
        public string WithdrawFunds(Transaction request)
        {
            try
            {
                Transaction withdrawtrans = new Transaction();
                withdrawtrans.AccountNumber = request.AccountNumber;
                withdrawtrans.Amount = request.Amount;

                bl.Serialize(withdrawtrans);
                return dh.LogWithdraw(withdrawtrans) > 0 ? "success" : null;
            }
            catch (Exception e)
            {
                dh.LogError(DateTime.Now, e.Message, "");
                return null;
            }
        }

        [WebMethod]
        public DataTable CheckBalance(string accountNumber)
        {
            DataTable Id = null;
                try
                {
                    Transaction tran = new Transaction();
                    //tran.AccountNumber = accountNumber;
                    Id = dh.CheckBalance(accountNumber);
                    bl.Serialize(Id);
                foreach (DataRow drow in Id.Rows)
                {
                    string AccountNo = drow["AccountNo"].ToString();
                    string Name = drow["Name"].ToString();
                    string AccountBalance = drow["AccountBalance"].ToString();
                }
                        return Id;
                }
                catch (Exception e)
                {
                    dh.LogError(DateTime.Now, e.Message, "");
                    return Id;
                }          
        }

        [WebMethod]
        public DataTable GetStatement(string accountNumber, DateTime fromDate, DateTime toDate)
        {
            DataTable Id = null;
            try
            {   
                Transaction tran = new Transaction();
                //tran.AccountNumber = accountNumber;
                Id = dh.GetStatement(accountNumber, fromDate, toDate);
                bl.Serialize(Id);
               
                return Id;
            }
            catch (Exception e)
            {
                dh.LogError(DateTime.Now, "Exception: " + e.Message, "");
                return Id;
            }
        }

        


    }
}
