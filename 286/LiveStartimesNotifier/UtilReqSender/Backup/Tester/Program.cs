using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UtilReqSender.ControlObjects;

namespace Tester
{
    class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessMTNYaka();
                    //p.ProcessFailedMTNYakaTransactions();

                    Console.WriteLine("DONE");
                    Thread.Sleep(new TimeSpan(0, 0, 3));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
