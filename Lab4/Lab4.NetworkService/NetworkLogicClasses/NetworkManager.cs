using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Google.Protobuf;
using Lab4.NetworkService.Wrappers;
using Snakes;

namespace Lab4.NetworkService.NetworkLogicClasses;

public class NetworkManager : IDisposable
{
    private readonly UdpClientWrapper _multicastListener;
    private readonly UdpClientWrapper _mainUdpClient;
    private readonly Channel<MessageWrapper> _channel;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private bool _isAlive = true;
    
    public NetworkManager(Channel<MessageWrapper> channel)
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

    public NetworkManager(Channel<MessageWrapper> channel, UdpClientWrapper mainUdpClient,
        UdpClientWrapper multicastListener)
    {
        _channel = channel;
        _multicastListener = mainUdpClient;
        _mainUdpClient = multicastListener;
        GetMessage();
        GetMulticastMessage();
    }

    public async Task SendMessage(GameMessage message, IPEndPoint endPoint)
    {
        await SendMessage(new MessageWrapper(message, endPoint));
    }
    
    public async Task SendMessage(MessageWrapper message)
    {
        await _semaphore.WaitAsync();
        try
        {
            await _mainUdpClient.SendAsync(message.Message.ToByteArray(), message.EndPoint);
        }
        catch(SocketException e){}
        catch (ObjectDisposedException e) {}
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task SendMulticastMessage(GameMessage message)
    {
        await _semaphore.WaitAsync();
        try
        {
            await _mainUdpClient.SendAsync(message.ToByteArray());
        }
        catch(SocketException e){}
        catch(ObjectDisposedException e){}
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task GetMessage()
    {
        while (_isAlive)
        {
            try
            { 
                var msg = await _mainUdpClient.ReceiveAsync(); 
                var parsedMsg = GameMessage.Parser.ParseFrom(msg.Buffer);
                await _channel.Writer.WriteAsync(new MessageWrapper(parsedMsg, msg.RemoteEndPoint));
            } 
            catch(SocketException e){}
            catch (ObjectDisposedException e) {}
        }
    }

    private async Task GetMulticastMessage()
    {
        while (_isAlive)
        {
            try
            { 
                var msg = await _multicastListener.ReceiveAsync();
                var parsedMsg = GameMessage.Parser.ParseFrom(msg.Buffer);
                await _channel.Writer.WriteAsync(new MessageWrapper(parsedMsg, msg.RemoteEndPoint));
            } 
            catch(SocketException e){}
            catch (ObjectDisposedException e) {}
        }
    }

    public void Dispose()
    {
        _isAlive = false;
        _multicastListener.Dispose();
        _mainUdpClient.Dispose();
    }
}