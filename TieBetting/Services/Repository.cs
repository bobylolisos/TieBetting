namespace TieBetting.Services;

public class Repository : IRepository
{
    private const string TeamsCollectionKey = "teams";
    private const string MatchesCollectionKey = "matches";

    private FirestoreDb _firestoreDb;
    private string _credentials;

    private async Task<FirestoreDb> CreateFirestoreDbAsync(bool sandbox = true)
    {
        _credentials = null;
        string filename;
        string projectId;
        if (sandbox)
        {
            filename = "sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json";
            projectId = "sandbox-73692";
        }
        else
        {
            filename = "tiebetting-firebase-adminsdk-xm5en-3de0c69790.json";
            projectId = "tiebetting";
        }

        try
        {
            Debug.WriteLine("CreateFirestoreDbAsync - Begin");
            if (_credentials == null)
            {
                Debug.WriteLine("CreateFirestoreDbAsync/OpenAppPackageFileAsync");
                using var stream = FileSystem.OpenAppPackageFileAsync(filename).Result;



                //await using var stream = await FileSystem.OpenAppPackageFileAsync("tiebetting-firebase-adminsdk-xm5en-3de0c69790.json");
                //await using var stream = await FileSystem.OpenAppPackageFileAsync("sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json");
                using var reader = new StreamReader(stream);
                _credentials = reader.ReadToEndAsync().Result;

            }

            Debug.WriteLine("CreateFirestoreDbAsync/Create FirestoreClientBuilder");
            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = _credentials };
            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreClientBuilder.BuildAsync");
            var firestoreClient = firestoreClientBuilder.BuildAsync().Result;

            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreDb.CreateAsync");
            _firestoreDb = FirestoreDb.CreateAsync(projectId, firestoreClient).Result;
            //_firestoreDb = await FirestoreDb.CreateAsync("sandbox-73692", firestoreClient);
            Debug.WriteLine("CreateFirestoreDbAsync - Done");
            return _firestoreDb;
        }
        catch (Exception e)
        {
            Debug.WriteLine("CreateFirestoreDbAsync - Failed");
            await Application.Current.MainPage.DisplayAlert("Firestore init failed", e.Message, "OK");

            Application.Current.Quit();
            throw;
        }
    }

    private async Task<FirestoreDb> CreateFirestoreDbAsync2()
    {
        if (_firestoreDb != null)
        {
            return _firestoreDb;
        }

        // "Json" file from Google should be located under the "Resource/Raw" folder with a BuildAction "MauiAsset"

        try
        {
            Debug.WriteLine("CreateFirestoreDbAsync - Begin");
            Debug.WriteLine("CreateFirestoreDbAsync/OpenAppPackageFileAsync");
            await using var stream = await FileSystem.OpenAppPackageFileAsync("tiebetting-firebase-adminsdk-xm5en-3de0c69790.json");
            //await using var stream = await FileSystem.OpenAppPackageFileAsync("sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json");
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            Debug.WriteLine("CreateFirestoreDbAsync/Create FirestoreClientBuilder");
            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = contents };
            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreClientBuilder.BuildAsync");
            var firestoreClient = await firestoreClientBuilder.BuildAsync();

            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreDb.CreateAsync");
            _firestoreDb = await FirestoreDb.CreateAsync("tiebetting", firestoreClient);
            //_firestoreDb = await FirestoreDb.CreateAsync("sandbox-73692", firestoreClient);
            Debug.WriteLine("CreateFirestoreDbAsync - Done");
            return _firestoreDb;
        }
        catch (Exception e)
        {
            Debug.WriteLine("CreateFirestoreDbAsync - Failed");
            await Application.Current.MainPage.DisplayAlert("Firestore init failed", e.Message, "OK");

            Application.Current.Quit();
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

    //private async Task AddTeamsToSandboxAsync(IReadOnlyCollection<Team> teams)
    //{
    //    var firestoreDb = await CreateFirestoreDbAsync(true);

    //    var batch = firestoreDb.StartBatch();

    //    foreach (var team in teams)
    //    {
    //        var documentReference = firestoreDb.Collection(TeamsCollectionKey).Document(team.Name);
    //        batch.Set(documentReference, team);
    //    }

    //    await batch.CommitAsync();

    //}    
    
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

    public async Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync()
    {
        Debug.WriteLine("GetPreviousOngoingMatchesAsync - Begin");

        var firestoreDb = await CreateFirestoreDbAsync();

        var matches = new List<Match>();

        var fieldPath = nameof(Match.Status);

        var matchesQuery = firestoreDb.Collection(MatchesCollectionKey)
            .WhereEqualTo(fieldPath, 1);
        Debug.WriteLine("GetPreviousOngoingMatchesAsync/GetSnapshotAsync - Begin");

        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        Debug.WriteLine("GetPreviousOngoingMatchesAsync/GetSnapshotAsync - Done");
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var match = documentSnapshot.ConvertTo<Match>();

            if (match.Day < (DateTime.Today - new DateTime(2022, 01, 01)).Days)
            {
                matches.Add(match);
            }
        }

        Debug.WriteLine("GetPreviousOngoingMatchesAsync - Done");
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
            Name = teamName,
            CurrentBetSession = 0,
            Image = "***.png",
            TotalBet = 0,
            TotalWin = 0
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

        //await AddTeamsToSandboxAsync(teams);

        return teams;
    }

    public async Task UpdateMatchAsync(Match match)
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var matchDocumentReference = firestoreDb.Collection(MatchesCollectionKey).Document(match.Id);
        await matchDocumentReference.SetAsync(match);
    }

    public async Task UpdateTeamAsync(Team team)
    {
        var firestoreDb = await CreateFirestoreDbAsync();

        var teamDocumentReference = firestoreDb.Collection(TeamsCollectionKey).Document(team.Name);
        await teamDocumentReference.SetAsync(team);
    }

}