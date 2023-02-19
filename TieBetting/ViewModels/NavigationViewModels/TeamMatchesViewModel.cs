namespace TieBetting.ViewModels.NavigationViewModels;

public class TeamMatchesViewModel : ViewModelNavigationBase
{
    private string _headerText;
    private string _headerImage;
    private string _selectedSeason;
    private IReadOnlyCollection<MatchViewModel> _allTeamMatches;
    private TeamViewModel _team;

    public TeamMatchesViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteToggleActiveStatusCommand, CanExecuteToggleActiveStatusCommand);
        TabBarItem4Command = new AsyncRelayCommand(ExecuteAbandonCommand);
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

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamMatchesViewNavigationParameter parameter)
        {
            var team = parameter.Team;
            HeaderImage = team.Image;
            HeaderText = team.Name;
            _team = team;
            UpdateLabelAndImageOnTabBarItem3();
        }
        else
        {
            throw new ArgumentException("Expected TeamMatchesViewNavigationParameter but not found!");
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

        await base.OnNavigatingToAsync(navigationParameter);

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
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");
    }
}