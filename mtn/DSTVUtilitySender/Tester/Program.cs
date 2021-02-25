using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;
using System.Threading;

namespace Tester
{
    class Program
    {
        //static Thread UpdateCustBalThread;
        public static void Main(string[] args)
        {
        //    Thread mtnThread = new Thread(new ThreadStart(ProcessDstvPayments));
        //    Thread reactivationRequeststhread = new Thread(new ThreadStart(ProcessReactivationRequests));
        //    mtnThread.Start();
        //    reactivationRequeststhread.Start();

        //    Console.WriteLine("DONE");
            while (true)
            {
                Procssor p = new Procssor();
                p.ProcessMtnDstv();
                Thread.Sleep(new TimeSpan(0, 0, 3));
            }
            //UpdateCustBalThread.Abort();
        }


        public static void ProcessDstvPayments()
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessMtnDstv();
                    Thread.Sleep(new TimeSpan(0, 0, 3));
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static void ProcessReactivationRequests()
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessReactivateRequest();
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
