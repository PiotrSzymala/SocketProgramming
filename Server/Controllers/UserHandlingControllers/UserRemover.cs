using System.Net.Sockets;
using Shared;
using Shared.Handlers;

namespace Server.Controllers.UserHandlingControllers;

public  class UserRemover
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
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);
        
        var message = dataSender.Send("Which user you want to delete?");
        _socket.Send(message);

        var username = dataReceiver.Receive(_socket);

        var userToDelete = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToDelete))
        {
            File.Delete($"users/{userToDelete.Username}.json");
            ServerExecuter.Users.Remove(userToDelete);
            
            ListSaver.SaveList();
            
            message = dataSender.Send("User has been deleted.");
            _socket.Send(message);
        }
        else
        {
            message = dataSender.Send("User does not exist.");
            _socket.Send(message);
        }
    }
}