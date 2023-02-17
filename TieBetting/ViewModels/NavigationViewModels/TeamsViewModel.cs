namespace TieBetting.ViewModels.NavigationViewModels;

public class TeamsViewModel : ViewModelNavigationBase
{
    public TeamsViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        NavigateToTeamMatchesViewCommand = new AsyncRelayCommand<TeamViewModel>(ExecuteNavigateToTeamMatchesViewCommand);
    }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    public AsyncRelayCommand<TeamViewModel> NavigateToTeamMatchesViewCommand { get; set; }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamsViewNavigationParameter teamsViewNavigationParameter)
        {
            foreach (var team in teamsViewNavigationParameter.Teams)
            {
                Teams.Add(team);
            }
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

    private async Task ExecuteNavigateToTeamMatchesViewCommand(TeamViewModel teamViewModel)
    {
        await NavigationService.NavigateToPageAsync<TeamMatchesView>(new TeamMatchesViewNavigationParameter(teamViewModel));
    }
}