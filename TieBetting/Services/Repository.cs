namespace TieBetting.Services;

public class Repository : IRepository
{
    private const string TeamsCollectionKey = "teams";
    private const string MatchesCollectionKey = "matches";

    private FirestoreDb _firestoreDb;
    private string _credentials;
    private IReadOnlyCollection<Match> _allMatchesCache = null;
    private List<Team> _allTeamsCache = null;


    private async Task<FirestoreDb> CreateFirestoreDbAsync(bool sandbox = false)
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

                using var reader = new StreamReader(stream);
                _credentials = reader.ReadToEndAsync().Result;

            }

            Debug.WriteLine("CreateFirestoreDbAsync/Create FirestoreClientBuilder");
            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = _credentials };
            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreClientBuilder.BuildAsync");
            var firestoreClient = firestoreClientBuilder.BuildAsync().Result;

            Debug.WriteLine("CreateFirestoreDbAsync/FirestoreDb.CreateAsync");
            _firestoreDb = FirestoreDb.CreateAsync(projectId, firestoreClient).Result;
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

    public async Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null)
    {
        try
        {
            Debug.WriteLine("GetNextMatchesAsync - Begin");

            var firestoreDb = await CreateFirestoreDbAsync();

            var allMatches = await GetAllMatchesAsync(firestoreDb);

            var upcomingMatches = allMatches.Where(x => x.Day >= DayProvider.TodayDay);
            if (numberOfMatches.HasValue)
            {
                return upcomingMatches.Take(numberOfMatches.Value).ToList();

            }
            return upcomingMatches.ToList();

        }
        finally
        {
            Debug.WriteLine("GetNextMatchesAsync - Done");
        }
    }

    private async Task<IReadOnlyCollection<Match>> GetAllMatchesAsync(FirestoreDb firestoreDb)
    {
        if (_allMatchesCache != null)
        {
            return _allMatchesCache;
        }

        Debug.WriteLine("GetAllMatchesAsync - Begin");

        var matches = new List<Match>();

        var fieldPath = nameof(Match.Day);

        var matchesQuery = firestoreDb.Collection(MatchesCollectionKey)
            .OrderBy(fieldPath);
        Debug.WriteLine("GetAllMatchesAsync/GetSnapshotAsync - Begin");

        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        Debug.WriteLine("GetAllMatchesAsync/GetSnapshotAsync - Done");
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var match = documentSnapshot.ConvertTo<Match>();
            matches.Add(match);
        }

        Debug.WriteLine("GetAllMatchesAsync - Done");

        _allMatchesCache = matches;
        return _allMatchesCache;
    }

    public async Task<IReadOnlyCollection<Match>> GetPreviousOngoingMatchesAsync()
    {
        try
        {
            Debug.WriteLine("GetPreviousOngoingMatchesAsync - Begin");

            var firestoreDb = await CreateFirestoreDbAsync();

            var allMatches = await GetAllMatchesAsync(firestoreDb);

            return allMatches.Where(x => x.Day < DayProvider.TodayDay && x.MatchStatus == MatchStatus.Active).ToList();
        }
        finally
        {
            Debug.WriteLine("GetPreviousOngoingMatchesAsync - Done");
        }
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
            Image = "***.png",
        };

        await AddTeamAsync(team);

        return team;
    }

    public async Task<IReadOnlyCollection<Team>> GetTeamsAsync()
    {
        if (_allTeamsCache != null)
        {
            return _allTeamsCache;
        }

        var firestoreDb = await CreateFirestoreDbAsync();

        List<Team> teams = new List<Team>();

        var matchesQuery = firestoreDb.Collection(TeamsCollectionKey);
        Debug.WriteLine("GetTeamsAsync/GetSnapshotAsync - Begin");
        var matchesQuerySnapshot = await matchesQuery.GetSnapshotAsync();
        foreach (var documentSnapshot in matchesQuerySnapshot.Documents)
        {
            var team = documentSnapshot.ConvertTo<Team>();
            var allMatches = await GetAllMatchesAsync(firestoreDb);
            team.AddMatches(allMatches);
            teams.Add(team);
        }

        Debug.WriteLine("GetTeamsAsync/GetSnapshotAsync - Done");

        _allTeamsCache = teams;

        /* --- Used when we want to update Sandbox to a copy of production firestore --- */
        //var sandboxFirestoreDb = await CreateFirestoreDbAsync(true);
        //foreach (var team in teams)
        //{
        //    await UpdateTeamAsync(team, sandboxFirestoreDb);
        //}

        return _allTeamsCache;
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

    /// <summary>
    /// Used when we want to update Sandbox to a copy of TieBetting firestore
    /// </summary>
    private async Task UpdateMatchesAsync(IReadOnlyCollection<Match> matches, FirestoreDb firestoreDb)
    {
        var batch = firestoreDb.StartBatch();

        foreach (var match in matches)
        {
            var documentReference = firestoreDb.Collection(MatchesCollectionKey).Document(match.Id);
            batch.Set(documentReference, match);
        }

        await batch.CommitAsync();
    }

    /// <summary>
    /// Used when we want to update Sandbox to a copy of TieBetting firestore
    /// </summary>
    private async Task UpdateTeamAsync(Team team, FirestoreDb firestoreDb)
    {
        var teamDocumentReference = firestoreDb.Collection(TeamsCollectionKey).Document(team.Name);
        await teamDocumentReference.SetAsync(team);
    }
}