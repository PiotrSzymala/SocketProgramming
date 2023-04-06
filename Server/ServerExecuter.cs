using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
using Server.Interfaces;
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
    private IUserCreator _userCreator;
    private IUserLogger _userLogger;
    private IUserPrivilegesChanger _userPrivilegesChanger;
    private IUserRemover _userRemover;
    private IMessageSender _messageSender;
    private IMessageChecker _messageChecker;
    private IMessageBoxCleaner _messageBoxCleaner;
    private Socket _socket;

    public ServerExecuter(IDataSender dataSender, IDataReceiver dataReceiver, IUserCreator userCreator, IUserLogger userLogger, IUserPrivilegesChanger userPrivilegesChanger, IUserRemover userRemover, IMessageSender messageSender, IMessageChecker messageChecker, IMessageBoxCleaner messageBoxCleaner, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _userCreator = userCreator;
        _userLogger = userLogger;
        _userPrivilegesChanger = userPrivilegesChanger;
        _userRemover = userRemover;
        _messageSender = messageSender;
        _messageChecker = messageChecker;
        _messageBoxCleaner = messageBoxCleaner;
        _socket = socket;
    }
    
    public  void ExecuteServer()
    {

        try
        {
            Console.WriteLine("Waiting connection ... ");

            var clientSocket = _socket;

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
                            if (_userLogger.LogUserIn(out currentlyLoggedUser))
                            {
                                logged = true;
                            }
                            break;

                        case "register":
                            _userCreator.CreateUser();
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

    private IMenu GetMenu(Privileges privileges, Socket socket, User user)
    =>
        privileges switch
        { 
            Privileges.Admin => new AdminMenu(socket,_dataReceiver, _dataSender,user,_userPrivilegesChanger,_userRemover),
            Privileges.User => new UserMenu(socket,_dataReceiver, _dataSender,user, _messageSender, _messageChecker,_messageBoxCleaner),
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