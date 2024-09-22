using System.Net;
using System.Net.Sockets;
using System.Text;
using Lab5;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5Tests.HandlersTests;

public class ConnectionHandlerTests
{
    [Fact]
    public void AcceptsIPv4TcpConnection()
    {
        ISocketHandler handler = new ConnectionHandler();
        var socketMock = new Mock<ISocket>();
        byte[] data = [0x05, 0x01, 0x00, 0x01, 0x7F, 0x00, 0x00, 0x01, 0x06, 0x13];
        socketMock.Setup(socket => socket.Receive(It.IsAny<byte[]>())).Returns(data);
        Socket underlyingSocketStub = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        underlyingSocketStub.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"),
            5050));
        socketMock.Setup(socket => socket.GetSocket()).Returns(underlyingSocketStub);
        handler.Handle(socketMock.Object, new Dictionary<ISocket, ISocketHandler>(), 
            new Dictionary<ISocket, ISocket>());
        socketMock.Verify(socket => socket.Send(It.Is<Byte[]>(data => data[1] == 0x00 ||
                                                                      data[1] == 0x04)), Times.Once);
    }

    [Fact]
    public void AcceptsDnsConnection()
    {
        ISocketHandler handler = new ConnectionHandler();
        var socketMock = new Mock<ISocket>();
        byte[] data = Utility.Concatinate([0x05, 0x01, 0x00, 0x03, 0x0A], Encoding.Default.GetBytes("google.com"));
        socketMock.Setup(socket => socket.Receive(It.IsAny<byte[]>())).Returns(data);
        Socket underlyingSocketStub = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        underlyingSocketStub.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"),
            5050));
        socketMock.Setup(socket => socket.GetSocket()).Returns(underlyingSocketStub);
        handler.Handle(socketMock.Object, new Dictionary<ISocket, ISocketHandler>(), 
            new Dictionary<ISocket, ISocket>());
        socketMock.Verify(socket => socket.Send(It.Is<Byte[]>(data => data[1] == 0x00 ||
                                                                      data[1] == 0x04)), Times.Once);
    }
}