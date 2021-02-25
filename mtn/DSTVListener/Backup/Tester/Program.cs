using System;
using System.Collections.Generic;
using System.Text;

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
                    TCPServer server = new TCPServer();
                    string request = "<ns2:getfinancialresourceinformationrequest xmlns:ns2=\"http://www.ericsson.com/e" +
                                     "m/emm/serviceprovider/backend/client\"><resource>FRI:2015897761@UDSTV.sp2/SP</res" +
                                     "ource><accountholderid>ID:256785975800/MSISDN</accountholderid><extension><optio" +
                                     "n>BQT</option></extension></ns2:getfinancialresourceinformationrequest>";
                    //string response = server.ProcessRequest(request);
                    server.ListenAndProcess();
                    //Console.WriteLine(response);
                    //Console.ReadLine();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
