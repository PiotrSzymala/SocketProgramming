using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Server
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            ServerExecuter serverExecuter = new ServerExecuter(new DataSender(), new DataReceiver());
            serverExecuter.ExecuteServer();
        }
        
    }
}