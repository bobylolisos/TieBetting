namespace TieBetting.Services;

public class Repository : IRepository
{
    private const string MatchesCollectionKey = "matches";

    private FirestoreDb _firestoreDb;

    private async Task<FirestoreDb> CreateFirestoreDbAsync()
    {
        if (_firestoreDb != null)
        {
            return _firestoreDb;
        }

        // "Json" file from Google should be located under the "Resource/Raw" folder with a BuildAction "MauiAsset"

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json");
            var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = contents };
            var firestoreClient = await firestoreClientBuilder.BuildAsync();

            _firestoreDb = await FirestoreDb.CreateAsync("sandbox-73692", firestoreClient);
            return _firestoreDb;
        }
        catch (Exception e)
        {
            await Application.Current.MainPage.DisplayAlert("Firestore init failed", e.Message, "OK");
            throw;
        }
    }

    public async Task AddMatchesAsync(IReadOnlyCollection<Match> matches)
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var batch = firestoreDb.StartBatch();

        foreach (var match in matches)
        {
            var documentReference = firestoreDb.Collection(MatchesCollectionKey).Document(match.Id);
            batch.Set(documentReference, match);
        }

        await batch.CommitAsync();

    }

    public async Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var matches = new List<Match>();

        var fieldPath = nameof(Match.Day);

        var matchesQuery = firestoreDb.Collection(MatchesCollectionKey)
            .WhereGreaterThanOrEqualTo(fieldPath, (DateTime.Today - new DateTime(2022, 01, 01)).Days)
            .OrderBy(fieldPath)
            .Limit(numberOfMatches ?? int.MaxValue);
        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var match = documentSnapshot.ConvertTo<Match>();
            matches.Add(match);
        }

        return matches;
    }
}