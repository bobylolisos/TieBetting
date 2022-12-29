namespace TieBetting.ViewModels;

public class MatchDetailsViewModel : ViewModelNavigationBase
{
    public MatchViewModel Match { get; set; }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchDetailsViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }
}