using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        MyCommand = new AsyncRelayCommand(ExecuteMyCommand);
    }

    private async Task ExecuteMyCommand()
    {
        var firestoreDb = await CreateFirestoreDb();

        
        var teams = firestoreDb.Collection("teams").ListDocumentsAsync();



        await foreach (var asd in teams)
        {
            var snapshot = await asd.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var userDictionary = snapshot.ToDictionary();

            }
        }

        var dbg = "";
    }
    private FirestoreDb _firestoreDbDontUseDirectly;

    public AsyncRelayCommand MyCommand { get; set; }

    private async Task<FirestoreDb> CreateFirestoreDb()
    {
        if (_firestoreDbDontUseDirectly != null)
        {
            return _firestoreDbDontUseDirectly;
        }

        // "Json" file from Google should be located under the "Resource/Raw" folder with a BuildAction "MauiAsset"

        try
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("sandbox-73692-firebase-adminsdk-6khte-b27b19a9d6.json");
            var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            var firestoreClientBuilder = new FirestoreClientBuilder { JsonCredentials = contents };
            var firestoreClient = await firestoreClientBuilder.BuildAsync();

            _firestoreDbDontUseDirectly = await FirestoreDb.CreateAsync("sandbox-73692", firestoreClient);
            return _firestoreDbDontUseDirectly;
        }
        catch (Exception e)
        {
            await Application.Current.MainPage.DisplayAlert("Firestore init failed", e.Message, "OK");
            throw;
        }
    }
}