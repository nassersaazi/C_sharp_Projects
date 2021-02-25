using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ConsoleApplication1.ControlObjects;

namespace UtilReqSenderForOthers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Procssor proc = new Procssor();
                    proc.ProcessStanbicYaka();
                    //proc.ProcessUmemePostPaid();
                    //proc.ProcessNWSC();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
