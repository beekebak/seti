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
        return new Snakes.GameConfig()
        {
            Width = config.Width,
            Height = config.Height,
            StateDelayMs = config.Timeout,
            FoodStatic = config.Food
        };
    }

    public static Lab4Core.GameConfig GetGameConfig(Snakes.GameConfig config)
    {
        return new Lab4Core.GameConfig()
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
    
    public static (int, int) GetCoord(GameState.Types.Coord coord)
    {
        return (coord.X, coord.Y);
    }

    /*public static GameState.Types.Snake GetSnakeDto(Lab4Core.GameObjects.Snake snake, int id, bool alive)
    {
        return new GameState.Types.Snake()
        {
            PlayerId = id,
            //WRONG Points = { snake.Body.ToList().ConvertAll(cell => GetCoordDto(cell.GetPosition().Item1, cell.GetPosition().Item2)) },
            State = alive ? GameState.Types.Snake.Types.SnakeState.Alive : GameState.Types.Snake.Types.SnakeState.Zombie,
            HeadDirection = GetDirectionDto(snake.GetForbiddenDirection())
        };
    }*/

    public static Lab4Core.GameObjects.Snake GetSnake(GameState.Types.Snake snake)
    {
        return new Lab4Core.GameObjects.Snake(snake.Points.ToList().ConvertAll(GetCoord),
            snake.PlayerId, GetDirection(snake.HeadDirection));
    }
    
    /*public static GameState GetGameStateDto(Master master)
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

    public static void ParseGameStateDto(GameState state, out GameField field, out List<Snake> snakes, out List<Player> players)
    {
        
    }

    /*public static GameAnnouncement GetGameAnnouncementDto(Master master)
    {
        
    }*/

    /*public static void ParseGameAnnouncementDto(GameAnnouncement gameAnnouncement)
    {
        
    }*/
}