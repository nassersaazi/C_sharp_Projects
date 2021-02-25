using System;
using System.Collections.Generic;
using System.Text;
using UtilReqSender.ControlObjects;
using UtilReqSender.EntityObjects;

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
                    p.ProcessMtnDstv();
                    //p.ProcessFailedMTNYakaTransactions();
                    Transaction tr = new Transaction();
                    
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
