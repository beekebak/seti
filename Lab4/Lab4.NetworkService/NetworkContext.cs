namespace Lab4.NetworkService;

public class NetworkContext
{
    private static int _idCounter = 0;
    public static int GetNewPlayerIndex()
    {
        _idCounter++;
        return _idCounter;
    }
}