namespace TieBetting.Services.Popup.PopupParameters;

public class EnterRatePopupParameter : PopupParameterBase
{
    public EnterRatePopupParameter(decimal? rate)
    {
        Rate = rate;
    }

    public decimal? Rate { get; }
}