using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public interface ISocketHandler
{
    public void Handle(ISocket socket, Dictionary<ISocket, ISocketHandler> selectableSockets, 
        Dictionary<ISocket, ISocket> clientToServerMap);
}