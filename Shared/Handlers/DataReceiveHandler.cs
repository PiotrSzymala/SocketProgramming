using System.Net.Sockets;

namespace Shared.Handlers;

public class DataReceiveHandler
{
    private IDataReceiver _dataReceiver;

    public DataReceiveHandler(IDataReceiver dataReceiver)
    {
        _dataReceiver = dataReceiver;
    }

    public string Receive(Socket socket)
    {
       return _dataReceiver.GetData(socket);
    }
}