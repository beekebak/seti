using System.Net;
using Lab4.NetworkService.Players;
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
        switch (player)
        {
            case Master: return NodeRole.Master;
            case Deputy: return NodeRole.Deputy;
            case RegularPlayer: return NodeRole.Normal;
            case Watcher: return NodeRole.Viewer;
            default: return NodeRole.Normal;
        }
    }
    
    public static Player GetPlayer(NodeRole role)
    {
        switch (role)
        {
            case NodeRole.Master: return new Master();
            case NodeRole.Deputy: return new Deputy();
            case NodeRole.Normal: return new RegularPlayer();
            case NodeRole.Viewer: return new Watcher();
            default: return new RegularPlayer();
        }
    }

    public static GamePlayer GetPlayerDto(Player player, IPEndPoint? address = null)
    {
        NodeRole role = GetRoleDto(player);
        GamePlayer playerDto = new GamePlayer
        {
            Id = player.Id,
            Name = player.Name,
            Role = role,
            Score = player.GetScore()
        };
        if (address != null)
        {
            playerDto.Port = address.Port;
            playerDto.IpAddress = address.Address.ToString();
        }
        return playerDto;
    }

    public static void ParsePlayerDto(GamePlayer playerDto, out string name, out int id, out int score,
        out IPEndPoint? address)
    {
        name = playerDto.Name;
        id = playerDto.Id;
        score = playerDto.Score;
        if(playerDto.HasPort && playerDto.HasIpAddress) 
            address = new IPEndPoint(IPAddress.Parse(playerDto.IpAddress), playerDto.Port);
        else address = null;
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

    public static GameState.Types.Snake GetSnakeDto(Lab4Core.GameObjects.Snake snake, int id, bool alive)
    {
        List<GameState.Types.Coord> points = new List<GameState.Types.Coord>{GetCoordDto(snake.Body[0].GetPosition())};
        for(int i = 1; i < snake.Body.Count; i++)
        {
            int deltaX = snake.Body[i].GetPosition().x - snake.Body[i-1].GetPosition().y;
            int deltaY = snake.Body[i].GetPosition().x - snake.Body[i-1].GetPosition().y;
            points.Add(GetCoordDto(deltaX, deltaY)); 
        }
        return new GameState.Types.Snake
        {
            PlayerId = id,
            Points = { points },
            State = alive ? GameState.Types.Snake.Types.SnakeState.Alive : GameState.Types.Snake.Types.SnakeState.Zombie,
            HeadDirection = GetDirectionDto(snake.GetForbiddenDirection())
        };
    }

    public static Lab4Core.GameObjects.Snake GetSnake(GameState.Types.Snake snake)
    {
        List<(int, int)> snakeBody = new List<(int, int)> { GetCoordinates(snake.Points[0]) };
        for (int i = 1; i < snake.Points.Count; i++)
        {
            int parsedX = snake.Points[i].X + snake.Points[i-1].X;
            int parsedY = snake.Points[i].Y + snake.Points[i-1].Y;
            snakeBody.Add((parsedX, parsedY));
        }
        return new Lab4Core.GameObjects.Snake(snake.Points.ToList().ConvertAll(GetCoordinates),
            snake.PlayerId, GetDirection(snake.HeadDirection));
    }
    
    public static GameState GetGameStateDto(Master master)
    {
        var players = new GamePlayers();
        players.Players.AddRange(master.Players.ConvertAll(player => GetPlayerDto(player)));
        return new GameState
        {
            StateOrder = master.Context!.StateOrder,
            Snakes = { master.Players.ConvertAll(player => GetSnakeDto(player.RelatedSnake!, player.Id, !player.IsDead)) },
            Foods = { master.Context.Field.GetFoodPositions().ConvertAll(pair => GetCoordDto(pair.x, pair.y)) },
            Players = players
        };
    }
    
    public static void ParseGameStateDto(GameState state, out List<(int x, int y)> foodCoords,
                                         out List<Lab4Core.GameObjects.Snake> snakes, out List<Player> players,
                                         out ScoreBoard scores)
    {
        foodCoords = state.Foods.ToList().ConvertAll(GetCoordinates);
        snakes = state.Snakes.ToList().ConvertAll(GetSnake);
        players = new List<Player>();
        scores = new ScoreBoard();
        int usedSnakesCount = 0;
        foreach (var player in state.Players.Players)
        {
            var current = GetPlayer(player.Role);
            if (current is not Watcher)
            {
                bool dead = state.Snakes[usedSnakesCount].State == GameState.Types.Snake.Types.SnakeState.Alive;
                current.UpdateData(player.Id, player.Name, snakes[usedSnakesCount], dead);
                scores.UpdateScore(snakes[usedSnakesCount], player.Score);
                usedSnakesCount++;
            }
        }
    }
    public static GameAnnouncement GetGameAnnouncementDto(Master master)
    {
        var players = new GamePlayers();
        players.Players.AddRange(master.Players.ConvertAll(player => GetPlayerDto(player)));
        return new GameAnnouncement
        {
            Players = players,
            Config = GetGameConfigDto(master.GetConfig()),
            GameName = master.GetConfig().Name
        };
    }

    public static void ParseGameAnnouncementDto(GameAnnouncement gameAnnouncement,
                                                out String gameName, out Lab4Core.GameConfig gameConfig)
    {
        gameName = gameAnnouncement.GameName;
        gameConfig = GetGameConfig(gameAnnouncement.Config);
    }
}