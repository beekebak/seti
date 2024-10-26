namespace Lab4Core;

public class Utility
{
    public static int getNewCoord(int old, int diff, int mod)
    {
        return (old+diff)%mod;
    }
}