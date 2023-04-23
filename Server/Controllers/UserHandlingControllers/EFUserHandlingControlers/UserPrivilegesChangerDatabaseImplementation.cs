using Server.Interfaces;
using Shared;
using Shared.Entities;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers.EFUserHandlingControlers;

public class UserPrivilegesChangerDatabaseImplementation : IUserPrivilegesChanger
{
    private MyBoardsContext _myBoardsContext;
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;

    public UserPrivilegesChangerDatabaseImplementation(MyBoardsContext myBoardsContext, IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _myBoardsContext = myBoardsContext;
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }

    public void ChangePrivileges()
    {
        var message = _dataSender.SendData("Which user's privileges you want to change?");
        _transferStructure.Send(message);
        
        var username = _dataReceiver.GetData();

        var userToChangePrivileges = _myBoardsContext.Users.FirstOrDefault(u => u.Username.Equals(username));

        if (_myBoardsContext.Users.Contains(userToChangePrivileges))
        {
            userToChangePrivileges.Privileges = userToChangePrivileges.Privileges == Privileges.Admin ? Privileges.User : Privileges.Admin;

            _myBoardsContext.SaveChanges();
            
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