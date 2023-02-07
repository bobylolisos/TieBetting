namespace TieBetting.ViewModels;

public class TeamMatchesViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private string _headerText;
    private string _headerImage;
    private string _selectedSeason;
    private IReadOnlyCollection<Team> _allTeams;
    private IReadOnlyCollection<Match> _allTeamMatches;
    private string _teamName;

    public TeamMatchesViewModel(INavigationService navigationService, IQueryService queryService) 
        : base(navigationService)
    {
        _queryService = queryService;
    }

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
                    Matches.Add(new MatchViewModel(match, _allTeams.GetTeam(match.HomeTeam), _allTeams.GetTeam(match.AwayTeam)));
                }
            }
        }
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamMatchesViewNavigationParameter parameter)
        {
            var team = parameter.TeamViewModel;
            HeaderImage = team.Image;
            HeaderText = team.Name;
            _teamName = team.Name;
        }
        else
        {
            throw new ArgumentException("Expected TeamMatchesViewNavigationParameter but not found!");
        }

        _allTeams = await _queryService.GetTeamsAsync();

        _allTeamMatches = await _queryService.GetAllMatchesForTeamAsync(_teamName);

        var seasons = _allTeamMatches.Select(x => x.Season).Distinct().OrderBy(x => x);

        foreach (var season in seasons)
        {
            Seasons.Add(season);
        }

        SelectedSeason = Seasons.Last();


        await base.OnNavigatingToAsync(navigationParameter);

    }
}