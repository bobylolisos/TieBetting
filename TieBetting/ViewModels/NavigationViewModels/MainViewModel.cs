namespace TieBetting.ViewModels.NavigationViewModels;

public class MainViewModel : ViewModelNavigationBase, IRecipient<RefreshRequiredMessage>
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<TeamViewModel> _teams;
    private bool _isRefreshing;
    private bool _isReloading;
    private bool _refreshRequired = true;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IQueryService queryService, ISaverService saverService, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _queryService = queryService;
        _saverService = saverService;
        _navigationService = navigationService;
        RefreshCommand = new AsyncRelayCommand(ExecuteRefreshCommand);
        NavigateToMatchDetailsViewCommand = new AsyncRelayCommand<MatchBettingViewModel>(ExecuteNavigateToMatchDetailsViewCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        TabBarItem2Command = new AsyncRelayCommand(ExecuteNavigateToSeasonMatchesViewCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteNavigateToStatisticsViewCommand);
        TabBarItem4Command = new AsyncRelayCommand(ExecuteNavigateToSettingsCommand);

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public ObservableCollection<MatchBettingGroupViewModel> Matches { get; set; } = new();

    public AsyncRelayCommand RefreshCommand { get; set; }

    public AsyncRelayCommand<MatchBettingViewModel> NavigateToMatchDetailsViewCommand { get; set; }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public bool IsReloading
    {
        get => _isReloading;
        set => SetProperty(ref _isReloading, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        await Reload();
    }

    public override async Task OnNavigatedBackAsync()
    {
        await Reload();
    }

    private async Task ExecuteRefreshCommand()
    {
        IsRefreshing = false;
        _refreshRequired = true;
        await Reload();
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

    private async Task ExecuteNavigateToSeasonMatchesViewCommand()
    {
        await _navigationService.NavigateToPageAsync<SeasonMatchesView>();
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

        var startDay = DayProvider.GetDay(new DateTime(2022, 12, 15));
        var latestMatches = matches.Where(x => x.Day > startDay).ToList();
        await _saverService.AddMatchesAsync(latestMatches);
    }

    private async Task ExecuteMyCommand()
    {
        await Task.Delay(1);

        //await ImportCalendarToDatabase();
    }

    private async Task Reload()
    {
        try
        {
            if (_refreshRequired != true)
            {
                return;
            }
            _queryService.ClearCache();

            IsReloading = true;

            var settings = await _queryService.GetSettingsAsync();
            Matches.Clear();

            _teams = await _queryService.GetTeamsAsync();

            var previousMatches = await _queryService.GetPreviousOngoingMatchesAsync();

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
            foreach (var match in fetchedUpcomingMatches)
            {
                if (match.Day == todayDay)
                {
                    todayMatches.Add(match);
                }
                else
                {
                    upcomingMatches.Add(match);
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

            _refreshRequired = false;
        }
        finally
        {
            IsReloading = false;
        }
    }

    public void Receive(RefreshRequiredMessage message)
    {
        _refreshRequired = true;
        Matches.Clear();
    }
}