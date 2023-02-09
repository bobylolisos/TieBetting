namespace TieBetting.ViewModels.NavigationViewModels;

public class MatchMaintenanceViewModel : ViewModelNavigationBase, ITabBarItem1Command, ITabBarItem2Command, ITabBarItem3Command
{
    public MatchMaintenanceViewModel(INavigationService navigationService)
        : base(navigationService)
    {
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

    private Task ExecuteChangeStatusCommand()
    {
        return Task.CompletedTask;
    }

    private bool CanExecuteChangeStatusCommand()
    {
        if (Match == null)
        {
            return false;
        }

        var homeTeamHasLaterActiveMatches = Match.HomeTeam.Matches.Any(x => x.Day > Match.Day && x.MatchStatus != MatchStatus.NotActive);
        if (homeTeamHasLaterActiveMatches)
        {
            return false;
        }

        var awayTeamHasLaterActiveMatches = Match.AwayTeam.Matches.Any(x => x.Day > Match.Day && x.MatchStatus != MatchStatus.NotActive);
        if (awayTeamHasLaterActiveMatches)
        {
            return false;
        }

        return Match.Status != MatchStatus.NotActive;
    }

    private Task ExecuteChangeDateCommand()
    {
        return Task.CompletedTask;
    }

    private bool CanExecuteChangeDateCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.Status == MatchStatus.NotActive;
    }

    private Task ExecuteDeleteMatchCommand()
    {
        return Task.CompletedTask;
    }

    private bool CanExecuteDeleteMatchCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.Status == MatchStatus.NotActive;
    }
}