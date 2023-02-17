﻿namespace TieBetting.ViewModels.NavigationViewModels;

public class AllMatchesViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private Settings _settings;
    private string _selectedSeason;
    private IReadOnlyCollection<MatchViewModel> _allMatches;
    private IReadOnlyCollection<TeamViewModel> _allTeams;

    public AllMatchesViewModel(INavigationService navigationService, IQueryService queryService)
        : base(navigationService)
    {
        _queryService = queryService;

        NavigateToMatchMaintenanceViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchMaintenanceViewCommand);
    }

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchMaintenanceViewCommand { get; }


    public ObservableCollection<string> Seasons { get; } = new();

    public ObservableCollection<MatchGroupViewModel> Matches { get; set; } = new();

    public int TotalBet => Matches.Sum(x => x.Sum(y => y.GetActivatedTotalBet()));

    public int TotalWin => (int)Matches.Sum(x => x.Sum(y => y.TotalWin ?? 0));

    public int CurrentBetSession => ResolveCurrentBetSession();

    public int Profit => TotalWin - TotalBet;

    public string MatchesWonPercent
    {
        get
        {
            var matchesCount = Matches.Sum(x => x.Count(x => x.IsDone()));
            var matchesWonCount = Matches.Sum(x => x.Count(x => x.IsWin()));
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
                var groupedSeasonMatches = _allMatches.Where(x => x.Season == SelectedSeason).GroupBy(x => x.Date).OrderBy(y => y.Key);
                var groupdViewModels = new List<MatchGroupViewModel>();
                foreach (var groupedSeasonMatch in groupedSeasonMatches)
                {
                    var singleGroupedMatches = new List<MatchViewModel>();
                    foreach (var match in groupedSeasonMatch)
                    {
                        singleGroupedMatches.Add(match);
                    }

                    groupdViewModels.Add(new MatchGroupViewModel($"{groupedSeasonMatch.Key:yyyy-MM-dd}", singleGroupedMatches));
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
            var matchesForTeam = Matches.SelectMany(x => x.Where(y => y.HomeTeamName == team.Name || y.AwayTeamName == team.Name)).OrderByDescending(x => x.Day);

            foreach (var match in matchesForTeam)
            {
                if (match.HomeTeamName == team.Name)
                {
                    if (currentSessionDone == false)
                    {
                        if (match.IsWin())
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
                        if (match.IsWin())
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
}