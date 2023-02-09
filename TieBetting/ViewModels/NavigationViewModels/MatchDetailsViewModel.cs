namespace TieBetting.ViewModels.NavigationViewModels;

public class MatchDetailsViewModel : ViewModelNavigationBase, IPubSub<MatchRateChangedMessage>
{
    private readonly IPopupService _popupService;

    public MatchDetailsViewModel(INavigationService navigationService, IPopupService popupService) 
        : base(navigationService)
    {
        _popupService = popupService;

        EnterRateCommand = new AsyncRelayCommand(ExecuteEnterRateCommand, CanExecuteEnterRateCommand);
        SetStatusCommand = new AsyncRelayCommand<MatchStatus>(ExecuteSetStatusCommand);
    }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchDetailsViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

    public MatchBettingViewModel Match { get; set; }

    public AsyncRelayCommand EnterRateCommand { get; set; }

    public AsyncRelayCommand<MatchStatus> SetStatusCommand { get; set; }

    public async void Receive(MatchRateChangedMessage message)
    {
        if (!Equals(message?.Rate, Match.Rate))
        {
            await Match.SetRate(message?.Rate);
            SetStatusCommand.NotifyCanExecuteChanged();
        }
    }

    public void RegisterMessages()
    {
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void UnregisterMessages()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }

    private async Task ExecuteEnterRateCommand()
    {
        await _popupService.OpenPopupAsync<EnterRateView>(new EnterRatePopupParameter(Match.Rate));
    }

    private bool CanExecuteEnterRateCommand()
    {
        if (Match.Status != MatchStatus.NotActive)
        {
            return false;
        }

        // Make sure we won't set rate on match before an active match reported status
        if (Match.HomeTeam.Matches.Any(x => x.MatchStatus == MatchStatus.Active))
        {
            return false;
        }
        if (Match.HomeTeam.Matches.Any(x => x.MatchStatus == MatchStatus.NotActive && x.Day >= DayProvider.TodayDay && x.Rate.HasValue))
        {
            return false;
        }

        if (Match.AwayTeam.Matches.Any(x => x.MatchStatus == MatchStatus.Active))
        {
            return false;
        }
        if (Match.AwayTeam.Matches.Any(x => x.MatchStatus == MatchStatus.NotActive && x.Day >= DayProvider.TodayDay && x.Rate.HasValue))
        {
            return false;
        }

        return true;
    }

    private async Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus != Match.Status)
        {
            await Match.SetStatus(matchStatus);

            await ExecuteNavigateBackCommand();
        }
    }
}