using MerchantListenerLibrary.Logic;

namespace MerchantListener
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer sv = new TCPServer();
            sv.ListenAndProcess();
        }
    }
}
