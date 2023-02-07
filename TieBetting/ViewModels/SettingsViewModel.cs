namespace TieBetting.ViewModels;

public class SettingsViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private Settings _settings;
    private int _expectedWinAmount;
    private int _upcomingMatchesToFetch;

    public SettingsViewModel(INavigationService navigationService, IQueryService queryService) 
        : base(navigationService)
    {
        _queryService = queryService;
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
        _settings = await _queryService.GetSettingsAsync();

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