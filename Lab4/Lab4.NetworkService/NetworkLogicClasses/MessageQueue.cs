using System.Collections.Concurrent;
using Lab4.NetworkService.Wrappers;

namespace Lab4.NetworkService.NetworkLogicClasses;
public class MessageQueue : IDisposable
{
    private ConcurrentDictionary<long, Timer> _sentMessages = new();
    private NetworkManager _networkManager;
    private int _timeout;

    public MessageQueue(NetworkManager networkManager, int timeout)
    {
        _networkManager = networkManager;
        _timeout = timeout;
    }
    
    public void DeregisterMessage(long id)
    {
        _sentMessages.TryRemove(id, out var timer);
        timer!.Dispose();
    }
    
    public void RegisterMessage(long id, MessageWrapper msg)
    {
        var timer = new Timer(ResendMessage, msg, 0, _timeout/10);
        _sentMessages.TryAdd(id, timer);
    }

    private void ResendMessage(object? msg)
    {
        _networkManager.SendMessage(msg as MessageWrapper);
    }

    public void Dispose()
    {
        foreach (var message in _sentMessages)
        {
            _sentMessages.TryRemove(message.Key, out var timer);
            timer?.Dispose();
        }
    }
}