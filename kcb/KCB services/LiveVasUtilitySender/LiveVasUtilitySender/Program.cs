using LiveVasUtilitySenderLibrary.ControlObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveVasUtilitySender
{
    public class Program
    {
        const int SLEEP_TIME = 3;
        public static void Main(string[] args)
        {
            Console.WriteLine("*********************************************************");
            Console.WriteLine("          Live Vas Utility Sender");
            Console.WriteLine("*********************************************************");

            while (true)
            {
                try
                {
                    Processor proc = new Processor();
                    proc.ProcessPendingTransactions();



                    Thread.Sleep(new TimeSpan(0, 0, SLEEP_TIME));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
