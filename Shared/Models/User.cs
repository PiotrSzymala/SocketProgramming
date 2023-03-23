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

    public User(string username, string password, Privileges privileges)
    {
        Username = username;
        Password = password;
        Privileges = privileges;
    }
}