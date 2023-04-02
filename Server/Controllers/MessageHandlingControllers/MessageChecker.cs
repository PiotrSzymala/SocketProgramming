using System.Net.Sockets;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public  class MessageChecker
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public MessageChecker(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public  void CheckInbox(User currentlyLoggedUser)
    {
        var dataSender = new DataSendHandler(_dataSender);
        
        
        if (currentlyLoggedUser.Inbox.Count == 0)
        {
            var message = dataSender.Send("Inbox is empty");
            _socket.Send(message);
        }
        else
        {
            MessagesDisplayer(currentlyLoggedUser.Inbox);
        }
    }
    private  void MessagesDisplayer(List<MessageToUser> messages)
    {
        var dataSender = new DataSendHandler(_dataSender);
        int counter = 1;

        string test = string.Empty;
        foreach (var message in messages)
        {
            test += $"\n\nMessage {counter}/{messages.Count}, From: {message.MessageAuthor}" +
                    $"\nContent: {message.MessageContent}\n";
            counter++;
        }
        var messageInfo = dataSender.Send(test);
        _socket.Send(messageInfo);
    }
}