using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public  class UserLogger
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public UserLogger(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public  bool LogUserIn(out User loggedUser)
    {
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);
        
        var message = dataSender.Send("Username:");
        _socket.Send(message);
        
        var username = dataReceiver.Receive(_socket);
        loggedUser = null;

        var user = ServerExecuter.Users.FirstOrDefault(x => x.Username.Equals(username));
        
        if (ServerExecuter.Users.Contains(user))
        {
            var json = File.ReadAllText($"users/{username}.json");
            var deserializedUser = JsonConvert.DeserializeObject<User>(json);
            
            message = dataSender.Send("Passsword:");
            _socket.Send(message);
        
            var password = dataReceiver.Receive(_socket);

            if (password.Equals(deserializedUser.Password))
            {
                message = dataSender.Send("Logged! Type 'help' to get list of commands:");
                _socket.Send(message);
                loggedUser = user;
                return true;
            }

            message = dataSender.Send("Wrong password!");
            _socket.Send(message);
            return false;
        }

        message = dataSender.Send("User does not exist.");
        _socket.Send(message);

        return false;
    }
}