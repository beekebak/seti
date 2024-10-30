using Lab4Core;
using Lab4Core.GameObjects;

namespace Lab4.NetworkService.Players;

public abstract class Player : IDisposable
{
    public Snake? RelatedSnake { get; protected set; }
    public GameContext? Context { get; protected set; }

    public event EventHandler<EventArgs>? ModelUpdated;
    
    protected void OnModelUpdated(EventArgs e) => ModelUpdated?.Invoke(this, e);

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
    
    public abstract void Dispose();
}