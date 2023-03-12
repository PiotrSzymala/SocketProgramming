using System.Text;

namespace Server;

public class Commands
{
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