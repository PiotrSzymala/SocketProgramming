using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers;

public class UserPrivilegesChanger : IUserPrivilegesChanger
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;

    public UserPrivilegesChanger(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    public void ChangePrivileges()
    {
        var message = _dataSender.SendData("Which user's privileges you want to change?");
        _transferStructure.Send(message);
        
        var username = _dataReceiver.GetData();

        var userToChangePrivileges = ListSaver.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (ListSaver.Users.Contains(userToChangePrivileges))
        {
            userToChangePrivileges.Privileges = userToChangePrivileges.Privileges == Privileges.Admin ? Privileges.User : Privileges.Admin;
            
            using (StreamWriter file = File.CreateText($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{userToChangePrivileges.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(userToChangePrivileges);
                file.Write(result);
            }
            ListSaver.SaveList();
            
            message = _dataSender.SendData($"Privileges for {userToChangePrivileges.Username} changed.");
            _transferStructure.Send(message);
        }
        else
        {
            message = _dataSender.SendData("User does not exist.");
            _transferStructure.Send(message);
        }
    }
}
