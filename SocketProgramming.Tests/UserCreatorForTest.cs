using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserCreatorForTest : IUserCreator
{
    public string Username;
    public string Password;
    public string SpecialPassword;
    public User User;
    public bool UserExist;
    public Privileges Privileges;
    
    public UserCreatorForTest()
    {
    }
    public void CreateUser()
    {
        var username = Username;

        if (!File.Exists($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{username}.json"))
        {
            var password = Password;

            var specialPassword = SpecialPassword;
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            Privileges = privileges;
            
            var user = new User(username, password, privileges, new List<MessageToUser>());
            ServerExecuter.Users.Add(user);
            
            User = user;
            
            var typeOfUser = user.Privileges == Privileges.Admin ? "Admin created" : "User created";
            
            UserExist = false;
        }
        else
        {
            // _socket.Send(message);
            UserExist = true;
        }
    }
}