using Server.Controllers.UserHandlingControllers;
using Shared.Models;

namespace SocketProgramming.Tests;

public class UserCreatorClassTest
{
    [Theory]
    [InlineData("testuser", true)]
    [InlineData("randomNickname", false)]
    public void CreateUserMethod_ToKnowWhetherUserAlreadyExistsOrNot_ReturnsTrueOrFalse(string username, bool userExist)
    {
        // arrange
        
        //To make sure that user exists
        using (File.Create($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/testuser.json"))
        {
        }

        File.Delete("/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/randomNickname.json");
        

        UserCreatorForTest userCreator = new UserCreatorForTest()
        {
            Username = username
        };


        // act
        
        userCreator.CreateUser();

        // assert
        
        Assert.Equal(userCreator.UserExist, userExist);
    }

    [Theory]
    [InlineData("piotr", "root123", Privileges.Admin)]
    [InlineData("test", "randomPassword", Privileges.User)]
    [InlineData("JohnDoe", "root12", Privileges.User)]
    public void CreateUserMethod_CheckIfMethodGrantPrivilegesCorrectly_ReturnsTrueOrFalse(string username,
        string specialPassword, Privileges privileges)
    {
        // arrange
        UserCreatorForTest userCreator = new UserCreatorForTest()
        {
            Username = username,
            SpecialPassword = specialPassword
        };
        
        // act
        userCreator.CreateUser();
        
        // assert
        Assert.Equal(privileges,userCreator.Privileges);
    }
}

