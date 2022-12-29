namespace TieBetting.Services;

public interface IRepository
{
    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task AddTeamAsync(Team team);

    Task<Team> CreateTeamAsync(string teamName);

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();
}