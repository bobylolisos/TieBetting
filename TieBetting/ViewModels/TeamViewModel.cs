namespace TieBetting.ViewModels;

public class TeamViewModel : ViewModelBase, IRecipient<MatchUpdatedMessage>
{
    private readonly List<MatchViewModel> _matches = new();
    private readonly List<bool> _statuses = new();
    private readonly Team _team;

    public TeamViewModel(Team team)
    {
        _team = team;

        WeakReferenceMessenger.Default.RegisterAll(this);

    }

    public string Name => _team.Name;

    public string Image => _team.Image;

    public int BetsInSession { get; private set; }

    public int LostMatchesInSession => _statuses.CountNumberOfPreviousLostMatches();

    public string MatchesWonPercent
    {
        get
        {
            var matchesWonCount = _statuses.Count(x => x);
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

    public int TotalBet { get; private set; }

    public int Profit => TotalWin - TotalBet;

    public IReadOnlyCollection<MatchViewModel> Matches => _matches;

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

        foreach (var match in Matches.Where(x => x.MatchStatus > 0).OrderByDescending(x => x.Day))
        {
            if (match.HomeTeamName == Name)
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin())
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

                if (match.IsDone())
                {
                    _statuses.Insert(0, match.IsWin());
                }

            }

            if (match.AwayTeamName == Name)
            {
                if (currentSessionDone == false)
                {
                    if (match.IsWin())
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

                if (match.IsDone())
                {
                    _statuses.Insert(0, match.IsWin());
                }
            }
        }

        TotalBet = totalBet;
        TotalWin = (int)totalWin;
        BetsInSession = currentSession;

        OnPropertyChanged(nameof(Statuses));
        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(TotalWin));
        OnPropertyChanged(nameof(BetsInSession));
        OnPropertyChanged(nameof(Profit));
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
}