using LiveVasUtilitySenderLibrary.EntityObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveVasUtilitySenderLibrary.ControlObjects
{
    public class Processor
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Processor));
        DatabaseHandler dh = new DatabaseHandler();
        BusinessLogic bll = new BusinessLogic();

        public void ProcessPendingTransactions()
        {
            DatabaseHandler dh = new DatabaseHandler();
            Transaction[] pendingTrans = dh.GetPendingTransactions(); // gets transactions from received taqble and takes to school fees db
            
            SendPendingTransactionsToUtility(pendingTrans);


        }

        private void SendPendingTransactionsToUtility(Transaction[] pendingTrans)
        {
            foreach (Transaction tran in pendingTrans)
            {
                DatabaseHandler dh = new DatabaseHandler();
                try
                {

                    CreateWorkerThreadToProcessTran(tran);

                   
                }
                catch (Exception)
                {
                    // dh.LogError("SendPendingTransactionsToUtility TranId: " + tran.VendorTranId + " : " + e.Message, tran.VendorCode, DateTime.Now, tran.UtilityCompany);
                    throw;
                }
            }
        }

        private void CreateWorkerThreadToProcessTran(Transaction tran)
        {
           
                DoWork(tran);
           
        }

        public void DoWork(Object obj)
        {
            DatabaseHandler dh = new DatabaseHandler();
            try
            {
                Transaction tran = obj as Transaction;

                ProcessTransaction(tran);


            }
            catch (Exception e)
            {
                dh.LogError(e.Message, "KCB_VAS", DateTime.Now, "ERROR");
            }
        }

        private void ProcessTransaction(Transaction tran)
        {
            PostResponse postResp = new PostResponse();
            if (tran.PassesValidation())
            {


                if (IsFlexipaySchools(tran))
                {
                    postResp = bll.SendToFlexipaySchools(tran);
                   
                }
            }
            //it fails validation
            else
            {
                postResp.StatusCode = "100";
                postResp.StatusDescription = tran.StatusDescription;
            }
            bll.HandleUtilityResponse(postResp, tran);
        }

        private bool IsFlexipaySchools(Transaction tran)
        {
            bool isFexipay = false;
            if (tran.UtilityCompany == "FLEXIPAY")
            {
                isFexipay = true;
            }
            return isFexipay;
        }
    }
}
