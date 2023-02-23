namespace TieBetting.Shared.Extensions;

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

    public static int CountMaxNumberOfLostMatches(this IEnumerable<bool> statuses)
    {
        var max = 0;
        var result = 0;

        foreach (var status in statuses)
        {
            if (status)
            {
                if (result > max)
                {
                    max = result;
                }
                result = 0;
                continue;
            }

            result++;
        }

        return Math.Max(result, max);
    }
}