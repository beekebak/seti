using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class ListenerHandler : ISocketHandler
{
    public void Handle(SocketConnection connection, List<SocketConnection> connections)
    {
        Socket newSocket = connection.Client.Accept();
        newSocket.Blocking = false;
        connections.Add(new (new DefaultSocketWrapper(newSocket), new AuthorizationHandler()));
    }
}