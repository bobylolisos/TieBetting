namespace TieBetting.Shared.Extensions;

public static class TeamExtensions
{
    public static TeamViewModel GetTeamOrDefault(this IReadOnlyCollection<TeamViewModel> teams, string teamName)
    {
        return teams.SingleOrDefault(y => y.Name == teamName);
    }
    public static IReadOnlyCollection<TeamViewModel> OrderByTeamName(this IReadOnlyCollection<TeamViewModel> teams)
    {
        var culture = new CultureInfo("sv-SE");
        var result = teams.OrderBy(x => x.Name, StringComparer.Create(culture, false));

        return result.ToList();
    }
}