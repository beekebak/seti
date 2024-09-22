namespace Lab5;

public static class Utility
{
    public static byte[] Concatinate(byte[] a, byte[] b)
    {
        byte[] result = new byte[a.Length + b.Length];
        Array.Copy(a, 0, result, 0, a.Length);
        Array.Copy(b, 0, result, a.Length, b.Length);
        return result;
    }
}