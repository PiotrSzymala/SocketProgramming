using Shared.Models;

namespace Server.Interfaces;

public interface IUserLogger
{
    public bool LogUserIn(out User loggedUser);
}