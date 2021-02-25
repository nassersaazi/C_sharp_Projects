using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using StartimesProcessor;

namespace StartimesUSSDTransactionCompleter
{
    public partial class StartimesUssdTransactionProcessor : ServiceBase
    {
        Thread tranThread;
        Thread senderThread;

        public StartimesUssdTransactionProcessor()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            tranThread = new Thread(new ThreadStart(ProcessRequests));
            senderThread = new Thread(new ThreadStart(SendPayments));
            tranThread.Start();
            senderThread.Start();
        }

        protected override void OnStop()
        {
            tranThread.Abort();
            senderThread.Abort();
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void ProcessRequests()
        {
            while (true)
            {
                try
                {
                    //go to payment provider and debit customer
                    Logic bll = new Logic();
                    bll.Reset();
                    bll.ProcessTransactions();
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        public void SendPayments()
        {
            while (true)
            {
                try
                {
                    //send payments to generic
                    Logic bll = new Logic();
                    bll.ProcessPayments();
                    Thread.Sleep(new TimeSpan(0, 0, 8));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
