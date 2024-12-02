using System.Net;
using Lab4.NetworkService.Players;
using Lab4.NetworkService.Wrappers;
using Lab4Core;
using Lab4Core.GameObjects;
using Snakes;

namespace Lab4.NetworkService;

public static class Converter
{
    public static Directions GetDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Directions.Up;
            case Direction.Down: return Directions.Down;
            case Direction.Left: return Directions.Left;
            case Direction.Right: return Directions.Right;
        }
        throw new Exception("Invalid Direction DTO");
    }

    public static Direction GetDirectionDto(Directions dir)
    {
        switch (dir)
        {
            case Directions.Up: return Direction.Up;
            case Directions.Down: return Direction.Down;
            case Directions.Left: return Direction.Left;
            case Directions.Right: return Direction.Right;
        }
        throw new Exception("Invalid Direction");
    }

    public static NodeRole GetRoleDto(Player player)
    {
        switch (player.Role)
        {
            case PlayerRole.Master: return NodeRole.Master;
            case PlayerRole.Deputy: return NodeRole.Deputy;
            case PlayerRole.Normal: return NodeRole.Normal;
            case PlayerRole.Viewer: return NodeRole.Viewer;
            default: return NodeRole.Normal;
        }
    }
    public static NodeRole GetRoleDto(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Master: return NodeRole.Master;
            case PlayerRole.Deputy: return NodeRole.Deputy;
            case PlayerRole.Normal: return NodeRole.Normal;
            case PlayerRole.Viewer: return NodeRole.Viewer;
            default: return NodeRole.Normal;
        }
    }
    
    public static Player GetPlayer(NodeRole role)
    {
        switch (role)
        {
            case NodeRole.Master: return new Player(PlayerRole.Master);
            case NodeRole.Deputy: return new Player(PlayerRole.Deputy);
            case NodeRole.Normal: return new Player(PlayerRole.Normal);
            case NodeRole.Viewer: return new Player(PlayerRole.Viewer);
            default: return new Player(PlayerRole.Normal);
        }
    }
    
    public static PlayerRole GetRole(NodeRole role)
    {
        switch (role)
        {
            case NodeRole.Master: return PlayerRole.Master;
            case NodeRole.Deputy: return PlayerRole.Deputy;
            case NodeRole.Normal: return PlayerRole.Normal;
            case NodeRole.Viewer: return PlayerRole.Viewer;
            default: return PlayerRole.Normal;
        }
    }

    public static GamePlayer GetPlayerDto(Player player, int score, IPEndPoint? address = null)
    {
        NodeRole role = GetRoleDto(player);
        GamePlayer playerDto = new GamePlayer
        {
            Id = player.Id,
            Name = player.Name,
            Role = role,
            Score = score
        };
        if (address != null)
        {
            playerDto.Port = address.Port;
            playerDto.IpAddress = address.Address.ToString();
        }
        return playerDto;
    }

    public static void ParsePlayerDto(GamePlayer playerDto, out int score, out PlayerWrapper wrapper)
    {
        score = playerDto.Score;
        IPEndPoint address;
        if(playerDto.HasPort && playerDto.HasIpAddress) 
            address = new IPEndPoint(IPAddress.Parse(playerDto.IpAddress), playerDto.Port);
        else address = null;
        wrapper = new PlayerWrapper(GetPlayer(playerDto.Role));
        wrapper.Player.SetupData(playerDto.Id, playerDto.Name);
        wrapper.EndPoint = address;
    }

    public static Snakes.GameConfig GetGameConfigDto(Lab4Core.GameConfig config)
    {
        return new Snakes.GameConfig
        {
            Width = config.Width,
            Height = config.Height,
            StateDelayMs = config.Timeout,
            FoodStatic = config.Food
        };
    }

    public static Lab4Core.GameConfig GetGameConfig(Snakes.GameConfig config)
    {
        return new Lab4Core.GameConfig
        {
            Food = config.FoodStatic,
            Height = config.Height,
            Width = config.Width,
            Timeout = config.StateDelayMs
        };
    }

    public static GameState.Types.Coord GetCoordDto(int x, int y)
    {
        return new GameState.Types.Coord()
        {
            X = x,
            Y = y
        };
    }
    
    public static GameState.Types.Coord GetCoordDto((int x, int y) coords)
    {
        return new GameState.Types.Coord()
        {
            X = coords.x,
            Y = coords.y
        };
    }
    
    public static (int, int) GetCoordinates(GameState.Types.Coord coord)
    {
        return (coord.X, coord.Y);
    }

    public static GameState.Types.Snake GetSnakeDto(Lab4Core.GameObjects.Snake snake)
    {
        List<GameState.Types.Coord> points = new List<GameState.Types.Coord>{GetCoordDto(snake.Body[0].GetPosition())};
        for(int i = 1; i < snake.Body.Count; i++)
        {
            int deltaX = snake.Body[i].GetPosition().x - snake.Body[i-1].GetPosition().x;
            int deltaY = snake.Body[i].GetPosition().y - snake.Body[i-1].GetPosition().y;
            points.Add(GetCoordDto(deltaX, deltaY)); 
        }
        return new GameState.Types.Snake
        {
            PlayerId = snake.PlayerId,
            Points = { points },
            State = snake.Alive ? GameState.Types.Snake.Types.SnakeState.Alive : GameState.Types.Snake.Types.SnakeState.Zombie,
            HeadDirection = GetDirectionDto(snake.Direction)
        };
    }

    public static Lab4Core.GameObjects.Snake GetSnake(GameState.Types.Snake snake)
    {
        List<(int, int)> snakeBody = new List<(int, int)> { GetCoordinates(snake.Points[0]) };
        for (int i = 1; i < snake.Points.Count; i++)
        {
            int parsedX = snake.Points[i].X + snakeBody[i-1].Item1;
            int parsedY = snake.Points[i].Y + snakeBody[i-1].Item2;
            snakeBody.Add((parsedX, parsedY));
        }
        return new Lab4Core.GameObjects.Snake(snakeBody,
            snake.PlayerId, GetDirection(snake.HeadDirection));
    }
    
    public static GameState GetGameStateDto(MultiplayerGameContext master, List<PlayerWrapper> wrappers)
    {
        var players = new GamePlayers();
        players.Players.AddRange(wrappers.ConvertAll(player => GetPlayerDto(player.Player,
            master.GetScore(player.Player), player.EndPoint)));
        return new GameState
        {
            StateOrder = master.Context.StateOrder,
            Snakes = { master.Players.ConvertAll(player => GetSnakeDto(player.RelatedSnake!)) },
            Foods = { master.Context.Field.GetFoodPositions().ConvertAll(pair => GetCoordDto(pair.x, pair.y)) },
            Players = players
        };
    }
    
    public static void ParseGameStateDto(GameState state, out List<(int x, int y)> foodCoords,
        out MyConcurrentList<PlayerWrapper> players, out ScoreBoard scores)
    {
        foodCoords = state.Foods.ToList().ConvertAll(GetCoordinates);
        var snakes = state.Snakes.ToList().ConvertAll(GetSnake);
        players = new MyConcurrentList<PlayerWrapper>();
        scores = new ScoreBoard();
        int usedSnakesCount = 0;
        foreach (var player in state.Players.Players)
        {
            var current = GetPlayer(player.Role);
            if (current.Role != PlayerRole.Viewer)
            {
                bool dead = state.Snakes[usedSnakesCount].State == GameState.Types.Snake.Types.SnakeState.Alive;
                current.UpdateData(player.Id, player.Name, snakes[usedSnakesCount], dead);
                scores.UpdateScore(snakes[usedSnakesCount], player.Score);
                usedSnakesCount++;
            }

            if (player.HasIpAddress && player.HasPort)
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(player.IpAddress), player.Port);
                players.Add(new PlayerWrapper(current, endPoint));
            }
            else
            {
                players.Add(new PlayerWrapper(current));
            }
        }
    }
    public static GameAnnouncement GetGameAnnouncementDto(MultiplayerGameContext master, 
        MyConcurrentList<PlayerWrapper> wrappers)
    {
        var players = new GamePlayers();
        players.Players.AddRange(wrappers.ConvertAll(player => GetPlayerDto(player.Player,
            master.GetScore(player.Player), player.EndPoint)));
        return new GameAnnouncement
        {
            Players = players,
            Config = GetGameConfigDto(master.GetConfig()!),
            GameName = master.GetConfig()!.Name
        };
    }

    public static void ParseGameAnnouncementDto(GameAnnouncement gameAnnouncement,
                                                out String gameName, out Lab4Core.GameConfig gameConfig,
                                                out MyConcurrentList<PlayerWrapper> players)
    {
        gameName = gameAnnouncement.GameName;
        gameConfig = GetGameConfig(gameAnnouncement.Config);
        players = new MyConcurrentList<PlayerWrapper>();
        foreach (var player in gameAnnouncement.Players.Players)
        {
            PlayerWrapper wrapper;
            ParsePlayerDto(player, out _, out wrapper);
            players.Add(wrapper);
        }
    }
}