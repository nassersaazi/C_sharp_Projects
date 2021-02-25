using System;
using System.Collections.Generic;
using System.Text;
using UtilMSQProcessor.ControlObjects;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            QueueProcessor proc = new QueueProcessor();
            while (true)
            {
                try
                {
                    //string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"+
                    //             "<tns:errorResponse  xmlns:tns=\"http://www.ericsson.com/lwac\" errorcode=\"AUTHORIZATION_FAILED\"><arguments name=\"SPID\" value=\"2560110000728\"/></tns:errorResponse >";
                    //proc.GetMTNResponse(xml);
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
