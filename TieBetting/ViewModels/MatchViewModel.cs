namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private readonly ISaverService _saverService;
    protected readonly Match Match;

    public MatchViewModel(ISaverService saverService, Match match, TeamViewModel homeTeam, TeamViewModel awayTeam)
    {
        _saverService = saverService;
        Match = match;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;

        Date = DayProvider.GetDate(match.Day).ToString("yyyy-MM-dd");
    }
    public TeamViewModel HomeTeam { get; }
    
    public TeamViewModel AwayTeam { get; }

    public string Id => Match.Id;

    public string Season => Match.Season;

    public int Day => Match.Day;

    public string HomeTeamName => Match.HomeTeam;

    public string HomeTeamImage => HomeTeam.Image;

    public int? HomeTeamBet => Match.HomeTeamBet;

    public double? HomeTeamWin => this.IsWin() ? Match.HomeTeamBet * Rate : 0;

    public string AwayTeamName => Match.AwayTeam;

    public string AwayTeamImage => AwayTeam.Image;

    public int? AwayTeamBet => Match.AwayTeamBet;

    public double? AwayTeamWin => this.IsWin() ? Match.AwayTeamBet * Rate : 0;

    public double? Rate => Match.Rate;

    public MatchStatus MatchStatus => (MatchStatus)Match.Status;

    public int? TotalBet => HomeTeamBet + AwayTeamBet;

    public double? TotalWin => HomeTeamWin + AwayTeamWin;

    public string Date { get; }

    public virtual async Task SetStatus(MatchStatus matchStatus)
    {
        Match.Status = (int)matchStatus;

        OnPropertyChanged(nameof(MatchStatus));

        await _saverService.UpdateMatchAsync(Match);
    }

    public virtual async Task SetRate(double? rate)
    {
        Match.Rate = null;
        Match.HomeTeamBet = null;
        Match.AwayTeamBet = null;
        Match.Status = 0;

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(MatchStatus));

        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(HomeTeamWin));

        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(AwayTeamWin));

        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(TotalWin));

        await _saverService.UpdateMatchAsync(Match);
    }

    public int GetActivatedHomeTeamBet()
    {
        if (MatchStatus is MatchStatus.Active or MatchStatus.Win or MatchStatus.Lost)
        {
            return HomeTeamBet ?? 0;
        }

        return 0;
    }

    public int GetActivatedAwayTeamBet()
    {
        if (MatchStatus is MatchStatus.Active or MatchStatus.Win or MatchStatus.Lost)
        {
            return AwayTeamBet ?? 0;
        }

        return 0;
    }


    public int GetActivatedTotalBet()
    {
        return GetActivatedHomeTeamBet() + GetActivatedAwayTeamBet();
    }
}