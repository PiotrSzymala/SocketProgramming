using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
using Server.Interfaces;
using Server.Models;
using Shared;
using Shared.Models;

namespace Server;

public class ServerExecuter : IServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;

    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private IUserCreator _userCreator;
    private IUserLogger _userLogger;
    private IUserPrivilegesChanger _userPrivilegesChanger;
    private IUserRemover _userRemover;
    private IMessageSender _messageSender;
    private IMessageChecker _messageChecker;
    private IMessageBoxCleaner _messageBoxCleaner;
    private ITransferStructure _transferStructure;
    private ILogger _logger;
    private IDisposeStructure _disposeStructure;

    public ServerExecuter(IDataSender dataSender, IDataReceiver dataReceiver, IUserCreator userCreator, IUserLogger userLogger, IUserPrivilegesChanger userPrivilegesChanger, IUserRemover userRemover, IMessageSender messageSender, IMessageChecker messageChecker, IMessageBoxCleaner messageBoxCleaner, ITransferStructure transferStructure, ILogger logger, IDisposeStructure disposeStructure)
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
        _transferStructure = transferStructure;
        _logger = logger;
        _disposeStructure = disposeStructure;
    }
    
    public  void ExecuteServer()
    {

        try
        {
            Console.WriteLine("Waiting connection ... ");

            var transferStructure = _transferStructure;

            Console.WriteLine("Connected");

            CrateDirectoryForUsers();
            CreateFileForUsers();

            string usersListString = File.ReadAllText("users/users.json");
            ListSaver.Users = JsonConvert.DeserializeObject<List<User>>(usersListString);

            var message = _dataSender.SendData("\nLogin or register:\n");
            transferStructure.Send(message);

            bool flag = true;
            bool logged = false;

            User currentlyLoggedUser = new User();

            while (flag)
            {
                while (!logged)
                {
                    var reply = _dataReceiver.GetData();

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
                            transferStructure.Send(wrong);
                            break;
                    }
                }
                

                var menu = GetMenu(currentlyLoggedUser.Privileges, transferStructure, currentlyLoggedUser);
                
                menu.DisplayMenu(ref flag, ref logged);
            }
        }

        catch (Exception exception)
        {
            _logger.WriteError(exception);
            Console.WriteLine($"Exception: {exception}");
            ListSaver.SaveList();
        }
    }

    private IMenu GetMenu(Privileges privileges, ITransferStructure iTransferStructure, User user)
    =>
        privileges switch
        { 
            Privileges.Admin => new AdminMenu(iTransferStructure,_dataReceiver, _dataSender,user,_userPrivilegesChanger,_userRemover,_disposeStructure),
            Privileges.User => new UserMenu(iTransferStructure,_dataReceiver, _dataSender,user, _messageSender, _messageChecker,_messageBoxCleaner,_disposeStructure),
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