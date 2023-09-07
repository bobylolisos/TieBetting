namespace TieBetting.Services.Popup;

public interface IPopupService
{
    public Task OpenPopupAsync<T>(PopupParameterBase parameter = null) where T : BasePopupPage;

    public Task ClosePopupAsync(bool confirmed);
}