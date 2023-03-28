using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Controllers;
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
            SaveList();
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
                SendMessage(currentlyLoggedUser);
                break;
            
            case "inbox":
                CheckInbox(currentlyLoggedUser);
                break;
            
            case "clear":
                ClearInbox(currentlyLoggedUser);
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
                SaveList();
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
                SendMessage(currentlyLoggedUser);
                break;
            
            case "inbox":
                CheckInbox(currentlyLoggedUser);
                break;
            
            case "clear":
                ClearInbox(currentlyLoggedUser);
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
                Commands.HelpCommand();
                break;

            case "logout":
                ClientSocket.Send(DataSender.SendData("Logged out!"));
                logged = false;
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

    private static void SendMessage(User sender)
    {
        var message = DataSender.SendData("To whom do you want to send the message?");
        ClientSocket.Send(message);

        var username = DataReceiver.GetData(ClientSocket);

        var userToSendMessage = Users.FirstOrDefault(u => u.Username.Equals(username));

        if (Users.Contains(userToSendMessage))
        {
            if (userToSendMessage.Inbox.Count < 5)
            {
                message = DataSender.SendData("Write your message:");
                ClientSocket.Send(message);

                var messageContent = DataReceiver.GetData(ClientSocket);

                userToSendMessage.Inbox.Add(new MessageToUser()
                {
                    MessageAuthor = sender.Username,
                    MessageContent = messageContent
                });
                
                using (StreamWriter file = File.CreateText($"users/{userToSendMessage.Username}.json"))
                {
                    var result = JsonConvert.SerializeObject(userToSendMessage);
                    file.Write(result);
                }

                message = DataSender.SendData("The message has been sent.");
                ClientSocket.Send(message);
            }
            else
            {
                message = DataSender.SendData($"The inbox of {userToSendMessage.Username} is full.");
                ClientSocket.Send(message);
            }
        }
        else
        {
            message = DataSender.SendData("User does not exist.");
            ClientSocket.Send(message);
        }
    }

    private static void CheckInbox(User currentlyLoggedUser)
    {
        if (currentlyLoggedUser.Inbox.Count == 0)
        {
            var message = DataSender.SendData("Inbox is empty");
            ClientSocket.Send(message);
        }
        MessagesDisplayer(currentlyLoggedUser.Inbox);
    }

    private static void MessagesDisplayer(List<MessageToUser> messages)
    {
        int counter = 1;

        string test = string.Empty;
        foreach (var message in messages)
        {
            test += $"\n\nMessage {counter}/{messages.Count}, From: {message.MessageAuthor}" +
                    $"\nContent: {message.MessageContent}\n";
            counter++;
        }
        var messageInfo = DataSender.SendData(test);
        ClientSocket.Send(messageInfo);
    }

    private static void ClearInbox(User currentlyLoggedUser)
    {
        var message = DataSender.SendData("All messages will be deleted. Are you sure? (y/n)");
        ClientSocket.Send(message);

        var decision = DataReceiver.GetData(ClientSocket);

        if (decision.ToLower() == "y")
        {
            currentlyLoggedUser.Inbox.Clear();
            
            using (StreamWriter file = File.CreateText($"users/{currentlyLoggedUser.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(currentlyLoggedUser);
                file.Write(result);
            }
            SaveList();
            message = DataSender.SendData("All messages deleted.");
            ClientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("Deleting canceled.");
            ClientSocket.Send(message);
        }
    }
}