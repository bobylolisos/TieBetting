namespace TieBetting.ViewModels;

public class TeamMatchesViewModel : ViewModelNavigationBase
{
    public TeamMatchesViewModel(INavigationService navigationService) 
        : base(navigationService)
    {
    }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamMatchesViewNavigationParameter parameter)
        {
            var team = parameter.TeamViewModel;
        }

        return base.OnNavigatingToAsync(navigationParameter);

    }
}