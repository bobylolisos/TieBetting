namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private readonly IRepository _repository;
    private readonly Match _match;
    private readonly Team _homeTeam;
    private readonly Team _awayTeam;

    public MatchViewModel(IRepository repository, Match match, Team homeTeam, Team awayTeam)
    {
        _repository = repository;
        _match = match;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;

        Date = match.Date.ToString("yyyy-MM-dd");


        HomeTeamLastSixStatuses = new List<bool?>();
        var homeTeamLastSixStatuses = homeTeam.Statuses.TakeLastItems(6);
        for (var i = homeTeamLastSixStatuses.Count(); i < 6; i++)
        {
            HomeTeamLastSixStatuses.Add(null);
        }

        foreach (var homeTeamLastSixStatus in homeTeamLastSixStatuses)
        {
            HomeTeamLastSixStatuses.Add(homeTeamLastSixStatus);
        }

        AwayTeamLastSixStatuses = new List<bool?>();
        var awayTeamLastSixStatuses = awayTeam.Statuses.TakeLastItems(6).Reverse();
        foreach (var awayTeamLastSixStatus in awayTeamLastSixStatuses)
        {
            AwayTeamLastSixStatuses.Add(awayTeamLastSixStatus);
        }
        for (var i = AwayTeamLastSixStatuses.Count; i < 6; i++)
        {
            AwayTeamLastSixStatuses.Add(null);
        }

    }

    public string Id => _match.Id;

    public string HomeTeam => _match.HomeTeam;

    public string HomeTeamImage => _homeTeam.Image;

    public int? HomeTeamBet => _match.HomeTeamBet;

    public List<bool?> HomeTeamLastSixStatuses { get; }

    public int HomeTeamTotalBet => _homeTeam.TotalBet;

    public int HomeTeamTotalWin => (int)_homeTeam.TotalWin;

    public int HomeTeamProfit => _homeTeam.Profit;

    public int HomeTeamCurrentBetSession => _homeTeam.CurrentBetSession;

    public string AwayTeam => _match.AwayTeam;

    public string AwayTeamImage => _awayTeam.Image;

    public int? AwayTeamBet => _match.AwayTeamBet;

    public List<bool?> AwayTeamLastSixStatuses { get; }

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

            for (int i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _homeTeam.CurrentBetSession >= 50)
                {
                    _match.HomeTeamBet = i;
                    break;
                }
            }
            for (int i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _awayTeam.CurrentBetSession >= 50)
                {
                    _match.AwayTeamBet = i;
                    break;
                }
            }

            if (_match.Status == (int)MatchStatus.NotActive)
            {
                _match.Status = (int)MatchStatus.Active;
            }
        }

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(Status));

        await _repository.UpdateMatch(_match);
    }

    public async Task SetStatus(MatchStatus matchStatus)
    {
        _match.Status = (int)matchStatus;
        OnPropertyChanged(nameof(Status));

        await _repository.UpdateMatch(_match);
    }
}