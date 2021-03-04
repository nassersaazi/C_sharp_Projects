using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using UtilReqSender.ControlObjects;
using System.IO;

namespace MTNYakaSenderService
{
    public partial class TestMTNYakaSenderService : ServiceBase
    {
        Thread mtnThread;
        Thread reactivationRequeststhread;
        DatabaseHandler dh = new DatabaseHandler();

        public TestMTNYakaSenderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            dh.SetPendingTransactionStatusToInserted();
            mtnThread = new Thread(new ThreadStart(ProcessDstvPayments));
            reactivationRequeststhread = new Thread(new ThreadStart(ProcessReactivationRequests));
            mtnThread.Start();
            reactivationRequeststhread.Start();
        }

        protected override void OnStop()
        {
            dh.SetPendingTransactionStatusToInserted();
            mtnThread.Abort();
            reactivationRequeststhread.Abort();
        }

        public void ProcessDstvPayments()
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessPegasusDstv();
                    Thread.Sleep(new TimeSpan(0, 0, 1));
                }

                catch (Exception ex)
                {
                }
            }
        }

        public void ProcessReactivationRequests()
        {
            while (true)
            {
                //try
                //{
                //    if (DbLayer.IP == DbLayer.currentIP)
                //    {
                Procssor p = new Procssor();
                p.ProcessReactivateRequest();
                Thread.Sleep(new TimeSpan(0, 1, 0));
                //    }
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }
    }
}
