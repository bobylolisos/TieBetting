namespace TieBetting.Services;

public interface ISaverService
{
    void ClearCache();

    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);
    
    Task AddTeamAsync(Team team);

    Task<Team> CreateTeamAsync(string teamName);

    Task UpdateMatchAsync(Match match);

    Task UpdateTeamAsync(Team team);
}