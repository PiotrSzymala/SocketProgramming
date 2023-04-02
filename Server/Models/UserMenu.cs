using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Shared;
using Shared.Models;

namespace Server.Models;

public class UserMenu : Menu
{
    public UserMenu(Socket socket, IDataReceiver dataReceiver, IDataSender dataSender, User user ): base(socket, dataReceiver, dataSender, user)
    {
        
    }

    public override void DisplayMenu( ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = _dataReceiver.GetData(_socket);
        Commands commands = new Commands(_dataSender, _socket);
        switch (deserializedRequestFromClient.ToLower())
        {
            case "send":
                MessageSender messageSender = new MessageSender(_dataSender, _dataReceiver, _socket);
                messageSender.SendMessage(_user);
                break;
            
            case "inbox":
                MessageChecker messageChecker = new MessageChecker(_dataSender, _dataReceiver, _socket);
                messageChecker.CheckInbox(_user);
                break;
            
            case "clear":
                MessageBoxCleaner messageBoxCleaner = new MessageBoxCleaner(_dataSender, _dataReceiver, _socket);
                messageBoxCleaner.ClearInbox(_user);
                break;
            
            case "uptime":
                commands.UptimeCommand();
                break;

            case "info":
                commands.InfoCommand();
                break;

            case "help":
                commands.HelpCommand();
                break;

            case "logout":
                _socket.Send(_dataSender.SendData("Logged out!"));
                logged = false;
                break;
            
            case "stop":
                ListSaver.SaveList();
                commands.StopCommand();
                flag = false;
                break;
            
            default:
                commands.WrongCommand();
                break;
        }
    }
}