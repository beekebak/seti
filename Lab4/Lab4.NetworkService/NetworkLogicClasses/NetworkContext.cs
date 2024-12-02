using System.Collections;
using System.Threading.Channels;
using Lab4.NetworkService.Players;
using Lab4.NetworkService.Wrappers;
using Snakes;

namespace Lab4.NetworkService.NetworkLogicClasses;

public class NetworkContext : IDisposable
{
    private static int _messageIdCounter = 0;
    public int Timeout { get; set; }
    public NetworkManager Manager { get; }
    public MessageQueue MessageQueue { get; }
    public Channel<MessageWrapper> MsgChannel { get; }
    
    public NetworkContext(int timeout = 1000)
    {
        Timeout = timeout;
        MsgChannel = Channel.CreateUnbounded<MessageWrapper>();
        Manager = new NetworkManager(MsgChannel);
        MessageQueue = new MessageQueue(Manager, timeout);
    }
    
    public static int GetNewMessageIndex()
    {
        _messageIdCounter++;
        return _messageIdCounter;
    }

    public async Task<MessageWrapper> ReadMessage()
    {
        return await MsgChannel.Reader.ReadAsync();
    }

    public void SendMessage(MessageWrapper message)
    {
        Manager.SendMessage(message);
        MessageQueue.RegisterMessage(Timeout, message);
    }
    
    public async Task SendMessageAsync(MessageWrapper message)
    {
        await Manager.SendMessage(message);
        MessageQueue.RegisterMessage(Timeout, message);
    }

    public void SendMulticastMessage(GameMessage message)
    {
        Manager.SendMulticastMessage(message);
    }
    
    public void Dispose()
    {
        _messageIdCounter = 0;
        MessageQueue.Dispose();
        Manager.Dispose();
    }
}