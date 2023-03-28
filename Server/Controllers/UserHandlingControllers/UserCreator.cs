using System.Net.Sockets;
using Newtonsoft.Json;
using Shared.Controllers;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public static class UserCreator
{
    public static void CreateUser(Socket clientSocket)
    {
         var message = DataSender.SendData("Set username");
         clientSocket.Send(message);
        
        var username = DataReceiver.GetData(clientSocket);

        if (!File.Exists($"users/{username}.json"))
        {
            message = DataSender.SendData("Set password");
            clientSocket.Send(message);
            
            var password = DataReceiver.GetData(clientSocket);

            message = DataSender.SendData("If you want admin account type in a special password:");
            clientSocket.Send(message);
            
            var specialPassword = DataReceiver.GetData(clientSocket);
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            
            var user = new User(username, password, privileges, new List<MessageToUser>());
            ServerExecuter.Users.Add(user);
            
            using (StreamWriter file = File.CreateText($"users/{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            ListSaver.SaveList();
            
            message = DataSender.SendData("User created!");
            clientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("User already exists!");
            clientSocket.Send(message);
        }
    }
}