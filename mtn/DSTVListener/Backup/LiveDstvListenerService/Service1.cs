using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using DSTVListener.ControlObjects;

namespace LiveDstvListenerService
{
    public partial class Service1 : ServiceBase
    {
        Thread dstvListener;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            dstvListener = new Thread(new ThreadStart(DoWork));
            dstvListener.Start();
            // TODO: Add code here to start your service.
        }

        protected override void OnStop()
        {
            dstvListener.Abort();
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void DoWork()
        {
            while (true)
            {
                try
                {
                    if (DbLayer.IP == DbLayer.currentIP)
                    {
                        TCPServer server = new TCPServer();
                        server.ListenAndProcess();
                        // Console.ReadLine();
                    }
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
