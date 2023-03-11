namespace TieBetting.Services;

public interface ISaverService
{
    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);
    
    Task<Team> CreateTeamAsync(string teamName);

    Task<Match> CreateMatchAsync(string season, TeamViewModel homeTeam, TeamViewModel awayTeam, DateTime date);

    Task UpdateMatchAsync(Match match);

    Task DeleteMatchAsync(Match match);

    Task UpdateTeamAsync(Team team);

    Task UpdateSettingsAsync(Settings settings);
}