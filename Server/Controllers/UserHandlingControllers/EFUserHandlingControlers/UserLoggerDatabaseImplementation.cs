using Server.Interfaces;
using Shared;
using Shared.Entities;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers.EFUserHandlingControlers;

public class UserLoggerDatabaseImplementation : IUserLogger
{
    private MyBoardsContext _myBoardsContext;
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;

    public UserLoggerDatabaseImplementation(MyBoardsContext myBoardsContext, IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _myBoardsContext = myBoardsContext;
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
        
        var user = _myBoardsContext.Users.FirstOrDefault(x => x.Username.Equals(username));

        if (_myBoardsContext.Users.Contains(user))
        {
            message = _dataSender.SendData("Passsword:");
            _transferStructure.Send(message);

            var password = _dataReceiver.GetData();

            if (user.Password.Equals(password))
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