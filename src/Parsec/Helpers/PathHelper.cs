namespace Parsec.Helpers;

public static class PathHelper
{
    private static readonly char[] PathSeparators =
    {
        '/', '\\'
    };

    public static string Normalize(string path)
    {
        return path.Replace('\\', '/');
    }

    public static string[] Split(string path)
    {
        return path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
    }
}
