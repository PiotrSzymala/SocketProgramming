using System.Net;

namespace Shared;

public static class Config
{
    private static IPHostEntry IpHost { get; } = Dns.GetHostEntry(Dns.GetHostName());
    public static IPAddress IpAddr { get; } = IpHost.AddressList[0];
    public static IPEndPoint LocalEndPoint { get; } = new(IpAddr, 11111);
    
}