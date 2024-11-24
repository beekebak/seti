using Lab4Core;
using Lab4Core.GameObjects;
using GameConfig = Lab4Core.GameConfig;

namespace Lab4.NetworkService.Players;

public abstract class Player : IDisposable
{
    public Snake? RelatedSnake { get; protected set; }
    public GameContext? Context { get; protected set; }

    public string Name { get; protected set; } = "<blank>";
    public int Id { get; protected set; } = NetworkContext.GetNewPlayerIndex();
    public bool IsDead { get; protected set; } = false;
    protected GameConfig Config { get; set; }
    public event EventHandler<EventArgs>? ModelUpdated;
    protected void OnModelUpdated(EventArgs e) => ModelUpdated?.Invoke(this, e);

    
    protected Player()
    {
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

    public abstract List<(string, int)> GetScores();

    public int GetScore()
    {
        return Context!.ScoreBoard.GetScore(RelatedSnake);
    }

    public void UpdateData(int id, string name, Snake? relatedSnake, bool dead = false)
    {
        Id = id;
        Name = name;
        RelatedSnake = relatedSnake;
        IsDead = dead;
    }
    
    public abstract void Dispose();
}