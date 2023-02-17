namespace TieBetting.Models;

[FirestoreData]
public class Team
{
    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public string Image { get; set; }
}