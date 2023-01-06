namespace TieBetting.Services.Popup.PopupParameters;

public class EnterRatePopupParameter : PopupParameterBase
{
    public EnterRatePopupParameter(double? rate)
    {
        Rate = rate;
    }

    public double? Rate { get; }
}