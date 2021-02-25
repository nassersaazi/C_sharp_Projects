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
                    //string request = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ns0:getfinancialresourceinformationrequest xmlns:ns0=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\"><resource>FRI:1004000801294@UDSTV.sp2/SP</resource><accountholderid>ID:256773826678/MSISDN</accountholderid><extension><option>PREW4</option></extension></ns0:getfinancialresourceinformationrequest>";
                    //string response = server.ProcessRequest(request);
                    server.ListenAndProcess();
                    //Console.WriteLine(response);
                    //Console.ReadLine();
                }
                catch (Exception  ex)
                {

                }
            }
        }
    }
}
