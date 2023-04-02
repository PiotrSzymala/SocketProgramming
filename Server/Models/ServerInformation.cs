namespace Server.Models;

public static class ServerInformation
{
    public static string ServerVersion { get; set; } = "0.0.2";
    public static DateTime ServerCreationDate { get; set; } = DateTime.Now;
}