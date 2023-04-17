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

    private ITransferStructure _transferStructure;
    private IDataReceiver _dataReceiver;
    private IDataSender _dataSender;
    private User _user;
    private IMessageSender _messageSender;
    private IMessageChecker _messageChecker;
    private IMessageBoxCleaner _messageBoxCleaner;
    private IDisposeStructure _disposeStructure;
    
    public UserMenu(ITransferStructure transferStructure, IDataReceiver dataReceiver, IDataSender dataSender, User user, IMessageSender messageSender, IMessageChecker messageChecker, IMessageBoxCleaner messageBoxCleaner, IDisposeStructure disposeStructure)
    {
        _transferStructure = transferStructure;
        _dataReceiver = dataReceiver;
        _dataSender = dataSender;
        _user = user;
        _messageSender = messageSender;
        _messageChecker = messageChecker;
        _messageBoxCleaner = messageBoxCleaner;
        _disposeStructure = disposeStructure;
    }

    public  void DisplayMenu( ref bool flag, ref bool logged)
    {
        var deserializedRequestFromClient = _dataReceiver.GetData();
        Commands commands = new Commands(_dataSender, _transferStructure,_disposeStructure);
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
                _transferStructure.Send(_dataSender.SendData("Logged out!"));
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