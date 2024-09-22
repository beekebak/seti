using System.Net.Sockets;

namespace Lab5.SocketWrapper;

public interface ISocket : IDisposable
{
    Socket Accept();
    byte[] Receive(byte[] buffer);
    void Send(byte[] data);
    void Close();
    bool Connected();
    Socket GetSocket();
    bool Equals(ISocket? socket);
}