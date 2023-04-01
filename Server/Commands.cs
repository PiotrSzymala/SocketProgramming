using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Server;

public class Commands
{
    private IDataSender _dataSender;
    private Socket _socket;

    public Commands(IDataSender dataSender, Socket socket)
    {
        _dataSender = dataSender;
        _socket = socket;
    }
    
    public  void UptimeCommand()
    {
        var currentWorkingTime = DateTime.Now;
        var timeSpan = currentWorkingTime - ServerExecuter.ServerCreationTime;
        var message = $"Running time: {timeSpan}";

        var result = _dataSender.SendData(message);
        _socket.Send(result);
    }

    public  void InfoCommand()
    {
        var message = $"Sever version number: {ServerInformation.ServerVersion}\n" +
                      $"Server creation date: {ServerInformation.ServerCreationDate}\n";
        
        var result = _dataSender.SendData(message);
        _socket.Send(result);
    }

    public  void HelpCommand()
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
                      
        var result = _dataSender.SendData(message);
        _socket.Send(result);
    }

    public  void HelpCommandForAdmin()
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
                      
        var result = _dataSender.SendData(message);
        _socket.Send(result);
    }
    
    public  void StopCommand()
    {
        var message = "Shutting down...";
        
        var result = _dataSender.SendData(message);
        _socket.Send(result);
        
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    public  void WrongCommand()
    {
        var message = "Wrong command\n";
       
        var result = _dataSender.SendData(message);
        _socket.Send(result);
    }
}