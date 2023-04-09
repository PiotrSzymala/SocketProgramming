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
            
            socket.Connect(Config.LocalEndPoint);

            ITransferStructure transferStructure = new SocketSender(socket);
            ILogger logger = new Logger();
            
            
            IClientExecuter clientExecuter = new ClientExecuter(new DataSender(), new DataReceiver(transferStructure),transferStructure,logger);
            clientExecuter.ExecuteClient();
        }
    }
}