using System.Net.Sockets;

namespace Lab5.SocketWrapper;

public class DefaultSocketWrapper(Socket socket) : ISocket
{
    public bool Connected()
    {
        return socket.Connected;
    }

    public Socket Accept()
    {
        return socket.Accept();
    }

    public byte[] Receive(byte[] buffer)
    {
        int bytesRead = socket.Receive(buffer);
        return buffer.Take(bytesRead).ToArray();
    }

    public void Send(byte[] data)
    {
        socket.Send(data);
    }

    public void Close()
    {
        socket.Close();
    }

    public void Dispose()
    {
        socket.Dispose();
    }

    public Socket GetSocket()
    {
        return socket;
    }

    public bool Equals(ISocket? other)
    {
        if (other is DefaultSocketWrapper wrapper)
        {
            return other.GetSocket().Equals(wrapper.GetSocket());
        }
        return false;
    }
}