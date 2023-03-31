using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;

namespace Server;

public class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();

    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private delegate void ChooseFunction(User currentlyLoggedUser, ref bool flag, ref bool logged);

    private Socket _socket;

    public ServerExecuter(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
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

            _socket = listener.Accept();

            Console.WriteLine("Connected");

            CrateDirectoryForUsers();
            CreateFileForUsers();

            string usersListString = File.ReadAllText("users/users.json");
            Users = JsonConvert.DeserializeObject<List<User>>(usersListString);

            var message = _dataSender.SendData("\nLogin or register:\n");
            _socket.Send(message);

            bool flag = true;
            bool logged = false;

            User currentlyLoggedUser = new User();

            while (flag)
            {
                while (!logged)
                {
                    var reply = _dataReceiver.GetData(_socket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            if (UserLogger.LogUserIn(out currentlyLoggedUser))
                            {
                                logged = true;
                            }

                            break;

                        case "register":
                            UserCreator creator = new UserCreator(new DataSender(), new DataReceiver(), _socket);
                            creator.CreateUser();
                            break;

                        default:
                            var wrong = _dataSender.SendData("wrong command");
                            _socket.Send(wrong);
                            break;
                    }
                }

                ChooseFunction myDel = currentlyLoggedUser.Privileges == Privileges.Admin ? MenuForAdmin : MenuForUser;

                myDel.Invoke(currentlyLoggedUser, ref flag, ref logged);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
            ListSaver.SaveList();
        }
    }

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

    private  void MenuForUser(User currentlyLoggedUser, ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = _dataReceiver.GetData(_socket);
        switch (deserializedRequestFromClient.ToLower())
        {
            case "send":
                MessageSender.SendMessage(currentlyLoggedUser);
                break;
            
            case "inbox":
                MessageChecker.CheckInbox(currentlyLoggedUser);
                break;
            
            case "clear":
                MessageBoxCleaner.ClearInbox(currentlyLoggedUser);
                break;
            
            case "uptime":
                Commands.UptimeCommand();
                break;

            case "info":
                Commands.InfoCommand();
                break;

            case "help":
                Commands.HelpCommand();
                break;

            case "logout":
                _socket.Send(_dataSender.SendData("Logged out!"));
                logged = false;
                break;
            
            case "stop":
                ListSaver.SaveList();
                Commands.StopCommand();
                flag = false;
                break;
            
            default:
                Commands.WrongCommand();
                break;
        }
    }

    private  void MenuForAdmin(User currentlyLoggedUser, ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = _dataReceiver.GetData(_socket);
        switch (deserializedRequestFromClient.ToLower())
        {
            case "send":
                MessageSender.SendMessage(currentlyLoggedUser);
                break;
            
            case "inbox":
                MessageChecker.CheckInbox(currentlyLoggedUser);
                break;
            
            case "clear":
                MessageBoxCleaner.ClearInbox(currentlyLoggedUser);
                break;
            
            case "change":
                UserPrivilegesChanger.ChangePrivileges();
                break;

            case "delete":
                UserRemover.RemoveUser();
                break;

            case "uptime":
                Commands.UptimeCommand();
                break;

            case "info":
                Commands.InfoCommand();
                break;

            case "help":
                Commands.HelpCommandForAdmin();
                break;

            case "logout":
                _socket.Send(_dataSender.SendData("Logged out!"));
                logged = false;
                break;
            
            case "stop":
                ListSaver.SaveList();
                Commands.StopCommand();
                flag = false;
                break;

            default:
                Commands.WrongCommand();
                break;
        }
    }
}