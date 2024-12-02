using Lab4.NetworkService.NetworkLogicClasses;
using Lab4Core;
using Snakes;

namespace Lab4.NetworkService.Wrappers;

public class PlayersContext : IDisposable
{
    public MyConcurrentList<PlayerWrapper> AllPlayers { get; set; } = new();
    public MyConcurrentList<PlayerWrapper> ActiveConnectionsPlayers { get; } = new();
    public PlayerWrapper? Master { get; set; }
    public PlayerWrapper? Deputy { get; set; }
    private NetworkContext _networkContext;
    private MultiplayerGameContext _multiplayerGameContext;
    private int _currentPlayerId;
    private object lockObject = new();

    public PlayersContext(int id, NetworkContext networkContext, MultiplayerGameContext multiplayerGameContext)
    {
        _currentPlayerId = id;
        _networkContext = networkContext;
        _multiplayerGameContext = multiplayerGameContext;
    }
    
    public PlayerWrapper GetCurrentPlayer()
    {
        return AllPlayers.Find(p => p.Player.Id == _currentPlayerId)!;
    }

    public void DeregisterPlayer(PlayerWrapper player)
    {
        lock (lockObject)
        {
            player.Dispose();
            ActiveConnectionsPlayers.Remove(player);
            AllPlayers.Remove(player);
            if (Master == GetCurrentPlayer() && player == Deputy)
            {
                Deputy = GetNewDeputy();
                if (Deputy == null) return;
                var msg = new GameMessage
                {
                    RoleChange = new GameMessage.Types.RoleChangeMsg
                    {
                        SenderRole = NodeRole.Master,
                        ReceiverRole = NodeRole.Deputy
                    },
                    MsgSeq = NetworkContext.GetNewMessageIndex(),
                    SenderId = _currentPlayerId,
                    ReceiverId = Deputy.Player.Id
                };
                _networkContext.SendMessage(new MessageWrapper(msg, Deputy.EndPoint!));
            }
            else if (GetCurrentPlayer() == Deputy && player == Master)
            {
                Master = Deputy;
                Deputy = GetNewDeputy();
                _multiplayerGameContext.BecomeMaster();
                foreach (var p in ActiveConnectionsPlayers)
                {
                    var msg = new GameMessage
                    {
                        RoleChange = new GameMessage.Types.RoleChangeMsg
                        {
                            SenderRole = NodeRole.Master,
                            ReceiverRole = Converter.GetRoleDto(p.Player)
                        },
                        MsgSeq = NetworkContext.GetNewMessageIndex(),
                        SenderId = _currentPlayerId,
                        ReceiverId = p.Player.Id
                    };
                    _networkContext.SendMessage(new MessageWrapper(msg, p.EndPoint!));
                }
            }
            else if (player == Master)
            {
                Master = Deputy;
                Deputy = null;
            }
        }
    }

    private PlayerWrapper? GetNewDeputy()
    {
        foreach (var p in AllPlayers)
        {
            if (p.Player.Role != PlayerRole.Normal) continue;
            p.Player.Role = PlayerRole.Deputy;
            return p;
        }
        return null;
    }

    public void RegisterPlayer(PlayerWrapper player)
    {
        if (AllPlayers.Find(p => p.Player.Id == player.Player.Id) == null) AllPlayers.Add(player);
        ActiveConnectionsPlayers.Add(player);
        player.SetActive(this, _networkContext);
    }
    
    public void Dispose()
    {
        foreach(var player in ActiveConnectionsPlayers) player.Dispose();
    }
}