namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelNavigationBase
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IRepository _repository;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<Team> _teams;
    private bool _isRefreshing;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IRepository repository, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _repository = repository;
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
            _repository.ClearCache();

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
        await _repository.AddMatchesAsync(latestMatches);
    }

    private async Task ExecuteMyCommand()
    {
        await Task.Delay(1);

        //await ImportCalendarToDatabase();
    }

    private async Task Reload()
    {
        var settings = await _repository.GetSettingsAsync();
        Matches.Clear();

        _teams = await _repository.GetTeamsAsync();

        var fetchedPreviousMatches = await _repository.GetPreviousOngoingMatchesAsync();

        var previousMatches = new List<MatchBettingViewModel>();
        foreach (var previousMatch in fetchedPreviousMatches.OrderBy(x => x.Day))
        {
            var homeTeam = _teams.SingleOrDefault(x => x.Name == previousMatch.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _repository.CreateTeamAsync(previousMatch.HomeTeam);
            }

            var awayTeam = _teams.SingleOrDefault(x => x.Name == previousMatch.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _repository.CreateTeamAsync(previousMatch.AwayTeam);
            }

            var matchViewModel = new MatchBettingViewModel(_repository, settings, previousMatch, homeTeam, awayTeam);

            previousMatches.Add(matchViewModel);
        }

        if (previousMatches.Any())
        {
            Matches.Add(new MatchBettingGroupViewModel("Previous", previousMatches));
        }


        var fetchedUpcomingMatches = await _repository.GetNextMatchesAsync(settings.UpcomingFetchCount);

        var todayDay = DayProvider.TodayDay;
        Debug.WriteLine("");
        Debug.WriteLine($"Today day is: {todayDay}");
        Debug.WriteLine("");

        var todayMatches = new List<MatchBettingViewModel>();
        var upcomingMatches = new List<MatchBettingViewModel>();
        foreach (var match in fetchedUpcomingMatches.OrderBy(x => x.Day))
        {
            var homeTeam = _teams.SingleOrDefault(x => x.Name == match.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _repository.CreateTeamAsync(match.HomeTeam);
            }

            var awayTeam = _teams.SingleOrDefault(x => x.Name == match.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _repository.CreateTeamAsync(match.AwayTeam);
            }

            var matchViewModel = new MatchBettingViewModel(_repository, settings, match, homeTeam, awayTeam);

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