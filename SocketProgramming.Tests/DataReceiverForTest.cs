using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Shared;
using Shared.Models;

namespace SocektProgramming.Tests;

public class DataReceiverForTest : IDataReceiver
{
    private  string[] _inputs;
    private int _counter = 0;

    public DataReceiverForTest(string name, string password, string specialPassword)
    {
        _inputs = new string[] { name, password, specialPassword };
    }
    
    public DataReceiverForTest(string name, string password)
    {
        _inputs = new string[] { name, password};
    }

    public string GetData()
    {
        if (_counter>=_inputs.Length)
        {
            _counter = 0;
        }
        
        return _inputs[_counter++];
    }
}