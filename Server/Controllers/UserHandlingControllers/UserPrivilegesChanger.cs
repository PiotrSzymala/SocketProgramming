using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserPrivilegesChanger
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public UserPrivilegesChanger(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public void ChangePrivileges()
    {
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);
        
        var message = dataSender.Send("Which user's privileges you want to change?");
        _socket.Send(message);
        
        var username = dataReceiver.Receive(_socket);

        var userToChangePrivileges = ServerExecuter.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ServerExecuter.Users.Contains(userToChangePrivileges))
        {
            userToChangePrivileges.Privileges = userToChangePrivileges.Privileges == Privileges.Admin ? Privileges.User : Privileges.Admin;
            
            using (StreamWriter file = File.CreateText($"users/{userToChangePrivileges.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(userToChangePrivileges);
                file.Write(result);
            }
            ListSaver.SaveList();
            
            message = dataSender.Send($"Privileges for {userToChangePrivileges.Username} changed.");
            _socket.Send(message);
        }
        else
        {
            message = dataSender.Send("User does not exist.");
            _socket.Send(message);
        }
    }
}
