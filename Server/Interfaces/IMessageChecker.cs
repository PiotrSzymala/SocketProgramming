using Shared.Models;

namespace Server.Interfaces;

public interface IMessageChecker
{
    public void CheckInbox(User currentlyLoggedUser);
    public void MessagesDisplayer(List<MessageToUser> messages);
}