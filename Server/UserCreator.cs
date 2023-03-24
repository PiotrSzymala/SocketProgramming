using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace Server;

public static class UserCreator
{
    public static void CreateUser(Socket clientSocket)
    {
         var message = DataSender.SendData("Set username");

        clientSocket.Send(message);
        
        var username = DataReceiver.GetData(clientSocket);

        if (!File.Exists($"{username}.json"))
        {
            message = DataSender.SendData("Set password");
            clientSocket.Send(message);
            
            var password = DataReceiver.GetData(clientSocket);

            message = DataSender.SendData("If you want admin account type in a special password:");
            clientSocket.Send(message);
            
            var specialPassword = DataReceiver.GetData(clientSocket);
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            var user = new User(username, password, privileges);
            
            using (StreamWriter file = File.CreateText($"{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            
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