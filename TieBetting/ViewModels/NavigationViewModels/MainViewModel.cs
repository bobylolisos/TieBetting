﻿namespace TieBetting.ViewModels.NavigationViewModels;

public class MainViewModel : ViewModelNavigationBase, IRecipient<RefreshRequiredMessage>, IRecipient<MatchUpdatedMessage>, IRecipient<MatchDeletedMessage>, IRecipient<SettingsUpdatedMessage>
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private readonly INavigationService _navigationService;
    private ObservableCollection<TeamViewModel> _teams;
    private bool _isReloading;

    public MainViewModel(IMessenger messenger, ICalendarFileDownloadService calendarFileDownloadService, IQueryService queryService, ISaverService saverService, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _queryService = queryService;
        _saverService = saverService;
        _navigationService = navigationService;
        NavigateToMatchBettingViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchBettingViewCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        TabBarItem2Command = new AsyncRelayCommand(ExecuteNavigateToSeasonMatchesViewCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteNavigateToStatisticsViewCommand);
        TabBarItem4Command = new AsyncRelayCommand(ExecuteNavigateToSettingsCommand);

        messenger.RegisterAll(this);
    }

    public ObservableCollection<MatchViewModel> Matches { get; set; } = new();


    public AsyncRelayCommand<MatchViewModel> NavigateToMatchBettingViewCommand { get; set; }

    public bool IsReloading
    {
        get => _isReloading;
        set => SetProperty(ref _isReloading, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        await ReloadAsync();
    }

    private async Task ExecuteNavigateToMatchBettingViewCommand(MatchViewModel viewModel)
    {
        await _navigationService.NavigateToPageAsync<MatchBettingView>(new MatchBettingViewNavigationParameter(viewModel));
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
        await _navigationService.NavigateToPageAsync<SeasonView>();
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
            IsReloading = true;

            Matches.Clear();

            var settings = await _queryService.GetSettingsAsync();

            var readOnlyCollection = await _queryService.GetTeamsAsync();
            _teams = new ObservableCollection<TeamViewModel>(readOnlyCollection);

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
        }
        finally
        {
            IsReloading = false;
        }
    }

    public async void Receive(RefreshRequiredMessage message)
    {
        await ReloadAsync();
    }

    public void Receive(MatchDeletedMessage message)
    {
        var match = Matches.SingleOrDefault(x => x.IsEqual(message.MatchId));

        if (match != null)
        {
            Matches.Remove(match);
        }
    }

    public void Receive(MatchUpdatedMessage message)
    {
        var match = Matches.SingleOrDefault(x => x.IsEqual(message.MatchId));

        if (match != null)
        {
            Matches.Remove(match);
            Matches.Add(match);
        }
    }

    public async void Receive(SettingsUpdatedMessage message)
    {
        await ReloadAsync();
    }
}