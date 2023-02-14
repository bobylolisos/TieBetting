namespace TieBetting.Services.Popup.PopupParameters;

public class SelectStatusPopupParameter : PopupParameterBase
{
    public SelectStatusPopupParameter(MatchViewModel match)
    {
        Match = match;
    }

    public MatchViewModel Match { get; }
}