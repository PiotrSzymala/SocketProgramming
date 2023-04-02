using System.Net.Sockets;
using Newtonsoft.Json;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public class MessageBoxCleaner
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;
    public MessageBoxCleaner(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    
    public  void ClearInbox(User currentlyLoggedUser)
    {
        var dataSender = new DataSendHandler(_dataSender);
        var dataReceiver = new DataReceiveHandler(_dataReceiver);

        var message = dataSender.Send("All messages will be deleted. Are you sure? (y/n)");
       _socket.Send(message);

        var decision = dataReceiver.Receive(_socket);

        if (decision.ToLower() == "y")
        {
            currentlyLoggedUser.Inbox.Clear();
            
            using (StreamWriter file = File.CreateText($"users/{currentlyLoggedUser.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(currentlyLoggedUser);
                file.Write(result);
            }

            ListSaver.SaveList();
            
            message = dataSender.Send("All messages deleted.");
            _socket.Send(message);
        }
        else
        {
            message = dataSender.Send("Deleting canceled.");
            _socket.Send(message);
        }
    }
}