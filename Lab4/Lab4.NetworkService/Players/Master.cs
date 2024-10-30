using Lab4Core;

namespace Lab4.NetworkService.Players;

public class Master: Player
{
    private Timer? _timer;
    
    private void UpdateState()
    {
        Context!.Play();
        Update();
    }
    
    public override void StartGame()
    {
        Context = GameContext.InitContext();
        RelatedSnake = Context.GetNewSnake();
        _timer?.Dispose();
        _timer = new Timer(x => UpdateState(), null, 0, Context.Delay);
    }

    public override void Dispose()
    {
        ClearEvent();
        _timer?.Dispose();
    }
}