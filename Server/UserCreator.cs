using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace Server;

public static class UserCreator
{
    public static void CreateUser(Socket clientSocket)
    {
         var message = SendToClient("Set username");

        clientSocket.Send(message);
        
        var username = GetDataFromClient(clientSocket);

        if (!File.Exists($"{username}.json"))
        {
            message = SendToClient("Set password");
            clientSocket.Send(message);
            
            var password = GetDataFromClient(clientSocket);

            message = SendToClient("If you want admin account type in a special password:");
            clientSocket.Send(message);
            
            var specialPassword = GetDataFromClient(clientSocket);

            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);

            var user = new User(username, password, privileges);
            

            using (StreamWriter file = File.CreateText($"{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            
            message = SendToClient("User created!");
            clientSocket.Send(message);
        }
        else
        {
            message = SendToClient("User already exists!");
            clientSocket.Send(message);
        }
    }

    private static string GetDataFromClient(Socket clientSocket)
    {
        var bytesU = new byte[1024];
        var numByte = clientSocket.Receive(bytesU);

        var received = Encoding.ASCII.GetString(bytesU, 0, numByte);
        var MessageToClient = JsonConvert.DeserializeObject<MyMessage>(received);

        var result = MessageToClient.Message;
        return result;
    }

    private static byte[] SendToClient(string messageContext)
    {
        var MessageToClient = new MyMessage();
        MessageToClient.Message = messageContext;
        var toSend = JsonConvert.SerializeObject(MessageToClient);
        var message = Encoding.ASCII.GetBytes(toSend);
        return message;
    }
}