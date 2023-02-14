namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    private readonly ISaverService _saverService;
    protected readonly Match Match;

    public MatchViewModel(ISaverService saverService, Match match, Team homeTeam, Team awayTeam)
    {
        _saverService = saverService;
        Match = match;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;

        Date = match.Date.ToString("yyyy-MM-dd");
    }
    public Team HomeTeam { get; }
    
    public Team AwayTeam { get; }

    public string Id => Match.Id;

    public int Day => Match.Day;

    public string HomeTeamName => Match.HomeTeam;

    public string HomeTeamImage => HomeTeam.Image;

    public int? HomeTeamBet => Match.HomeTeamBet;

    public double? HomeTeamWin => Match.HomeTeamBet * Rate;

    public string AwayTeamName => Match.AwayTeam;

    public string AwayTeamImage => AwayTeam.Image;

    public int? AwayTeamBet => Match.AwayTeamBet;

    public double? AwayTeamWin => Match.AwayTeamBet * Rate;

    public double? Rate => Match.Rate;

    public MatchStatus Status => (MatchStatus)Match.Status;

    public int? TotalBet => HomeTeamBet + AwayTeamBet;

    public double? TotalWin => HomeTeamWin + AwayTeamWin;

    public string Date { get; }

    public virtual async Task SetStatus(MatchStatus matchStatus)
    {
        Match.Status = (int)matchStatus;

        OnPropertyChanged(nameof(Status));

        await _saverService.UpdateMatchAsync(Match);
    }

    public virtual async Task SetRate(double? rate)
    {
        Match.Rate = null;
        Match.HomeTeamBet = null;
        Match.AwayTeamBet = null;
        Match.Status = 0;

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(Status));

        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(HomeTeamWin));

        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(AwayTeamWin));

        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(TotalWin));

        await _saverService.UpdateMatchAsync(Match);
    }
}