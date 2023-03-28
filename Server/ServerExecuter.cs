using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;

namespace Server;

public static class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();

    private delegate void ChooseFunction(User currentlyLoggedUser, ref bool flag, ref bool logged);

    public static Socket ClientSocket { get; set; }

    public static void ExecuteServer()
    {
        using Socket listener = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(Config.LocalEndPoint);
            listener.Listen(10);

            Console.WriteLine("Waiting connection ... ");

            ClientSocket = listener.Accept();

            Console.WriteLine("Connected");

            CrateDirectoryForUsers();
            CreateFileForUsers();

            string usersListString = File.ReadAllText("users/users.json");
            Users = JsonConvert.DeserializeObject<List<User>>(usersListString);

            var message = DataSender.SendData("\nLogin or register:\n");
            ClientSocket.Send(message);

            bool flag = true;
            bool logged = false;

            User curentlyLoggedUser = new User();

            while (flag)
            {
                while (!logged)
                {
                    var reply = DataReceiver.GetData(ClientSocket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            if (UserLogger.LogUserIn(out curentlyLoggedUser))
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

                ChooseFunction myDel = curentlyLoggedUser.Privileges == Privileges.Admin ? MenuForAdmin : MenuForUser;

                myDel.Invoke(curentlyLoggedUser, ref flag, ref logged);
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

    private static void MenuForUser(User currentlyLoggedUser, ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
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
                ClientSocket.Send(DataSender.SendData("Logged out!"));
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

    private static void MenuForAdmin(User currentlyLoggedUser, ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
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
                ClientSocket.Send(DataSender.SendData("Logged out!"));
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