using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;

using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserLogger : IUserLogger
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    //private Socket _socket;
    private ITransferStructure _transferStructure;
    public UserLogger(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public bool LogUserIn(out User loggedUser)
    {
        var message = _dataSender.SendData("Username:");
        _transferStructure.Send(message);
        
        var username = _dataReceiver.GetData();
        loggedUser = null;

        var user = ServerExecuter.Users.FirstOrDefault(x => x.Username.Equals(username));
        
        if (ServerExecuter.Users.Contains(user))
        {
            var json = File.ReadAllText($"users/{username}.json");
            var deserializedUser = JsonConvert.DeserializeObject<User>(json);
            
            message = _dataSender.SendData("Passsword:");
            _transferStructure.Send(message);
        
            var password = _dataReceiver.GetData();

            if (password.Equals(deserializedUser.Password))
            {
                message = _dataSender.SendData("Logged! Type 'help' to get list of commands:");
                _transferStructure.Send(message);
                loggedUser = user;
                return true;
            }

            message = _dataSender.SendData("Wrong password!");
            _transferStructure.Send(message);
            return false;
        }

        message = _dataSender.SendData("User does not exist.");
        _transferStructure.Send(message);

        return false;
    }
}