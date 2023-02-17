namespace TieBetting.Services;

public class QueryService : IQueryService
{
    private readonly IFirestoreRepository _repository;
    private readonly ISaverService _saverService;

    private List<TeamViewModel> _teams;
    private List<MatchBettingViewModel> _matches;
    private Settings _settings;

    public QueryService(IFirestoreRepository repository, ISaverService saverService)
    {
        _repository = repository;
        _saverService = saverService;
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

    public async Task<IReadOnlyCollection<MatchBettingViewModel>> GetMatchesAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _matches.OrderBy(x => x.Day).ToList();
    }

    public async Task<IReadOnlyCollection<MatchBettingViewModel>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        await EnsureDatabaseIsLoaded();

        var upcomingMatches = _matches.Where(x => x.Day >= DayProvider.TodayDay);
        if (numberOfMatches.HasValue)
        {
            return upcomingMatches.Take(numberOfMatches.Value).ToList();

        }
        return upcomingMatches.OrderBy(x => x.Day).ToList();
    }

    public async Task<IReadOnlyCollection<MatchBettingViewModel>> GetPreviousOngoingMatchesAsync()
    {
        await EnsureDatabaseIsLoaded();

        return _matches.Where(x => x.Day < DayProvider.TodayDay && x.IsActive()).OrderBy(x => x.Day).ToList();
    }

    private async Task EnsureDatabaseIsLoaded()
    {
        if (_settings == null || _teams == null || _matches == null)
        {
            _settings = await _repository.GetSettingsAsync();

            var teamsList = new List<TeamViewModel>();
            var matchesList = new List<MatchBettingViewModel>();

            var teams = await _repository.GetTeamsAsync();
            foreach (var team in teams)
            {
                teamsList.Add(new TeamViewModel(team));
            }

            var matches = await _repository.GetMatchesAsync();
            foreach (var match in matches.OrderBy(x => x.Day))
            {
                var homeTeam = teamsList.GetTeamOrDefault(match.HomeTeam);

                if (homeTeam == null)
                {
                    var savedHomeTeam = await _saverService.CreateTeamAsync(match.HomeTeam);
                    homeTeam = new TeamViewModel(savedHomeTeam);
                    teamsList.Add(homeTeam);
                }

                var awayTeam = teamsList.GetTeamOrDefault(match.AwayTeam);
                if (awayTeam == null)
                {
                    var savedAwayTeam = await _saverService.CreateTeamAsync(match.AwayTeam);
                    awayTeam = new TeamViewModel(savedAwayTeam);
                    teamsList.Add(awayTeam);
                }

                var matchViewModel = new MatchBettingViewModel(_saverService, _settings, match, homeTeam, awayTeam);

                homeTeam.AddMatch(matchViewModel);
                awayTeam.AddMatch(matchViewModel);

                matchesList.Add(matchViewModel);
            }

            teamsList.ForEach(x => x.ReCalculateValues());

            _teams = new List<TeamViewModel>(teamsList.OrderByTeamName());
            _matches = new List<MatchBettingViewModel>(matchesList.OrderBy(x => x.Day));
        }

    }

}