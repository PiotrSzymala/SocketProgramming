using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Server
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Socket socket = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            
            ServerExecuter serverExecuter = new ServerExecuter(
                Factory.CreateDataSender(),Factory.CreateDataReceiver(), Factory.CreateUserCreator(socket), Factory.CreateUserLogger(socket),
                Factory.CreateUserUserPrivilegesChanger(socket),Factory.CreateUserRemover(socket),Factory.CreateMessageSender(socket),
                Factory.CreateMessageChecker(socket),Factory.CreateMessageBoxCleaner(socket),socket);
            
            serverExecuter.ExecuteServer();
        }
        
    }
}