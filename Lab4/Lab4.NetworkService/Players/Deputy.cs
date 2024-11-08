using Snakes;

namespace Lab4.NetworkService.Players;

public class Deputy : RegularPlayer
{
    public override NodeRole GetNodeRole()
    {
        return NodeRole.Deputy;
    }
}