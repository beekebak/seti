using System.Collections.Concurrent;

namespace Lab4.NetworkService;

public class MessageQueue : IDisposable
{
    ConcurrentDictionary<int, Snakes.GameMessage> sentMessages = new();
    NetworkManager _networkManager;

    public MessageQueue(NetworkManager networkManager)
    {
        _networkManager = networkManager;
    }
    
    public void RegisterMessage(int id, Snakes.GameMessage msg, int timeout)
    {
        sentMessages.TryAdd(id, msg);
        Task.Run(async () =>
        {
            await Task.Delay(timeout);
            ResendMessage(id, msg);
        });
    }

    public void DeregisterMessage(int id)
    {
        sentMessages.TryRemove(id, out _);
    }

    private void ResendMessage(int id, Snakes.GameMessage msg)
    {
        try
        {
            if (!sentMessages.ContainsKey(id)) return;
            _networkManager.SendMessage(msg);
        } catch(ObjectDisposedException){}
    }

    public void Dispose()
    {
        foreach (var message in sentMessages)
        {
            sentMessages.TryRemove(message.Key, out _);
        }
    }
}