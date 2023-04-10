using System.Net.Sockets;
using Server.Controllers;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Shared;
using Shared.Controllers;
using Shared.Models;
using SocektProgramming.Tests;

namespace SocketProgramming.Tests;

public class MessageSenderClassTests
{
    [Theory]
    [InlineData("Jerry", "rsd123","root123",false)]
    [InlineData("Jack", "rfgsgsa", "fdfs",true)]
    [InlineData("Jill", "fdsfl", "tdst",true)]
    public void SendMessageMethod_CheckIfInboxCapacityIndicatorWorksCorrectly_ReturnsCorrectCapacityResult(string name, string password, string specialPassword, bool shouldBeFull)
    {
        // arrange

        var socket = new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(Config.LocalEndPoint);

        SocketSender sender = new SocketSender(socket);
        
        DataReceiverForTest dataReceiverForCreator = new DataReceiverForTest(name,password,specialPassword);
        DataReceiverForTest dataReceiverForSender= new DataReceiverForTest(name, "Test message");
        DataReceiverForTest dataReceiverForRemover = new DataReceiverForTest(name);

        var userCreator = new UserCreator(new DataSender(), dataReceiverForCreator, sender); 
        var messageSender= new MessageSender(new DataSender(),dataReceiverForSender,sender);
        var userRemover= new UserRemover(new DataSender(),dataReceiverForRemover,sender);
        
        
        // act
        var createdUser = userCreator.CreateUser();

        for (int i = 0; i <= 6; i++)
        {
            messageSender.SendMessage(createdUser);
        }
        
        // assert
        
        Assert.Equal(shouldBeFull,messageSender.IsInboxFull);

        
        // rearrange
        
        File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{name}.json");
        
        ListSaver.Users.Remove(createdUser);
        
        socket.Shutdown(SocketShutdown.Both);
        socket.Close();
    }
}