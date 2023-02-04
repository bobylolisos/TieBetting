namespace TieBetting.ViewModels;

public class AllMatchesViewModel : ViewModelNavigationBase
{
    private readonly IRepository _repository;
    private Settings _settings;
    private string _selectedSeason;
    private IReadOnlyCollection<Match> _allMatches;
    private IReadOnlyCollection<Team> _allTeams;

    public AllMatchesViewModel(INavigationService navigationService, IRepository repository) 
        : base(navigationService)
    {
        _repository = repository;
    }

    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchGroupViewModel> Matches { get; set; } = new();

    public int TotalBet => Matches.Sum(x => x.Sum(y => y.TotalBet ?? 0));

    public int TotalWin => (int)Matches.Sum(x => x.Sum(y => y.Status == MatchStatus.Win ? y.TotalWin ?? 0 : 0));

    public int CurrentBetSession => 0;

    public int Profit => TotalWin - TotalBet - CurrentBetSession;

    public string MatchesWonPercent
    {
        get
        {
            var matchesCount = Matches.Sum(x => x.Count(x => x.Status > 0));
            var matchesWonCount = Matches.Sum(x => x.Count(x => x.Status == MatchStatus.Win));
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
                var groupdSeasonMatches = _allMatches.Where(x => x.Season == SelectedSeason).GroupBy(x => x.Date).OrderBy(y => y.Key);
                var groupdViewModels = new List<MatchGroupViewModel>();
                foreach (var groupdSeasonMatch in groupdSeasonMatches)
                {
                    var singleGroupedMatches = new List<MatchViewModel>();
                    foreach (var match in groupdSeasonMatch)
                    {
                        singleGroupedMatches.Add(new MatchViewModel(_repository, _settings, match, _allTeams.Single(y => y.Name == match.HomeTeam), _allTeams.Single(y => y.Name == match.AwayTeam)));
                    }

                    groupdViewModels.Add(new MatchGroupViewModel($"{groupdSeasonMatch.Key:yyyy-MM-dd}", singleGroupedMatches));
                }
                foreach (var groupdViewModel in groupdViewModels)
                {
                    Matches.Add(groupdViewModel);
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

        _settings = await _repository.GetSettingsAsync();

        _allTeams = await _repository.GetTeamsAsync();

        _allMatches = await _repository.GetAllMatchesAsync();

        var seasons = _allMatches.Select(x => x.Season).Distinct().OrderBy(x => x);

        foreach (var season in seasons)
        {
            Seasons.Add(season);
        }

        SelectedSeason = Seasons.Single(x => x == _settings.DefaultSeason);



        await base.OnNavigatingToAsync(navigationParameter);
    }
}