using Newtonsoft.Json;

namespace Shared.Models;

public class User
{
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
   
    [JsonProperty("privileges")]
    public Privileges Privileges { get; set; } = Privileges.User;

    [JsonProperty("inbox")]
    public List<MessageToUser> Inbox { get; set; } = new List<MessageToUser>();

    public User(string username, string password, Privileges privileges, List<MessageToUser> inbox)
    {
        Username = username;
        Password = password;
        Privileges = privileges;
        Inbox = inbox;
    }

    public User()
    {
    }
}