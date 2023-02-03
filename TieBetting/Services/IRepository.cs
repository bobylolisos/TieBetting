namespace TieBetting.Services;

public interface IRepository
{
    Task<Settings> GetSettingsAsync();

    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync();

    Task AddTeamAsync(Team team);

    Task<Team> CreateTeamAsync(string teamName);

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task UpdateMatchAsync(Match match);
 
    Task UpdateTeamAsync(Team team);
}