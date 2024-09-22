using Lab5.SocketHandlers;
using Lab5.SocketWrapper;

namespace Lab5Tests.HandlersTests;

public class AuthorizationHandlerTests
{
    [Fact]
    public void AcceptsConnectionWithNoAuthentication()
    {
        ISocketHandler handler = new AuthorizationHandler();
        var socketMock = new Mock<ISocket>();
        byte[] data = [0x05, 0x01, 0x00];
        socketMock.Setup(socket => socket.Receive(It.IsAny<byte[]>())).Returns(data);
        handler.Handle(socketMock.Object, new Dictionary<ISocket, ISocketHandler>(), 
            new Dictionary<ISocket, ISocket>());
        byte[] expectedAnswer = [0x05, 0x00];
        socketMock.Verify(socket => socket.Send(expectedAnswer), Times.Once);
    }

    [Fact]
    public void RejectConnectionWithAuthentication()
    {
        ISocketHandler handler = new AuthorizationHandler();
        var socketMock = new Mock<ISocket>();
        byte[] data = [0x05, 0x01, 0x01];
        socketMock.Setup(socket => socket.Receive(It.IsAny<byte[]>())).Returns(data);
        handler.Handle(socketMock.Object, new Dictionary<ISocket, ISocketHandler>(),
            new Dictionary<ISocket, ISocket>());
        byte[] expectedAnswer = [0x05, 0xFF];
        socketMock.Verify(socket => socket.Send(expectedAnswer), Times.Once);
    }
}