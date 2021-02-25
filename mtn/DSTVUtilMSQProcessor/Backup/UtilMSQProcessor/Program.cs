using System;
using System.Collections.Generic;
using System.Text;
using UtilMSQProcessor.ControlObjects;

namespace UtilMSQProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QueueProcessor proc = new QueueProcessor();
            while (true)
            {
                try
                {
                    proc.ProcessDstvQueue();
                    Console.WriteLine("Finnished");
                    //DateTime d = DateTime.Parse("25/09/2014");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
