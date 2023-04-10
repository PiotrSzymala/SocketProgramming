using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class UserPrivilegesChangerClassTests
{
    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void ChangePrivilegesMethod_TryToCheckCreatedUserPrivileges_ReturnsCorrectChangingResult(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForPrivilegesChanger = new DataReceiverForTest(name);

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender); 
        var userPrivilegesChanger= new UserPrivilegesChanger(new DataSender(),dataReceiverForPrivilegesChanger,sender);
        
        // act
        var createdUser =  userCreator.CreateUser();
        var beginPrivileges = createdUser.Privileges;
        
        userPrivilegesChanger.ChangePrivileges();
        var changedPrivilege = createdUser.Privileges;
        
        // assert
        Assert.NotEqual(beginPrivileges, changedPrivilege);
        
        //rearrange
        File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
        
        ListSaver.Users.Remove(createdUser);
       
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}