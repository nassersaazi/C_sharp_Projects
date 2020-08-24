using LiveVasUtilityTranProcessorLibrary.EntityObjects;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LiveVasUtilityTranProcessorLibrary.ControlObjects
{
    public class Processor
    {
        public static readonly ILog log = LogManager.GetLogger(typeof(Processor));
        DatabaseHandler dh = new DatabaseHandler();
        BussinessLogic bll = new BussinessLogic();

        public void ProcessPendingTransactions()
        {

            Transaction[] pendingTrans = dh.GetPendingTransactions();
            //Console.WriteLine("Total Pending :" + pendingTrans.Length);
            SendPendingTransactionsToUtility(pendingTrans);
        }

      
       
        private void SendPendingTransactionsToUtility(Transaction[] pendingTrans)
        {
            foreach (Transaction tran in pendingTrans)
            {
                DatabaseHandler dh = new DatabaseHandler();
                try
                {
                    //Console.WriteLine("************************************************************");
                    //Console.WriteLine("VendorTranId : " + tran.VendorTranId + " : PaymentDate" + tran.PaymentDate);


                    CreateWorkerThreadToProcessTran(tran);

                    //Console.WriteLine("************************************************************");
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
                //postResp = bll.SendToFlexipaySchools(tran);
                postResp = bll.SendToUtilitiesApi(tran);
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
