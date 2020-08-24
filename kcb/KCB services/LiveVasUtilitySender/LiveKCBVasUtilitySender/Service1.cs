using LiveVasUtilitySenderLibrary.ControlObjects;
using log4net;
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

namespace LiveKCBVasUtilitySender
{
    public partial class Service1 : ServiceBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(Service1));
        Thread processTrans;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            processTrans = new Thread(new ThreadStart(ProcessTransactions));
            processTrans.Start();
        }

        public void ProcessTransactions()
        {

            while (true)
            {

                try
                {
                    Processor processor = new Processor();
                    processor.ProcessPendingTransactions();

                }
                catch (Exception ex)
                {


                }

            }
        }

        protected override void OnStop()
        {
            processTrans.Abort();
        }

        
    }
}
