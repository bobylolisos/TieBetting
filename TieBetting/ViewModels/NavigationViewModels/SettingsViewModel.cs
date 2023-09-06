namespace TieBetting.ViewModels.NavigationViewModels;

public class SettingsViewModel : ViewModelNavigationBase
{
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private Settings _settings;
    private int _expectedWinAmount;
    private int _upcomingMatchesToFetch;
    private int _warnToBetWhenRateExceeds;
    private int _warnToBetWhenLostMatchesExceeds;

    public SettingsViewModel(INavigationService navigationService, IQueryService queryService, ISaverService saverService)
        : base(navigationService)
    {
        _queryService = queryService;
        _saverService = saverService;
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

    public int WarnToBetWhenRateExceeds
    {
        get => _warnToBetWhenRateExceeds;
        set
        {
            if (value == _warnToBetWhenRateExceeds) return;
            _warnToBetWhenRateExceeds = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(WarnToBetWhenRateExceedsText));
        }
    }

    public string WarnToBetWhenRateExceedsText => WarnToBetWhenRateExceeds.ToString().Insert(1, ".");

    public int WarnToBetWhenLostMatchesExceeds
    {
        get => _warnToBetWhenLostMatchesExceeds;
        set
        {
            if (value == _warnToBetWhenLostMatchesExceeds) return;
            _warnToBetWhenLostMatchesExceeds = value;
            OnPropertyChanged();
        }
    }

    public int UpcomingMatchesToFetch
    {
        get => _upcomingMatchesToFetch;
        set => SetProperty(ref _upcomingMatchesToFetch, value);
    }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        _settings = await _queryService.GetSettingsAsync();

        ExpectedWinAmount = _settings.ExpectedWinAmount;
        WarnToBetWhenRateExceeds = _settings.WarnToBetWhenRateExceeds;
        WarnToBetWhenLostMatchesExceeds = _settings.WarnToBetWhenLostMatchesExceeds;
        UpcomingMatchesToFetch = _settings.UpcomingFetchCount;

        await base.OnNavigatingToAsync(navigationParameter);
    }

    public override async Task OnNavigatedFromAsync(bool isForwardNavigation)
    {
        if (_settings.ExpectedWinAmount != ExpectedWinAmount || 
            _settings.WarnToBetWhenRateExceeds != WarnToBetWhenRateExceeds ||
            _settings.WarnToBetWhenLostMatchesExceeds != WarnToBetWhenLostMatchesExceeds ||
            _settings.UpcomingFetchCount != UpcomingMatchesToFetch)
        {
            _settings.ExpectedWinAmount = ExpectedWinAmount;
            _settings.WarnToBetWhenRateExceeds = WarnToBetWhenRateExceeds;
            _settings.WarnToBetWhenLostMatchesExceeds = WarnToBetWhenLostMatchesExceeds;
            _settings.UpcomingFetchCount = UpcomingMatchesToFetch;

            await _saverService.UpdateSettingsAsync(_settings);
        }

        await base.OnNavigatedFromAsync(isForwardNavigation);
    }
}