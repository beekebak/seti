using System.Text.Json;
using Lab4.NetworkService.Players;
using Lab4Core;
using Lab4Core.GameObjects;

namespace Lab4.NetworkService;

public class MultiplayerGameContext
{
    private Timer? _timer;
    public GameContext Context { get; private set; }
    public MyConcurrentList<Player> Players { get; set; }
    public GameConfig Config { get; set; }
    public string Name { get; set; }
    public event Action? ModelUpdated;
    private MultiplayerGameContext() { }
    
    private void UpdateState()
    {
        Context.Play();
        ModelUpdated?.Invoke();
    }

    private GameConfig ReadGameConfig()
    {
        static string Path()
        {
            return "Config/GameContext.json";
        }
        var config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(Path()));
        if(config == null) throw new FormatException("bad GameConfig");
        return config;
    }
    
    public int GetScore(Player player)
    {
        if (player.Role == PlayerRole.Viewer) return -1;
        return Context.ScoreBoard.GetScore(player.RelatedSnake!);
    }
    
    public static MultiplayerGameContext StartGame(int Id, string name = "")
    {
        MultiplayerGameContext context = new MultiplayerGameContext();
        context.Config = context.ReadGameConfig();
        context.Players = [new Player(PlayerRole.Master)];
        context.Players[0].Id = Id;
        context.Context = new GameContext(context.Config.Width, context.Config.Height, context.Config.Food,
            context.Config.Timeout);
        context.Players[0].RelatedSnake = context.Context.GetNewSnake(context.Players[0].Id);
        context._timer = new Timer(_ =>
        {
            context.UpdateState();
        }, null, 0, context.Context.Delay);
        context.Name = name;
        return context;
    }

    public static MultiplayerGameContext LoadGame(GameConfig config)
    {
        MultiplayerGameContext context = new MultiplayerGameContext();
        context.Config = config;
        context.Players = new MyConcurrentList<Player>();
        context.Context = new GameContext(context.Config.Width, context.Config.Height, context.Config.Food,
            context.Config.Timeout);
        return context;
    }

    public void UpdateGame(List<(int x, int y)> food, MyConcurrentList<Player> players, ScoreBoard scores)
    {
        Players = players;
        Context.UpdateGameContext(food, players.ConvertAll(player => player.RelatedSnake!), scores);
        ModelUpdated?.Invoke();
    }
    
    public List<(string, int)> GetScores()
    {
        List<(string, int)> scores = new List<(string, int)>();
        foreach (var player in Players)
        {
            scores.Add((player.Name, Context.ScoreBoard.GetScore(player.RelatedSnake!)));
        }
        return scores;
    }

    public GameConfig GetConfig()
    {
        return Config;
    }
    
    public void BecomeMaster()
    {
        _timer = new Timer(_ => UpdateState(), null, 0, Context.Delay);
    }
    
    public void Dispose()
    {
        _timer?.Dispose();
        ModelUpdated = null;
    }
}