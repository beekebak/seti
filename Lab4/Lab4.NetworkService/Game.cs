using System.Net;
using Lab4.NetworkService.NetworkLogicClasses;
using Lab4.NetworkService.Players;
using Lab4.NetworkService.Wrappers;
using Lab4Core;
using Lab4Core.GameObjects;
using Snakes;
using GameConfig = Lab4Core.GameConfig;

namespace Lab4.NetworkService;

public class Game : IDisposable
{
    private int _playerIdCounter = 2;
    private NetworkContext _networkContext;
    private PlayersContext? _playersContext;
    private List<MultiplayerGameContextWrapper> _multiplayerGames = new List<MultiplayerGameContextWrapper>();
    private MultiplayerGameContext? _currentMultiplayerGame;
    private int _lastGameStateOrder = -1;
    private int? _currentPlayerId = null;
    private bool _waitingForJoinAck = false;
    private Timer? _AnnouncementTimer;
    
    public event Action? ModelUpdated;
    public event Action? GameFound;
    public event Action? ErrorOccurred;

    public Game()
    {
        _networkContext = new NetworkContext();
    }

    public async Task Start()
    {
        _networkContext.SendMessage(new MessageWrapper(new GameMessage
        {
            Discover = new GameMessage.Types.DiscoverMsg(),
            MsgSeq = NetworkContext.GetNewMessageIndex()
        }, new IPEndPoint(IPAddress.Parse("239.192.0.4"), 9192)));
        while (true)
        {
            var msg = await _networkContext.ReadMessage();
            switch (msg.Message.TypeCase)
            {
                case GameMessage.TypeOneofCase.Ping:
                    HandlePing(msg);
                    break;
                case GameMessage.TypeOneofCase.Steer: 
                    HandleSteer(msg);
                    break;
                case GameMessage.TypeOneofCase.Ack:
                    HandleAck(msg);
                    break;
                case GameMessage.TypeOneofCase.State:
                    HandleState(msg);
                    break;
                case GameMessage.TypeOneofCase.Announcement:
                    HandleAnnouncement(msg);
                    break;
                case GameMessage.TypeOneofCase.Join:
                    HandleJoin(msg);
                    break;
                case GameMessage.TypeOneofCase.Error:
                    HandleError(msg);
                    break;
                case GameMessage.TypeOneofCase.RoleChange:
                    HandleRoleChange(msg);
                    break;
                case GameMessage.TypeOneofCase.Discover:
                    HandleDiscover(msg);
                    break;
                case GameMessage.TypeOneofCase.None:
                    break;
            }
        }
    } 

    private void HandleAck(MessageWrapper ack)
    {
        if (_waitingForJoinAck)
        {
            _playersContext = new PlayersContext(ack.Message.ReceiverId, _networkContext, _currentMultiplayerGame);
            _waitingForJoinAck = false;
        }
        _networkContext.MessageQueue.DeregisterMessage(ack.Message.MsgSeq);
    }

    private void HandlePing(MessageWrapper ping)
    {
        PlayerWrapper? owner = GetIpOwner(ping.EndPoint);
        if(owner == null) return;
        if(_playersContext == null) return;
        _playersContext.RegisterPlayer(owner);
        SendAck(ping);
    }

    private void HandleSteer(MessageWrapper steer)
    {
        PlayerWrapper? owner = GetIpOwner(steer.EndPoint);
        if(owner == null) return;
        if(_playersContext == null) return;
        owner.Player.Move(Converter.GetDirection(steer.Message.Steer.Direction));
        _playersContext.RegisterPlayer(owner);
        SendAck(steer);
    }

    private void HandleState(MessageWrapper state)
    {
        if (state.Message.State.State.StateOrder < _lastGameStateOrder) return;
        PlayerWrapper? owner = GetIpOwner(state.EndPoint);
        if(owner == null) return;
        if(_playersContext == null) return;
        state.Message.State.State.StateOrder = _lastGameStateOrder;
        List<(int x, int y)> foodCoords; 
        MyConcurrentList<PlayerWrapper> players;
        ScoreBoard scores;
        Converter.ParseGameStateDto(state.Message.State.State, out foodCoords, out players, out scores);
        UpdateReceivedInfo(foodCoords, players, scores);
        _playersContext.RegisterPlayer(owner);
        SendAck(state);
    }

    private void HandleAnnouncement(MessageWrapper announcement)
    {
        GameConfig config;
        MyConcurrentList<PlayerWrapper> players;
        string name;
        Converter.ParseGameAnnouncementDto(announcement.Message.Announcement.Games[0], out name, out config, out players);
        foreach (var game in _multiplayerGames)
        {
            if (game.Context.Name == name)
            {
                game.Context.Players = players.ConvertAll(player => player.Player);
                game.MasterEndPoint = announcement.EndPoint;
                return;
            }
        }
        var newGame = MultiplayerGameContext.LoadGame(config);
        newGame.Players = players.ConvertAll(player => player.Player);
        newGame.Name = name;
        _multiplayerGames.Add(new MultiplayerGameContextWrapper(newGame, announcement.EndPoint));
        GameFound?.Invoke();
    }

    private void HandleDiscover(MessageWrapper discover)
    {
        if(_currentMultiplayerGame == null) return;
        if(_playersContext == null) return;
        if(_playersContext!.Master != _playersContext.GetCurrentPlayer()) return;
        var msg = new GameMessage
        {
            Announcement = new GameMessage.Types.AnnouncementMsg
            {
                Games = {Converter.GetGameAnnouncementDto(_currentMultiplayerGame, _playersContext.AllPlayers)}
            },
            MsgSeq = NetworkContext.GetNewMessageIndex()
        };
        _networkContext.SendMessage(new MessageWrapper(msg, discover.EndPoint));
    }

    private void HandleJoin(MessageWrapper join)
    {
        string playerName = join.Message.Join.PlayerName;
        Player player = Converter.GetPlayer(join.Message.Join.RequestedRole);
        player.SetupData(_playerIdCounter++, playerName);
        if(player.Role != PlayerRole.Viewer)
            player.RelatedSnake = _currentMultiplayerGame!.Context.GetNewSnake(player.Id);
        if (player.RelatedSnake == null && player.Role != PlayerRole.Viewer)
        {
            var msg = new GameMessage
            {
                Error = new GameMessage.Types.ErrorMsg
                {
                    ErrorMessage = "Нет пространства для добавления змейки"
                },
                MsgSeq = NetworkContext.GetNewMessageIndex()
            };
            _networkContext.SendMessage(new MessageWrapper(msg, join.EndPoint));
            return;
        }
        _playersContext.RegisterPlayer(new PlayerWrapper(player, join.EndPoint));
        SendAck(join);
    }

    private void HandleError(MessageWrapper error)
    {
        ErrorOccurred?.Invoke();
        SendAck(error);
    }

    private void HandleRoleChange(MessageWrapper roleChange)
    {
        var senderRole = Converter.GetRole(roleChange.Message.RoleChange.SenderRole);
        var receiverRole = Converter.GetRole(roleChange.Message.RoleChange.ReceiverRole);
        if (senderRole == PlayerRole.Master)
        {
            _playersContext.Master = _playersContext.AllPlayers.Find(p => 
                p.Player.Id == roleChange.Message.SenderId);
        }
        if (receiverRole == PlayerRole.Deputy)
        {
            _playersContext.Deputy = _playersContext.GetCurrentPlayer();
            _playersContext.Deputy.Player.Role = PlayerRole.Deputy;
        }

        if (receiverRole == PlayerRole.Viewer)
        {
            _playersContext.GetCurrentPlayer().Player.Role = PlayerRole.Viewer;
            var playerRelatedSnake = _playersContext.GetCurrentPlayer().Player.RelatedSnake;
            if (playerRelatedSnake != null)
            {
                playerRelatedSnake.Alive = false;
                _playersContext.GetCurrentPlayer().Player.RelatedSnake = null;
            }
        }
        if (senderRole == PlayerRole.Viewer)
        {
            var player = _playersContext.AllPlayers.Find(p =>
                p.Player.Id == roleChange.Message.SenderId)!;
            _playersContext.DeregisterPlayer(player);
            _playersContext.AllPlayers.Remove(player);
        }
        SendAck(roleChange);
    }

    private void SendAck(MessageWrapper old)
    {
        SendAck(old.EndPoint, old.Message.MsgSeq);
    }
    
    private void SendAck(IPEndPoint endPoint, long msgSeq)
    {
        PlayerWrapper? owner = GetIpOwner(endPoint);
        if(owner == null) return;
        GameMessage message = new GameMessage
        {
            Ack = new GameMessage.Types.AckMsg(),
            MsgSeq = msgSeq,
            SenderId = _currentPlayerId ?? 0,
            ReceiverId = owner.Player.Id
        };
        _networkContext.Manager.SendMessage(message, endPoint);
    }

    private PlayerWrapper? GetIpOwner(IPEndPoint endPoint)
    {
        return _playersContext.ActiveConnectionsPlayers.Where(player => player.EndPoint != null).ToList().
            Find(player => player.EndPoint!.Equals(endPoint));
    }
    
    public void InitializeGameStart()
    {
        ClearContext();
        int id = _playerIdCounter;
        _currentMultiplayerGame = MultiplayerGameContext.StartGame(id);
        _currentMultiplayerGame.ModelUpdated += () =>
        {
            ModelUpdated?.Invoke();
        };
        _networkContext.Timeout = _currentMultiplayerGame.Config.Timeout;
        _playersContext = new PlayersContext(id, _networkContext, _currentMultiplayerGame);
        var player = new Player(PlayerRole.Master);
        player.RelatedSnake = _currentMultiplayerGame!.Context.GetNewSnake(id);
        _playersContext.AllPlayers = [new PlayerWrapper(player)];
        _playersContext.Master = _playersContext.AllPlayers[0];
        _AnnouncementTimer = new Timer(_ => {_networkContext.SendMulticastMessage(new GameMessage
        {
            Announcement = new GameMessage.Types.AnnouncementMsg
            {
                Games = { Converter.GetGameAnnouncementDto(_currentMultiplayerGame, _playersContext.AllPlayers) }
            }
        });}, null, 0, 1000);
    }

    public void InitializeGameLoad(MultiplayerGameContextWrapper wrapper, string name, NodeRole role)
    {
        ClearContext();
        _currentMultiplayerGame = MultiplayerGameContext.LoadGame(wrapper.Context.Config);
        _networkContext.Timeout = wrapper.Context.Config.Timeout;
        _networkContext.SendMessage(new MessageWrapper(new GameMessage
        {
            Join = new GameMessage.Types.JoinMsg
            {
                PlayerName = name,
                GameName = wrapper.Context.Config.Name,
                RequestedRole = role
            },
            MsgSeq = 0
        }, wrapper.MasterEndPoint));
        _waitingForJoinAck = true;
    }

    public void Move(Directions direction)
    {
        if(_playersContext == null) return;
        if(_playersContext!.GetCurrentPlayer().Player.Move(direction)) return;
        if(_playersContext.Master != _playersContext.GetCurrentPlayer()) _networkContext.SendMessage(new MessageWrapper(new GameMessage
        {
            Steer = new GameMessage.Types.SteerMsg
            {
                Direction = Converter.GetDirectionDto(direction)
            },
            MsgSeq = NetworkContext.GetNewMessageIndex()
        }, _playersContext.Master!.EndPoint!));
    }

    private void UpdateReceivedInfo(List<(int x, int y)> food, MyConcurrentList<PlayerWrapper> players, ScoreBoard scores)
    {
        _playersContext.AllPlayers = players;
        var newMaster = players.Find(p => p.Player.Role == PlayerRole.Master);
        _playersContext.Master?.Dispose();
        _playersContext.Master = newMaster;
        var newDeputy = players.Find(p => p.Player.Role == PlayerRole.Deputy);
        if (newDeputy != null)
        {
            _playersContext.Deputy?.Dispose();
            _playersContext.Deputy = newDeputy;
        }
        _currentMultiplayerGame!.UpdateGame(food, players.ConvertAll(p => p.Player), scores);
        GameFound?.Invoke();
    }

    public async Task LeaveGame()
    {
        await _networkContext.SendMessageAsync(new MessageWrapper(new GameMessage
        {
            RoleChange = new GameMessage.Types.RoleChangeMsg
            {
                SenderRole = Converter.GetRoleDto(PlayerRole.Viewer),
                ReceiverRole = Converter.GetRoleDto(_playersContext!.Master!.Player)
            }
        }, _playersContext!.Master!.EndPoint!));
        ClearContext();
    }

    public void ClearContext()
    {
        _AnnouncementTimer?.Dispose();
        _playersContext?.Dispose();
        _networkContext.MessageQueue.Dispose();
        _playerIdCounter = 2;
    }

    public List<(String, int)> GetScores()
    {
        return _currentMultiplayerGame?.GetScores() ?? new List<(String, int)>();
    }

    public int GetFieldHeight()
    {
        return _currentMultiplayerGame?.Context.Field.GetHeight() ?? 0;
    }
    
    public int GetFieldWidth()
    {
        return _currentMultiplayerGame?.Context.Field.GetWidth() ?? 0;
    }

    public int GetCellColor(int x, int y)
    {
        return _currentMultiplayerGame?.Context.Field.GetCell(x, y).GetColorId() ?? 0;
    }
    
    public void Dispose()
    {
        _AnnouncementTimer?.Dispose();
        _playersContext?.Dispose();
        _currentMultiplayerGame?.Dispose();
        _networkContext.Dispose();
        _playerIdCounter = 2;

        ErrorOccurred = null;
        ModelUpdated = null;
        GameFound = null;
    }
}