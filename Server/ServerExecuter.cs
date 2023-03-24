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

            var message = Encoding.ASCII.GetBytes("\nEnter command: \n\n");

            clientSocket.Send(message);

            bool flag = true;


            while (flag)
            {
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
        switch (deserializedRequestFromClient.ToLower())
        {
            case "uptime":
                toSend = Commands.UptimeCommand();
                clientSocket.Send(toSend);
                break;

            case "info":
                toSend = Commands.InfoCommand();
                clientSocket.Send(toSend);
                break;

            case "help":
                toSend = Commands.HelpCommand();
                clientSocket.Send(toSend);
                break;
            
            case "login":
                 UserCreator.CreateUser(clientSocket);
                break;

            case "stop":
                toSend = Commands.StopCommand();
                clientSocket.Send(toSend);

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();

                flag = false;

                break;

            default:
                toSend = Commands.WrongCommand();
                clientSocket.Send(toSend);
                break;
        }
    }
}