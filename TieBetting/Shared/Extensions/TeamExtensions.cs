namespace TieBetting.Shared.Extensions;

public static class TeamExtensions
{
    public static Team GetTeam(this IReadOnlyCollection<Team> teams, string teamName)
    {
        return teams.Single(y => y.Name == teamName);
    }

    public static Team GetTeamOrDefault(this IReadOnlyCollection<Team> teams, string teamName)
    {
        return teams.SingleOrDefault(y => y.Name == teamName);
    }
}