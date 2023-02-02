namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private readonly IRepository _repository;
    private readonly Match _match;
    private readonly Team _homeTeam;
    private readonly Team _awayTeam;

    private const int MinWin = 50;
    private const int MaxBet = 250;

    public MatchViewModel(IRepository repository, Match match, Team homeTeam, Team awayTeam)
    {
        _repository = repository;
        _match = match;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;

        Date = match.Date.ToString("yyyy-MM-dd");
    }

    public string Id => _match.Id;

    public string HomeTeam => _match.HomeTeam;

    public string HomeTeamImage => _homeTeam.Image;

    public int? HomeTeamBet => _match.HomeTeamBet;

    public List<bool?> HomeTeamLastSixStatuses => GetHomeTeamLastSixStatuses();

    public int HomeTeamTotalBet => _homeTeam.TotalBet;

    public int HomeTeamTotalWin => (int)_homeTeam.TotalWin;

    public int HomeTeamProfit => _homeTeam.Profit;

    public int HomeTeamCurrentBetSession => _homeTeam.CurrentBetSession;

    public string AwayTeam => _match.AwayTeam;

    public string AwayTeamImage => _awayTeam.Image;

    public int? AwayTeamBet => _match.AwayTeamBet;

    public List<bool?> AwayTeamLastSixStatuses => GetAwayTeamLastSixStatuses();

    public int AwayTeamTotalBet => _awayTeam.TotalBet;

    public int AwayTeamTotalWin => (int)_awayTeam.TotalWin;

    public int AwayTeamProfit => _awayTeam.Profit;

    public int AwayTeamCurrentBetSession => _awayTeam.CurrentBetSession;

    public double? Rate => _match.Rate;

    public MatchStatus Status => (MatchStatus)_match.Status;

    public string Date { get; }

    public int? TotalBet => HomeTeamBet + AwayTeamBet;

    public async Task SetRate(double? rate)
    {
        if (rate.HasValue == false)
        {
            _match.Rate = null;
            _match.HomeTeamBet = null;
            _match.AwayTeamBet = null;
            _match.Status = 0;
        }
        else if (rate.Value < 2)
        {
            // Todo: Visa dialog, orimligt värde
            if (Debugger.IsAttached)
                Debugger.Break();
            return;
        }
        else
        {
            _match.Rate = rate.Value;

            for (var i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _homeTeam.CurrentBetSession >= MinWin)
                {
                    _match.HomeTeamBet = i < MaxBet ? i : MaxBet;
                    break;
                }
            }

            for (var i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _awayTeam.CurrentBetSession >= MinWin)
                {
                    _match.AwayTeamBet = i < MaxBet ? i : MaxBet;
                    break;
                }
            }
        }

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(Status));

        await _repository.UpdateMatchAsync(_match);
    }

    public async Task SetStatus(MatchStatus matchStatus)
    {
        _match.Status = (int)matchStatus;

        await _repository.UpdateMatchAsync(_match);

        _homeTeam.NotifyMatchStatusChanged();
        _awayTeam.NotifyMatchStatusChanged();

        OnPropertyChanged(nameof(Status));

        OnPropertyChanged(nameof(HomeTeamTotalBet));
        OnPropertyChanged(nameof(HomeTeamTotalWin));
        OnPropertyChanged(nameof(HomeTeamProfit));
        OnPropertyChanged(nameof(HomeTeamCurrentBetSession));
        OnPropertyChanged(nameof(HomeTeamLastSixStatuses));

        OnPropertyChanged(nameof(AwayTeamTotalBet));
        OnPropertyChanged(nameof(AwayTeamTotalWin));
        OnPropertyChanged(nameof(AwayTeamProfit));
        OnPropertyChanged(nameof(AwayTeamCurrentBetSession));
        OnPropertyChanged(nameof(AwayTeamLastSixStatuses));
    }

    private List<bool?> GetHomeTeamLastSixStatuses()
    {
        var result = new List<bool?>();

        var homeTeamLastSixStatuses = _homeTeam.Statuses.TakeLastItems(6).ToList();
        for (var i = homeTeamLastSixStatuses.Count; i < 6; i++)
        {
            result.Add(null);
        }

        foreach (var homeTeamLastSixStatus in homeTeamLastSixStatuses)
        {
            result.Add(homeTeamLastSixStatus);
        }

        return result;
    }

    private List<bool?> GetAwayTeamLastSixStatuses()
    {
        var result = new List<bool?>();

        var awayTeamLastSixStatuses = _awayTeam.Statuses.TakeLastItems(6).Reverse();
        foreach (var awayTeamLastSixStatus in awayTeamLastSixStatuses)
        {
            result.Add(awayTeamLastSixStatus);
        }

        for (var i = result.Count; i < 6; i++)
        {
            result.Add(null);
        }

        return result;

    }
}