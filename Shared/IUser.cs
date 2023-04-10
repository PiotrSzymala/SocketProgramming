using Shared.Models;

namespace Shared;

public interface IUser
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Privileges Privileges { get; set; } 
    public List<MessageToUser> Inbox { get; set; }

}