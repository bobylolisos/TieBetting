namespace TieBetting.ViewModels.NavigationViewModels;

public class TeamMaintenanceViewModel : ViewModelNavigationBase, IRecipient<TeamUpdatedMessage>, IRecipient<MatchUpdatedMessage>
{
    private readonly IQueryService _queryService;
    private readonly IMessenger _messenger;
    private string _headerText;
    private string _headerImage;
    private string _selectedSeason;
    private IReadOnlyCollection<MatchViewModel> _allTeamMatches;
    private TeamViewModel _team;

    public TeamMaintenanceViewModel(INavigationService navigationService, IQueryService queryService, IMessenger messenger)
        : base(navigationService)
    {
        _queryService = queryService;
        _messenger = messenger;
        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteToggleActiveStatusCommand, CanExecuteToggleActiveStatusCommand);
        TabBarItem4Command = new AsyncRelayCommand(ExecuteAbandonCommand, CanExecuteAbandonCommand);
    }

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchMaintenanceViewCommand { get; }

    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchViewModel> Matches { get; set; } = new();

    public bool IsDormant => _team != null && _team.IsDormant;

    public string HeaderImage
    {
        get => _headerImage;
        set => SetProperty(ref _headerImage, value);
    }

    public string HeaderText
    {
        get => _headerText;
        set => SetProperty(ref _headerText, value);
    }

    public string SelectedSeason
    {
        get => _selectedSeason;
        set
        {
            if (SetProperty(ref _selectedSeason, value))
            {
                Matches.Clear();
                var seasonMatches = _allTeamMatches.Where(x => x.Season == SelectedSeason).OrderBy(x => x.Day);
                foreach (var match in seasonMatches)
                {
                    Matches.Add(match);
                }
            }
        }
    }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamMaintenanceViewNavigationParameter parameter)
        {
            var team = parameter.Team;
            HeaderImage = team.Image;
            HeaderText = team.Name;
            _team = team;
            UpdateLabelAndImageOnTabBarItem3();
        }
        else
        {
            throw new ArgumentException("Expected TeamMaintenanceViewNavigationParameter but not found!");
        }

        _allTeamMatches = _team.Matches;

        var seasons = _allTeamMatches.Select(x => x.Season).Distinct().OrderBy(x => x);

        foreach (var season in seasons)
        {
            Seasons.Add(season);
        }

        SelectedSeason = Seasons.Last();

        OnPropertyChanged(nameof(IsDormant));
        NotifyTabItemsCanExecuteChanged();
        TabBarItem4Command.NotifyCanExecuteChanged();

        _messenger.RegisterAll(this);

        return Task.CompletedTask;
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
        var teams = await _queryService.GetTeamsAsync();
        _team = teams.Single(x => x.Name == _team.Name);
        _allTeamMatches = _team.Matches;

        var selectedSeason = SelectedSeason;
        SelectedSeason = null;
        SelectedSeason = selectedSeason;
    }

    private void UpdateLabelAndImageOnTabBarItem3()
    {
        TabBarItem3Label = IsDormant ? "Activate" : "To dormant";
        TabBarItem3Image = IsDormant ? "play.svg" : "pause.svg";
    }

    private async Task ExecuteNavigateToMatchMaintenanceViewCommand(MatchViewModel matchViewModel)
    {
        await NavigationService.NavigateToPageAsync<MatchMaintenanceView>(new MatchMaintenanceViewNavigationParameter(matchViewModel));
    }

    private async Task ExecuteToggleActiveStatusCommand()
    {
        if (_team.Matches.Any(x => x.IsActive(_team.Name)))
        {
            await Application.Current.MainPage.DisplayAlert(TabBarItem3Label, "You have an active match. Complete that match first and then try again.", "Ok");
            return;
        }

        await _team.ToggleActiveStatusAsync();
        UpdateLabelAndImageOnTabBarItem3();
        OnPropertyChanged(nameof(IsDormant));
        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteToggleActiveStatusCommand()
    {
        return _team != null;
    }

    private async Task ExecuteAbandonCommand()
    {
        var lastLostMatch = GetLastLostMatch();

        await lastLostMatch.SetAbandonAsync(_team);
    }

    private bool CanExecuteAbandonCommand()
    {
        if (_team == null)
        {
            return false;
        }

        var lastLostMatch = GetLastLostMatch();

        if (lastLostMatch == null)
        {
            return false;
        }

        return true;
    }

    private MatchViewModel GetLastLostMatch()
    {
        var lastDoneMatch = _team.Matches.LastOrDefault(x => x.IsAnyDone());
        if (lastDoneMatch != null && lastDoneMatch.IsAnyLost())
        {
            if (lastDoneMatch.HomeTeam == _team && lastDoneMatch.IsLost(TeamType.HomeTeam))
            {
                return lastDoneMatch;
            }
            if (lastDoneMatch.AwayTeam == _team && lastDoneMatch.IsLost(TeamType.AwayTeam))
            {
                return lastDoneMatch;
            }
        }

        return null;
    }

    public void Receive(TeamUpdatedMessage message)
    {
        NotifyTabItemsCanExecuteChanged();
    }

    public void Receive(MatchUpdatedMessage message)
    {
        NotifyTabItemsCanExecuteChanged();
    }
}