using System.Net;
using System.Threading.Channels;
using Lab4.NetworkService.NetworkLogicClasses;
using Lab4.NetworkService.Wrappers;
using Snakes;

namespace Lab4Tests.NetworkLogicClassesTests;

public class NetworkManagerTests
{
    [Fact]
    public async void NetworkManager_DoesntThrowExceptionEvenIfDisposed_Test()
    {
        var manager = new NetworkManager(Channel.CreateUnbounded<MessageWrapper>());
        manager.Dispose();

        var ex = await Record.ExceptionAsync(() =>
            manager.SendMessage(new MessageWrapper(new GameMessage(), new IPEndPoint(0x0f000001, 80))));
        var exMult = await Record.ExceptionAsync(() =>
            manager.SendMulticastMessage(new GameMessage()));
        Assert.Null(ex);
        Assert.Null(exMult);
    }

    private NetworkManager CreateNetworkManagerWithMocks(Mock<UdpClientWrapper> main, Mock<UdpClientWrapper> multicast)
    {
        return new NetworkManager(Channel.CreateUnbounded<MessageWrapper>(), main.Object, multicast.Object);
    }
    
}