namespace Shared.Models;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Privileges Privileges { get; set; } = Privileges.User;

    public User(string username, string password, Privileges privileges)
    {
        Username = username;
        Password = password;
        Privileges = privileges;
    }
}