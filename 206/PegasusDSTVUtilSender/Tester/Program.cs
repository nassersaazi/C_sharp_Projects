using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;
using System.Threading;

namespace Tester
{
    class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Procssor p = new Procssor();
                p.ProcessPegasusDstv();
                Thread.Sleep(new TimeSpan(0, 0, 3));
            }
        }


        public static void ProcessDstvPayments()
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessPegasusDstv();
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
