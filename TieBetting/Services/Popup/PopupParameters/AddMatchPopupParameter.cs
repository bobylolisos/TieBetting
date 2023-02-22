namespace TieBetting.Services.Popup.PopupParameters;

public class AddMatchPopupParameter : PopupParameterBase
{
    public AddMatchPopupParameter(string season)
    {
        Season = season;
    }

    public string Season { get; }
}