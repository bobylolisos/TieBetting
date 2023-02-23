namespace TieBetting.ViewModels.NavigationViewModels;

public class SeasonMatchesViewModel : ViewModelNavigationBase, IPubSub<MatchCreatedMessage>, IPubSub<MatchUpdatedMessage>
{
    private readonly IQueryService _queryService;
    private readonly IPopupService _popupService;
    private Settings _settings;
    private string _selectedSeason;
    private IReadOnlyCollection<MatchViewModel> _allMatches;
    private IReadOnlyCollection<TeamViewModel> _allTeams;

    public SeasonMatchesViewModel(INavigationService navigationService, IQueryService queryService, IPopupService popupService)
        : base(navigationService)
    {
        _queryService = queryService;
        _popupService = popupService;

        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteNavigateToAddMatchViewCommand);
    }

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchMaintenanceViewCommand { get; }


    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchViewModel> Matches { get; set; } = new();

    public int TotalBet => Matches.Sum(y => y.GetActivatedTotalBet());

    public int TotalWin => (int)Matches.Sum(y => y.TotalWin ?? 0);

    public int CurrentBetSession => ResolveCurrentBetSession();

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



        await base.OnNavigatingToAsync(navigationParameter);
    }

    private int ResolveCurrentBetSession()
    {
        if (_allTeams == null)
        {
            return 0;
        }

        var currentSession = 0;
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
                        if (match.IsWin(TeamType.HomeTeam))
                        {
                            currentSessionDone = true;
                        }
                        else
                        {
                            currentSession += match.GetActivatedHomeTeamBet();
                        }
                    }
                }

                if (match.AwayTeamName == team.Name)
                {
                    if (currentSessionDone == false)
                    {
                        if (match.IsWin(TeamType.AwayTeam))
                        {
                            currentSessionDone = true;
                        }
                        else
                        {
                            currentSession += match.GetActivatedAwayTeamBet();
                        }
                    }
                }
            }

        }

        return currentSession;
    }

    private async Task ExecuteNavigateToMatchMaintenanceViewCommand(MatchViewModel matchViewModel)
    {
        await NavigationService.NavigateToPageAsync<MatchMaintenanceView>(new MatchMaintenanceViewNavigationParameter(matchViewModel));
    }

    private async Task ExecuteNavigateToAddMatchViewCommand()
    {
        await _popupService.OpenPopupAsync<EditMatchPopupView>(new AddMatchPopupParameter(SelectedSeason));
    }

    public void RegisterMessages()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void UnregisterMessages()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    public void Receive(MatchCreatedMessage message)
    {
        Matches.Add(message.Match);
    }

    public void Receive(MatchUpdatedMessage message)
    {
        var item = Matches.FirstOrDefault(x => x.Id == message.MatchId);
        Matches.Remove(item);
        Matches.Add(item);
    }
}