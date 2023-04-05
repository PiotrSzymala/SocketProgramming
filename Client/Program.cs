using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Client
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Socket socket = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            ClientExecuter clientExecuter = new ClientExecuter(new DataSender(), new DataReceiver(),socket);
            clientExecuter.ExecuteClient();
        }
    }
}