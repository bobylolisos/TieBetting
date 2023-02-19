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
    public int HomeTeamStatus { get; set; }

    [FirestoreProperty]
    public string AwayTeam { get; set; }

    [FirestoreProperty]
    public int? AwayTeamBet { get; set; }

    [FirestoreProperty]
    public int AwayTeamStatus { get; set; }
    
    [FirestoreProperty]
    public double? Rate { get; set; }

    [FirestoreProperty]
    public int Day { get; set; }
}