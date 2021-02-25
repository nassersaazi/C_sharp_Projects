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
    public partial class StartimesNotifier : ServiceBase
    {
        Thread mtnThread;
        //Thread yakaFailed;
        public StartimesNotifier()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            mtnThread = new Thread(new ThreadStart(DoWork));
            ///yakaFailed = new Thread(new ThreadStart(DoYakaFailedWork));
            mtnThread.Start();
            ///yakaFailed.Start();
        }

        protected override void OnStop()
        {
            mtnThread.Abort();
            ///yakaFailed.Abort();
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void DoWork() 
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessMTNYaka();
                    Console.WriteLine("DONE");
                    Thread.Sleep(new TimeSpan(0, 0, 5));
                }
                catch (Exception ex)
                {
                    //string filePath = @"E:\MTNUmemeQueueErrors\DBInserterLogs\"+DateTime.Now.ToString("ddMMyyyy");
                    //File.WriteAllText("", ex.Message);
                }
            }
        }

        //public void DoYakaFailedWork()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            //Procssor p = new Procssor();
        //            //p.ProcessFailedMTNYakaTransactions();
        //            //Console.WriteLine("DONE");
        //            //Thread.Sleep(new TimeSpan(0, 1, 0));
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //        }
        //    }
        //}
    }
}
