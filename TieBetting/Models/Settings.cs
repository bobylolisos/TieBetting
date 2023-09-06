namespace TieBetting.Models;

[FirestoreData]
public class Settings
{
    [FirestoreProperty]
    public string Id { get; set; }

    [FirestoreProperty]
    public int ExpectedWinAmount { get; set; }

    [FirestoreProperty]
    public int WarnToBetWhenRateExceeds { get; set; }

    [FirestoreProperty]
    public int WarnToBetWhenLostMatchesExceeds { get; set; }

    [FirestoreProperty]
    public int UpcomingFetchCount { get; set; }

    [FirestoreProperty]
    public string DefaultSeason { get; set; }

    [FirestoreProperty]
    public List<string> Seasons { get; set; }
}