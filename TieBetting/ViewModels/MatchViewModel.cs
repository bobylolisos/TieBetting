namespace TieBetting.ViewModels;

public class MatchViewModel : ViewModelBase, IRecipient<TeamUpdatedMessage>
{
    private readonly ISaverService _saverService;

    public MatchViewModel(IMessenger messenger, ISaverService saverService, Match match, TeamViewModel homeTeam, TeamViewModel awayTeam)
    {
        _saverService = saverService;
        Match = match;
        HomeTeam = homeTeam;
        AwayTeam = awayTeam;

        messenger.RegisterAll(this);
    }

    public Match Match { get; }

    public TeamViewModel HomeTeam { get; }
    
    public TeamViewModel AwayTeam { get; }

    /// <summary>
    /// For grouping in CollectionView
    /// </summary>
    public string GroupHeader => ResolveGroupHeader();

    public string Id => Match.Id;

    public string Season => Match.Season;

    public int Day => Match.Day;

    public string HomeTeamName => Match.HomeTeam;

    public string HomeTeamImage => HomeTeam.Image;

    public int? HomeTeamBet => Match.HomeTeamBet;

    public double? HomeTeamWin => this.IsWin(TeamType.HomeTeam) ? Match.HomeTeamBet * Rate : 0;

    public MatchStatus HomeTeamMatchStatus => (MatchStatus)Match.HomeTeamStatus;

    public string AwayTeamName => Match.AwayTeam;

    public string AwayTeamImage => AwayTeam.Image;

    public int? AwayTeamBet => Match.AwayTeamBet;

    public double? AwayTeamWin => this.IsWin(TeamType.AwayTeam) ? Match.AwayTeamBet * Rate : 0;

    public MatchStatus AwayTeamMatchStatus => (MatchStatus)Match.AwayTeamStatus;

    public double? Rate => Match.Rate;

    public MatchStatus MatchStatus => ResolveMatchStatus();

    public int? TotalBet => HomeTeamBet + AwayTeamBet;

    public double? TotalWin => HomeTeamWin + AwayTeamWin;

    public string Date => DayProvider.GetDate(Day).ToString("yyyy-MM-dd");

    public virtual async Task SetStatusAsync(MatchStatus matchStatus)
    {
        await SetStatusAndUpdateAsync(matchStatus);

        OnPropertyChanged(nameof(MatchStatus));
    }

    public virtual async Task SetAbandonAsync(TeamViewModel team)
    {
        if (team == HomeTeam)
        {
            Match.HomeTeamStatus = (int)MatchStatus.Abandoned;
        }
        else
        {
            Match.AwayTeamStatus = (int)MatchStatus.Abandoned;
        }

        await _saverService.UpdateMatchAsync(Match);

        OnPropertyChanged(nameof(MatchStatus));
    }

    public virtual async Task SetRate(double? rate)
    {
        Match.Rate = null;
        Match.HomeTeamBet = null;
        Match.HomeTeamStatus = 0;
        Match.AwayTeamBet = null;
        Match.AwayTeamStatus = 0;

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

    public async Task SetDateAsync(DateTime matchDate)
    {
        Match.Day = DayProvider.GetDay(matchDate);

        await _saverService.UpdateMatchAsync(Match, true);

        OnPropertyChanged(nameof(GroupHeader));
        OnPropertyChanged(nameof(Date));
    }

    public int GetActivatedHomeTeamBet()
    {
        // Todo: Kan jag kolla på HomeTeamMatchStatus != MatchStatus.NotActive istället?
        if (HomeTeamMatchStatus is MatchStatus.Active or MatchStatus.Win or MatchStatus.Lost or MatchStatus.Abandoned)
        {
            return HomeTeamBet ?? 0;
        }

        return 0;
    }

    public int GetActivatedAwayTeamBet()
    {
        // Todo: Kan jag kolla på AwayTeamMatchStatus != MatchStatus.NotActive istället?
        if (AwayTeamMatchStatus is MatchStatus.Active or MatchStatus.Win or MatchStatus.Lost or MatchStatus.Abandoned)
        {
            return AwayTeamBet ?? 0;
        }

        return 0;
    }


    public int GetActivatedTotalBet()
    {
        return GetActivatedHomeTeamBet() + GetActivatedAwayTeamBet();
    }

    private MatchStatus ResolveMatchStatus()
    {
        if (this.IsActiveOrDone(TeamType.HomeTeam) && this.IsAbandoned(TeamType.HomeTeam) == false)
        {
            return HomeTeamMatchStatus;
        }

        if (this.IsActiveOrDone(TeamType.AwayTeam) && this.IsAbandoned(TeamType.AwayTeam) == false)
        {
            return AwayTeamMatchStatus;
        }

        if (this.IsAbandoned(TeamType.HomeTeam) || this.IsAbandoned(TeamType.AwayTeam))
        {
            return MatchStatus.Abandoned;
        }

        if (this.IsDormant(TeamType.HomeTeam) || this.IsDormant(TeamType.AwayTeam))
        {
            return MatchStatus.Dormant;
        }

        return MatchStatus.NotActive;
    }

    protected async Task SetStatusAndUpdateAsync(MatchStatus matchStatus)
    {
        if (matchStatus == MatchStatus.NotActive)
        {
            Match.HomeTeamStatus = (int)MatchStatus.NotActive;
            Match.AwayTeamStatus = (int)MatchStatus.NotActive;
        }

        if (matchStatus == MatchStatus.Active)
        {
            if (HomeTeam.IsDormant)
            {
                Match.HomeTeamStatus = (int)MatchStatus.Dormant;
            }
            else
            {
                Match.HomeTeamStatus = (int)MatchStatus.Active;
            }

            if (AwayTeam.IsDormant)
            {
                Match.AwayTeamStatus = (int)MatchStatus.Dormant;
            }
            else
            {
                Match.AwayTeamStatus = (int)MatchStatus.Active;
            }
        }

        if (matchStatus == MatchStatus.Win || matchStatus == MatchStatus.Lost)
        {
            if (Match.HomeTeamStatus != (int)MatchStatus.Dormant)
            {
                Match.HomeTeamStatus = (int)matchStatus;
            }
            if (Match.AwayTeamStatus != (int)MatchStatus.Dormant)
            {
                Match.AwayTeamStatus = (int)matchStatus;
            }
        }

        await _saverService.UpdateMatchAsync(Match);
    }

    private string ResolveGroupHeader()
    {
        if (Day < DayProvider.TodayDay)
        {
            return $"{Date}   Previous";
        }

        if (Day == DayProvider.TodayDay)
        {
            return $"{Date}   Today";
        }

        return Date;
    }

    public void Receive(TeamUpdatedMessage message)
    {
        if (HomeTeamName == message.TeamName || AwayTeamName == message.TeamName)
        {
            OnTeamUpdated();
        }
    }

    protected virtual void OnTeamUpdated()
    {
    }
}