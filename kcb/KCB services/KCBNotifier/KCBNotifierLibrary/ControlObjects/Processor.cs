using KCBNotifierLibrary.EntityObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KCBNotifierLibrary.ControlObjects
{
    public class Processor
    {
    
        DatabaseHandler dh = new DatabaseHandler();
        BusinessLogic bl = new BusinessLogic();

        

        public void NotifyProcessedTransactions(bool isDebugging)
        {

            Transaction[] processedTrans = dh.GetPocessedTransactions();
            Console.WriteLine("Total Not Notified to vendor :" + processedTrans.Length);
            NotifyTransactionsToVendor(processedTrans, isDebugging);
        }

        private void NotifyTransactionsToVendor(Transaction[] processedTrans, bool isDebugging)
        {
            foreach (Transaction tran in processedTrans)
            {
                DatabaseHandler dh = new DatabaseHandler();
                try
                {
                    Console.WriteLine("************************************************************");
                    Console.WriteLine("VendorTranId : " + tran.VendorTranId + " : PaymentDate" + tran.PaymentDate);


                    CreateNotifierThread(tran, isDebugging);

                    Console.WriteLine("************************************************************");
                }
                catch (Exception)
                {
                    // dh.LogError("SendPendingTransactionsToUtility TranId: " + tran.VendorTranId + " : " + e.Message, tran.VendorCode, DateTime.Now, tran.UtilityCompany);
                    throw;
                }
            }
        }

        private void CreateNotifierThread(Transaction tran, bool isDebugging)
        {
            if (isDebugging)
            {

                NotifyVendor(tran);
            }
            else
            {

                // Thread notifierThread = new Thread(new ParameterizedThreadStart(NotifyVendor));
                // notifierThread.Start(tran);
            }

        }



       private void NotifyVendor(Transaction obj)
        {
            try
            {
                Transaction tran = obj as Transaction;

                bl.NotifyProcessedTransaction(tran);


            }
            catch (Exception e)
            {
                dh.LogError(e.Message, "KCB_VAS", DateTime.Now, "ERROR");
            }
        }
        

    }
}
