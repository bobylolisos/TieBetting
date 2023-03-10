namespace TieBetting.ViewModels;

public class TeamViewModel : ViewModelBase, IRecipient<MatchUpdatedMessage>, IRecipient<TeamUpdatedMessage>
{
    private readonly List<MatchViewModel> _matches = new();
    private readonly List<MatchStatus> _statuses = new();
    private readonly ISaverService _saverService;
    private readonly Team _team;

    public TeamViewModel(ISaverService saverService, Team team)
    {
        _saverService = saverService;
        _team = team;

        WeakReferenceMessenger.Default.RegisterAll(this);

    }

    public string Name => _team.Name;

    public string Image => _team.Image;

    public bool IsActive => _team.IsActive;

    public bool IsDormant => !IsActive;

    public int BetsInSession { get; private set; }

    public int LostMatchesInSession => _statuses.CountNumberOfPreviousLostMatches();

    public int MatchesWon => _statuses.Count(x => x == MatchStatus.Win);

    public string MatchesWonPercent
    {
        get
        {
            var matchesWonCount = MatchesWon;
            if (matchesWonCount == 0)
            {
                return "0 %";
            }

            var percent = (int)(matchesWonCount / (double)_statuses.Count * 100);
            return $"{percent} %";
        }
    }

    public List<MatchStatus> Statuses => _statuses;

    public int TotalWin { get; private set; }

    public double ExactTotalWin { get; private set; }

    public int TotalBet { get; private set; }

    public int AbandonedBets { get; private set; }

    public int Profit => TotalWin - TotalBet;

    public IReadOnlyCollection<MatchViewModel> Matches => _matches.OrderBy(x => x.Day).ToList();

    public void AddMatch(MatchViewModel match)
    {
        _matches.Add(match);
    }

    //public void NotifyMatchStatusChanged()
    //{
    //    ReCalculateValues();
    //}

    public void ReCalculateValues()
    {
        double totalWin = 0;
        var totalBet = 0;
        var currentSession = 0;
        var currentSessionDone = false;
        var abandonBets = 0;
        var abandonSession = false;
        _statuses.Clear();

        foreach (var match in Matches.OrderByDescending(x => x.Day))
        {
            if (match.HomeTeamName == Name && match.IsActiveOrDone(TeamType.HomeTeam))
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin(TeamType.HomeTeam) || match.IsAbandoned(TeamType.HomeTeam))
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.GetActivatedHomeTeamBet();
                    }
                }

                if (match.IsAbandoned(TeamType.HomeTeam) || abandonSession)
                {
                    if (match.IsWin(TeamType.HomeTeam))
                    {
                        abandonSession = false;
                    }
                    else if (match.IsLost(TeamType.HomeTeam) || match.IsAbandoned(TeamType.HomeTeam))
                    {
                        abandonBets += match.GetActivatedHomeTeamBet();
                        abandonSession = true;
                    }
                }

                totalBet += match.GetActivatedHomeTeamBet();
                totalWin += match.HomeTeamWin ?? 0;

                if (match.IsDone(TeamType.HomeTeam))
                {
                    _statuses.Insert(0, match.HomeTeamMatchStatus);
                }

            }

            if (match.AwayTeamName == Name && match.IsActiveOrDone(TeamType.AwayTeam))
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin(TeamType.AwayTeam) || match.IsAbandoned(TeamType.AwayTeam))
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.GetActivatedAwayTeamBet();
                    }
                }

                if (match.IsAbandoned(TeamType.AwayTeam) || abandonSession)
                {
                    if (match.IsWin(TeamType.AwayTeam))
                    {
                        abandonSession = false;
                    }
                    else if (match.IsLost(TeamType.AwayTeam) || match.IsAbandoned(TeamType.AwayTeam))
                    {
                        abandonBets += match.GetActivatedAwayTeamBet();
                        abandonSession = true;
                    }
                }

                totalBet += match.GetActivatedAwayTeamBet();
                totalWin += match.AwayTeamWin ?? 0;

                if (match.IsDone(TeamType.AwayTeam))
                {
                    _statuses.Insert(0, match.AwayTeamMatchStatus);
                }
            }
        }

        TotalBet = totalBet;
        ExactTotalWin = totalWin;
        TotalWin = (int)totalWin;
        BetsInSession = currentSession;
        AbandonedBets = abandonBets;

        OnPropertyChanged(nameof(Statuses));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(ExactTotalWin));
        OnPropertyChanged(nameof(TotalWin));
        OnPropertyChanged(nameof(BetsInSession));
        OnPropertyChanged(nameof(AbandonedBets));
        OnPropertyChanged(nameof(Profit));
        OnPropertyChanged(nameof(MatchesWon));
        OnPropertyChanged(nameof(MatchesWonPercent));
        OnPropertyChanged(nameof(LostMatchesInSession));
        OnPropertyChanged(nameof(IsDormant));
    }

    public void Receive(MatchUpdatedMessage message)
    {
        if (Matches.HasMatch(message.MatchId))
        {
            ReCalculateValues();
        }
    }

    public void Receive(TeamUpdatedMessage message)
    {
        if (Name == message.TeamName)
        {
            ReCalculateValues();
        }
    }

    public async Task ToggleActiveStatusAsync()
    {
        _team.IsActive = !_team.IsActive;
        await _saverService.UpdateTeamAsync(_team);
    }
}