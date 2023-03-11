namespace TieBetting.Services;

public interface IDialogService
{
    Task ShowMessage(string title, string message);

    Task<bool> ShowQuestion(string title, string message);
}