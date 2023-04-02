using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public  class MessageSender
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public MessageSender(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public  void SendMessage(User sender)
    {
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);
        
        var message = dataSender.Send("To whom do you want to send the message?");
        _socket.Send(message);

        var username = dataReceiver.Receive(_socket);

        var userToSendMessage = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToSendMessage))
        {
            if (userToSendMessage.Inbox.Count >= 5 && userToSendMessage.Privileges != Privileges.Admin)
            {
                message = dataSender.Send($"The inbox of {userToSendMessage.Username} is full.");
                _socket.Send(message);
            }
            else
            {
                message = dataSender.Send("Write your message:");
                _socket.Send(message);

                var messageContent =dataReceiver.Receive(_socket);

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
                
                message = dataSender.Send("The message has been sent.");
                _socket.Send(message);
            }
        }
        else
        {
            message = dataSender.Send("User does not exist.");
            _socket.Send(message);
        }
    }
}