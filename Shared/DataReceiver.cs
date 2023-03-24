using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared.Models;

namespace Shared;


    public static class DataReceiver
    {
        public static string GetData(Socket socket)
        {
            var bytesToReceive = new byte[1024];
            var numByte = socket.Receive(bytesToReceive);

            var receivedString = Encoding.ASCII.GetString(bytesToReceive, 0, numByte);
            var MessageToClient = JsonConvert.DeserializeObject<MyMessage>(receivedString);

            var result = MessageToClient.Message;
            return result;
        }
    }
