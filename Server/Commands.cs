using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

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
        var message = "Possible commands: \n" +
                      "uptime - returns server lifetime.\n" +
                      "info - returns server's version and creation date.\n" +
                      "help - returns list of possible commands with short description.\n" +
                      "stop - stops server and client running.\n";
                      
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