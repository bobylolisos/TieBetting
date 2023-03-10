namespace TieBetting.ViewModels;

public class MatchBettingViewModel : MatchViewModel
{
    private readonly ISaverService _saverService;
    private readonly Settings _settings;

    public MatchBettingViewModel(ISaverService saverService, Settings settings, Match match, TeamViewModel homeTeam, TeamViewModel awayTeam) 
        : base(saverService, match, homeTeam, awayTeam)
    {
        _saverService = saverService;
        _settings = settings;
    }

    public int HomeTeamTotalBet => HomeTeam.TotalBet;

    public int HomeTeamTotalWin => (int)HomeTeam.TotalWin;

    public int HomeTeamProfit => HomeTeam.Profit;

    public int HomeTeamCurrentBetSession => HomeTeam.BetsInSession;

    public int HomeTeamLostMatches => HomeTeam.LostMatchesInSession;

    public int AwayTeamTotalBet => AwayTeam.TotalBet;

    public int AwayTeamTotalWin => (int)AwayTeam.TotalWin;

    public int AwayTeamProfit => AwayTeam.Profit;

    public int AwayTeamCurrentBetSession => AwayTeam.BetsInSession;

    public int AwayTeamLostMatches => AwayTeam.LostMatchesInSession;

    public override async Task SetRate(double? rate)
    {
        if (rate.HasValue == false)
        {
            Match.Rate = null;
            Match.HomeTeamBet = null;
            Match.HomeTeamStatus = 0;
            Match.AwayTeamBet = null;
            Match.AwayTeamStatus = 0;
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
            Match.Rate = rate.Value;

            Match.HomeTeamBet = 0;
            if (HomeTeam.IsActive)
            {
                for (var i = 1; i < int.MaxValue; i++)
                {
                    var win = i * Rate;

                    if (win - i - HomeTeam.BetsInSession >= _settings.ExpectedWinAmount)
                    {
                        Match.HomeTeamBet = i;
                        break;
                    }
                }
            }

            Match.AwayTeamBet = 0;
            if (AwayTeam.IsActive)
            {
                for (var i = 1; i < int.MaxValue; i++)
                {
                    var win = i * Rate;

                    if (win - i - AwayTeam.BetsInSession >= _settings.ExpectedWinAmount)
                    {
                        Match.AwayTeamBet = i;
                        break;
                    }
                }
            }
        }

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(MatchStatus));

        await _saverService.UpdateMatchAsync(Match);
    }

    public override async Task SetStatusAsync(MatchStatus matchStatus)
    {
        await SetStatusAndUpdateAsync(matchStatus);

        HomeTeam.NotifyMatchStatusChanged();
        AwayTeam.NotifyMatchStatusChanged();

        OnPropertyChanged(nameof(MatchStatus));

        OnPropertyChanged(nameof(HomeTeamTotalBet));
        OnPropertyChanged(nameof(HomeTeamTotalWin));
        OnPropertyChanged(nameof(HomeTeamProfit));
        OnPropertyChanged(nameof(HomeTeamCurrentBetSession));
        OnPropertyChanged(nameof(HomeTeamLostMatches));

        OnPropertyChanged(nameof(AwayTeamTotalBet));
        OnPropertyChanged(nameof(AwayTeamTotalWin));
        OnPropertyChanged(nameof(AwayTeamProfit));
        OnPropertyChanged(nameof(AwayTeamCurrentBetSession));
        OnPropertyChanged(nameof(AwayTeamLostMatches));
    }
}