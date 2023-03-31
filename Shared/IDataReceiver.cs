using System.Net.Sockets;

namespace Shared;

public interface IDataReceiver
{
     string GetData(Socket socket);
}