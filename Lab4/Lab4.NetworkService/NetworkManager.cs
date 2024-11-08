using System.Net;
using System.Net.Sockets;

namespace Lab4.NetworkService;

public class NetworkManager : IDisposable
{
    private readonly UdpClient _multicastListener;
    private readonly UdpClient _mainUdpClient;

    public NetworkManager()
    {
        _multicastListener = new UdpClient();
        _multicastListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _multicastListener.Client.Bind(new IPEndPoint(IPAddress.Any, 9192));
        _multicastListener.JoinMulticastGroup(IPAddress.Parse("239.192.0.4"));
        
        _mainUdpClient = new UdpClient();
    }

    public void Dispose()
    {
        _multicastListener.Dispose();
        _mainUdpClient.Dispose();
    }
    
}