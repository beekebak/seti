using System.Net;

namespace Lab4.NetworkService.Wrappers;

public class MultiplayerGameContextWrapper
{
    public MultiplayerGameContext Context { set; get; }
    public IPEndPoint MasterEndPoint { set; get; }

    public MultiplayerGameContextWrapper(MultiplayerGameContext context, IPEndPoint masterEndPoint)
    {
        Context = context;
        MasterEndPoint = masterEndPoint;
    }
}