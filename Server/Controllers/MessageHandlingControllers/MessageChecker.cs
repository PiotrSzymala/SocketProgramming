using Shared.Controllers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public static class MessageChecker
{
    public static void CheckInbox(User currentlyLoggedUser)
    {
        if (currentlyLoggedUser.Inbox.Count == 0)
        {
            var message = DataSender.SendData("Inbox is empty");
            ServerExecuter.ClientSocket.Send(message);
        }
        else
        {
            MessagesDisplayer(currentlyLoggedUser.Inbox);
        }
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
        ServerExecuter.ClientSocket.Send(messageInfo);
    }
}