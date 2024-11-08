using System.Text.Json;
using Lab4Core;
using Lab4Core.GameObjects;
using Snakes;
using GameConfig = Lab4Core.GameConfig;

namespace Lab4.NetworkService.Players;

public abstract class Player : IDisposable
{
    public Snake? RelatedSnake { get; protected set; }
    public GameContext? Context { get; protected set; }

    public string Name { get; }
    public int Id { get; } = NetworkContext.GetNewPlayerIndex();
    protected GameConfig Config { get; }
    public event EventHandler<EventArgs>? ModelUpdated;
    protected void OnModelUpdated(EventArgs e) => ModelUpdated?.Invoke(this, e);

    protected Player()
    {
        static string Path()
        {
            return "Config/GameContext.json";
        }
        Config = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(Path()));
        if(Config == null) throw new FormatException("bad GameConfig");
        Name = Config.Name;
    }
    
    public virtual void Move(Directions direction)
    {
        if(RelatedSnake == null) return;
        if(direction != RelatedSnake.GetForbiddenDirection()) RelatedSnake.Direction = direction;
    }

    public abstract void StartGame();

    protected void Update()
    {
        OnModelUpdated(EventArgs.Empty);
    }

    protected void ClearEvent()
    {
        ModelUpdated = null;
    }

    public abstract NodeRole GetNodeRole();

    public abstract List<(string, int)> GetScores();
    
    public abstract void Dispose();
}