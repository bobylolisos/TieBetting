namespace TieBetting.ViewModels.NavigationViewModels;

public class MatchMaintenanceViewModel : ViewModelNavigationBase, ITabBarItem1Command, ITabBarItem2Command, ITabBarItem3Command
{
    private readonly IPopupService _popupService;

    public MatchMaintenanceViewModel(INavigationService navigationService, IPopupService popupService)
        : base(navigationService)
    {
        _popupService = popupService;
        TabBarItem1Command = new AsyncRelayCommand(ExecuteChangeStatusCommand, CanExecuteChangeStatusCommand);
        TabBarItem2Command = new AsyncRelayCommand(ExecuteChangeDateCommand, CanExecuteChangeDateCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteDeleteMatchCommand, CanExecuteDeleteMatchCommand);
    }

    public MatchViewModel Match { get; set; }

    public AsyncRelayCommand TabBarItem1Command { get; }

    public AsyncRelayCommand TabBarItem2Command { get; }

    public AsyncRelayCommand TabBarItem3Command { get; }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchMaintenanceViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
            OnPropertyChanged(nameof(TabBarItem1Command));
            OnPropertyChanged(nameof(TabBarItem2Command));
            OnPropertyChanged(nameof(TabBarItem3Command));
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

    private async Task ExecuteChangeStatusCommand()
    {
        await _popupService.OpenPopupAsync<SelectStatusPopupView>(new SelectStatusPopupParameter(Match));

        OnPropertyChanged(nameof(Match));
    }

    private bool CanExecuteChangeStatusCommand()
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

    private async Task ExecuteChangeDateCommand()
    {
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");
    }

    private bool CanExecuteChangeDateCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.IsNotActive();
    }

    private async Task ExecuteDeleteMatchCommand()
    {
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");
    }

    private bool CanExecuteDeleteMatchCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.IsNotActive();
    }
}