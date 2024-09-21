using System.Net;
using System.Net.Sockets;

namespace Lab5.SocketWrapper;

public interface ISocket
{
    void Bind(IPEndPoint localEP);
    void Listen(int backlog);
    Socket Accept();
    byte[] Receive(byte[] buffer);
    void Send(byte[] data);
    void Close();
    bool Connected();
}