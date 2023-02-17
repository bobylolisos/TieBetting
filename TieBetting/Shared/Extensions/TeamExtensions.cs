namespace TieBetting.Shared.Extensions;

public static class TeamExtensions
{
    public static TeamViewModel GetTeamOrDefault(this IReadOnlyCollection<TeamViewModel> teams, string teamName)
    {
        return teams.SingleOrDefault(y => y.Name == teamName);
    }
}