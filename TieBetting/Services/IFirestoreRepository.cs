namespace TieBetting.Services;

public interface IFirestoreRepository
{
    Task<Settings> GetSettingsAsync();

    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetMatchesAsync();

    Task<Team> CreateTeamAsync(string teamName);

    Task<Match> CreateMatchAsync(string season, string homeTeam, string awayTeam, DateTime date);

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task UpdateMatchAsync(Match match);

    Task DeleteMatchAsync(Match match);
    
    Task UpdateTeamAsync(Team team);

    Task UpdateSettingsAsync(Settings settings);
}