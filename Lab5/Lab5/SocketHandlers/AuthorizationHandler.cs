using System.Net.Sockets;
using Lab5.SocketWrapper;

namespace Lab5.SocketHandlers;

public class AuthorizationHandler : ISocketHandler
{
    public void Handle(ISocket socket, Dictionary<ISocket, ISocketHandler> selectableSockets)
    {
        byte[] buffer = new byte[258];
        buffer = socket.Receive(buffer);
        if (buffer[0] != 0x05)
        {
            socket.Close();
            return;
        }
        int length = buffer[1];
        for (int i = 0; i < length; i++)
        {
            if(buffer[i+2] != 0x00) continue;
            byte[] answer = { 0x05, 0x00 };
            socket.Send(answer);
            return;
        }
        socket.Close();
    }
}