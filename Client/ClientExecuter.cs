using System.Net.Sockets;
using Shared;
using Shared.Controllers;

namespace Client;

public  class ClientExecuter
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private Socket _socket;

    public ClientExecuter(IDataSender dataSender, IDataReceiver dataReceiver, Socket socket)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _socket = socket;
    }
    public void ExecuteClient()
    {
        try
        {
                // Socket sender = new Socket(Config.IpAddr.AddressFamily,
                // SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _socket.Connect(Config.LocalEndPoint);

                Console.WriteLine($"Socket connected to -> {_socket.RemoteEndPoint}");
                
                var firstResponseFromServer = _dataReceiver.GetData(_socket);
                Console.WriteLine(firstResponseFromServer);

                string response;
                
                do
                {
                    var commandToSend = Console.ReadLine().ToLower();
                    var message = _dataSender.SendData(commandToSend);
                    _socket.Send(message);

                     response = _dataReceiver.GetData(_socket);

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