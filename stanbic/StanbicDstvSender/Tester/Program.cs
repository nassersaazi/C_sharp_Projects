using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApplication1.ControlObjects;
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
                    Procssor proc = new Procssor();

                    proc.ProcessStanbicDSTVTransactions();
                    //proc.ProcessStanbicSchoolsTransactions();
                    //proc.ProcessStanbicNWSC();
                    //proc.ProcessStanbicYaka();
                    //proc.ProcessStanbicUmemePostPaid();


                    Thread.Sleep(new TimeSpan(0, 0, 5));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
