namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private readonly Match _match;
    private readonly Team _homeTeam;
    private readonly Team _awayTeam;

    public MatchViewModel(Match match, Team homeTeam, Team awayTeam)
    {
        _match = match;
        _homeTeam = homeTeam;
        _awayTeam = awayTeam;

        Date = match.Date.ToString("yyyy-MM-dd");


        HomeTeamLastTenStatuses = new List<bool?>();
        var homeTeamLastTenStatuses = homeTeam.Statuses.TakeLastItems(10);
        for (var i = homeTeamLastTenStatuses.Count(); i < 10; i++)
        {
            HomeTeamLastTenStatuses.Add(null);
        }

        foreach (var homeTeamLastTenStatus in homeTeamLastTenStatuses)
        {
            HomeTeamLastTenStatuses.Add(homeTeamLastTenStatus);
        }

        AwayTeamLastTenStatuses = new List<bool?>();
        var awayTeamLastTenStatuses = awayTeam.Statuses.TakeLastItems(10).Reverse();
        foreach (var awayTeamLastTenStatus in awayTeamLastTenStatuses)
        {
            AwayTeamLastTenStatuses.Add(awayTeamLastTenStatus);
        }
        for (var i = AwayTeamLastTenStatuses.Count; i < 10; i++)
        {
            AwayTeamLastTenStatuses.Add(null);
        }

    }

    public string Id => _match.Id;

    public string HomeTeam => _match.HomeTeam;

    public string HomeTeamImage => _homeTeam.Image;

    public int? HomeTeamBet => _match.HomeTeamBet;

    public List<bool?> HomeTeamLastTenStatuses { get; }

    public string AwayTeam => _match.AwayTeam;

    public string AwayTeamImage => _awayTeam.Image;

    public int? AwayTeamBet => _match.AwayTeamBet;

    public List<bool?> AwayTeamLastTenStatuses { get; }

    public double? Rate => _match.Rate;

    public MatchStatus Status => (MatchStatus)_match.Status;

    public string Date { get; }

    public int? TotalBet => HomeTeamBet + AwayTeamBet;

    public void SetRate(double? rate)
    {
        if (rate.HasValue == false)
        {
            _match.Rate = null;
            _match.HomeTeamBet = null;
            _match.AwayTeamBet = null;
            _match.Status = 0;
        }
        else
        {
            _match.Rate = rate.Value;

            for (int i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _homeTeam.PreviousBet > 50)
                {
                    _match.HomeTeamBet = i;
                    break;
                }
            }
            for (int i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - _awayTeam.PreviousBet > 50)
                {
                    _match.AwayTeamBet = i;
                    break;
                }
            }
        }

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(Status));
    }

    public void SetStatus(MatchStatus matchStatus)
    {
        _match.Status = (int)matchStatus;
        OnPropertyChanged(nameof(Status));
    }
}