namespace TieBetting.ViewModels;

public class SettingsViewModel : ViewModelNavigationBase
{
    private readonly IRepository _repository;
    private Settings _settings;
    private int _expectedWinAmount;
    private int _upcomingMatchesToFetch;

    public SettingsViewModel(INavigationService navigationService, IRepository repository) 
        : base(navigationService)
    {
        _repository = repository;
    }

    public int ExpectedWinAmount
    {
        get => _expectedWinAmount;
        set
        {
            if (SetProperty(ref _expectedWinAmount, value))
            {
                OnPropertyChanged(nameof(ExpectedWinAmountChanged));
            }
        }
    }

    public bool ExpectedWinAmountChanged => ExpectedWinAmount != _settings?.ExpectedWinAmount;

    public int UpcomingMatchesToFetch
    {
        get => _upcomingMatchesToFetch;
        set => SetProperty(ref _upcomingMatchesToFetch, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        _settings = await _repository.GetSettingsAsync();

        ExpectedWinAmount = _settings.ExpectedWinAmount;
        UpcomingMatchesToFetch = _settings.UpcomingFetchCount;

        await base.OnNavigatingToAsync(navigationParameter);
    }

    public override Task OnNavigatedFromAsync(bool isForwardNavigation)
    {
        // Todo: Save settings
        return base.OnNavigatedFromAsync(isForwardNavigation);
    }
}