using System.Net.Sockets;
using Lab5;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5Tests.HandlersTests;

public class DataHandlerTests
{
    private (Mock<ISocket>, SocketConnection, Mock<ISocket>) SetupConnection()
    {
        Mock<ISocket> client = new Mock<ISocket>();
        Socket underlyingClientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        client.Setup(socket => socket.GetSocket()).Returns(underlyingClientSocket);
        Mock<ISocket> server = new Mock<ISocket>();
        Socket underlyingServerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        client.Setup(socket => socket.GetSocket()).Returns(underlyingServerSocket);
        SocketConnection connection = new SocketConnection(client.Object, new DataHandler());
        connection.Server = server.Object;
        return (client, connection, server);
    }
    
    [Fact]
    public void SendsDataFromClientToServerTest()
    {
        (Mock<ISocket> clientStub, var connection, Mock<ISocket> serverMock) = SetupConnection();
        byte[] data = "TEST VALUE"u8.ToArray();
        clientStub.Setup(socket => socket.Available()).Returns(data.Length);
        serverMock.Setup(socket => socket.Available()).Returns(0);
        clientStub.SetupSequence(socket => socket.Receive(It.IsAny<Byte[]>())).Returns(data).Returns([]);
        connection.Handler.Handle(connection, new List<SocketConnection>());
        serverMock.Verify(socket => socket.Send(data), Times.Once);
    }

    [Fact]
    public void SendsDataFromServerToClientTest()
    {
        (Mock<ISocket> clientMock, var connection, Mock<ISocket> serverStub) = SetupConnection();
        byte[] data = "TEST VALUE"u8.ToArray();
        serverStub.Setup(socket => socket.Available()).Returns(data.Length);
        clientMock.Setup(socket => socket.Available()).Returns(0);
        serverStub.SetupSequence(socket => socket.Receive(It.IsAny<Byte[]>())).Returns(data).Returns([]);
        connection.Handler.Handle(connection, new List<SocketConnection>());
        clientMock.Verify(socket => socket.Send(data), Times.Once);
    }

    [Fact]
    public void SendsDataBothSidesTest()
    {
        (Mock<ISocket> clientMock, var connection, Mock<ISocket> serverMock) = SetupConnection();
        byte[] data = "TEST VALUE"u8.ToArray();
        serverMock.Setup(socket => socket.Available()).Returns(data.Length);
        clientMock.Setup(socket => socket.Available()).Returns(data.Length);
        serverMock.SetupSequence(socket => socket.Receive(It.IsAny<Byte[]>())).Returns(data).Returns([]);
        clientMock.SetupSequence(socket => socket.Receive(It.IsAny<Byte[]>())).Returns(data).Returns([]);
        connection.Handler.Handle(connection, new List<SocketConnection>());
        clientMock.Verify(socket => socket.Send(data), Times.Once);
        serverMock.Verify(socket => socket.Send(data), Times.Once);
    }
}