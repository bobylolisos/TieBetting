namespace TieBetting.Services;

public class DialogService : IDialogService
{
    public async Task ShowMessage(string title, string message)
    {
        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
    }
}