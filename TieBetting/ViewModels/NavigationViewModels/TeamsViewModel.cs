namespace TieBetting.ViewModels.NavigationViewModels;

public class TeamsViewModel : ViewModelNavigationBase
{
    public TeamsViewModel(INavigationService navigationService)
        : base(navigationService)
    {
        NavigateToTeamMaintenanceViewCommand = new AsyncRelayCommand<TeamViewModel>(ExecuteNavigateToTeamMaintenanceViewCommand);
    }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    public AsyncRelayCommand<TeamViewModel> NavigateToTeamMaintenanceViewCommand { get; set; }

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

    private async Task ExecuteNavigateToTeamMaintenanceViewCommand(TeamViewModel teamViewModel)
    {
        await NavigationService.NavigateToPageAsync<TeamMaintenanceView>(new TeamMaintenanceViewNavigationParameter(teamViewModel));
    }
}