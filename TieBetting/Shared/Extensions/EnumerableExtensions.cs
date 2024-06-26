﻿namespace TieBetting.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> TakeLastItems<T>(this IEnumerable<T> source, int n)
    {
        return source.Skip(Math.Max(0, source.Count() - n));
    }

    public static int CountNumberOfPreviousLostMatches(this IEnumerable<MatchStatus> statuses)
    {
        var result = 0;

        foreach (var status in statuses.Reverse())
        {
            if (status == MatchStatus.Win || status == MatchStatus.Abandoned)
            {
                return result;
            }

            result++;
        }

        return result;
    }

    public static int CountMaxNumberOfPreviousLostMatches(this IEnumerable<MatchStatus> statuses)
    {
        var max = 0;
        var result = 0;

        foreach (var status in statuses.Reverse())
        {
            if (status == MatchStatus.Win || status == MatchStatus.Abandoned)
            {
                if (result > max)
                {
                    max = result;
                    result = 0;
                }
            }
            else
            {
                result++;
            }
        }

        if (result > max)
        {
            max = result;
        }
        return max;
    }

    public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            observableCollection.Add(item);
        }
    }
}