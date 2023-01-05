namespace TieBetting.Services.Popup;

public interface IPopupViewModel
{
    Task OnOpenPopupAsync(PopupParameterBase parameter = null);
    Task OnClosePopupAsync();
}