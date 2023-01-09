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

    public static string ResolveTeamName(this string stringValue)
    {
        var teamName = stringValue.ToLower().ToLower();

        switch (teamName)
        {
            case { } str when str.Contains("aik"):
                return "Aik";
            case { } str when str.Contains("almtuna"):
                return "Almtuna";
            case { } str when str.Contains("björklöven"):
                return "Björklöven";
            case { } str when str.Contains("djurgården"):
                return "Djurgården";
            case { } str when str.Contains("karlskoga"):
                return "Karlskoga";
            case { } str when str.Contains("kristianstad"):
                return "Kristianstad";
            case { } str when str.Contains("modo"):
                return "Modo";
            case { } str when str.Contains("mora"):
                return "Mora";
            case { } str when str.Contains("södertälje"):
                return "Södertälje";
            case { } str when str.Contains("tingsryd"):
                return "Tingsryd";
            case { } str when str.Contains("hästen"):
                return "Vita hästen";
            case { } str when str.Contains("västervik"):
                return "Västervik";
            case { } str when str.Contains("västerås"):
                return "Västerås";
            case { } str when str.Contains("östersund"):
                return "Östersund";

            default: throw new ArgumentException($"Unable to resolve team name <{stringValue}>");
        }

    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeLastItems<T>(this IEnumerable<T> source, int n)
    {
        return source.Skip(Math.Max(0, source.Count() - n));
    }

    public static int CountNumberOfPreviousLostMatches(this IEnumerable<bool> statuses)
    {
        var result = 0;

        foreach (var status in statuses.Reverse())
        {
            if (status)
            {
                return result;
            }

            result++;
        }

        return result;
    }

}