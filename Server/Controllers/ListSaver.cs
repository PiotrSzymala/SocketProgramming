using Newtonsoft.Json;

namespace Server.Controllers;

public static class ListSaver
{
    public static void SaveList()
    {
        using StreamWriter listWriter = File.CreateText($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/users.json");
        var result = JsonConvert.SerializeObject(ServerExecuter.Users);
        listWriter.Write(result);
    }
}