namespace TieBetting.ViewModels.NavigationViewModels;

public class StatisticsViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private int _totalBet;
    private int _totalWin;
    private int _totalProfit;
    private int _betsInSession;
    private int _currentProfit;
    private int _matchesCount;
    private int _longestLostStreak;
    private int _bestTeamProfit;
    private string _bestTeamProfitTeamName;
    private int _worstTeamProfit;
    private string _worstTeamProfitTeamName;
    private string _longestLostStreakTeamName;
    private int _longestLostStreakInSession;
    private string _longestLostStreakInSessionTeamName;
    private int _matchesWonCount;

    public StatisticsViewModel(INavigationService navigationService, IQueryService queryService)
        : base(navigationService)
    {
        _queryService = queryService;
    }

    public int TotalBet
    {
        get => _totalBet;
        set => SetProperty(ref _totalBet, value);
    }

    public int TotalWin
    {
        get => _totalWin;
        set => SetProperty(ref _totalWin, value);
    }

    public int TotalProfit
    {
        get => _totalProfit;
        set => SetProperty(ref _totalProfit, value);
    }

    public int BetsInSession
    {
        get => _betsInSession;
        set => SetProperty(ref _betsInSession, value);
    }

    public int CurrentProfit
    {
        get => _currentProfit;
        set => SetProperty(ref _currentProfit, value);
    }

    public int MatchesCount
    {
        get => _matchesCount;
        set
        {
            if (SetProperty(ref _matchesCount, value))
            {
                OnPropertyChanged(nameof(MatchesWonPercent));
            }
        }
    }

    public int MatchesWonCount
    {
        get => _matchesWonCount;
        set
        {
            if (SetProperty(ref _matchesWonCount, value))
            {
                OnPropertyChanged(nameof(MatchesWonPercent));
            }
        }
    }

    public string MatchesWonPercent
    {
        get
        {
            if (MatchesWonCount == 0)
            {
                return "0 %";
            }

            var percent = (int)(MatchesWonCount / (double)MatchesCount * 100);
            return $"{percent} %";
        }
    }

    public int LongestLostStreak
    {
        get => _longestLostStreak;
        set => SetProperty(ref _longestLostStreak, value);
    }

    public string LongestLostStreakTeamName
    {
        get => _longestLostStreakTeamName;
        set => SetProperty(ref _longestLostStreakTeamName, value);
    }

    public int LongestLostStreakInSession
    {
        get => _longestLostStreakInSession;
        set => SetProperty(ref _longestLostStreakInSession, value);
    }

    public string LongestLostStreakInSessionTeamName
    {
        get => _longestLostStreakInSessionTeamName;
        set => SetProperty(ref _longestLostStreakInSessionTeamName, value);
    }

    public int BestTeamProfit
    {
        get => _bestTeamProfit;
        set => SetProperty(ref _bestTeamProfit, value);
    }

    public string BestTeamProfitTeamName
    {
        get => _bestTeamProfitTeamName;
        set => SetProperty(ref _bestTeamProfitTeamName, value);
    }

    public int WorstTeamProfit
    {
        get => _worstTeamProfit;
        set => SetProperty(ref _worstTeamProfit, value);
    }

    public string WorstTeamProfitTeamName
    {
        get => _worstTeamProfitTeamName;
        set => SetProperty(ref _worstTeamProfitTeamName, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        await base.OnNavigatingToAsync(navigationParameter);

        var teams = await _queryService.GetTeamsAsync();

        if (teams.Any() != true)
        {
            return;
        }

        TotalBet = teams.Sum(x => x.TotalBet) + teams.Sum(x => x.CurrentBetSession);
        TotalWin = (int)teams.Sum(x => x.TotalWin);
        BetsInSession = teams.Sum(x => x.CurrentBetSession);
        CurrentProfit = TotalWin - TotalBet;

        var hasMatches = teams.Any(x => x.Statuses.Any());
        if (hasMatches)
        {
            MatchesCount = teams.Sum(x => x.Statuses.Count) / 2;
            MatchesWonCount = teams.SelectMany(x => x.Statuses).Count(y => y) / 2;

            LongestLostStreak = teams.Max(x => x.Statuses.CountMaxNumberOfLostMatches());
            var longestLostStreakTeams = teams.Where(x => x.Statuses.CountMaxNumberOfLostMatches() == LongestLostStreak).ToList();
            var longestLostStreakMultipleTeamsText = longestLostStreakTeams.Count > 1 ? $"+ {longestLostStreakTeams.Count - 1}" : "";
            LongestLostStreakTeamName = $"{longestLostStreakTeams.First().Name} {longestLostStreakMultipleTeamsText}";

            LongestLostStreakInSession = teams.Max(x => x.Statuses.CountNumberOfPreviousLostMatches());
            var longestLostStreakInSessionTeams = teams.Where(x => x.Statuses.CountNumberOfPreviousLostMatches() == LongestLostStreakInSession).ToList();
            var longestLostStreakInSessionMultipleTeamsText = longestLostStreakInSessionTeams.Count > 1 ? $"+ {longestLostStreakInSessionTeams.Count - 1}" : "";
            LongestLostStreakInSessionTeamName = $"{longestLostStreakInSessionTeams.First().Name} {longestLostStreakInSessionMultipleTeamsText}";

        }

        BestTeamProfit = teams.Max(x => x.Profit);
        BestTeamProfitTeamName = teams.First(x => x.Profit == BestTeamProfit).Name;

        WorstTeamProfit = teams.Min(x => x.Profit);
        WorstTeamProfitTeamName = teams.First(x => x.Profit == WorstTeamProfit).Name;


    }
}