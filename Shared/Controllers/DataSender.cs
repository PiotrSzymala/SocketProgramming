using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared.Controllers;

public class DataSender : IDataSender
{
    public byte[] SendData(string messageContext)
    {
        var clientServerCommand = new CommandFromUser();
        clientServerCommand.Command = messageContext;
        var serializedCommand = JsonConvert.SerializeObject(clientServerCommand);
        var dataToSend = Encoding.ASCII.GetBytes(serializedCommand);
        return dataToSend;
    }
}