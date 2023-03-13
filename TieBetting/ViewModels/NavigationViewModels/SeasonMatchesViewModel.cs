namespace TieBetting.ViewModels.NavigationViewModels;

public class SeasonMatchesViewModel : ViewModelNavigationBase, IRecipient<MatchCreatedMessage>
{
    private readonly IQueryService _queryService;
    private readonly IPopupService _popupService;
    private readonly IMessenger _messenger;
    private Settings _settings;
    private string _selectedSeason;
    private IReadOnlyCollection<MatchViewModel> _allMatches;
    private IReadOnlyCollection<TeamViewModel> _allTeams;

    public SeasonMatchesViewModel(INavigationService navigationService, IQueryService queryService, IPopupService popupService, IMessenger messenger)
        : base(navigationService)
    {
        _queryService = queryService;
        _popupService = popupService;
        _messenger = messenger;

        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteNavigateToAddMatchViewCommand);
    }

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchMaintenanceViewCommand { get; }


    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchViewModel> Matches { get; set; } = new();

    public int TotalBet => Matches.Sum(y => y.GetActivatedTotalBet());

    public int TotalWin => (int)Matches.Sum(y => y.TotalWin ?? 0);

    public int CurrentBetSession { get; private set; }

    public int AbandonedBets { get; private set; }

    public int Profit => TotalWin - TotalBet;

    public string MatchesWonPercent
    {
        get
        {
            var matchesCount = Matches.Count(x => x.IsAnyDone());
            var matchesWonCount = Matches.Count(x => x.IsAnyWin());
            if (matchesWonCount == 0)
            {
                return "0 %";
            }

            var percent = (int)(matchesWonCount / (double)matchesCount * 100);
            return $"{percent} %";
        }
    }
    public string SelectedSeason
    {
        get => _selectedSeason;
        set
        {
            if (SetProperty(ref _selectedSeason, value))
            {
                Matches.Clear();

                var seasonMatches = _allMatches.Where(x => x.Season == SelectedSeason);
                foreach (var matchViewModel in seasonMatches)
                {
                    Matches.Add(matchViewModel);
                }

                CalculateValues();

                OnPropertyChanged(nameof(TotalBet));
                OnPropertyChanged(nameof(TotalWin));
                OnPropertyChanged(nameof(Profit));
                OnPropertyChanged(nameof(MatchesWonPercent));
                OnPropertyChanged(nameof(CurrentBetSession));
            }
        }
    }


    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        Seasons.Clear();

        _settings = await _queryService.GetSettingsAsync();

        _allTeams = await _queryService.GetTeamsAsync();

        _allMatches = await _queryService.GetMatchesAsync();

        var seasons = _allMatches.Select(x => x.Season).Distinct().OrderBy(x => x);

        foreach (var season in seasons)
        {
            Seasons.Add(season);
        }

        SelectedSeason = Seasons.Single(x => x == _settings.DefaultSeason);

        _messenger.RegisterAll(this);

        await base.OnNavigatingToAsync(navigationParameter);
    }

    public override Task OnNavigatedFromAsync(bool isForwardNavigation)
    {
        if (!isForwardNavigation)
        {
            _messenger.UnregisterAll(this);
        }

        return Task.CompletedTask;
    }

    public override async Task OnNavigatedBackAsync()
    {
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        _allMatches = await _queryService.GetMatchesAsync();

        var selectedSeason = SelectedSeason;
        SelectedSeason = null;
        SelectedSeason = selectedSeason;
    }

    private void CalculateValues()
    {
        CurrentBetSession = 0;
        AbandonedBets = 0;

        if (_allTeams == null)
        {
            return;
        }

        var currentSession = 0;
        var abandonedBets = 0;
        var abandonSession = false;
        foreach (var team in _allTeams)
        {
            var currentSessionDone = false;
            var matchesForTeam = Matches.Where(y => y.HomeTeamName == team.Name || y.AwayTeamName == team.Name).OrderByDescending(x => x.Day);

            foreach (var match in matchesForTeam)
            {
                if (match.HomeTeamName == team.Name)
                {
                    if (currentSessionDone == false)
                    {
                        if (match.IsWin(TeamType.HomeTeam) || match.IsAbandoned(TeamType.HomeTeam))
                        {
                            currentSessionDone = true;
                        }
                        else
                        {
                            currentSession += match.GetActivatedHomeTeamBet();
                        }
                    }

                    if (match.IsAbandoned(TeamType.HomeTeam) || abandonSession)
                    {
                        if (match.IsWin(TeamType.HomeTeam))
                        {
                            abandonSession = false;
                        }
                        else if (match.IsLost(TeamType.HomeTeam) || match.IsAbandoned(TeamType.HomeTeam))
                        {
                            abandonedBets += match.GetActivatedHomeTeamBet();
                            abandonSession = true;
                        }
                    }
                }

                if (match.AwayTeamName == team.Name)
                {
                    if (currentSessionDone == false)
                    {
                        if (match.IsWin(TeamType.AwayTeam) || match.IsAbandoned(TeamType.AwayTeam))
                        {
                            currentSessionDone = true;
                        }
                        else
                        {
                            currentSession += match.GetActivatedAwayTeamBet();
                        }
                    }

                    if (match.IsAbandoned(TeamType.AwayTeam) || abandonSession)
                    {
                        if (match.IsWin(TeamType.AwayTeam))
                        {
                            abandonSession = false;
                        }
                        else if (match.IsLost(TeamType.AwayTeam) || match.IsAbandoned(TeamType.AwayTeam))
                        {
                            abandonedBets += match.GetActivatedAwayTeamBet();
                            abandonSession = true;
                        }
                    }
                }
            }

        }

        CurrentBetSession = currentSession;
        AbandonedBets = abandonedBets;
        OnPropertyChanged(nameof(CurrentBetSession));
        OnPropertyChanged(nameof(AbandonedBets));
    }

    private async Task ExecuteNavigateToMatchMaintenanceViewCommand(MatchViewModel matchViewModel)
    {
        await NavigationService.NavigateToPageAsync<MatchMaintenanceView>(new MatchMaintenanceViewNavigationParameter(matchViewModel));
    }

    private async Task ExecuteNavigateToAddMatchViewCommand()
    {
        await _popupService.OpenPopupAsync<EditMatchPopupView>(new AddMatchPopupParameter(SelectedSeason));
    }

    public async void Receive(MatchCreatedMessage message)
    {
        await ReloadAsync();
    }

}