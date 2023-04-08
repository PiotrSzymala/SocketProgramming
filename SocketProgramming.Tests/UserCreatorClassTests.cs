using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class UserCreatorClassTests
{
    [Theory]
    [InlineData("JohnDoe","test", "root123", Privileges.Admin)]
    [InlineData("JohnWick","myPassword", "IhaveNoIdeaWhatIsTheSpecialPassword", Privileges.User)]
    public void CreateUserMethod_CheckIfMethodGrantPrivilegesCorrectly_ReturnsTrueOrFalse(string name, string password, string specialPassword, Privileges privileges)
    {
        // arrange
        
        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);

        DataReceiverForTest dataReceiverForTest = new DataReceiverForTest(name, password, specialPassword);
        
        var userCreator = new UserCreator(new DataSender(),dataReceiverForTest,sender);
        
        // act
        userCreator.CreateUser(out User user);


        // assert
        Assert.Equal(privileges, user.Privileges);
        
        //rearrange
       File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");

       ListSaver.Users.Remove(user);
       socket.Close();
    }

     [Theory]
     [InlineData("John")]
     [InlineData("Kenny")]
     [InlineData("Anna")]
     public void CreateUserMethod_CheckIfUserExist_ReturnsFalse(string name)
     {
         // arrange
         
         var socket = new Socket(Config.IpAddr.AddressFamily,
             SocketType.Stream, ProtocolType.Tcp);

         socket.Connect(Config.LocalEndPoint);

         SocketSender sender = new SocketSender(socket);

         DataReceiverForTest dataReceiverForTest = new DataReceiverForTest(name,"test", "sg");
        
         var userCreator = new UserCreator(new DataSender(),dataReceiverForTest,sender);

         //act
         userCreator.CreateUser(out User user);
         
         //asert
         Assert.Equal(false,userCreator.IsUserExist);
         
         //rearrange
         File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
         ListSaver.Users.Remove(user);
         
         socket.Close();

     }
     
     [Theory]
     [InlineData("John")]
     [InlineData("Kenny")]
     [InlineData("Anna")]
     public void CreateUserMethod_CheckIfUserExist_ReturnsTrue(string name)
     {
         // arrange
         var testUser = new User(name, "password", Privileges.User, new List<MessageToUser>());
         ListSaver.Users.Add(testUser); //Simulating that user already exist

         var socket = new Socket(Config.IpAddr.AddressFamily,
             SocketType.Stream, ProtocolType.Tcp);

         socket.Connect(Config.LocalEndPoint);

         SocketSender sender = new SocketSender(socket);

         DataReceiverForTest dataReceiverForTest = new DataReceiverForTest(name,"test", "sg");
        
        var userCreator = new UserCreator(new DataSender(),dataReceiverForTest,sender);

         //act
         userCreator.CreateUser(out User user);
         
         //asert
         Assert.Equal(true,userCreator.IsUserExist);
         
         //rearrange
         File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
         ListSaver.Users.Remove(testUser);
         
         
         socket.Close();
     }
}

