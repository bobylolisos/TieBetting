namespace TieBetting.ViewModels;

public class EnterRateViewModel : ViewModelBase, IPopupViewModel
{
    private decimal? _rate;

    public decimal? Rate
    {
        get => _rate;
        set => SetProperty(ref _rate, value);
    }

    public Task OnOpenPopupAsync(PopupParameterBase parameter = null)
    {
        if (parameter is EnterRatePopupParameter enterRatePopupParameter)
        {
            Rate = enterRatePopupParameter.Rate;
        }

        return Task.CompletedTask;
    }

    public async Task OnClosePopupAsync()
    {
        await Task.Delay(1);

        WeakReferenceMessenger.Default.Send(new MatchRateChangedMessage(Rate));
    }
}