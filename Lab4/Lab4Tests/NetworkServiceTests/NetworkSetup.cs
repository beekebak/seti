using Lab4.NetworkService;

namespace Lab4Tests.NetworkServiceTests;

public class NetworkSetup
{
    public static (NetworkContext, NetworkContext) Setup()
    {
        var master = new NetworkContext(new NetworkManager());
        var player = new NetworkContext(new NetworkManager());
        return (master, player);
    }
}