namespace TieBetting.ViewModels;

public class TeamViewModel : ViewModelBase, IRecipient<MatchUpdatedMessage>
{
    private readonly List<MatchViewModel> _matches = new();
    private readonly List<bool> _statuses = new();
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

    public int MatchesWon => _statuses.Count(x => x);

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

    public List<bool> Statuses => _statuses;

    public int TotalWin { get; private set; }

    public double ExactTotalWin { get; private set; }

    public int TotalBet { get; private set; }

    public int Profit => TotalWin - TotalBet;

    public IReadOnlyCollection<MatchViewModel> Matches => _matches.OrderBy(x => x.Day).ToList();

    public void AddMatch(MatchViewModel match)
    {
        _matches.Add(match);
    }

    public void NotifyMatchStatusChanged()
    {
        ReCalculateValues();
    }

    public void ReCalculateValues()
    {
        double totalWin = 0;
        var totalBet = 0;
        var currentSession = 0;
        var currentSessionDone = false;
        _statuses.Clear();

        foreach (var match in Matches.OrderByDescending(x => x.Day))
        {
            if (match.HomeTeamName == Name && match.IsActiveOrDone(TeamType.HomeTeam))
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin(TeamType.HomeTeam) || match.IsAbandon(TeamType.HomeTeam))
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.GetActivatedHomeTeamBet();
                    }
                }
                totalBet += match.GetActivatedHomeTeamBet();
                totalWin += match.HomeTeamWin ?? 0;

                if (match.IsDone(TeamType.HomeTeam))
                {
                    _statuses.Insert(0, match.IsWin(TeamType.HomeTeam));
                }

            }

            if (match.AwayTeamName == Name && match.IsActiveOrDone(TeamType.AwayTeam))
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin(TeamType.AwayTeam) || match.IsAbandon(TeamType.AwayTeam))
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.GetActivatedAwayTeamBet();
                    }
                }
                totalBet += match.GetActivatedAwayTeamBet();
                totalWin += match.AwayTeamWin ?? 0;

                if (match.IsDone(TeamType.AwayTeam))
                {
                    _statuses.Insert(0, match.IsWin(TeamType.AwayTeam));
                }
            }
        }

        TotalBet = totalBet;
        ExactTotalWin = totalWin;
        TotalWin = (int)totalWin;
        BetsInSession = currentSession;

        OnPropertyChanged(nameof(Statuses));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(ExactTotalWin));
        OnPropertyChanged(nameof(TotalWin));
        OnPropertyChanged(nameof(BetsInSession));
        OnPropertyChanged(nameof(Profit));
        OnPropertyChanged(nameof(MatchesWon));
        OnPropertyChanged(nameof(MatchesWonPercent));
        OnPropertyChanged(nameof(LostMatchesInSession));
    }

    public void Receive(MatchUpdatedMessage message)
    {
        if (Matches.HasMatch(message.MatchId))
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