namespace TieBetting.Services;

public class SaverService : ISaverService
{
    private readonly IFirestoreRepository _repository;

    public SaverService(IFirestoreRepository repository)
    {
        _repository = repository;
    }

    public void ClearCache()
    {
        _repository.ClearCache();
    }

    public Task AddMatchesAsync(IReadOnlyCollection<Match> matches)
    {
        return _repository.AddMatchesAsync(matches);
    }

    public Task AddTeamAsync(Team team)
    {
        return _repository.AddTeamAsync(team);
    }

    public Task<Team> CreateTeamAsync(string teamName)
    {
        return _repository.CreateTeamAsync(teamName);
    }

    public Task UpdateMatchAsync(Match match)
    {
        return _repository.UpdateMatchAsync(match);
    }

    public Task UpdateTeamAsync(Team team)
    {
        return _repository.UpdateTeamAsync(team);
    }
}