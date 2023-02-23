﻿namespace TieBetting.ViewModels.NavigationViewModels;

public class MainViewModel : ViewModelNavigationBase, IRecipient<RefreshRequiredMessage>
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<TeamViewModel> _teams;
    private bool _isReloading;
    private bool _refreshRequired = true;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IQueryService queryService, ISaverService saverService, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _queryService = queryService;
        _saverService = saverService;
        _navigationService = navigationService;
        NavigateToMatchDetailsViewCommand = new AsyncRelayCommand<MatchBettingViewModel>(ExecuteNavigateToMatchDetailsViewCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        TabBarItem2Command = new AsyncRelayCommand(ExecuteNavigateToSeasonMatchesViewCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteNavigateToStatisticsViewCommand);
        TabBarItem4Command = new AsyncRelayCommand(ExecuteNavigateToSettingsCommand);

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public ObservableCollection<MatchBettingViewModel> Matches { get; set; } = new();


    public AsyncRelayCommand<MatchBettingViewModel> NavigateToMatchDetailsViewCommand { get; set; }

    public bool IsReloading
    {
        get => _isReloading;
        set => SetProperty(ref _isReloading, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        await ReloadAsync();
    }

    public override async Task OnNavigatedBackAsync()
    {
        await ReloadAsync();
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

    private async Task ReloadAsync()
    {
        try
        {
            if (_refreshRequired != true)
            {
                return;
            }

            IsReloading = true;

            var settings = await _queryService.GetSettingsAsync();
            Matches.Clear();

            _teams = await _queryService.GetTeamsAsync();

            var previousMatches = await _queryService.GetPreviousOngoingMatchesAsync();

            foreach (var previousMatch in previousMatches)
            {
                Matches.Add(previousMatch);
            }


            var fetchedUpcomingMatches = await _queryService.GetNextMatchesAsync(settings.UpcomingFetchCount);

            var todayDay = DayProvider.TodayDay;
            Debug.WriteLine("");
            Debug.WriteLine($"Today day is: {todayDay}");
            Debug.WriteLine("");

            foreach (var upcomingMatch in fetchedUpcomingMatches)
            {
                Matches.Add(upcomingMatch);
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