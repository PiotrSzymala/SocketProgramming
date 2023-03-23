using System.Text;
using Shared;
using Shared.Models;

namespace Server;

public static class Commands
{
    public static byte[] UptimeCommand()
    {
        var currentWorkingTime = DateTime.Now;
        var timeSpan = currentWorkingTime - ServerExecuter.ServerCreationTime;
        
        MyMessage jsonResponse = new MyMessage
        {
            Message = $"Running time: {timeSpan}"
        };

        var resultFromServer =JsonSerializer.Serialize(jsonResponse);
        byte[] message = Encoding.ASCII.GetBytes(resultFromServer);

        return message;
    }
    public static byte[] InfoCommand()
    {
        MyMessage jsonResponse = new MyMessage
        {
            Message = $"Sever version number: {ServerInformation.ServerVersion}\n" +
                      $"Server creation date: {ServerInformation.ServerCreationDate}\n"
        };

        var resultFromServer =JsonSerializer.Serialize(jsonResponse);
        byte[] message = Encoding.ASCII.GetBytes(resultFromServer);
        
        return message;
    }
    public static byte[] HelpCommand()
    {
        MyMessage jsonResponse = new MyMessage
        {
            Message = "Possible commands: \n" +
                      "uptime - returns server lifetime.\n" +
                      "info - returns server's version and creation date.\n" +
                      "help - returns list of possible commands with short description.\n" +
                      "stop - stops server and client running.\n"
        };

        var resultFromServer =JsonSerializer.Serialize(jsonResponse);
        byte[] message = Encoding.ASCII.GetBytes(resultFromServer);
               

           return message;
    }

    public static byte[] StopCommand()
    {
        MyMessage jsonResponse = new MyMessage
        {
            Message = "Shutting down..."
        };

        var resultFromServer =JsonSerializer.Serialize(jsonResponse);
        byte[] message = Encoding.ASCII.GetBytes(resultFromServer);
        
        return message;
    }

    public static byte[] WrongCommand()
    {
        MyMessage jsonResponse = new MyMessage
        {
            Message = "Wrong command\n"
        };
        
        var resultFromServer =JsonSerializer.Serialize(jsonResponse);
        byte[] message = Encoding.ASCII.GetBytes(resultFromServer);
       
        return message;
    }
    
}