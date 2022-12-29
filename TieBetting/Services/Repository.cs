namespace TieBetting.Services;

public class Repository : IRepository
{
    private const string TeamsCollectionKey = "teams";
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
            Debug.WriteLine("CreateFirestoreDbAsync - Begin");
            var stream = await FileSystem.OpenAppPackageFileAsync("sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json");
            var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = contents };
            var firestoreClient = await firestoreClientBuilder.BuildAsync();

            _firestoreDb = await FirestoreDb.CreateAsync("sandbox-73692", firestoreClient);
            Debug.WriteLine("CreateFirestoreDbAsync - Done");
            return _firestoreDb;
        }
        catch (Exception e)
        {
            Debug.WriteLine("CreateFirestoreDbAsync - Failed");
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
        Debug.WriteLine("GetNextMatchesAsync - Begin");

        var firestoreDb = await CreateFirestoreDbAsync();

        var matches = new List<Match>();

        var fieldPath = nameof(Match.Day);

        var matchesQuery = firestoreDb.Collection(MatchesCollectionKey)
            .WhereGreaterThanOrEqualTo(fieldPath, (DateTime.Today - new DateTime(2022, 01, 01)).Days)
            .OrderBy(fieldPath)
            .Limit(numberOfMatches ?? int.MaxValue);
        Debug.WriteLine("GetNextMatchesAsync/GetSnapshotAsync - Begin");

        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        Debug.WriteLine("GetNextMatchesAsync/GetSnapshotAsync - Done");
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var match = documentSnapshot.ConvertTo<Match>();
            matches.Add(match);
        }

        Debug.WriteLine("GetNextMatchesAsync - Done");
        return matches;
    }

    public async Task AddTeamAsync(Team team)
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var collection = firestoreDb.Collection("teams");

        var document = collection.Document(team.Name);
        await document.SetAsync(team);
    }

    public async Task<Team> CreateTeamAsync(string teamName)
    {
        var team = new Team
        {
            Name = teamName
        };

        await AddTeamAsync(team);

        return team;
    }

    public async Task<IReadOnlyCollection<Team>> GetTeamsAsync()
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var teams = new List<Team>();

        var matchesQuery = firestoreDb.Collection(TeamsCollectionKey);
        Debug.WriteLine("GetTeamsAsync/GetSnapshotAsync - Begin");
        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var team = documentSnapshot.ConvertTo<Team>();
            teams.Add(team);
        }

        Debug.WriteLine("GetTeamsAsync/GetSnapshotAsync - Done");
        return teams;
    }
}