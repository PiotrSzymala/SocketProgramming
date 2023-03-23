using System.Net.Sockets;
using System.Text;
using Shared;
using Shared.Models;

namespace Client;

public static class ClientExecuter
{
    public static void ExecuteClient()
    {
        try
        {
            using Socket sender = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(Config.LocalEndPoint);

                Console.WriteLine($"Socket connected to -> {sender.RemoteEndPoint}");

                byte[] initialCommand = new byte[1024];

                int initComm = sender.Receive(initialCommand);

                string encodingInitComm = Encoding.ASCII.GetString(initialCommand, 0, initComm);
                Console.WriteLine(encodingInitComm);

                MyMessage commandToSend = new MyMessage();

                do
                {
                    commandToSend.Message = Console.ReadLine().ToLower();

                    var serializedCommand = JsonSerializer.Serialize(commandToSend);

                    byte[] commandInJsonFormat = Encoding.ASCII.GetBytes(serializedCommand);

                    sender.Send(commandInJsonFormat);

                    byte[] messageReceived = new byte[1024];

                    int bytesReceived = sender.Receive(messageReceived);

                    string fromServerResult = Encoding.ASCII.GetString(messageReceived,
                        0, bytesReceived);

                    var responseFromServer = JsonDeserializer.Deserialize(fromServerResult);

                    Console.WriteLine($"Response from Server -> {responseFromServer.Message}");
                    
                } while (commandToSend.Message != "stop");

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }

            catch (ArgumentNullException argumentNullException)
            {
                Console.WriteLine($"ArgumentNullException: {argumentNullException}");
            }

            catch (SocketException socketException)
            {
                Console.WriteLine($"SocketException: {socketException}");
            }

            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception}");
            }
        }

        catch (Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}