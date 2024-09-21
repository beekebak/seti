using System.Net;
using System.Net.Sockets;
using Lab5;
using Lab5.SocketHandlers;
using Lab5.SocketWrapper;
using Moq;

namespace Lab5Tests;

public class Tests
{
    [Fact]
    public void AcceptsConnectionWithNoAuthentication()
    {
        ISocketHandler handler = new AuthorizationHandler();
        var socketMock = new Mock<ISocket>();
        byte[] data = { 0x05, 0x01, 0x00 };
        socketMock.Setup(socket => socket.Receive(It.IsAny<byte[]>())).Returns(data);
        handler.Handle(socketMock.Object, new Dictionary<ISocket, ISocketHandler>());
        byte[] expectedAnswer = { 0x05, 0x00 };
        socketMock.Verify(socket => socket.Send(expectedAnswer), Times.Once);
    }
}