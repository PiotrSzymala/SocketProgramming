using Shared.Models;

namespace SocketProgramming.Tests;

public class MessageToUserMessageContentPropertyLengthTests
{
    [Theory]
    [InlineData("testAuthor", "hello new user!")]
    [InlineData("testAuthor", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. Sed. " +
                              "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. " +
                              "Sed. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis quis blandit est. Sed. Lorem")]//content length = 260
    public void MessageToUserContentLength_ForContentLongerThan255Characters_ReturnsShortenedMessage(string author, string content)
    {
        //arrange
        
        MessageToUser messageToUser = new MessageToUser(author, content);
        
        
        //assert
        
        Assert.True(messageToUser.MessageContent.Length <= 255);
    }
}