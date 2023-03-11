namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamMaintenanceViewNavigationParameter : NavigationParameterBase
{
    public TeamMaintenanceViewNavigationParameter(TeamViewModel team)
    {
        Team = team;
    }

    public TeamViewModel Team { get; }
}