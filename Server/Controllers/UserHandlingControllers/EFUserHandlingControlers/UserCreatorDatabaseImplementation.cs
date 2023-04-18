using Server.Interfaces;
using Shared;
using Shared.Entities;
using Shared.Models;

namespace Server.Controllers.UserHandlingControllers.EFUserHandlingControlers;

public class UserCreatorDatabaseImplementation : IUserCreator
{
    private MyBoardsContext _myBoardsContext;
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    public bool IsUserExist;
    
    public UserCreatorDatabaseImplementation(MyBoardsContext myBoardsContext, IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _myBoardsContext = myBoardsContext;
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    
    public User CreateUser()
    {
        var message = _dataSender.SendData("Set username");
        _transferStructure.Send(message);
       
        var username = _dataReceiver.GetData();


        var existingUsers = _myBoardsContext.Users.ToList();
        var searchedUser = existingUsers.FirstOrDefault(x => x.Username.Equals(username));

        if (!existingUsers.Contains(searchedUser))
        {
            message = _dataSender.SendData("Set password");
            _transferStructure.Send(message);
            
            var password = _dataReceiver.GetData();

            message = _dataSender.SendData("If you want admin account type in a special password:");
            _transferStructure.Send(message);
            
            var specialPassword = _dataReceiver.GetData();
            var privileges = (specialPassword == "root123" ? Privileges.Admin : Privileges.User);
            
            var user = new User(username, password, privileges, new List<MessageToUser>());

            _myBoardsContext.Add(user);
            _myBoardsContext.SaveChanges();
            
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