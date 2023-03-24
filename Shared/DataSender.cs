using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared;

public static class DataSender
{
    public static byte[] SendData(string messageContext)
    {
        var MessageToClient = new MyMessage();
        MessageToClient.Message = messageContext;
        var toSend = JsonConvert.SerializeObject(MessageToClient);
        var message = Encoding.ASCII.GetBytes(toSend);
        return message;
    }
}