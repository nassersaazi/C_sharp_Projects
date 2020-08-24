using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pegbankTranProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("===========PEGBANK SERVICE============");
            Console.WriteLine("======================================");

            Thread w1, w2;
            w1 = new Thread(new ThreadStart(start1));
            w2 = new Thread(new ThreadStart(start2));
            w1.Start();
            w2.Start();
            
        }

        public static void start1()
        {
            while (true)
            {
                Console.WriteLine("==========Processing Deposits==============");
                var proc = new QueueProcessor();
                try
                {
                    proc.DepositsFromQueue();
                }
                catch (Exception ert)
                {
                    throw ert;
                }
            }
        }

        public static void start2()
        {
            while (true)
            {
                Console.WriteLine("==========Processing Withdraws==============");
                var proc = new QueueProcessor();
                try
                {
                    proc.WithdrawsFromQueue();
                }
                catch (Exception ert)
                {
                    throw ert;
                }
            }
        }
    }
}
