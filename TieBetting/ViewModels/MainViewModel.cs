namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelNavigationBase
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<Team> _teams;
    private bool _isRefreshing;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IQueryService queryService, ISaverService saverService, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _queryService = queryService;
        _saverService = saverService;
        _navigationService = navigationService;
        RefreshCommand = new AsyncRelayCommand(ExecuteRefreshCommand);
        NavigateToMatchDetailsViewCommand = new AsyncRelayCommand<MatchBettingViewModel>(ExecuteNavigateToMatchDetailsViewCommand);
        NavigateToStatisticsViewCommand = new AsyncRelayCommand(ExecuteNavigateToStatisticsViewCommand);
        NavigateToTeamsViewCommand = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        NavigateToAllMatchesViewCommand = new AsyncRelayCommand(ExecuteNavigateToAllMatchesViewCommand);
        NavigateToSettingsCommand = new AsyncRelayCommand(ExecuteNavigateToSettingsCommand);
    }

    public ObservableCollection<MatchBettingGroupViewModel> Matches { get; set; } = new();

    public AsyncRelayCommand RefreshCommand { get; set; }

    public AsyncRelayCommand<MatchBettingViewModel> NavigateToMatchDetailsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToTeamsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToAllMatchesViewCommand { get; set; }

    public AsyncRelayCommand NavigateToStatisticsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToSettingsCommand { get; set; }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        await Reload();
    }

    private async Task ExecuteRefreshCommand()
    {
        try
        {
            _saverService.ClearCache();

            await Reload();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task ExecuteNavigateToMatchDetailsViewCommand(MatchBettingViewModel viewModel)
    {
        await _navigationService.NavigateToPageAsync<MatchDetailsView>(new MatchDetailsViewNavigationParameter(viewModel));
    }

    private async Task ExecuteNavigateToStatisticsViewCommand()
    {
        await _navigationService.NavigateToPageAsync<StatisticsView>();
    }

    private async Task ExecuteNavigateToTeamsViewCommand()
    {
        await _navigationService.NavigateToPageAsync<TeamsView>(new TeamsViewNavigationParameter(_teams));
    }

    private async Task ExecuteNavigateToAllMatchesViewCommand()
    {
        await _navigationService.NavigateToPageAsync<AllMatchesView>();
    }

    private async Task ExecuteNavigateToSettingsCommand()
    {
        await _navigationService.NavigateToPageAsync<SettingsView>();
    }

    private async Task ImportCalendarToDatabase()
    {
        //var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        var href = "https://calendar.ramses.nu/calendar/748/show/shl-2022-2023.ics";

        var matches = await _calendarFileDownloadService.DownloadAsync(href);

        var dateTime = new DateTime(2022, 12, 15);
        var latestMatches = matches.Where(x => x.Date > dateTime).ToList();
        await _saverService.AddMatchesAsync(latestMatches);
    }

    private async Task ExecuteMyCommand()
    {
        await Task.Delay(1);

        //await ImportCalendarToDatabase();
    }

    private async Task Reload()
    {
        var settings = await _queryService.GetSettingsAsync();
        Matches.Clear();

        _teams = await _queryService.GetTeamsAsync();

        var fetchedPreviousMatches = await _queryService.GetPreviousOngoingMatchesAsync();

        var previousMatches = new List<MatchBettingViewModel>();
        foreach (var previousMatch in fetchedPreviousMatches.OrderBy(x => x.Day))
        {
            var homeTeam = _teams.GetTeamOrDefault(previousMatch.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _saverService.CreateTeamAsync(previousMatch.HomeTeam);
            }

            var awayTeam = _teams.GetTeamOrDefault(previousMatch.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _saverService.CreateTeamAsync(previousMatch.AwayTeam);
            }

            var matchViewModel = new MatchBettingViewModel(_saverService, settings, previousMatch, homeTeam, awayTeam);

            previousMatches.Add(matchViewModel);
        }

        if (previousMatches.Any())
        {
            Matches.Add(new MatchBettingGroupViewModel("Previous", previousMatches));
        }


        var fetchedUpcomingMatches = await _queryService.GetNextMatchesAsync(settings.UpcomingFetchCount);

        var todayDay = DayProvider.TodayDay;
        Debug.WriteLine("");
        Debug.WriteLine($"Today day is: {todayDay}");
        Debug.WriteLine("");

        var todayMatches = new List<MatchBettingViewModel>();
        var upcomingMatches = new List<MatchBettingViewModel>();
        foreach (var match in fetchedUpcomingMatches.OrderBy(x => x.Day))
        {
            var homeTeam = _teams.GetTeamOrDefault(match.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _saverService.CreateTeamAsync(match.HomeTeam);
            }

            var awayTeam = _teams.GetTeamOrDefault(match.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _saverService.CreateTeamAsync(match.AwayTeam);
            }

            var matchViewModel = new MatchBettingViewModel(_saverService, settings, match, homeTeam, awayTeam);

            if (match.Day == todayDay)
            {
                todayMatches.Add(matchViewModel);
            }
            else
            {
                upcomingMatches.Add(matchViewModel);
            }
        }

        if (todayMatches.Any())
        {
            Matches.Add(new MatchBettingGroupViewModel($"{DateTime.Today:yyyy-MM-dd}   Today", todayMatches));
        }
        else
        {
            Matches.Add(new MatchBettingGroupViewModel($"{DateTime.Today:yyyy-MM-dd}   Today, no matches",
                new List<MatchBettingViewModel>()));
        }

        if (upcomingMatches.Any())
        {
            var groupedUpcomingMatches = upcomingMatches.GroupBy(x => x.Date).Select(x => x.ToList());
            foreach (var groupedUpcomingMatch in groupedUpcomingMatches)
            {
                Matches.Add(new MatchBettingGroupViewModel(groupedUpcomingMatch[0].Date, groupedUpcomingMatch));
            }
        }

        if (Matches.Any() == false)
        {
            Matches.Add(new MatchBettingGroupViewModel("No matches", new List<MatchBettingViewModel>()));
        }
    }

}