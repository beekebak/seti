using System.Collections.Concurrent;
using Lab4.NetworkService.Players;
using Snakes;

namespace Lab4.NetworkService;

public class PlayersTimeoutManager
{
    ConcurrentDictionary<Player, Timer> _players = new();
    List<Player> _activePlayers;
    NetworkManager _networkManager;
    private int _timeout;

    public PlayersTimeoutManager(NetworkManager networkManager, List<Player> activePlayers, int timeout = 1000)
    {
        _networkManager = networkManager;
        _timeout = timeout;
        _activePlayers = activePlayers;
    }

    public void RegisterPlayer(Player player, Timer? timer = null)
    {
        timer?.Dispose();
        timer = new Timer(SendPing, player, 0, _timeout/10*8);
        _players.TryAdd(player, timer);
    }

    public void UnregisterPlayer(Object? playerObj)
    {
        Player? player = playerObj as Player;
        _players.TryGetValue(player, out Timer? timer);
        timer!.Dispose();
        _players.TryRemove(player, out _);
        _activePlayers.Remove(player);
    }

    private void SendPing(Object? playerObj)
    {
        Player? player = playerObj as Player;
        GameMessage gameMessage = new GameMessage
        {
            MsgSeq = NetworkContext.GetNewMessageIndex(),
            Ping = new GameMessage.Types.PingMsg()
        };
        _networkManager.SendMessage(gameMessage);
        _players.TryGetValue(player, out Timer? timer);
        timer.Dispose();
        _players.TryRemove(player, out _);
        _players.TryAdd(player, new Timer(UnregisterPlayer, player, 0, _timeout/10*2));
    }
}