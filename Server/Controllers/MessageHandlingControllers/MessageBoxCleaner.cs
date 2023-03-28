using Newtonsoft.Json;
using Shared.Controllers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public static class MessageBoxCleaner
{
    public static void ClearInbox(User currentlyLoggedUser)
    {
        var message = DataSender.SendData("All messages will be deleted. Are you sure? (y/n)");
        ServerExecuter.ClientSocket.Send(message);

        var decision = DataReceiver.GetData(ServerExecuter.ClientSocket);

        if (decision.ToLower() == "y")
        {
            currentlyLoggedUser.Inbox.Clear();
            
            using (StreamWriter file = File.CreateText($"users/{currentlyLoggedUser.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(currentlyLoggedUser);
                file.Write(result);
            }

            ListSaver.SaveList();
            
            message = DataSender.SendData("All messages deleted.");
            ServerExecuter.ClientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("Deleting canceled.");
            ServerExecuter.ClientSocket.Send(message);
        }
    }
}