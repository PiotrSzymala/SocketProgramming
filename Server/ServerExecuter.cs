using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Server;

public static class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();
    public static MyMessage MessageToClient = new MyMessage();

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
                byte[] receivedBytes = new byte[1024];
                string dataFromClient = string.Empty;

                int bytesToEncode = clientSocket.Receive(receivedBytes);

                dataFromClient += Encoding.ASCII.GetString(receivedBytes,
                    0, bytesToEncode);

                var deserializedClientRequest = JsonConvert.DeserializeObject<MyMessage>(dataFromClient);

                ChooseOption(deserializedClientRequest, clientSocket, ref flag);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }

    private static void ChooseOption(MyMessage deserializedRequestFromClient, Socket clientSocket, ref bool flag)
    {
        byte[] toSend;
        switch (deserializedRequestFromClient.Message.ToLower())
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
                 CreateUser(clientSocket);
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

    private static void CreateUser(Socket clientSocket)
    {
         var message = SendToClient("Set username");

        clientSocket.Send(message);
        
        var username = GetDataFromClient(clientSocket);

        if (!File.Exists($"{username}.json"))
        {
            message = SendToClient("Set password");
            clientSocket.Send(message);
            
            var password = GetDataFromClient(clientSocket);

            message = SendToClient("If you want admin account type in a special password:");
            clientSocket.Send(message);
            
            var specialPassword = GetDataFromClient(clientSocket);

            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);

            var user = new User(username, password, privileges);
            Users.Add(user);

            using (StreamWriter file = File.CreateText($"{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            
            message = SendToClient("User created!");
            clientSocket.Send(message);
        }
        else
        {
            message = SendToClient("User already exists!");
            clientSocket.Send(message);
        }
    }

    private static string GetDataFromClient(Socket clientSocket)
    {
        var bytesU = new byte[1024];
        var numByte = clientSocket.Receive(bytesU);

        var received = Encoding.ASCII.GetString(bytesU, 0, numByte);
        MessageToClient = JsonConvert.DeserializeObject<MyMessage>(received);

        var result = MessageToClient.Message;
        return result;
    }

    private static byte[] SendToClient(string messageContext)
    {
        MessageToClient.Message = messageContext;
        var toSend = JsonConvert.SerializeObject(MessageToClient);
        var message = Encoding.ASCII.GetBytes(toSend);
        return message;
    }
}