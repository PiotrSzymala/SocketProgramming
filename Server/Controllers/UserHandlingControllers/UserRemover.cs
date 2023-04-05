using System.Net.Sockets;
using Server.Interfaces;
using Shared;
using Shared.Handlers;

namespace Server.Controllers.UserHandlingControllers;

public  class UserRemover : IUserRemover
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public UserRemover(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public void RemoveUser()
    {
        var message = _dataSender.SendData("Which user you want to delete?");
        _socket.Send(message);

        var username = _dataReceiver.GetData(_socket);

        var userToDelete = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToDelete))
        {
            File.Delete($"users/{userToDelete.Username}.json");
            ServerExecuter.Users.Remove(userToDelete);
            
            ListSaver.SaveList();
            
            message = _dataSender.SendData("User has been deleted.");
            _socket.Send(message);
        }
        else
        {
            message = _dataSender.SendData("User does not exist.");
            _socket.Send(message);
        }
    }
}