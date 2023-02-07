namespace TieBetting.Services;

public interface IQueryService
{
    Task<Settings> GetSettingsAsync();

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task<IReadOnlyCollection<Match>> GetAllMatchesAsync();

    Task<IReadOnlyCollection<Match>> GetAllMatchesForTeamAsync(string teamName);

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync();

}