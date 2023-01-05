namespace TieBetting.Services;

public interface IRepository
{
    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync();

    Task AddTeamAsync(Team team);

    Task<Team> CreateTeamAsync(string teamName);

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task UpdateRateAndBets(double rate, string homeTeamId, int homeTeamBets, string awayTeamId, int awayTeamBets);
    Task UpdateMatch(Match match);
}