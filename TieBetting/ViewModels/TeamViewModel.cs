namespace TieBetting.ViewModels;

public class TeamViewModel
{
    public TeamViewModel(Team team)
    {
        Team = team;
    }

    public Team Team { get; }

    public string Name => Team.Name;

    public string Image => Team.Image;

    public int TotalBet => Team.TotalBet;

    public int TotalWin => (int)Team.TotalWin;

    public int Profit => Team.Profit;

    public int BetsInSession => Team.CurrentBetSession;

    public int LostMatchesInSession => Team.Statuses.CountNumberOfPreviousLostMatches();

    public string MatchesWonPercent
    {
        get
        {
            var matchesWonCount = Team.Statuses.Count(x => x);
            if (matchesWonCount == 0)
            {
                return "0 %";
            }

            var percent = (int)(matchesWonCount / (double)Team.Statuses.Count * 100);
            return $"{percent} %";
        }
    }
}