namespace TieBetting.Services;

public class SaverService : ISaverService
{
    private readonly IFirestoreRepository _repository;
    private readonly IMessenger _messenger;

    public SaverService(IFirestoreRepository repository, IMessenger messenger)
    {
        _repository = repository;
        _messenger = messenger;
    }

    public Task AddMatchesAsync(IReadOnlyCollection<Match> matches)
    {
        return _repository.AddMatchesAsync(matches);
    }

    public async Task<Team> CreateTeamAsync(string teamName)
    {
        var team = await _repository.CreateTeamAsync(teamName);

        return team;
    }

    public async Task<Match> CreateMatchAsync(string season, TeamViewModel homeTeam, TeamViewModel awayTeam, DateTime date)
    {
        var match = await _repository.CreateMatchAsync(season, homeTeam.Name, awayTeam.Name, date);

        _messenger.Send(new MatchCreatedMessage(match));
        _messenger.Send(new RefreshRequiredMessage());

        return match;
    }

    public async Task UpdateMatchAsync(Match match)
    {
        await _repository.UpdateMatchAsync(match);

        _messenger.Send(new MatchUpdatedMessage(match.Id));
        _messenger.Send(new TeamUpdatedMessage(match.HomeTeam));
        _messenger.Send(new TeamUpdatedMessage(match.AwayTeam));
    }

    public async Task DeleteMatchAsync(Match match)
    {
        await _repository.DeleteMatchAsync(match);

        _messenger.Send(new MatchDeletedMessage(match.Id));
    }

    public async Task UpdateTeamAsync(Team team)
    {
        await _repository.UpdateTeamAsync(team);

        _messenger.Send(new TeamUpdatedMessage(team.Name));
    }

    public Task UpdateSettingsAsync(Settings settings)
    {
        return _repository.UpdateSettingsAsync(settings);
    }
}