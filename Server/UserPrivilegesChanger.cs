using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace Server;

public static class UserPrivilegesChanger
{
    public static void ChangePrivileges()
    {
        var message = DataSender.SendData("Which user's privileges you want to change?");
        ServerExecuter.ClientSocket.Send(message);
        
        var nickname = DataReceiver.GetData(ServerExecuter.ClientSocket);

        var userToChangePrivileges = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(nickname));

        if (ServerExecuter.Users.Contains(userToChangePrivileges))
        {
            userToChangePrivileges.Privileges = userToChangePrivileges.Privileges == Privileges.Admin ? Privileges.User : Privileges.Admin;
            
            using (StreamWriter file = File.CreateText($"users/{userToChangePrivileges.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(userToChangePrivileges);
                file.Write(result);
            }
            
            message = DataSender.SendData($"Privileges for {userToChangePrivileges.Username} changed.");
            ServerExecuter.ClientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("User does not exist.");
            ServerExecuter.ClientSocket.Send(message);
        }
    }
}
