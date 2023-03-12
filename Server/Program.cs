// A C# Program for Server

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Client;

namespace Server
{
    class Program
    {
        public static readonly DateTime ServerStartCount = DateTime.Now;
        
        static void Main(string[] args)
        {
            ServerExecuter.ExecuteServer();
        }
        
    }
}