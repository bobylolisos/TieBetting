namespace TieBetting.Models;

[FirestoreData]
public class Team
{
    private IReadOnlyCollection<Match> _matches;
    private readonly List<bool> _statuses = new();

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Image { get; set; }

    public List<bool> Statuses => _statuses;

    public double TotalWin { get; private set; }

    public int TotalBet { get; private set; }

    public int Profit => (int)TotalWin - TotalBet - CurrentBetSession;

    public int CurrentBetSession { get; private set; }

    public IReadOnlyCollection<Match> Matches => _matches;

    public void AddMatches(IReadOnlyCollection<Match> allTeamsMatches)
    {
        _matches = allTeamsMatches.Where(x => x.HomeTeam == Name || x.AwayTeam == Name).OrderBy(x => x.Day).ToList();

        ReCalculateValues();
    }

    public void NotifyMatchStatusChanged()
    {
        ReCalculateValues();
    }

    private void ReCalculateValues()
    {
        double totalWin = 0;
        var totalBet = 0;
        var currentSession = 0;
        var currentSessionDone = false;
        _statuses.Clear();

        foreach (var match in Matches.Where(x => x.Status > 0).OrderByDescending(x => x.Day))
        {
            if (match.HomeTeam == Name)
            {
                if (currentSessionDone == false)
                {
                    if (match.MatchStatus == MatchStatus.Win)
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.HomeTeamBet ?? 0;
                    }
                }
                totalBet += currentSessionDone && match.HomeTeamBet.HasValue ? match.HomeTeamBet.Value : 0;
                totalWin += match.HomeTeamWin;

                if (match.MatchStatus == MatchStatus.Lost || match.MatchStatus == MatchStatus.Win)
                {
                    Statuses.Insert(0, match.MatchStatus == MatchStatus.Win);
                }

            }

            if (match.AwayTeam == Name)
            {
                if (currentSessionDone == false)
                {
                    if (match.MatchStatus == MatchStatus.Win)
                    {
                        currentSessionDone = true;
                    }
                    else
                    {
                        currentSession += match.AwayTeamBet ?? 0;
                    }
                }
                totalBet += currentSessionDone && match.AwayTeamBet.HasValue ? match.AwayTeamBet.Value : 0;
                totalWin += match.AwayTeamWin;

                if (match.MatchStatus == MatchStatus.Lost || match.MatchStatus == MatchStatus.Win)
                {
                    Statuses.Insert(0, match.MatchStatus == MatchStatus.Win);
                }
            }
        }

        TotalBet = totalBet;
        TotalWin = totalWin;
        CurrentBetSession = currentSession;
    }
}