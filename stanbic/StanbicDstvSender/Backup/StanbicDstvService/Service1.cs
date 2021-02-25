using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using ConsoleApplication1.ControlObjects;

namespace StanbicDstvService
{
    public partial class Service1 : ServiceBase
    {
        
        Thread dstvThread;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            dstvThread = new Thread(new ThreadStart(DoDSTVWork));
            dstvThread.Start();
        }

        protected override void OnStop()
        {
            dstvThread.Abort();
        }

        
        public void DoDSTVWork()
        {
            while (true)
            {
                try
                {
                    //if (DbLayer.IP == DbLayer.currentIP)
                    //{
                        //Thread.Sleep(new TimeSpan(0, 0, 25));
                        Procssor proc = new Procssor();
                        proc.ProcessStanbicDSTVTransactions();
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                   // }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
