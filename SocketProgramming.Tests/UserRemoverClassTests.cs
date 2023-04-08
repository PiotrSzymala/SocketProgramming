using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class UserRemoverClassTests
{
    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void RemoveUserMethod_TryToDeleteCreatedUser_RemovesUserCorrectly(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForRemover = new DataReceiverForTest(name);

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender); 
        var userRemover= new UserRemover(new DataSender(),dataReceiverForRemover,sender);
        
        // act
        userCreator.CreateUser(out User createdUser);
        
        
        // assert
        Assert.NotEqual(null, createdUser);
        
        userRemover.RemoveUser();
        
        Assert.True((!ListSaver.Users.Contains(createdUser)) && userRemover.UserRemoveSuccess);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
    
    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void RemoveUserMethod_TryToDeleteUserThatDontExist_RemoveUserFail(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForRemover = new DataReceiverForTest(name);
        
        var userRemover= new UserRemover(new DataSender(),dataReceiverForRemover,sender);
        
        // act
        
        userRemover.RemoveUser();
        
        // assert
        
        Assert.False(userRemover.UserRemoveSuccess);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}