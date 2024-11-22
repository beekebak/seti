using System.Text.Json;
using Lab4Core;
using Snakes;

namespace Lab4.NetworkService.Players;

public class Master: Player
{
    private Timer? _timer;
    public List<Player> Players { get; private set; }
    
    private void UpdateState()
    {
        Context!.Play();
        Update();
    }

    private void ReadGameConfig()
    {
        static string Path()
        {
            return "Config/GameContext.json";
        }
        Config = JsonSerializer.Deserialize<Lab4Core.GameConfig>(File.ReadAllText(Path()));
        if(Config == null) throw new FormatException("bad GameConfig");
        Name = Config.Name;
    }
    
    public override void StartGame()
    {
        ReadGameConfig();
        Players = new List<Player>();
        Players.Add(this);
        Context = new GameContext(Config.Width, Config.Height, Config.Food, Config.Timeout);
        RelatedSnake = Context.GetNewSnake(Id);
        _timer?.Dispose();
        _timer = new Timer(x => UpdateState(), null, 0, Context.Delay);
    }

    public override void Dispose()
    {
        ClearEvent();
        _timer?.Dispose();
    }

    public override List<(string, int)> GetScores()
    {
        List<(string, int)> scores = new List<(string, int)>();
        foreach (var player in Players)
        {
            scores.Add((player.Name, Context!.ScoreBoard.GetScore(player.RelatedSnake)));
        }
        return scores;
    }
}