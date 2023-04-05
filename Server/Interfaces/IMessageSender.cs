using Shared.Models;

namespace Server.Interfaces;

public interface IMessageSender
{
    public void SendMessage(User sender);
}