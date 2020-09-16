using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MerchantListenerService
{
    public partial class Service1 : ServiceBase
    {
        Thread worker, worker2, worker3,worker4,worker5;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            worker = new Thread(new ThreadStart(DoWork));
            //worker2 = new Thread(new ThreadStart(DoWork2));
            //worker3 = new Thread(new ThreadStart(DoWor3));
            //worker4 = new Thread(new ThreadStart(DoWork4));
            //worker5 = new Thread(new ThreadStart(DoWork5));
            worker.Start();
            worker2.Start();
            worker3.Start();
            worker3.Start();
            worker4.Start();
            worker5.Start();
        }

        protected override void OnStop()
        {
            worker.Abort();
            worker2.Abort();
            worker3.Abort();
            worker4.Abort();
            worker5.Abort();

        }

        public void DoWork()
        {
            while (true)
            {
                //try
                //{
                //    TCPServer sv = new TCPServer();
                //    sv.ListenAndProcess();
                //    //Thread.Sleep(new TimeSpan(0, 0,5));
                //}
                //catch (Exception ex)
                //{
                //    File.WriteAllText(@"E:\error.txt", ex.Message + "on :" + ex.StackTrace + " on " + DateTime.Now.ToString());
                //}

            }
        }
    }
}




