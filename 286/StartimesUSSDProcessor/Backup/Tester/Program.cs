using System;
using System.Collections.Generic;
using System.Text;
using StartimesProcessor;
using System.Threading;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    
                    Logic bll = new Logic();
                    //bll.Reset();
                    //bll.ProcessTransactions();
                    bll.ProcessPayments();
                }
                catch (Exception ee)
                {
                    Console.WriteLine("EXCEPTION: " + ee.Message);
                }
                Console.WriteLine("LOOPED THROUGH");
                Thread.Sleep(new TimeSpan(0, 0, 3));
            }
        }
    }
}
