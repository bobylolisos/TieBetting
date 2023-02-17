using static Android.Provider.Telephony.Mms;

namespace TieBetting.Services;

public class SaverService : ISaverService
{
    private readonly IFirestoreRepository _repository;

    public SaverService(IFirestoreRepository repository)
    {
        _repository = repository;
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

    public async Task UpdateMatchAsync(Match match)
    {
        await _repository.UpdateMatchAsync(match);

        WeakReferenceMessenger.Default.Send(new MatchUpdatedMessage(match.Id));
    }

    public Task UpdateTeamAsync(Team team)
    {
        return _repository.UpdateTeamAsync(team);
    }
}