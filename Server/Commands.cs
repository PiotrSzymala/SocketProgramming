using System.Text;

namespace Server;

public class Commands
{
    public static byte[] UptimeCommand()
    {
        var result = (DateTime.Now - Server.Program.ServerStartCount).ToString();

        byte[] message = Encoding.ASCII.GetBytes($"$Running time: {result}");

        return message;
    }
    public static byte[] InfoCommand()
    {
        byte[] message = Encoding.ASCII.GetBytes(
            $"Sever version number: {ServerInformation.ServerVersion}\n"+
            $"Server creation date: {ServerInformation.ServerCreationDate}"
        );

        return message;
    }
    public static byte[] HelpCommand()
    {
           byte[] message = Encoding.ASCII.GetBytes( 
               "Possible commands: \n" +
               "uptime - returns server lifetime.\n" +
               "info - returns server's version and creation date.\n" +
               "help - returns list of possible commands with short description.\n" +
               "stop - stops server and client running.\n");

           return message;
    }

    
}