using System.Net;
using System.Net.Sockets;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5;

public class SocksServer : IServer
{
    private void ClearSockets(List<SocketConnection> selectableSockets)
    {
        List<SocketConnection> socketsToClear = new List<SocketConnection>();
        foreach (var socketConnection in selectableSockets)
        {
            if(socketConnection.Handler is ListenerHandler) continue;
            bool serverConnected = socketConnection.Server?.Connected() ?? true;
            if(!socketConnection.Client.Connected() || !serverConnected) socketsToClear.Add(socketConnection);
        }

        foreach (var socket in socketsToClear)
        {
            socket.Dispose();
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
        List<SocketConnection> selectableSockets = new List<SocketConnection>
        { new (new DefaultSocketWrapper(tcpListener), new ListenerHandler()) };
        while (true)
        {
            try
            {
                List<Socket> sockets = selectableSockets.ConvertAll(connection => connection.Client)
                    .ConvertAll(socket => socket.GetSocket()).
                    Concat(selectableSockets.Where(connection => connection.Server != null).
                        Select(socket => socket.Server!.GetSocket())).ToList();
                Socket.Select(sockets, null, null, 1000);
                sockets.ForEach(selectedSocket =>
                {
                    SocketConnection? wrappedConnection = selectableSockets.Find(connection =>
                        selectedSocket == connection.Client.GetSocket() ||
                        selectedSocket == connection.Server?.GetSocket());
                    if(wrappedConnection?.Handler is ListenerHandler || wrappedConnection?.Client.Available() > 0 ||
                       wrappedConnection?.Server?.Available() > 0)
                    wrappedConnection.Handler?.Handle(wrappedConnection, selectableSockets);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            ClearSockets(selectableSockets);
        }
    }
}