using Lab4Core;
using Snakes;

namespace Lab4.NetworkService.Players;

public class Master: Player
{
    private Timer? _timer;
    private List<Player> _players;
    
    private void UpdateState()
    {
        Context!.Play();
        Update();
    }
    
    public override void StartGame()
    {
        _players = new List<Player>();
        _players.Add(this);
        Context = GameContext.InitContext(Config.Width, Config.Height, Config.Food, Config.Timeout);
        RelatedSnake = Context.GetNewSnake(Id);
        _timer?.Dispose();
        _timer = new Timer(x => UpdateState(), null, 0, Context.Delay);
    }

    public override NodeRole GetNodeRole()
    {
        return NodeRole.Master;
    }

    public override void Dispose()
    {
        ClearEvent();
        _timer?.Dispose();
    }

    public override List<(string, int)> GetScores()
    {
        List<(string, int)> scores = new List<(string, int)>();
        foreach (var player in _players)
        {
            scores.Add((player.Name, Context!.ScoreBoard.GetScore(player.RelatedSnake)));
        }
        return scores;
    }
}