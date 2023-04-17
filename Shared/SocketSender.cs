using System.Net.Sockets;
using Shared.Models;

namespace Shared;

public class SocketSender : ITransferStructure, IDisposeStructure
{
    public Socket _socket;
    public SocketSender(Socket socket)
    {
        _socket = socket;
    }

    public void Send(byte[] data)
    {
        _socket.Send(data);
    }

    public int Receive(byte[] data)
    {
        return _socket.Receive(data);
    }

    public void Shutdown()
    {
        _socket.Shutdown(SocketShutdown.Both);
    }

    public void Close()
    {
        _socket.Close();
    }
}