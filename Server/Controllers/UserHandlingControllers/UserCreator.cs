using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserCreator
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public UserCreator(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public void CreateUser()
    {
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);
         
        var message = dataSender.Send("Set username");
        _socket.Send(message);
        
        var username = dataReceiver.Receive(_socket);

        if (!File.Exists($"users/{username}.json"))
        {
            message = dataSender.Send("Set password");
            _socket.Send(message);
            
            var password = dataReceiver.Receive(_socket);

            message = dataSender.Send("If you want admin account type in a special password:");
            _socket.Send(message);
            
            var specialPassword = dataReceiver.Receive(_socket);
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            
            var user = new User(username, password, privileges, new List<MessageToUser>());
            ServerExecuter.Users.Add(user);
            
            using (StreamWriter file = File.CreateText($"users/{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            ListSaver.SaveList();

            var typeOfUser = user.Privileges == Privileges.Admin ? "Admin created" : "User created";
                
            message = dataSender.Send(typeOfUser);
            _socket.Send(message);
        }
        else
        {
            message = dataSender.Send("User already exists!");
            _socket.Send(message);
        }
    }
}