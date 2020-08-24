using KCBNotifierLibrary.ControlObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KCBNotifier
{
    public class Program
    {
        const int SLEEP_TIME = 5;
        public static void Main(string[] args)
        {
            Console.WriteLine("*********************************************************");
            Console.WriteLine("            Live KCB Vas Notifier                        ");
            Console.WriteLine("*********************************************************");

            while (true)
            {
                try
                {
                    Processor proc = new Processor();
                    proc.NotifyProcessedTransactions(true);



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
