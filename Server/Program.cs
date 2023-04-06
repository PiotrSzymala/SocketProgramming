using System.Net.Sockets;
using Shared;

namespace Server
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Socket socket = Factory.CreateSocket();
            
            socket.Bind(Config.LocalEndPoint);
            socket.Listen(10);
            
            socket = socket.Accept();

            var dataSender = Factory.CreateDataSender();
            var dataReceiver = Factory.CreateDataReceiver();

            var userCreator = Factory.CreateUserCreator(socket);
            var userLogger = Factory.CreateUserLogger(socket);
            var userPrivilegesChanger = Factory.CreateUserUserPrivilegesChanger(socket);
            var userRemover = Factory.CreateUserRemover(socket);

            var messageSender = Factory.CreateMessageSender(socket);
            var messageChecker = Factory.CreateMessageChecker(socket);
            var messageBoxCleaner = Factory.CreateMessageBoxCleaner(socket);

            ServerExecuter serverExecuter = new ServerExecuter(dataSender, dataReceiver, userCreator, userLogger,
                userPrivilegesChanger, userRemover, messageSender, messageChecker, messageBoxCleaner, socket);
            
            serverExecuter.ExecuteServer();
        }
        
    }
}