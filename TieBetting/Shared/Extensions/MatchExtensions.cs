namespace TieBetting.Shared.Extensions;

public static class MatchExtensions
{
    public static bool IsNotActive(this MatchViewModel match)
    {
        return match.MatchStatus == MatchStatus.NotActive;
    }

    public static bool IsActive(this MatchViewModel match)
    {
        return match.MatchStatus == MatchStatus.Active;
    }
    
    public static bool IsWin(this MatchViewModel match)
    {
        return match.MatchStatus == MatchStatus.Win;
    }

    public static bool IsDone(this MatchViewModel match)
    {
        return match.MatchStatus == MatchStatus.Win || match.MatchStatus == MatchStatus.Lost;
    }

    public static bool IsActiveOrDone(this MatchViewModel match)
    {
        return match.MatchStatus == MatchStatus.Active || match.IsDone();
    }

    public static bool HasMatch(this IReadOnlyCollection<MatchViewModel> matches, string matchId)
    {
        return matches.Any(x => x.Id == matchId);
    }

    public static bool HasHomeTeamBet(this MatchViewModel match)
    {
        return match.HomeTeamBet.HasValue && match.HomeTeamBet > 0;
    }

    public static bool HasAwayTeamBet(this MatchViewModel match)
    {
        return match.AwayTeamBet.HasValue && match.AwayTeamBet > 0;
    }
}