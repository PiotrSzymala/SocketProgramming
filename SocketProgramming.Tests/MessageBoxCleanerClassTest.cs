using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class MessageBoxCleanerClassTest
{
    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void ClearInboxMethod_CheckIfInboxIsClearedCorrectly_ReturnsCorrectCleaningResult(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForSender= new DataReceiverForTest(name, "Test message");
        DataReceiverForTest dataReceiverForCleaner = new DataReceiverForTest("y");

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender); 
        var messageSender= new MessageSender(new DataSender(),dataReceiverForSender,sender);
        var messageBoxCleaner= new MessageBoxCleaner(new DataSender(),dataReceiverForCleaner,sender);
        
        // act
       var createdUser = userCreator.CreateUser();
        var initialNumberOfMessages = createdUser.Inbox.Count;
        
        messageSender.SendMessage(createdUser);
        var numberOfMessagesAfterSend = createdUser.Inbox.Count;
        
        messageBoxCleaner.ClearInbox(createdUser);
        var endNumberOfMessages = createdUser.Inbox.Count;
        
        
        // assert
        Assert.Equal(0, initialNumberOfMessages);
        Assert.Equal(1, numberOfMessagesAfterSend);
        Assert.Equal(0, endNumberOfMessages);
        Assert.True(messageBoxCleaner.CleaningSuccess);
        
        //rearrange
        File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
        
        ListSaver.Users.Remove(createdUser);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
    
    [Theory]
    [InlineData("Jerry", "rsd123","root123")]
    [InlineData("Jack", "rfgsgsa", "fdfs")]
    [InlineData("Jill", "fdsfl", "tdst")]
    public void ClearInboxMethod_CheckIfInboxProcessAbortWorks_ReturnsCorrectCleaningAbortResult(string name, string password, string specialPassword)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForCleaner = new DataReceiverForTest("n");

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender);
        var messageBoxCleaner= new MessageBoxCleaner(new DataSender(),dataReceiverForCleaner,sender);
        
        // act
        
        var createdUser = userCreator.CreateUser();
        messageBoxCleaner.ClearInbox(createdUser);
        
        // assert
        
        Assert.False(messageBoxCleaner.CleaningSuccess);
        
        //rearrange
        File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
        
        ListSaver.Users.Remove(createdUser);

        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}