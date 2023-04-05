using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;
using Shared.Handlers;
using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public class MessageBoxCleaner : IMessageBoxCleaner
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
    
    public void ClearInbox(User currentlyLoggedUser)
    {
        var message = _dataSender.SendData("All messages will be deleted. Are you sure? (y/n)");
       _socket.Send(message);

        var decision = _dataReceiver.GetData(_socket);

        if (decision.ToLower() == "y")
        {
            currentlyLoggedUser.Inbox.Clear();
            
            using (StreamWriter file = File.CreateText($"users/{currentlyLoggedUser.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(currentlyLoggedUser);
                file.Write(result);
            }

            ListSaver.SaveList();
            
            message = _dataSender.SendData("All messages deleted.");
            _socket.Send(message);
        }
        else
        {
            message = _dataSender.SendData("Deleting canceled.");
            _socket.Send(message);
        }
    }
}