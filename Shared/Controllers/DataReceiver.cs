using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared.Controllers;

public class DataReceiver : IDataReceiver
    {
        public string GetData(Socket socket)
        {
            var bytesToReceive = new byte[1024];
            var numByte = socket.Receive(bytesToReceive);

            var receivedString = Encoding.ASCII.GetString(bytesToReceive, 0, numByte);
            var clientServerResponse = JsonConvert.DeserializeObject<CommandFromUser>(receivedString);

            var receivedCommand = clientServerResponse.Command;
            return receivedCommand;
        }
    }
