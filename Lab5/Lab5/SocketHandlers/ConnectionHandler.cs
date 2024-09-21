using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class ConnectionHandler : ISocketHandler
{
    public void Handle(ISocket socket, Dictionary<ISocket, ISocketHandler> selectableSockets)
    {
        
    }
}