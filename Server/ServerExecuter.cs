using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace Server;

public static class ServerExecuter
{
    public static readonly DateTime ServerCreationTime = DateTime.Now;
    public static List<User> Users = new List<User>();

    public delegate void ChooseFunction(ref bool flag, ref bool logged); 
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

            User log = new User();

            while (flag)
            {
                while (!logged)
                {
                    var reply = DataReceiver.GetData(ClientSocket);

                    switch (reply.ToLower())
                    {
                        case "login":
                            if (UserLogger.LogUserIn(out log))
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
                
                ChooseFunction myDel = log.Privileges == Privileges.Admin ? MenuForAdmin : MenuForUser;
                
                myDel.Invoke(ref flag,ref logged);
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine($"Exception: {exception}");
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

    private static void MenuForUser( ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
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
            
            case "logout":
                ClientSocket.Send(DataSender.SendData("Logged out!"));
                logged = false;
                break;

            default:
                 Commands.WrongCommand();
                 break;
        }
    }

    private static void BasicCommandsMenu(ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
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
            
            case "logout":
                ClientSocket.Send(DataSender.SendData("Logged out!"));
                logged = false;
                break;

            default:
                Commands.WrongCommand();
                break;
        }
    }
    private static void MenuForAdmin(ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = DataReceiver.GetData(ClientSocket);
        switch (deserializedRequestFromClient.ToLower())
        {
            case "delete":
                RemoveUser();
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

            case "stop":
                SaveList();
                Commands.StopCommand();
                flag = false;
                break;
            
            case "logout":
                ClientSocket.Send(DataSender.SendData("Logged out!"));
                logged = false;
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

    private static void RemoveUser()
    {
        var message = DataSender.SendData("Which user you want to delete?");
        ClientSocket.Send(message);

        var nickname = DataReceiver.GetData(ClientSocket);

        var userToDelete = Users.FirstOrDefault(u => u.Username.Equals(nickname));

        if (Users.Contains(userToDelete))
        {
            File.Delete($"users/{userToDelete.Username}.json");
            Users.Remove(userToDelete);
            
            message = DataSender.SendData("User has been deleted.");
            ClientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("User does not exist.");
            ClientSocket.Send(message);
        }
    }
}