using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using MsaveLibrary;

namespace LiveXtraFloatTransactionProcessor
{
    public partial class Service1 : ServiceBase
    {
        Thread penaltyThread, penaltyThread2, penaltyThread3, penaltyThread4, penaltyThread5, notitifications, reverseTransactions;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            penaltyThread = new Thread(new ThreadStart(ApplyPenaltiesOnDefaulters));
            penaltyThread.Start();

            penaltyThread2 = new Thread(new ThreadStart(ApplyPenaltiesOnDefaulters2));
            penaltyThread2.Start();

            //penaltyThread3 = new Thread(new ThreadStart(ApplyPenaltiesOnDefaulters3));
            //penaltyThread3.Start();

            //penaltyThread4 = new Thread(new ThreadStart(ApplyPenaltiesOnDefaulters4));
            //penaltyThread4.Start();

            //penaltyThread5 = new Thread(new ThreadStart(ApplyPenaltiesOnDefaulters5));
            //penaltyThread5.Start();

            notitifications = new Thread(new ThreadStart(NotificationSender));
            notitifications.Start();

            reverseTransactions = new Thread(new ThreadStart(CompleteRreverseTransactions));
            reverseTransactions.Start();

        }

        protected override void OnStop()
        {
            //agentReg.Abort();

            //loanRepay.Abort();

            //loanApplication.Abort();

            penaltyThread.Abort();

            penaltyThread2.Abort();
            //penaltyThread3.Abort();
            //penaltyThread4.Abort();
            //penaltyThread5.Abort();

            notitifications.Abort();

            reverseTransactions.Abort();
        }


        //-------------------------------------------------------------------------------------
        private void NotificationSender()
        {
            RequestProcessor proc = new RequestProcessor();
            while (true)
            {
                // proc.("MTN");
                Thread.Sleep(new TimeSpan(0, 0, 5));
            }
        }



        private void CompleteRreverseTransactions()
        {
            RequestProcessor proc = new RequestProcessor();
            while (true)
            {
                proc.ReverseTransactions();
                Thread.Sleep(new TimeSpan(0, 0, 3));
            }
        }
        private void ApplyPenaltiesOnDefaulters()
        {
            RequestProcessor proc = new RequestProcessor();
            while (true)
            {

                proc.HandlePenaltiesAndRecovery(new string[] { "2", "10", "1" });
                Thread.Sleep(new TimeSpan(0, 30, 0));
            }
        }


        private void ApplyPenaltiesOnDefaulters2()
        {
            RequestProcessor proc = new RequestProcessor();
            while (true)
            {
                proc.HandlePenaltiesAndRecovery(new string[] { "11", "30", "2" });
                Thread.Sleep(new TimeSpan(1, 0, 0));
            }
        }

        //private void ApplyPenaltiesOnDefaulters3()
        //{
        //    RequestProcessor proc = new RequestProcessor();
        //    while (true)
        //    {
        //        // proc.ProcessDefaulterPenalties();
        //        proc.HandlePenaltiesAndRecovery(new string[] { "31", "60", "3" });
        //        Thread.Sleep(new TimeSpan(1, 30, 0));
        //    }
        //}

        //private void ApplyPenaltiesOnDefaulters4()
        //{
        //    RequestProcessor proc = new RequestProcessor();
        //    while (true)
        //    {
        //        // proc.ProcessDefaulterPenalties();
        //        proc.HandlePenaltiesAndRecovery(new string[] { "61", "120", "4" });
        //        Thread.Sleep(new TimeSpan(1, 30, 0));
        //    }
        //}

        //private void ApplyPenaltiesOnDefaulters5()
        //{
        //    RequestProcessor proc = new RequestProcessor();
        //    while (true)
        //    {
        //        // proc.ProcessDefaulterPenalties();
        //        proc.HandlePenaltiesAndRecovery(new string[] { "121", (365 * 5).ToString(), "5" });
        //        Thread.Sleep(new TimeSpan(1, 30, 0));
        //    }
        //}


    }
}
