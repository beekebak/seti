namespace Lab4Core;

public class Utility
{
    public static int GetNewCoord(int old, int diff, int mod)
    {
        return (mod+old+diff)%mod;
    }

    public static int RevertCoord(int coord, int mod)
    {
        return mod - coord - 1;
    }
}