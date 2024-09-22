using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class ListenerHandler : ISocketHandler
{
    public void Handle(ISocket socket, Dictionary<ISocket, ISocketHandler> selectableSockets, 
        Dictionary<ISocket, ISocket> clientToServerMap)
    {
        Socket newSocket = socket.Accept();
        newSocket.Blocking = false;
        selectableSockets.Add(new DefaultSocketWrapper(newSocket), new AuthorizationHandler());
    }
}