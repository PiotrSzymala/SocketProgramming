using Newtonsoft.Json;
using Shared.Controllers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public static class MessageSender
{
    public static void SendMessage(User sender)
    {
        var message = DataSender.SendData("To whom do you want to send the message?");
        ServerExecuter.ClientSocket.Send(message);

        var username = DataReceiver.GetData(ServerExecuter.ClientSocket);

        var userToSendMessage = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToSendMessage))
        {
            if (userToSendMessage.Inbox.Count >= 5 && userToSendMessage.Privileges != Privileges.Admin)
            {
                message = DataSender.SendData($"The inbox of {userToSendMessage.Username} is full.");
                ServerExecuter.ClientSocket.Send(message);
            }
            else
            {
                message = DataSender.SendData("Write your message:");
                ServerExecuter.ClientSocket.Send(message);

                var messageContent = DataReceiver.GetData(ServerExecuter.ClientSocket);

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
                
                message = DataSender.SendData("The message has been sent.");
                ServerExecuter.ClientSocket.Send(message);
            }
        }
        else
        {
            message = DataSender.SendData("User does not exist.");
            ServerExecuter.ClientSocket.Send(message);
        }
    }
}