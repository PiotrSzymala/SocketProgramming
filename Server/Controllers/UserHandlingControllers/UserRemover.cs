using Shared.Controllers;

namespace Server.Controllers.UserHandlingControllers;

public static class UserRemover
{
    public static void RemoveUser()
    {
        var message = DataSender.SendData("Which user you want to delete?");
        ServerExecuter.ClientSocket.Send(message);

        var username = DataReceiver.GetData(ServerExecuter.ClientSocket);

        var userToDelete = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToDelete))
        {
            File.Delete($"users/{userToDelete.Username}.json");
            ServerExecuter.Users.Remove(userToDelete);
            
            ListSaver.SaveList();
            
            message = DataSender.SendData("User has been deleted.");
            ServerExecuter.ClientSocket.Send(message);
        }
        else
        {
            message = DataSender.SendData("User does not exist.");
            ServerExecuter.ClientSocket.Send(message);
        }
    }
}