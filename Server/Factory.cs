using System.Net.Sockets;
using Server.Controllers.MessageHandlingControllers;
using Server.Controllers.UserHandlingControllers;
using Server.Interfaces;
using Shared;
using Shared.Controllers;

namespace Server;
public static class Factory
{
    
    public static Socket CreateSocket()
    {
        return new Socket(Config.IpAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
    }
    public static IDataSender CreateDataSender()
    {
        return new DataSender();
    }
    public static IDataReceiver CreateDataReceiver()
    {
        return new DataReceiver();
    }
    public static IUserCreator CreateUserCreator(Socket socket)
    {
        return new UserCreator(CreateDataSender(),CreateDataReceiver(),socket);
    }
    public static IUserLogger CreateUserLogger(Socket socket)
    {
        return new UserLogger(CreateDataSender(), CreateDataReceiver(), socket);
    }
    public static IUserRemover CreateUserRemover(Socket socket)
    {
        return new UserRemover(CreateDataSender(),CreateDataReceiver(),socket);
    }
    public static IUserPrivilegesChanger CreateUserUserPrivilegesChanger(Socket socket)
    {
        return new UserPrivilegesChanger(CreateDataSender(),CreateDataReceiver(),socket);
    }
    public static IMessageBoxCleaner CreateMessageBoxCleaner(Socket socket)
    {
        return new MessageBoxCleaner(CreateDataSender(),CreateDataReceiver(),socket);
    }
    public static IMessageChecker CreateMessageChecker(Socket socket)
    {
        return new MessageChecker(CreateDataSender(),CreateDataReceiver(),socket);
    }
    public static IMessageSender CreateMessageSender(Socket socket)
    {
        return new MessageSender(CreateDataSender(), CreateDataReceiver(), socket);
    }
}