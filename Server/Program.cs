using System.Net.Sockets;
using Server.Interfaces;
using Shared;
using Shared.Models;

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

            ITransferStructure transferStructure = new SocketSender(socket);
            IDisposeStructure disposeStructure = new SocketSender(socket);

            var dataSender = Factory.CreateDataSender();
            var dataReceiver = Factory.CreateDataReceiver(transferStructure);

            var userCreator = Factory.CreateUserCreator(transferStructure);
            var userLogger = Factory.CreateUserLogger(transferStructure);
            var userPrivilegesChanger = Factory.CreateUserUserPrivilegesChanger(transferStructure);
            var userRemover = Factory.CreateUserRemover(transferStructure);

            var messageSender = Factory.CreateMessageSender(transferStructure);
            var messageChecker = Factory.CreateMessageChecker(transferStructure);
            var messageBoxCleaner = Factory.CreateMessageBoxCleaner(transferStructure);
            
           

            var logger = Factory.CreateLogger();

            IServerExecuter serverExecuter = new ServerExecuter(dataSender, dataReceiver, userCreator, userLogger,
                userPrivilegesChanger, userRemover, messageSender, messageChecker, messageBoxCleaner, transferStructure,logger, disposeStructure );
            
            serverExecuter.ExecuteServer();
        }
        
    }
}