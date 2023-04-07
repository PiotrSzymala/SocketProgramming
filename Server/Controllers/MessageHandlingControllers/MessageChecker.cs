using System.Net.Sockets;
using Server.Interfaces;
using Shared;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public  class MessageChecker : IMessageChecker
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    public MessageChecker(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public  void CheckInbox(User currentlyLoggedUser)
    {
        if (currentlyLoggedUser.Inbox.Count == 0)
        {
            var message = _dataSender.SendData("Inbox is empty");
            _transferStructure.Send(message);
        }
        else
        {
            MessagesDisplayer(currentlyLoggedUser.Inbox);
        }
    }
    public void MessagesDisplayer(List<MessageToUser> messages)
    {
        int counter = 1;

        string test = string.Empty;
        foreach (var message in messages)
        {
            test += $"\n\nMessage {counter}/{messages.Count}, From: {message.MessageAuthor}" +
                    $"\nContent: {message.MessageContent}\n";
            counter++;
        }
        var messageInfo = _dataSender.SendData(test);
        _transferStructure.Send(messageInfo);
    }
}