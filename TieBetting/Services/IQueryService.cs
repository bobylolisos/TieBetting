namespace TieBetting.Services;

public interface IQueryService
{
    void ClearCache();

    Task<Settings> GetSettingsAsync();

    Task<IReadOnlyCollection<TeamViewModel>> GetTeamsAsync();

    Task<IReadOnlyCollection<MatchViewModel>> GetMatchesAsync();

    Task<IReadOnlyCollection<MatchViewModel>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<MatchViewModel>> GetPreviousOngoingMatchesAsync();

}