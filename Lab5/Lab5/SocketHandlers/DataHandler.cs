using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class DataHandler : ISocketHandler
{
    public void Handle(SocketConnection connection, List<SocketConnection> connections)
    {
        byte[] buffer;
        if (connection.Client.Available() > 0)
        {
            buffer = new byte[4096];
            ISocket fromSocket = connection.Client;
            ISocket toSocket = connection.Server!;
            while ((buffer = fromSocket.Receive(buffer)).Length > 0) 
            {
                toSocket.Send(buffer);
            }
        }
        if (connection.Server!.Available() > 0)
        {
            buffer = new byte[4096];
            ISocket fromSocket = connection.Server!;
            ISocket toSocket = connection.Client;
            while ((buffer = fromSocket.Receive(buffer)).Length > 0 )
            {
                toSocket.Send(buffer);
            }
        }
    }
}