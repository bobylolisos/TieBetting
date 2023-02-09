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

    public async Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        var allMatches = await _repository.GetAllMatchesAsync();

        var upcomingMatches = allMatches.Where(x => x.Day >= DayProvider.TodayDay);
        if (numberOfMatches.HasValue)
        {
            return upcomingMatches.Take(numberOfMatches.Value).ToList();

        }
        return upcomingMatches.ToList();
    }

    public Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync()
    {
        return _repository.GetPreviousOngoingMatchesAsync();
    }
}