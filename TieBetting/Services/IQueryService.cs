namespace TieBetting.Services;

public interface IQueryService
{
    Task<Settings> GetSettingsAsync();

    Task<IReadOnlyCollection<Team>> GetTeamsAsync();

    Task<IReadOnlyCollection<Match>> GetAllMatchesAsync();

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync();

}