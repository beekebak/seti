using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class AuthorizationHandler : ISocketHandler
{
    public void Handle(SocketConnection connection, List<SocketConnection> connections)
    {
        ISocket socket = connection.Client;
        byte[] buffer = new byte[258];
        try
        {
            buffer = socket.Receive(buffer);
        }
        catch (SocketException e)
        {
            Console.WriteLine(e.Message);
        }

        if (buffer[0] != 0x05)
        {
            socket.Close();
            return;
        }
        int length = buffer[1];
        for (int i = 0; i < length; i++)
        {
            if(buffer[i+2] != 0x00) continue;
            socket.Send([0x05, 0x00]);
            connection.Handler = new ConnectionHandler();
            return;
        }
        connection.Handler = new ConnectionHandler();
        socket.Send([0x05, 0xFF]);
    }
}