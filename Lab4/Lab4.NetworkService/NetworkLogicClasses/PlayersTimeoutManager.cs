using Lab4.NetworkService.Wrappers;

namespace Lab4.NetworkService.NetworkLogicClasses;

public class PlayersTimeoutManager: IDisposable
{
    private Timer? _timer;
    private PlayersContext _playersContext;

    public PlayersTimeoutManager(PlayersContext playersContext)
    {
        _playersContext = playersContext;
    }
    
    public void RegisterPlayer(PlayerWrapper player, int timeout)
    {
        _timer?.Dispose();
        _timer = new Timer(UnregisterPlayer, player, 0, timeout/10*8);
    }

    public void UnregisterPlayer(Object? playerObj)
    {
        PlayerWrapper? player = playerObj as PlayerWrapper;
        _playersContext.DeregisterPlayer(player!);
        _timer?.Dispose();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}