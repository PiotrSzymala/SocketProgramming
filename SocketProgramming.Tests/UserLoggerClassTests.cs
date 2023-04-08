using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class UserLoggerClassTests
{
    [Theory]
    [InlineData("Jerry","rsd123")]
    [InlineData("Jack","rfgsgsa")]
    [InlineData("Jill","fdsfl")]
    public void LoggerUserMethod_TryToLogInWithUsersThatDontExist_ReturnsNull(string name, string password)
    {
        // arrange
         
        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);

        DataReceiverForTest dataReceiverForTest = new DataReceiverForTest(name,password);
        
        var userLogger = new UserLogger(new DataSender(),dataReceiverForTest,sender);
        
        // act
        userLogger.LogUserIn(out User user);
        
        // assert
        Assert.Equal(null, user);
        
        socket.Close();

    }

    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void LoggerUserMethod_TryToLogInWithUsersThatExist_ReturnsCorrectLoggingResult(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForLogger = new DataReceiverForTest(name,password);

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender); 
        var userLogger = new UserLogger(new DataSender(),dataReceiverForLogger,sender);
        
        // act
        userCreator.CreateUser(out User createdUser);
        userLogger.LogUserIn(out User resultOfLogging);
        
        // assert
        Assert.Equal(createdUser, resultOfLogging);
        
        //rearrange
        File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
        
        ListSaver.Users.Remove(createdUser);
        
        socket.Close();

    }
}