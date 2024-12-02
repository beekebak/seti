using System.Net;
using Lab4.NetworkService.NetworkLogicClasses;
using Lab4.NetworkService.Players;

namespace Lab4.NetworkService.Wrappers;

public class PlayerWrapper : IDisposable
{
    public Player Player { get; private set; }
    public IPEndPoint? EndPoint { get; set; }
    private Pinger? _pinger;
    private PlayersTimeoutManager? _playersTimeoutManager;

    public PlayerWrapper(Player player, IPEndPoint? endPoint = null)
    {
        Player = player;
        EndPoint = endPoint;
    }
    
    public void SetActive(PlayersContext playersContext, NetworkContext networkContext)
    {
        _pinger ??= new Pinger(networkContext.MessageQueue, networkContext.Manager);
        _playersTimeoutManager ??= new PlayersTimeoutManager(playersContext);
        _pinger.RegisterPlayer(this, networkContext.Timeout);
        _playersTimeoutManager.RegisterPlayer(this, networkContext.Timeout);
    }

    public void Dispose()
    {
        _playersTimeoutManager?.Dispose();
        _pinger?.Dispose();
    }
}