using Shared.Models;

namespace Server.Interfaces;

public interface IUserCreator
{
    public void CreateUser(out User createdUser);
}