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
    private ITransferStructure _transferStructure;
    public bool IsUserExist;
    public UserCreator(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public User CreateUser()
    {
        var message = _dataSender.SendData("Set username");
        _transferStructure.Send(message);
       
        var username = _dataReceiver.GetData();

        var searchedUser = ListSaver.Users.FirstOrDefault(x => x.Username.Equals(username));
        
        if (!ListSaver.Users.Contains(searchedUser))
        {
            message = _dataSender.SendData("Set password");
            _transferStructure.Send(message);
            
            var password = _dataReceiver.GetData();

            message = _dataSender.SendData("If you want admin account type in a special password:");
            _transferStructure.Send(message);
            
            var specialPassword = _dataReceiver.GetData();
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            
            var user = new User(username, password, privileges, new List<MessageToUser>());
            ListSaver.Users.Add(user);

            
            using (StreamWriter file = File.CreateText($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{username}.json"))
            {
                var result = JsonConvert.SerializeObject(user);
                file.Write(result);
            }
            ListSaver.SaveList();

            var typeOfUser = user.Privileges == Privileges.Admin ? "Admin created" : "User created";
                
            IsUserExist = false;
            message = _dataSender.SendData(typeOfUser);
            _transferStructure.Send(message);

            return user;
        }
        
        IsUserExist = true;
        message = _dataSender.SendData("User already exists!");
        _transferStructure.Send(message);
        return null;
    }
}