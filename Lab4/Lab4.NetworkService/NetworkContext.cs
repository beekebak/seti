using System.Threading.Channels;
using Snakes;

namespace Lab4.NetworkService;

public class NetworkContext : IDisposable
{
    private static int _playerIdCounter = 0;
    private static int _messageIdCounter = 0;
    private readonly NetworkManager _manager;
    private readonly MessageQueue _messageQueue;
    private readonly Channel<GameMessage> _channel;
    private readonly PlayersTimeoutManager _playersTimeoutManager;

    public NetworkContext()
    {
        _channel = Channel.CreateUnbounded<GameMessage>();
        _manager = new NetworkManager(_channel);
        _messageQueue = new MessageQueue(_manager);
        _playersTimeoutManager = new PlayersTimeoutManager(_manager, null);
    }
    
    public static int GetNewPlayerIndex()
    {
        _playerIdCounter++;
        return _playerIdCounter;
    }
    
    public static int GetNewMessageIndex()
    {
        _messageIdCounter++;
        return _messageIdCounter;
    }

    public void UpdateNetworkContext()
    {
    }
    
    public void Dispose()
    {
        _manager.Dispose();
        _messageQueue.Dispose();
    }
}