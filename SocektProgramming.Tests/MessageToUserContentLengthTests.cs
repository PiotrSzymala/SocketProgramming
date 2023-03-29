using Shared.Models;

namespace SocektProgramming.Tests;

public class MessageToUserContentLengthTests
{
    [Fact]
    public void MessageToUserContentLength_ForContentLongerThan255Characters_ReturnsShortenedMessage()
    {
        //arrange
        
        MessageToUser messageToUser = new MessageToUser("test", "first");
        
        //input length = 260
        string input = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. Sed. " +
                       "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. " +
                       "Sed. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. Sed. Lorem";
        //act

        messageToUser.MessageContent = input;
        
        //assert
        
        Assert.Equal(messageToUser.MessageContent.Length, 255);
    }
}