using Newtonsoft.Json;
using Shared.Models;

namespace Server.Controllers;

public static class ListSaver
{
    public static List<User> Users = new List<User>();
    public static void SaveList()
    {
        using StreamWriter listWriter = File.CreateText($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/users.json");
        var result = JsonConvert.SerializeObject(Users);
        listWriter.Write(result);
    }
}