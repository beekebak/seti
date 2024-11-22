using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Snakes;

namespace Lab4.NetworkService;

public class NetworkManager : IDisposable
{
    private readonly UdpClientWrapper _multicastListener;
    private readonly UdpClientWrapper _mainUdpClient;

    public NetworkManager()
    {
        var multicastClient = new UdpClient();
        multicastClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        multicastClient.Client.Bind(new IPEndPoint(IPAddress.Any, 9192));
        multicastClient.JoinMulticastGroup(IPAddress.Parse("239.192.0.4"));
        _multicastListener = new UdpClientWrapper(multicastClient);
        
        _mainUdpClient = new UdpClientWrapper(new UdpClient());
    }

    public async Task SendMessage(GameMessage message)
    {
        await _mainUdpClient.SendAsync(message.ToByteArray());
    }

    public async Task<GameMessage> GetMessage()
    {
        var msg =  await _mainUdpClient.ReceiveAsync();
        return GameMessage.Parser.ParseFrom(msg.Buffer);
    }

    public async Task<GameMessage> GetMulticastMessage()
    {
        var msg =  await _multicastListener.ReceiveAsync();
        return GameMessage.Parser.ParseFrom(msg.Buffer);
    }

    public void Dispose()
    {
        _multicastListener.Dispose();
        _mainUdpClient.Dispose();
    }
}