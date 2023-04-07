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
    public static IDataReceiver CreateDataReceiver(ITransferStructure transferStructure)
    {
        return new DataReceiver(transferStructure);
    }
    public static IUserCreator CreateUserCreator(ITransferStructure transferStructure)
    {
        return new UserCreator(CreateDataSender(),CreateDataReceiver(transferStructure),transferStructure);
    }
    public static IUserLogger CreateUserLogger(ITransferStructure transferStructure)
    {
        return new UserLogger(CreateDataSender(), CreateDataReceiver(transferStructure), transferStructure);
    }
    public static IUserRemover CreateUserRemover(ITransferStructure transferStructure)
    {
        return new UserRemover(CreateDataSender(),CreateDataReceiver(transferStructure),transferStructure);
    }
    public static IUserPrivilegesChanger CreateUserUserPrivilegesChanger(ITransferStructure transferStructure)
    {
        return new UserPrivilegesChanger(CreateDataSender(),CreateDataReceiver(transferStructure),transferStructure);
    }
    public static IMessageBoxCleaner CreateMessageBoxCleaner(ITransferStructure transferStructure)
    {
        return new MessageBoxCleaner(CreateDataSender(),CreateDataReceiver(transferStructure),transferStructure);
    }
    public static IMessageChecker CreateMessageChecker(ITransferStructure transferStructure)
    {
        return new MessageChecker(CreateDataSender(),CreateDataReceiver(transferStructure),transferStructure);
    }
    public static IMessageSender CreateMessageSender(ITransferStructure transferStructure)
    {
        return new MessageSender(CreateDataSender(), CreateDataReceiver(transferStructure), transferStructure);
    }
}