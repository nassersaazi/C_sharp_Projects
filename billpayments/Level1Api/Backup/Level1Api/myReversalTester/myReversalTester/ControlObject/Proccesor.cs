using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using myReversalTester.EntityObject;
using System.Data;

namespace myReversalTester.ControlObject
{
    class Proccesor
        {

        BusinessLogic bll = new BusinessLogic();
        DatabaseHandler dh = new DatabaseHandler();
        internal void RequestedReversal()
        {
            try 
            {
                if (waterResp.PostError.Equals("NONE"))
                {
                    dp.UpdateSentTransaction(resp.PegPayPostId, "", "1");
                    resp.StatusCode = "0";
                    resp.StatusDescription = "SUCCESS";
                    bll.ReverseTransactionAtStanbic(trans);

                }
                else if (waterResp.PostError.Contains("GENERAL"))
                {
                    resp.StatusCode = "500";
                    resp.StatusDescription = waterResp.PostError;
                }
                else
                {
                    resp.PegPayPostId = "";
                    resp.StatusCode = "100";
                    resp.StatusDescription = waterResp.PostError + " AT NWSC";
                }
            
            } catch (Exception ex)
           {
               dh.LogError(ex.Message, "PREPAID_SERVICE", DateTime.Now, "ALL");
               /// put conf string to the db and test if values are received then put the log method
           }
            
           // Transaction trans = new Transaction();
            
           /* try
            {
               DataTable dt = dh.GetReversePrepaidTransactions();

               foreach (DataRow dr in dt.Rows)
               {
                   try
                   {
                       Transaction trans = GetTransObj(dr);
                       Console.WriteLine("confirmimg Transaction " + trans.VendorTransactionRef);

                       ProcessTransaction(trans);
                   }
                   catch (Exception ex)
                   {
                       dh.LogError(ex.Message, "PREPAID_SERVICE", DateTime.Now, "ALL");
                   }
               }
           }
           catch (Exception ex)
           {
               dh.LogError(ex.Message, "PREPAID_SERVICE", DateTime.Now, "ALL");
               /// put conf string to the db and test if values are received then put the log method
           }*/

        }


        private Transaction GetTransObj(DataRow dr)
        {
            Transaction trans = new Transaction();
            string utility = dr["UtilityCode"].ToString();
            int TranId = Int32.Parse(dr["TranId"].ToString());
            trans.UtilityCode = dr["UtilityCode"].ToString();
            trans.BankID = dr["VendorToken"].ToString();
            trans.PaymentType = dr["PaymentType"].ToString();
            trans.Area = dr["Area"].ToString();
            trans.TranId = TranId;
            trans.TransNo = dr["TransNo"].ToString();
            trans.UtilityTranRef = dr["UtilityTranRef"].ToString();
            trans.Narration = dr["TranNarration"].ToString();
            trans.CustName = dr["CustomerName"].ToString();
            trans.CustRef = dr["CustomerRef"].ToString();
            trans.CustomerTel = dr["CustomerTel"].ToString();
            trans.CustomerType = dr["CustomerType"].ToString();
            trans.VendorTransactionRef = dr["VendorTranId"].ToString();
            trans.TransactionType = dr["TranType"].ToString();
            trans.VendorCode = dr["VendorCode"].ToString();
            trans.Password = "";
            trans.Teller = dr["Teller"].ToString();
            trans.Reversal = dr["Reversal"].ToString().ToUpper();
            trans.PaymentDate = dr["PaymentDate"].ToString();
            trans.TransactionAmount = dr["TranAmount"].ToString();
            trans.TransactionAmount = (double.Parse(trans.TransactionAmount)).ToString();
            trans.DigitalSignature = "";
            trans.Email = "";
            trans.Status = dr["Status"].ToString();
            trans.SentTovendor = 0;// Convert.ToInt32(dr["SentToVendor"].ToString());
            if (trans.Reversal.Equals("FALSE"))
            {
                trans.Reversal = "0";
            }
            else
            {
                trans.Reversal = "1";
            }
            trans.Offline = "0";
            return trans;
        }

        private void ProcessTransaction(Transaction trans)
        {
            string UtilityCode = trans.UtilityCode.ToUpper();
            PostResponse utilityResp = new PostResponse();
            Console.WriteLine("Going to " + trans.UtilityCode);
            if (UtilityCode == "UMEME")
            {
                utilityResp = bll.SendToUmeme(trans);
            }
          /*  else if (UtilityCode == "NWSC")
            {
               utilityResp = bll.SendToNWSC(trans);
            }*/
            else
            {
               utilityResp = bll.GetUtilityNotSupportedResponse();
            }
            Console.WriteLine("Utility response=" + utilityResp.StatusDescription);

            bll.HandleUtilityResponse(utilityResp, trans);
            Console.WriteLine("Prepaid Reverse processing Finished");
        }
    }
}
