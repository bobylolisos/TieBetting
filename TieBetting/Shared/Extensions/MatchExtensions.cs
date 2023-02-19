namespace TieBetting.Shared.Extensions;

public static class MatchExtensions
{
    public static bool IsNotActive(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.NotActive;
        }

        return match.AwayTeamMatchStatus == MatchStatus.NotActive;
    }

    public static bool IsActive(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Active;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Active;
    }

    public static bool IsAnyActive(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Active || match.AwayTeamMatchStatus == MatchStatus.Active;
    }

    public static bool IsWin(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Win;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Win;
    }

    public static bool IsAnyWin(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Win || match.AwayTeamMatchStatus == MatchStatus.Win;
    }

    public static bool IsDormant(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Dormant;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Dormant;
    }

    public static bool IsDone(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Win || match.HomeTeamMatchStatus == MatchStatus.Lost;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Win || match.AwayTeamMatchStatus == MatchStatus.Lost;
    }

    public static bool IsAnyDone(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Win || match.HomeTeamMatchStatus == MatchStatus.Lost || match.AwayTeamMatchStatus == MatchStatus.Win || match.AwayTeamMatchStatus == MatchStatus.Lost;
    }

    public static bool IsActiveOrDone(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Active || match.IsDone(teamType);
        }

        return match.AwayTeamMatchStatus == MatchStatus.Active || match.IsDone(teamType);
    }

    public static bool IsAnyActiveOrDone(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Active || match.AwayTeamMatchStatus == MatchStatus.Active || match.IsAnyDone();
    }

    public static bool HasMatch(this IReadOnlyCollection<MatchViewModel> matches, string matchId)
    {
        return matches.Any(x => x.Id == matchId);
    }

    public static bool HasBet(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamBet.HasValue && match.HomeTeamBet > 0;
        }

        // Away team
        return match.AwayTeamBet.HasValue && match.AwayTeamBet > 0;
    }
}