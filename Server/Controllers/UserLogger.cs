using Newtonsoft.Json;
using Shared;
using Shared.Controllers;
using Shared.Models;

namespace Server.Controllers;

public static class UserLogger
{
    public static bool LogUserIn(out User loggedUser)
    {
        var message = DataSender.SendData("Username:");
        ServerExecuter.ClientSocket.Send(message);
        
        var username = DataReceiver.GetData(ServerExecuter.ClientSocket);
        loggedUser = null;

        var user = ServerExecuter.Users.FirstOrDefault(x => x.Username.Equals(username));
        
        if (ServerExecuter.Users.Contains(user))
        {
            var json = File.ReadAllText($"users/{username}.json");
            var deserializedUser = JsonConvert.DeserializeObject<User>(json);
            
            message = DataSender.SendData("Passsword:");
            ServerExecuter.ClientSocket.Send(message);
        
            var password = DataReceiver.GetData(ServerExecuter.ClientSocket);

            if (password.Equals(deserializedUser.Password))
            {
                message = DataSender.SendData("Logged!");
                ServerExecuter.ClientSocket.Send(message);
                loggedUser = user;
                return true;
            }

            message = DataSender.SendData("Wrong password!");
            ServerExecuter.ClientSocket.Send(message);
            return false;
        }

        message = DataSender.SendData("User does not exist.");
        ServerExecuter.ClientSocket.Send(message);

        return false;
    }
}