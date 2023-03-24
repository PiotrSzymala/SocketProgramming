using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
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

                
                string commandToSend;
                do
                {
                    commandToSend = Console.ReadLine().ToLower();
                    var message = DataSender.SendData(commandToSend);
                    sender.Send(message);

                    var response = DataReceiver.GetData(sender);

                    Console.WriteLine($"Response from Server -> {response}");
                } while (commandToSend != "stop");
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