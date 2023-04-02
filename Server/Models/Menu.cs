using System.Net.Sockets;
using Shared;
using Shared.Models;

namespace Server.Models;

public  class Menu
{
     protected Socket _socket;
     protected IDataReceiver _dataReceiver;
     protected IDataSender _dataSender;
     protected User _user;

     protected Menu(Socket socket, IDataReceiver dataReceiver, IDataSender dataSender, User user)
    {
        _socket = socket;
        _dataReceiver = dataReceiver;
        _dataSender = dataSender;
        _user = user;
    }
    
    public virtual void DisplayMenu( ref bool flag, ref bool logged)
    {
        
    }
}