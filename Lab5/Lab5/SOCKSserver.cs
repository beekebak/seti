using System.Net;
using System.Net.Sockets;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5;

public class SocksServer : IServer
{
    private void ClearSockets(Dictionary<ISocket, ISocketHandler> selectableSockets)
    {
        List<ISocket> socketsToClear = new List<ISocket>();
        foreach (var socket in selectableSockets.Keys)
        {
            if(!socket.Connected() && selectableSockets[socket] is not ListenerHandler) socketsToClear.Add(socket);
        }

        foreach (var socket in socketsToClear)
        {
            selectableSockets.Remove(socket);
        }
    }

    private void ConfigureSocket(Socket socket)
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1080); 
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        socket.Blocking = false;
        socket.Bind(endPoint);
        socket.Listen(10);
    }

    public void Run()
    {
        using Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ConfigureSocket(tcpListener);
        Dictionary<ISocket, ISocketHandler> selectableSockets = new Dictionary<ISocket, ISocketHandler>{
            {new DefaultSocketWrapper(tcpListener), new ListenerHandler()}};
        Dictionary<ISocket, ISocket> clientToServerSocketsMap = new Dictionary<ISocket, ISocket>();
        while (true)
        {
            List<Socket> sockets = selectableSockets.Keys.ToList().ConvertAll(socket => socket.GetSocket());
            Socket.Select(sockets, null, null, 1000);
            sockets.ForEach(selectedSocket =>
            {
                ISocket wrappedSocket = selectableSockets.Keys.First(examinedSocket =>
                    examinedSocket.GetSocket() == selectedSocket);
                selectableSockets[wrappedSocket].Handle(wrappedSocket, selectableSockets, clientToServerSocketsMap);
            });
            ClearSockets(selectableSockets);
        }
    }
}