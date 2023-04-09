using System.Net.Sockets;
using Shared;

namespace Client;

public  class ClientExecuter : IClientExecuter
{
    private IDataSender _dataSender;
    private IDataReceiver _dataReceiver;
    private ITransferStructure _transferStructure;
    private ILogger _logger;

    public ClientExecuter(IDataSender dataSender, IDataReceiver dataReceiver, ITransferStructure transferStructure, ILogger logger)
    {
        _dataSender = dataSender;
        _dataReceiver = dataReceiver;
        _transferStructure = transferStructure;
        _logger = logger;
    }
    public void ExecuteClient()
    {
        
            try
            {
                Console.WriteLine($"Connected");
                
                var firstResponseFromServer = _dataReceiver.GetData();
                Console.WriteLine(firstResponseFromServer);

                string response;
                
                do
                {
                    var commandToSend = Console.ReadLine().ToLower();
                    var message = _dataSender.SendData(commandToSend);
                    _transferStructure.Send(message);

                     response = _dataReceiver.GetData();

                    Console.WriteLine($"Response from Server -> {response}");
                } while (response != "Shutting down...");
            }

            catch (ArgumentNullException argumentNullException)
            {
                _logger.WriteError(argumentNullException);
                Console.WriteLine($"ArgumentNullException: {argumentNullException}");
            }

            catch (SocketException socketException)
            {
                _logger.WriteError(socketException);
                Console.WriteLine($"SocketException: {socketException}");
            }

            catch (Exception exception)
            {
                _logger.WriteError(exception);
                Console.WriteLine($"Exception: {exception}");
            }
    }
}