using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace Server;

public static class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();
    public static Socket ClientSocket { get; set; }

    public static void ExecuteServer()
    {
        using Socket listener = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        try
        {
            if (!Directory.Exists("users"))
            {
                Directory.CreateDirectory("users");
            }
            if (!File.Exists("users/users.json"))
            {
                using var sw = new StreamWriter("users/users.json");
                sw.Write("[]");
            }
            string usd = File.ReadAllText("users/users.json");
            Users = JsonConvert.DeserializeObject<List<User>>(usd);

            listener.Bind(Config.LocalEndPoint);
            listener.Listen(10);

            Console.WriteLine("Waiting connection ... ");

            ClientSocket = listener.Accept();

            Console.WriteLine("Connected");
                
            var message = DataSender.SendData("\nEnter command:\n");
            ClientSocket.Send(message);

            bool flag = true;
            bool logged = false;

            while (flag)
            {
                while (!logged)
                {
                    var reply = DataReceiver.GetData(ClientSocket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            if (UserLogger.LogUserIn())
                            {
                                logged = true;
                            }
                            break;

                        case "register":
                            UserCreator.CreateUser(ClientSocket);
                            break;
                        
                        default:
                            var wrong = DataSender.SendData("wrong command");
                            ClientSocket.Send(wrong);
                            break;
                    }
                }
                
                var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
                ChooseOption(deserializedRequestFromClient, ref flag);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
        }
    }

    private static void ChooseOption(string deserializedRequestFromClient, ref bool flag)
    {
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
                SaveList();
                Commands.StopCommand();
                flag = false;
                break;

            default:
                 Commands.WrongCommand();
                 break;
        }
    }

    private static void SaveList()
    {
        using StreamWriter listWriter = File.CreateText("users/users.json");
        var result = JsonConvert.SerializeObject(Users);
        listWriter.Write(result);
    }
}