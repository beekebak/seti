using System.Net.Sockets;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5;

public class SocketConnection : IDisposable
{
    public ISocket Client { get; set; }
    public ISocket? Server { get; set; }
    public ISocketHandler Handler { get; set; }

    public SocketConnection(ISocket client, ISocketHandler handler)
    {
        Client = client;
        Handler = handler;
    }

    public void Dispose()
    {
        Client.Dispose();
        Server?.Dispose();
    }
}