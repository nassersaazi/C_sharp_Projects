using System;
using System.Collections.Generic;
using System.Text;

namespace DSTVListener
{
    class Program
    {
        public static void Main(string[] args)
        {
            TCPServer server = new TCPServer();
            server.ListenAndProcess();
            Console.ReadLine();
        }
    }
}
