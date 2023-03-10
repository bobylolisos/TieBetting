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

    public async Task<TeamViewModel> CreateTeamAsync(string teamName)
    {
        var team = await _repository.CreateTeamAsync(teamName);

        return new TeamViewModel(this, team);
    }

    public async Task<MatchViewModel> CreateMatchAsync(string season, TeamViewModel homeTeam, TeamViewModel awayTeam, DateTime date)
    {
        var match = await _repository.CreateMatchAsync(season, homeTeam.Name, awayTeam.Name, date);

        var vm = new MatchViewModel(this, match, homeTeam, awayTeam);

        homeTeam.AddMatch(vm);
        awayTeam.AddMatch(vm);

        WeakReferenceMessenger.Default.Send(new MatchCreatedMessage(vm));
        WeakReferenceMessenger.Default.Send(new RefreshRequiredMessage());

        return vm;
    }

    public async Task UpdateMatchAsync(Match match, bool refreshRequired = false)
    {
        await _repository.UpdateMatchAsync(match);

        WeakReferenceMessenger.Default.Send(new MatchUpdatedMessage(match.Id));
        WeakReferenceMessenger.Default.Send(new TeamUpdatedMessage(match.HomeTeam));
        WeakReferenceMessenger.Default.Send(new TeamUpdatedMessage(match.AwayTeam));

        if (refreshRequired)
        {
            WeakReferenceMessenger.Default.Send(new RefreshRequiredMessage());
        }
    }

    public async Task UpdateTeamAsync(Team team)
    {
        await _repository.UpdateTeamAsync(team);

        WeakReferenceMessenger.Default.Send(new TeamUpdatedMessage(team.Name));
    }

    public Task UpdateSettingsAsync(Settings settings)
    {
        return _repository.UpdateSettingsAsync(settings);
    }
}