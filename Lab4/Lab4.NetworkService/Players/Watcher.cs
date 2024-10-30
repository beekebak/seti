using Lab4Core;

namespace Lab4.NetworkService.Players;

public class Watcher: Player
{
    public override void StartGame()
    {
        throw new NotImplementedException();
    }

    public override void Move(Directions direction) { }
    
    public override void Dispose()
    {
        ClearEvent();
    }
}