using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;

namespace UtilReqSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Procssor p = new Procssor();
                    p.ProcessMTNYaka();
                   // p.ProcessFailedMTNYakaTransactions();
                    Console.WriteLine("DONE");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
