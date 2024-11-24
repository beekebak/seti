using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Google.Protobuf;
using Snakes;

namespace Lab4.NetworkService;

public class NetworkManager : IDisposable
{
    private readonly UdpClientWrapper _multicastListener;
    private readonly UdpClientWrapper _mainUdpClient;
    private readonly Channel<GameMessage> _channel;
    public NetworkManager(Channel<GameMessage> channel)
    {
        _channel = channel;
        var multicastClient = new UdpClient();
        multicastClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        multicastClient.Client.Bind(new IPEndPoint(IPAddress.Any, 9192));
        multicastClient.JoinMulticastGroup(IPAddress.Parse("239.192.0.4"));
        _multicastListener = new UdpClientWrapper(multicastClient);
        _mainUdpClient = new UdpClientWrapper(new UdpClient());
        GetMessage();
        GetMulticastMessage();
    }

    public async Task SendMessage(GameMessage message)
    {
        await _mainUdpClient.SendAsync(message.ToByteArray());
    }

    public async Task GetMessage()
    {
        try
        {
            while (true)
            {
                var msg = await _mainUdpClient.ReceiveAsync();
                var parsedMsg = GameMessage.Parser.ParseFrom(msg.Buffer);
                _channel.Writer.WriteAsync(parsedMsg);
            }
        } catch (ObjectDisposedException e) {}
    }

    public async Task GetMulticastMessage()
    {
        try
        {
            while (true)
            {
                var msg = await _multicastListener.ReceiveAsync();
                var parsedMsg = GameMessage.Parser.ParseFrom(msg.Buffer);
                _channel.Writer.WriteAsync(parsedMsg);
            }
        } catch (ObjectDisposedException e) {}
    }

    public void Dispose()
    {
        _multicastListener.Dispose();
        _mainUdpClient.Dispose();
    }
}