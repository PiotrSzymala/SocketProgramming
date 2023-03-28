using System.Net.Sockets;
using Shared.Controllers;

namespace Server;

public static class Commands
{
    
    public static void UptimeCommand()
    {
        var currentWorkingTime = DateTime.Now;
        var timeSpan = currentWorkingTime - ServerExecuter.ServerCreationTime;
        var message = $"Running time: {timeSpan}";

        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
    }

    public static void InfoCommand()
    {
        var message = $"Sever version number: {ServerInformation.ServerVersion}\n" +
                      $"Server creation date: {ServerInformation.ServerCreationDate}\n";
        
        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
    }

    public static void HelpCommand()
    {
        var message = "\nPossible commands: \n" +
                      "send - send message to other user.\n" +
                      "inbox - check your inbox.\n" +
                      "clear - clear your inbox\n" +
                      "uptime - return server lifetime.\n" +
                      "info - return server's version and creation date.\n" +
                      "help - return list of possible commands with short description.\n" +
                      "logout - log out from your account.\n" +
                      "stop - stop server and client running.\n";
                      
        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
    }

    public static void HelpCommandForAdmin()
    {
        var message = "\nPossible commands: \n" +
                      "change - change user's privileges\n" +
                      "delete - delete user\n"+
                      "send - send message to other user.\n" +
                      "inbox - check your inbox.\n" +
                      "clear - clear your inbox\n" +
                      "uptime - return server lifetime.\n" +
                      "info - return server's version and creation date.\n" +
                      "help - return list of possible commands with short description.\n" +
                      "logout - log out from your account.\n" +
                      "stop - stop server and client running.\n";
                      
        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
    }
    
    public static void StopCommand()
    {
        var message = "Shutting down...";
        
        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
        
        ServerExecuter.ClientSocket.Shutdown(SocketShutdown.Both);
        ServerExecuter.ClientSocket.Close();
    }

    public static void WrongCommand()
    {
        var message = "Wrong command\n";
       
        var result = DataSender.SendData(message);
        ServerExecuter.ClientSocket.Send(result);
    }
}