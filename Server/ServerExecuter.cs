using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
using Server.Controllers.UserHandlingControllers;
using Server.Models;
using Shared;
using Shared.Models;

namespace Server;

public class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();

    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;

    public ServerExecuter(IDataSender dataSender, IDataReceiver dataReceiver)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
    }
    
    public  void ExecuteServer()
    {
        using Socket listener = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(Config.LocalEndPoint);
            listener.Listen(10);

            Console.WriteLine("Waiting connection ... ");

            var clientSocket = listener.Accept();

            Console.WriteLine("Connected");

            CrateDirectoryForUsers();
            CreateFileForUsers();

            string usersListString = File.ReadAllText("users/users.json");
            Users = JsonConvert.DeserializeObject<List<User>>(usersListString);

            var message = _dataSender.SendData("\nLogin or register:\n");
            clientSocket.Send(message);

            bool flag = true;
            bool logged = false;

            User currentlyLoggedUser = new User();

            while (flag)
            {
                while (!logged)
                {
                    var reply = _dataReceiver.GetData(clientSocket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            UserLogger userLogger = new UserLogger(_dataSender, _dataReceiver, clientSocket);
                            if (userLogger.LogUserIn(out currentlyLoggedUser))
                            {
                                logged = true;
                            }

                            break;

                        case "register":
                            UserCreator creator = new UserCreator(_dataSender, _dataReceiver, clientSocket);
                            creator.CreateUser();
                            break;

                        default:
                            var wrong = _dataSender.SendData("wrong command");
                            clientSocket.Send(wrong);
                            break;
                    }
                }
                

                var menu = GetMenu(currentlyLoggedUser.Privileges, clientSocket, currentlyLoggedUser);
                
                menu.DisplayMenu(ref flag, ref logged);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            ListSaver.SaveList();
        }
    }

    private Menu GetMenu(Privileges privileges, Socket socket, User user)
    =>
        privileges switch
        { 
            Privileges.Admin => new AdminMenu(socket,_dataReceiver, _dataSender,user),
            Privileges.User => new UserMenu(socket,_dataReceiver, _dataSender,user),
            _ => throw  new NotImplementedException()
        };

    private static void CreateFileForUsers()
    {
        if (!File.Exists("users/users.json"))
        {
            using var sw = new StreamWriter("users/users.json");
            sw.Write("[]");
        }
    }

    private static void CrateDirectoryForUsers()
    {
        if (!Directory.Exists("users"))
        {
            Directory.CreateDirectory("users");
        }
    }
}