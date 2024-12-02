using System.Collections.Concurrent;
using Lab4.NetworkService.Players;
using Lab4.NetworkService.Wrappers;
using Snakes;

namespace Lab4.NetworkService.NetworkLogicClasses;

public class Pinger : IDisposable
{
    private Timer? _timer;
    private NetworkManager _networkManager;
    private MessageQueue _messageQueue;

    public Pinger(MessageQueue queue, NetworkManager manager)
    {
        _networkManager = manager;
        _messageQueue = queue;
    }
    
    public void RegisterPlayer(PlayerWrapper player, int timeout)
    {
        _timer?.Dispose();
        _timer = new Timer(Ping, player, 0, timeout/10);
    }

    private void Ping(object? playerObj)
    {
        PlayerWrapper? player = playerObj as PlayerWrapper;
        GameMessage msg = new GameMessage();
        msg.Ping = new GameMessage.Types.PingMsg();
        msg.MsgSeq = NetworkContext.GetNewMessageIndex();
        _networkManager.SendMessage(msg, player!.EndPoint!);
        _messageQueue.RegisterMessage(msg.MsgSeq, new MessageWrapper(msg, player.EndPoint!));
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
    }
}