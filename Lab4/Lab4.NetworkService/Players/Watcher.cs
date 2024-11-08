using Lab4Core;
using Snakes;

namespace Lab4.NetworkService.Players;

public class Watcher: Player
{
    public override void StartGame()
    {
        throw new NotImplementedException();
    }

    public override void Move(Directions direction) { }

    public override NodeRole GetNodeRole()
    {
        return NodeRole.Viewer;
    }

    public override List<(string, int)> GetScores()
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        ClearEvent();
    }
}