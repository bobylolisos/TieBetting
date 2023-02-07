namespace TieBetting.Services;

public class QueryService : IQueryService
{
    private readonly IFirestoreRepository _repository;

    public QueryService(IFirestoreRepository repository)
    {
        _repository = repository;
    }

    public Task<Settings> GetSettingsAsync()
    {
        return _repository.GetSettingsAsync();
    }

    public Task<IReadOnlyCollection<Team>> GetTeamsAsync()
    {
        return _repository.GetTeamsAsync();
    }

    public Task<IReadOnlyCollection<Match>> GetAllMatchesAsync()
    {
        return _repository.GetAllMatchesAsync();
    }

    public async Task<IReadOnlyCollection<Match>> GetAllMatchesForTeamAsync(string teamName)
    {
        var allMatches = await _repository.GetAllMatchesAsync();

        return allMatches.Where(x => x.HomeTeam == teamName || x.AwayTeam == teamName).ToList();
    }

    public Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        return _repository.GetNextMatchesAsync(numberOfMatches);
    }

    public Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync()
    {
        return _repository.GetPreviousOngoingMatchesAsync();
    }
}