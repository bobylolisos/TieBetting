namespace TieBetting.Models;

[FirestoreData]
public class Match
{
    [FirestoreProperty]
    public string Season { get; set; }

    [FirestoreProperty]
    public string Id { get; set; }

    [FirestoreProperty]
    public string HomeTeam { get; set; }

    [FirestoreProperty]
    public int? HomeTeamBet { get; set; }

    [FirestoreProperty]
    public string AwayTeam { get; set; }

    [FirestoreProperty]
    public int? AwayTeamBet { get; set; }

    [FirestoreProperty]
    public double? Rate { get; set; }

    [FirestoreProperty]
    public int Day { get; set; }

    [FirestoreProperty]
    public int Status { get; set; }

    public MatchStatus MatchStatus => (MatchStatus)Status;

    public DateTime Date => DayProvider.GetDate(Day);

    public double HomeTeamWin => MatchStatus == MatchStatus.Win && Rate.HasValue && HomeTeamBet.HasValue ? HomeTeamBet.Value * Rate.Value : 0;

    public double AwayTeamWin => MatchStatus == MatchStatus.Win && Rate.HasValue && AwayTeamBet.HasValue ? AwayTeamBet.Value * Rate.Value : 0;
}