using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public  class MessageSender : IMessageSender
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    public MessageSender(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public void SendMessage(User sender)
    {
        var message = _dataSender.SendData("To whom do you want to send the message?");
        _transferStructure.Send(message);

        var username = _dataReceiver.GetData();

        var userToSendMessage = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToSendMessage))
        {
            if (userToSendMessage.Inbox.Count >= 5 && userToSendMessage.Privileges != Privileges.Admin)
            {
                message = _dataSender.SendData($"The inbox of {userToSendMessage.Username} is full.");
                _transferStructure.Send(message);
            }
            else
            {
                message = _dataSender.SendData("Write your message:");
                _transferStructure.Send(message);

                var messageContent = _dataReceiver.GetData();

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
                ListSaver.SaveList();
                
                message = _dataSender.SendData("The message has been sent.");
                _transferStructure.Send(message);
            }
        }
        else
        {
            message = _dataSender.SendData("User does not exist.");
            _transferStructure.Send(message);
        }
    }
}