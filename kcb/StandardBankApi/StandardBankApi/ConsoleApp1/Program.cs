using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyInterfaces.BillerEngineWebService;

namespace ConsoleApp1
{
    public class Program
    { 
        public static void Main(string[] args)
        {

            string UserName = "pegususUser", Password = "SwipeUser@AFSFASF16163", ServiceId = "28", ServiceName = "TRANSACTIONCALLBACK", TransactionReference = "TT191949YGL2", TransactionStatus = "1", TransactionStatusDescription = "success", ReversalStaus = "REVERSED", SessionId = "23qaqeq28743";
            customMap Payload = null;
            try
            {
                BillerEngineWebService bl = new BillerEngineWebService();
                billerTransactionResponse res;
                res = bl.KCBBillerTransactionCallBack(UserName, Password, ServiceId, ServiceName, TransactionReference, TransactionStatus, TransactionStatusDescription, ReversalStaus, SessionId, Payload);

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            } 

        }

        
    }
}
