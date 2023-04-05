using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Server.Interfaces;
using Shared;
using Shared.Models;

namespace Server.Models;

public class UserMenu : IMenu
{

    private Socket _socket;
    private IDataReceiver _dataReceiver;
    private IDataSender _dataSender;
    private User _user;
    private IMessageSender _messageSender;
    private IMessageChecker _messageChecker;
    private IMessageBoxCleaner _messageBoxCleaner;
    public UserMenu(Socket socket, IDataReceiver dataReceiver, IDataSender dataSender, User user, IMessageSender messageSender, IMessageChecker messageChecker, IMessageBoxCleaner messageBoxCleaner)
    {
        _socket = socket;
        _dataReceiver = dataReceiver;
        _dataSender = dataSender;
        _user = user;
        _messageSender = messageSender;
        _messageChecker = messageChecker;
        _messageBoxCleaner = messageBoxCleaner;
    }

    public  void DisplayMenu( ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = _dataReceiver.GetData(_socket);
        Commands commands = new Commands(_dataSender, _socket);
        switch (deserializedRequestFromClient.ToLower())
        {
            case "send":
                _messageSender.SendMessage(_user);
                break;
            
            case "inbox":
                _messageChecker.CheckInbox(_user);
                break;
            
            case "clear":
                _messageBoxCleaner.ClearInbox(_user);
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