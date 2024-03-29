using System.Net.Sockets;
using Server.Interfaces;
using Shared;

namespace Server.Controllers.UserHandlingControllers;

public  class UserRemover : IUserRemover
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    public bool UserRemoveSuccess;
    public UserRemover(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public void RemoveUser()
    {
        var message = _dataSender.SendData("Which user you want to delete?");
        _transferStructure.Send(message);

        var username = _dataReceiver.GetData();

        var userToDelete = ListSaver.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ListSaver.Users.Contains(userToDelete))
        {
            File.Delete($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{userToDelete.Username}.json");
            ListSaver.Users.Remove(userToDelete);
            
            ListSaver.SaveList();
            
            message = _dataSender.SendData("User has been deleted.");
            _transferStructure.Send(message);

            UserRemoveSuccess = true;
        }
        else
        {
            message = _dataSender.SendData("User does not exist.");
            _transferStructure.Send(message);

            UserRemoveSuccess = false;
        }
    }
}