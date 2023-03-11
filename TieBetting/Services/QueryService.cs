namespace TieBetting.Services;

public class QueryService : IQueryService, IRecipient<MatchCreatedMessage>, IRecipient<MatchDeletedMessage>
{
    private readonly IFirestoreRepository _repository;
    private readonly ISaverService _saverService;
    private readonly IMessenger _messenger;

    private List<TeamViewModel> _teams;
    private List<MatchViewModel> _matches;
    private Settings _settings;

    public QueryService(IFirestoreRepository repository, ISaverService saverService, IMessenger messenger)
    {
        _repository = repository;
        _saverService = saverService;
        _messenger = messenger;

        _messenger.RegisterAll(this);
    }

    public void ClearCache()
    {
        _settings = null;
        _teams = null;
        _matches = null;
    }

    public async Task<Settings> GetSettingsAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _settings;
    }

    public async Task<IReadOnlyCollection<TeamViewModel>> GetTeamsAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _teams;
    }

    public async Task<IReadOnlyCollection<MatchViewModel>> GetMatchesAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _matches.OrderBy(x => x.Day).ToList();
    }

    public async Task<IReadOnlyCollection<MatchViewModel>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        await EnsureDatabaseIsLoaded();

        var upcomingMatches = _matches.OrderBy(x => x.Day).Where(x => x.Day >= DayProvider.TodayDay);
        if (numberOfMatches.HasValue)
        {
            return upcomingMatches.Take(numberOfMatches.Value).ToList();

        }
        return upcomingMatches.OrderBy(x => x.Day).ToList();
    }

    public async Task<IReadOnlyCollection<MatchViewModel>> GetPreviousOngoingMatchesAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _matches.Where(x => x.Day < DayProvider.TodayDay && x.IsAnyActive()).OrderBy(x => x.Day).ToList();
    }

    private async Task EnsureDatabaseIsLoaded()
    {
        if (_settings == null || _teams == null || _matches == null)
        {
            _settings = await _repository.GetSettingsAsync();

            var teamsList = new List<TeamViewModel>();
            var matchesList = new List<MatchViewModel>();

            var teams = await _repository.GetTeamsAsync();
            foreach (var team in teams)
            {
                teamsList.Add(new TeamViewModel(_messenger, _saverService, team));
            }

            var matches = await _repository.GetMatchesAsync();
            matches = matches.Where(x => x.HomeTeam == "Aik" || x.AwayTeam == "Aik" || x.HomeTeam == "Björklöven" || x.AwayTeam == "Björklöven").ToList();
            foreach (var match in matches.OrderBy(x => x.Day))
            {
                var homeTeamViewModel = teamsList.GetTeamOrDefault(match.HomeTeam);

                if (homeTeamViewModel == null)
                {
                    var homeTeam = await _saverService.CreateTeamAsync(match.HomeTeam);
                    homeTeamViewModel = new TeamViewModel(_messenger, _saverService, homeTeam);

                    teamsList.Add(homeTeamViewModel);
                }

                var awayTeamViewModel = teamsList.GetTeamOrDefault(match.AwayTeam);
                if (awayTeamViewModel == null)
                {
                    var awayTeam = await _saverService.CreateTeamAsync(match.AwayTeam);
                    awayTeamViewModel = new TeamViewModel(_messenger, _saverService, awayTeam);
                    teamsList.Add(awayTeamViewModel);
                }

                var matchViewModel = new MatchViewModel(_messenger, _saverService, _settings, match, homeTeamViewModel, awayTeamViewModel);

                homeTeamViewModel.AddMatch(matchViewModel);
                awayTeamViewModel.AddMatch(matchViewModel);

                matchesList.Add(matchViewModel);
            }

            teamsList.ForEach(x => x.ReCalculateValues());

            _teams = new List<TeamViewModel>(teamsList.OrderByTeamName());
            _matches = new List<MatchViewModel>(matchesList.OrderBy(x => x.Day));
        }

    }

    public void Receive(MatchCreatedMessage message)
    {
        var match = message.Match;

        var homeTeam = _teams.GetTeamOrDefault(match.HomeTeam);
        var awayTeam = _teams.GetTeamOrDefault(match.AwayTeam);

        var matchViewModel = new MatchViewModel(_messenger, _saverService, _settings, match, homeTeam, awayTeam);
        homeTeam.AddMatch(matchViewModel);
        awayTeam.AddMatch(matchViewModel);

        _matches.Add(matchViewModel);
    }

    public void Receive(MatchDeletedMessage message)
    {
        var match = _matches.Single(x => x.IsEqual(message.MatchId));
        _matches.Remove(match);

        foreach (var team in _teams)
        {
            team.RemoveMatch(message.MatchId);
        }
    }
}