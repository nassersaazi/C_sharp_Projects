using MerchantListener.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
