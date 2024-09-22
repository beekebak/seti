using System.Net.Sockets;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5;

public class SocketConnection
{
    private ISocket Client { get; set; }
    private ISocket Server { get; set; }
    private ISocketHandler Handler { get; set; }
}