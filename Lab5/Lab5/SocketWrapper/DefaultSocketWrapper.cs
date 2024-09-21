using System.Net;
using System.Net.Sockets;

namespace Lab5.SocketWrapper;

public class DefaultSocketWrapper : ISocket
{
    private Socket _socket;

    public bool Connected()
    {
        return _socket.Connected;
    }

    public DefaultSocketWrapper(Socket socket)
    {
        _socket = socket;
    }

    public void Bind(IPEndPoint localEp)
    {
        _socket.Bind(localEp);
    }

    public void Listen(int backlog)
    {
        _socket.Listen(backlog);
    }

    public Socket Accept()
    {
        return _socket.Accept();
    }

    public byte[] Receive(byte[] buffer)
    {
        int bytesRead = _socket.Receive(buffer);
        return buffer.Take(bytesRead).ToArray();
    }

    public void Send(byte[] data)
    {
        _socket.Send(data);
    }

    public void Close()
    {
        _socket.Close();
    }
}