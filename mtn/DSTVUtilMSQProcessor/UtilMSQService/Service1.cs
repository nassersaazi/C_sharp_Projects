using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using UtilMSQProcessor.ControlObjects;

namespace UtilMSQService
{
    public partial class Service1 : ServiceBase
    {
        Thread msmqThread;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            msmqThread = new Thread(new ThreadStart(DoWork));
            msmqThread.Start();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            msmqThread.Abort();
        }

        public void DoWork() 
        {
            
            while (true)
            {
                try
                {
                    //if (DbLayer.IP == DbLayer.currentIP)
                    //{
                        QueueProcessor proc = new QueueProcessor();
                        proc.ProcessDstvQueue();
                        Console.WriteLine("Finnished");
                  //  }
                }
                catch (Exception ex)
                {
                    //logging
                    //DoWork();
                    //Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
