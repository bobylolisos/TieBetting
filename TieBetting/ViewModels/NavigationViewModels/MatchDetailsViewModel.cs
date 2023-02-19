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
        TabBarItem1Command = new AsyncRelayCommand(ExecuteShowSelectStatusPopupView, CanExecuteShowSelectStatusPopupView);
    }

    public MatchBettingViewModel Match { get; set; }

    public AsyncRelayCommand EnterRateCommand { get; }

    public AsyncRelayCommand<MatchStatus> SetStatusCommand { get; }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchDetailsViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
            NotifyTabItemsCanExecuteChanged();
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

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
        await _popupService.OpenPopupAsync<EnterRatePopupView>(new EnterRatePopupParameter(Match.Rate));
    }

    private bool CanExecuteEnterRateCommand()
    {
        if (Match.IsActiveOrDone())
        {
            return false;
        }

        // Make sure we won't set rate on match before an active match reported status
        if (Match.HomeTeam.Matches.Any(x => x.IsActive()))
        {
            return false;
        }
        if (Match.HomeTeam.Matches.Any(x => x.IsNotActive() && x.Day >= DayProvider.TodayDay && x.Day != Match.Day && x.Rate.HasValue))
        {
            return false;
        }

        if (Match.AwayTeam.Matches.Any(x => x.IsActive()))
        {
            return false;
        }
        if (Match.AwayTeam.Matches.Any(x => x.IsNotActive() && x.Day >= DayProvider.TodayDay && x.Day != Match.Day && x.Rate.HasValue))
        {
            return false;
        }

        return true;
    }

    private async Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus != Match.MatchStatus)
        {
            await Match.SetStatus(matchStatus);

            await ExecuteNavigateBackCommand();

            OnPropertyChanged(nameof(IsTabBarVisible));
        }
    }

    private async Task ExecuteShowSelectStatusPopupView()
    {
        await _popupService.OpenPopupAsync<SelectStatusPopupView>(new SelectStatusPopupParameter(Match));

        OnPropertyChanged(nameof(Match));
        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteShowSelectStatusPopupView()
    {
        if (Match == null)
        {
            return false;
        }

        var homeTeamHasLaterActiveMatches = Match.HomeTeam.Matches.Any(x => x.Day > Match.Day && x.IsActiveOrDone());
        if (homeTeamHasLaterActiveMatches)
        {
            return false;
        }

        var awayTeamHasLaterActiveMatches = Match.AwayTeam.Matches.Any(x => x.Day > Match.Day && x.IsActiveOrDone());
        if (awayTeamHasLaterActiveMatches)
        {
            return false;
        }

        return Match.IsActiveOrDone();
    }


    public bool IsTabBarVisible => Match?.IsActiveOrDone() ?? false;

}