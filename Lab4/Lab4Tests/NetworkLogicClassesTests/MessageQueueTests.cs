using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Google.Protobuf;
using Lab4.NetworkService.NetworkLogicClasses;
using Lab4.NetworkService.Wrappers;
using Snakes;

namespace Lab4Tests.NetworkLogicClassesTests;

public class MessageQueueTests
{
    [Fact]
    public void MessageQueue_ResendsMessage_Test()
    {
        var msg = new MessageWrapper(new GameMessage(), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5050));
        var client = new Mock<UdpClientWrapper>();
        var manager = new NetworkManager(Channel.CreateUnbounded<MessageWrapper>(), client.Object,
            new Mock<UdpClientWrapper>().Object);
        var queue = new MessageQueue(manager, 100);
        
        queue.RegisterMessage(1, msg);
        Task.Delay(10).Wait();
        
        client.Verify(wrapper => wrapper.SendAsync(It.Is<Byte[]>(data => data == msg.Message.ToByteArray())));
    }
}