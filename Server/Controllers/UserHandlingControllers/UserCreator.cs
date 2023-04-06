using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserCreator : IUserCreator
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
        var message = _dataSender.SendData("Set username");
        _socket.Send(message);

        var username = _dataReceiver.GetData(_socket);
        
        if (!File.Exists($"users/{username}.json"))
        {
            message = _dataSender.SendData("Set password");
            _socket.Send(message);
            
            var password = _dataReceiver.GetData(_socket);

            message = _dataSender.SendData("If you want admin account type in a special password:");
            _socket.Send(message);
            
            var specialPassword = _dataReceiver.GetData(_socket);
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
                
            message = _dataSender.SendData(typeOfUser);
            _socket.Send(message);
        }
        else
        {
            message = _dataSender.SendData("User already exists!");
            _socket.Send(message);
        }
    }
}