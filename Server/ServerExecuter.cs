using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace Server;

public static class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();

    public static void ExecuteServer()
    {
        using Socket listener = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(Config.LocalEndPoint);
            listener.Listen(10);

            Console.WriteLine("Waiting connection ... ");

            using Socket clientSocket = listener.Accept();

            Console.WriteLine("Connected");
                
            var message = DataSender.SendData("\nEnter command:\n");
            clientSocket.Send(message);

            bool flag = true;
            bool logged = false;

            while (flag)
            {
                while (!logged)
                {
                    var reply = DataReceiver.GetData(clientSocket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            logged = true;
                            var toSend =  DataSender.SendData("Logged!");
                            clientSocket.Send(toSend);
                            break;

                        case "register":
                            UserCreator.CreateUser(clientSocket);
                            break;
                        
                        default:
                            var wrong = DataSender.SendData("wrong command");
                            clientSocket.Send(wrong);
                            break;
                    }
                }
                
                var deserializedRequestFromClient = DataReceiver.GetData(clientSocket);
                ChooseOption(deserializedRequestFromClient, clientSocket, ref flag);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }

    private static void ChooseOption(string deserializedRequestFromClient, Socket clientSocket, ref bool flag)
    {
        byte[] toSend;
        Commands.ClientSocket = clientSocket;
        switch (deserializedRequestFromClient.ToLower())
        {
            case "uptime":
                Commands.UptimeCommand();
                break;

            case "info":
                 Commands.InfoCommand();
                 break;

            case "help":
                 Commands.HelpCommand();
                 break;

            case "stop":
                Commands.StopCommand();
                flag = false;
                break;

            default:
                 Commands.WrongCommand();
                 break;
        }
    }
}