namespace TieBetting.Services.Popup.PopupParameters;

public class EditMatchPopupParameter : PopupParameterBase
{
    public EditMatchPopupParameter(MatchViewModel match)
    {
        Match = match;
    }

    public MatchViewModel Match { get; }
}