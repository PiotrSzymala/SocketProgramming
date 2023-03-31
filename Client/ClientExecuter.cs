using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Client;

public  class ClientExecuter
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _sender;

    public ClientExecuter(IDataSender dataSender, IDataReceiver dataReceiver, Socket sender)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _sender = sender;
    }
    public void ExecuteClient()
    {
        try
        {
                _sender = new Socket(Config.IpAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp); //Going to be moved.

            try
            {
                _sender.Connect(Config.LocalEndPoint);

                Console.WriteLine($"Socket connected to -> {_sender.RemoteEndPoint}");
                
                var firstResponseFromServer = _dataReceiver.GetData(_sender);
                Console.WriteLine(firstResponseFromServer);

                string response;
                
                do
                {
                    var commandToSend = Console.ReadLine().ToLower();
                    var message = _dataSender.SendData(commandToSend);
                    _sender.Send(message);

                     response = _dataReceiver.GetData(_sender);

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