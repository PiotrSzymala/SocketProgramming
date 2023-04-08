using System.Net.Sockets;
using Newtonsoft.Json;
using Server.Interfaces;
using Shared;

using Shared.Models;

namespace Server.Controllers.MessageHandlingControllers;

public class MessageBoxCleaner : IMessageBoxCleaner
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    public bool CleaningSuccess;
    public MessageBoxCleaner(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
    }
    
    public void ClearInbox(User currentlyLoggedUser)
    {
        var message = _dataSender.SendData("All messages will be deleted. Are you sure? (y/n)");
       _transferStructure.Send(message);

        var decision = _dataReceiver.GetData();

        if (decision.ToLower() == "y")
        {
            currentlyLoggedUser.Inbox.Clear();
            
            using (StreamWriter file = File.CreateText($"/Users/piotrszymala/RiderProjects/SocketProgramming/Server/bin/Debug/net7.0/users/{currentlyLoggedUser.Username}.json"))
            {
                var result = JsonConvert.SerializeObject(currentlyLoggedUser);
                file.Write(result);
            }

            ListSaver.SaveList();
            
            message = _dataSender.SendData("All messages deleted.");
            _transferStructure.Send(message);

            CleaningSuccess = true;
        }
        else
        {
            message = _dataSender.SendData("Deleting canceled.");
            _transferStructure.Send(message);

            CleaningSuccess = false;
        }
    }
}