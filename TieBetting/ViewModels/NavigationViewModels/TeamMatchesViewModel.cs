namespace TieBetting.ViewModels.NavigationViewModels;

public class TeamMatchesViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private string _headerText;
    private string _headerImage;
    private string _selectedSeason;
    private IReadOnlyCollection<Team> _allTeams;
    private IReadOnlyCollection<Match> _allTeamMatches;
    private Team _team;

    public TeamMatchesViewModel(INavigationService navigationService, IQueryService queryService, ISaverService saverService)
        : base(navigationService)
    {
        _queryService = queryService;
        _saverService = saverService;

        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
        AbandonSessionCommand = new AsyncRelayCommand(ExecuteAbandonSessionCommand);
    }

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchMaintenanceViewCommand { get; }

    public AsyncRelayCommand AbandonSessionCommand { get; }

    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchViewModel> Matches { get; set; } = new();

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
                    Matches.Add(new MatchViewModel(_saverService, match, _allTeams.GetTeam(match.HomeTeam), _allTeams.GetTeam(match.AwayTeam)));
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
        }
        else
        {
            throw new ArgumentException("Expected TeamMatchesViewNavigationParameter but not found!");
        }

        _allTeams = await _queryService.GetTeamsAsync();

        _allTeamMatches = _team.Matches;

        var seasons = _allTeamMatches.Select(x => x.Season).Distinct().OrderBy(x => x);

        foreach (var season in seasons)
        {
            Seasons.Add(season);
        }

        SelectedSeason = Seasons.Last();


        await base.OnNavigatingToAsync(navigationParameter);

    }

    private async Task ExecuteNavigateToMatchMaintenanceViewCommand(MatchViewModel matchViewModel)
    {
        await NavigationService.NavigateToPageAsync<MatchMaintenanceView>(new MatchMaintenanceViewNavigationParameter(matchViewModel));
    }

    private async Task ExecuteAbandonSessionCommand()
    {
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");
    }
}