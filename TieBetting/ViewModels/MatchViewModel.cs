namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase
{
    protected readonly Match Match;

    public MatchViewModel(Match match, Team homeTeam, Team awayTeam)
    {
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
}