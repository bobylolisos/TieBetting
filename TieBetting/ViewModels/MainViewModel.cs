namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelNavigationBase
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IRepository _repository;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<Team> _teams;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IRepository repository, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _repository = repository;
        _navigationService = navigationService;
        NavigateToMatchDetailsViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchDetailsViewCommand);
        NavigateToStatisticsViewCommand = new AsyncRelayCommand(ExecuteNavigateToStatisticsViewCommand);
        NavigateToTeamsViewCommand = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        NavigateToMatchesViewCommand = new AsyncRelayCommand(ExecuteNavigateToMatchesViewCommand);
        NavigateToSettingsCommand = new AsyncRelayCommand(ExecuteNavigateToSettingsCommand);
    }

    public List<MatchGroupViewModel> Matches { get; set; } = new();

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchDetailsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToTeamsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToMatchesViewCommand { get; set; }

    public AsyncRelayCommand NavigateToStatisticsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToSettingsCommand { get; set; }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        var settings = await _repository.GetSettingsAsync();
        Matches.Clear();

        _teams = await _repository.GetTeamsAsync();

        var fetchedPreviousMatches = await _repository.GetPreviousOngoingMatchesAsync();

        var previousMatches = new List<MatchViewModel>();
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

            var matchViewModel = new MatchViewModel(_repository, settings, previousMatch, homeTeam, awayTeam);

            previousMatches.Add(matchViewModel);
        }

        if (previousMatches.Any())
        {
            Matches.Add(new MatchGroupViewModel("Previous", previousMatches));
        }


        var fetchedUpcomingMatches = await _repository.GetNextMatchesAsync(settings.UpcomingFetchCount);

        var todayDay = DayProvider.TodayDay;
        Debug.WriteLine("");
        Debug.WriteLine($"Today day is: {todayDay}");
        Debug.WriteLine("");

        var todayMatches = new List<MatchViewModel>();
        var upcomingMatches = new List<MatchViewModel>();
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

            var matchViewModel = new MatchViewModel(_repository, settings, match, homeTeam, awayTeam);

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
            Matches.Add(new MatchGroupViewModel($"{DateTime.Today:yyyy-MM-dd}   Today", todayMatches));
        }
        else
        {
            Matches.Add(new MatchGroupViewModel($"{DateTime.Today:yyyy-MM-dd}   Today, no matches", new List<MatchViewModel>()));

        }
        if (upcomingMatches.Any())
        {
            var groupedUpcomingMatches = upcomingMatches.GroupBy(x => x.Date).Select(x => x.ToList());
            foreach (var groupedUpcomingMatch in groupedUpcomingMatches)
            {
                Matches.Add(new MatchGroupViewModel(groupedUpcomingMatch[0].Date, groupedUpcomingMatch));
            }
        }

        if (Matches.Any() == false)
        {
            Matches.Add(new MatchGroupViewModel("No matches", new List<MatchViewModel>()));
        }
    }

    private async Task ExecuteNavigateToMatchDetailsViewCommand(MatchViewModel viewModel)
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

    private Task ExecuteNavigateToMatchesViewCommand()
    {
        return Task.CompletedTask;
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
}