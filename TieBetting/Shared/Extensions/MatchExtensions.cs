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

    public static bool IsAnyNotActive(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.NotActive || match.AwayTeamMatchStatus == MatchStatus.NotActive;
    }

    public static bool IsNotActive(this MatchViewModel match, string teamName)
    {
        if (teamName == match.HomeTeamName && match.HomeTeamMatchStatus == MatchStatus.NotActive)
        {
            return true;
        }

        if (teamName == match.AwayTeamName && match.AwayTeamMatchStatus == MatchStatus.NotActive)
        {
            return true;
        }

        return true;
    }

    public static bool IsActive(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Active;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Active;
    }

    public static bool IsActive(this MatchViewModel match, string teamName)
    {
        if (teamName == match.HomeTeamName && match.HomeTeamMatchStatus == MatchStatus.Active)
        {
            return true;
        }

        if (teamName == match.AwayTeamName && match.AwayTeamMatchStatus == MatchStatus.Active)
        {
            return true;
        }

        return false;
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

    public static bool IsLost(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Lost;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Lost;
    }

    public static bool IsAnyLost(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Lost || match.AwayTeamMatchStatus == MatchStatus.Lost;
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
            return match.HomeTeamMatchStatus == MatchStatus.Win || match.HomeTeamMatchStatus == MatchStatus.Lost || match.HomeTeamMatchStatus == MatchStatus.Abandon;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Win || match.AwayTeamMatchStatus == MatchStatus.Lost || match.AwayTeamMatchStatus == MatchStatus.Abandon;
    }

    public static bool IsAnyDone(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Win || match.HomeTeamMatchStatus == MatchStatus.Lost || match.HomeTeamMatchStatus == MatchStatus.Abandon || 
               match.AwayTeamMatchStatus == MatchStatus.Win || match.AwayTeamMatchStatus == MatchStatus.Lost || match.AwayTeamMatchStatus == MatchStatus.Abandon;
    }

    public static bool IsActiveOrDone(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Active || match.IsDone(teamType);
        }

        return match.AwayTeamMatchStatus == MatchStatus.Active || match.IsDone(teamType);
    }

    public static bool IsActiveOrDone(this MatchViewModel match, string teamName)
    {
        if (teamName == match.HomeTeamName && match.IsActiveOrDone(TeamType.HomeTeam))
        {
            return true;
        }

        if (teamName == match.AwayTeamName && match.IsActiveOrDone(TeamType.AwayTeam))
        {
            return true;
        }

        return false;
    }

    public static bool IsAnyActiveOrDone(this MatchViewModel match)
    {
        return match.HomeTeamMatchStatus == MatchStatus.Active || match.AwayTeamMatchStatus == MatchStatus.Active || match.IsAnyDone();
    }

    public static bool IsAbandon(this MatchViewModel match, TeamType teamType)
    {
        if (teamType == TeamType.HomeTeam)
        {
            return match.HomeTeamMatchStatus == MatchStatus.Abandon;
        }

        return match.AwayTeamMatchStatus == MatchStatus.Abandon;
    }

    public static bool HasMatch(this IReadOnlyCollection<MatchViewModel> matches, string matchId)
    {
        return matches.Any(x => x.Id == matchId);
    }
}