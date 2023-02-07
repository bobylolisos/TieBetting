namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamMatchesViewNavigationParameter : NavigationParameterBase
{
    public TeamMatchesViewNavigationParameter(Team team)
    {
        Team = team;
    }

    public Team Team { get; }
}