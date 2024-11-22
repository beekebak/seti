namespace Lab4.NetworkService;

public class NetworkContext : IDisposable
{
    private static int _idCounter = 0;
    private readonly NetworkManager _manager;

    public NetworkContext(NetworkManager manager)
    {
        _manager = manager;
    }
    
    public static int GetNewPlayerIndex()
    {
        _idCounter++;
        return _idCounter;
    }
    
    public void Dispose()
    {
        _manager.Dispose();
    }
}