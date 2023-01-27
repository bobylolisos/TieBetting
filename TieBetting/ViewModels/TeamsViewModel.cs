namespace TieBetting.ViewModels;

public class TeamsViewModel : ViewModelNavigationBase
{
    public TeamsViewModel(INavigationService navigationService) 
        : base(navigationService)
    {
    }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamsViewNavigationParameter teamsViewNavigationParameter)
        {
            foreach (var team in teamsViewNavigationParameter.Teams)
            {
                Teams.Add(new TeamViewModel(team));
            }
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }
}