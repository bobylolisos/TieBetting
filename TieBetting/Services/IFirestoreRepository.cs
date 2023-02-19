namespace TieBetting.Services;

public interface IFirestoreRepository
{
    Task<Settings> GetSettingsAsync();

    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetMatchesAsync();

    Task AddTeamAsync(Team team);

    Task<Team> CreateTeamAsync(string teamName);

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task UpdateMatchAsync(Match match);

    Task UpdateTeamAsync(Team team);

    Task UpdateSettingsAsync(Settings settings);
}