using System.Net.Sockets;
using Shared;
using Shared.Controllers;

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
                
                var firstResponseFromServer = DataReceiver.GetData(sender);
                Console.WriteLine(firstResponseFromServer);

                string response;
                
                do
                {
                    var commandToSend = Console.ReadLine().ToLower();
                    var message = DataSender.SendData(commandToSend);
                    sender.Send(message);

                     response = DataReceiver.GetData(sender);

                    Console.WriteLine($"Response from Server -> {response}");
                } while (response != "Shutting down...");
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