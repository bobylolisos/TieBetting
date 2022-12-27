namespace TieBetting.Shared.Extensions;

public static class StringExtensions
{
    public static bool HasContent(this string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}