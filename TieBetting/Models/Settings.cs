namespace TieBetting.Models;

[FirestoreData]
public class Settings
{

    [FirestoreProperty]
    public int ExpectedWinAmount { get; set; }

    [FirestoreProperty]
    public int UpcomingFetchCount { get; set; }
}