using Shared.Models;

namespace Server.Interfaces;

public interface IMessageBoxCleaner
{
    public void ClearInbox(User currentlyLoggedUser);
}