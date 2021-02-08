using MerchantListenerLibrary.Logic;
using System.Threading;

namespace MerchantListener
{
    public class Program
    {
        

        static void Main(string[] args)
        {
            TCPServer sv = new TCPServer();
            sv.ListenAndProcess();
            
        }
    }
}
