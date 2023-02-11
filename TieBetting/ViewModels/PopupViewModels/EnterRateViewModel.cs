namespace TieBetting.ViewModels.PopupViewModels;

public class EnterRateViewModel : ViewModelBase, IPopupViewModel
{
    private string _rate;

    public EnterRateViewModel()
    {
        DigitCommand = new RelayCommand<string>(ExecuteDigitCommand, CanExecuteDigitCommand);
        CommaCommand = new RelayCommand(ExecuteCommaCommand, CanExecuteCommaCommand);
        RemoveDigitCommand = new RelayCommand(ExecuteRemoveDigitCommand, CanExecuteRemoveDigitCommand);
    }

    public RelayCommand<string> DigitCommand { get; set; }

    public RelayCommand CommaCommand { get; set; }

    public RelayCommand RemoveDigitCommand { get; set; }

    public string Rate
    {
        get => _rate;
        set
        {
            if (value.Length > 4)
            {
                return;
            }

            if (SetProperty(ref _rate, value))
            {
                DigitCommand.NotifyCanExecuteChanged();
                CommaCommand.NotifyCanExecuteChanged();
                RemoveDigitCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public Task OnOpenPopupAsync(PopupParameterBase parameter = null)
    {
        if (parameter is EnterRatePopupParameter enterRatePopupParameter)
        {
            Rate = enterRatePopupParameter.Rate?.ToString() ?? string.Empty;
        }

        return Task.CompletedTask;
    }

    public async Task<bool> OnClosePopupAsync()
    {
        await Task.Delay(1);

        double? rate = null;

        if (Rate.Any())
        {
            rate = GetDoubleFromString(Rate);

            if (rate == 0)
            {
                rate = null;
            }
        }

        if (rate.HasValue)
        {
            if (rate.Value > 7)
            {
                await Application.Current.MainPage.DisplayAlert("Rate", "To high rate", "Ok");
                return false;
            }
        }

        WeakReferenceMessenger.Default.Send(new MatchRateChangedMessage(rate));

        return true;
    }

    private void ExecuteDigitCommand(string digit)
    {
        Rate += digit;
    }

    private bool CanExecuteDigitCommand(string digit)
    {
        if (Rate != null && Rate.Any())
        {
            if (Rate.Length == 1)
            {
                return false;
            }

            return true;
        }

        if (digit == "0" || digit == "1" || digit == "8" || digit == "9")
        {
            return false;
        }

        return true;
    }


    private double GetDoubleFromString(string str)
    {
        if (double.TryParse(str, out var dec1))
        {
            return dec1;
        }
        // Ugly fix to cover different languages, Todo: Fix this
        str = str.Replace('.', ',');
        if (double.TryParse(str, out var dec2))
        {
            return dec2;
        }

        throw new InvalidOperationException("Unable to parse Rate from string");
    }

    private void ExecuteCommaCommand()
    {
        Rate += ".";
    }

    private bool CanExecuteCommaCommand()
    {
        return Rate != null && Rate.IsEmpty() == false && Rate.Contains('.') == false && Rate.Length < 3;
    }

    private void ExecuteRemoveDigitCommand()
    {
        if (Rate.Length > 0)
        {
            Rate = Rate.Remove(Rate.Length - 1, 1);
        }
    }

    private bool CanExecuteRemoveDigitCommand()
    {
        return Rate != null && Rate.Any();
    }

}