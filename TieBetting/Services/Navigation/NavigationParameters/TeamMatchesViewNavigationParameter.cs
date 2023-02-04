namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamMatchesViewNavigationParameter : NavigationParameterBase
{
    public TeamMatchesViewNavigationParameter(TeamViewModel teamViewModel)
    {
        TeamViewModel = teamViewModel;
    }

    public TeamViewModel TeamViewModel { get; }
}