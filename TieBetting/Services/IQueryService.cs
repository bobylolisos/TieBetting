namespace TieBetting.Services;

public interface IQueryService
{
    void ClearCache();

    Task<Settings> GetSettingsAsync();

    Task<IReadOnlyCollection<TeamViewModel>> GetTeamsAsync();

    Task<IReadOnlyCollection<MatchBettingViewModel>> GetMatchesAsync();

    Task<IReadOnlyCollection<MatchBettingViewModel>> GetNextMatchesAsync(int? numberOfMatches = null);

    Task<IReadOnlyCollection<MatchBettingViewModel>> GetPreviousOngoingMatchesAsync();

}