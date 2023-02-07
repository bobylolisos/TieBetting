﻿namespace TieBetting.ViewModels;

public class MatchBettingViewModel : MatchViewModel
{
    private readonly IRepository _repository;
    private readonly Settings _settings;

    public MatchBettingViewModel(IRepository repository, Settings settings, Match match, Team homeTeam, Team awayTeam) 
        : base(match, homeTeam, awayTeam)
    {
        _repository = repository;
        _settings = settings;
    }

    public List<bool?> HomeTeamLastSixStatuses => GetHomeTeamLastSixStatuses();

    public int HomeTeamTotalBet => HomeTeam.TotalBet;

    public int HomeTeamTotalWin => (int)HomeTeam.TotalWin;

    public int HomeTeamProfit => HomeTeam.Profit;

    public int HomeTeamCurrentBetSession => HomeTeam.CurrentBetSession;

    public List<bool?> AwayTeamLastSixStatuses => GetAwayTeamLastSixStatuses();

    public int AwayTeamTotalBet => AwayTeam.TotalBet;

    public int AwayTeamTotalWin => (int)AwayTeam.TotalWin;

    public int AwayTeamProfit => AwayTeam.Profit;

    public int AwayTeamCurrentBetSession => AwayTeam.CurrentBetSession;

    public async Task SetRate(double? rate)
    {
        if (rate.HasValue == false)
        {
            Match.Rate = null;
            Match.HomeTeamBet = null;
            Match.AwayTeamBet = null;
            Match.Status = 0;
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

            for (var i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - HomeTeam.CurrentBetSession >= _settings.ExpectedWinAmount)
                {
                    Match.HomeTeamBet = i;
                    break;
                }
            }

            for (var i = 1; i < int.MaxValue; i++)
            {
                var win = i * Rate;

                if (win - i - AwayTeam.CurrentBetSession >= _settings.ExpectedWinAmount)
                {
                    Match.AwayTeamBet = i;
                    break;
                }
            }
        }

        OnPropertyChanged(nameof(Rate));
        OnPropertyChanged(nameof(HomeTeamBet));
        OnPropertyChanged(nameof(AwayTeamBet));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(Status));

        await _repository.UpdateMatchAsync(Match);
    }

    public async Task SetStatus(MatchStatus matchStatus)
    {
        Match.Status = (int)matchStatus;

        await _repository.UpdateMatchAsync(Match);

        HomeTeam.NotifyMatchStatusChanged();
        AwayTeam.NotifyMatchStatusChanged();

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

        var homeTeamLastSixStatuses = HomeTeam.Statuses.TakeLastItems(6).ToList();
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

        var awayTeamLastSixStatuses = AwayTeam.Statuses.TakeLastItems(6).Reverse();
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