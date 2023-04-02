using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Client;

public  class ClientExecuter
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;

    public ClientExecuter(IDataSender dataSender, IDataReceiver dataReceiver)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
    }
    public void ExecuteClient()
    {
        try
        {
                Socket sender = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(Config.LocalEndPoint);

                Console.WriteLine($"Socket connected to -> {sender.RemoteEndPoint}");
                
                var firstResponseFromServer = _dataReceiver.GetData(sender);
                Console.WriteLine(firstResponseFromServer);

                string response;
                
                do
                {
                    var commandToSend = Console.ReadLine().ToLower();
                    var message = _dataSender.SendData(commandToSend);
                    sender.Send(message);

                     response = _dataReceiver.GetData(sender);

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