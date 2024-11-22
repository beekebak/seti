using Snakes;

namespace Lab4.NetworkService.Players;

public class RegularPlayer: Player
{
    public override void StartGame()
    {
        throw new NotImplementedException();
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