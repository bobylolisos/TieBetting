namespace TieBetting.Services.Navigation.NavigationParameters;

public class MatchMaintenanceViewNavigationParameter : NavigationParameterBase
{
    public MatchMaintenanceViewNavigationParameter(MatchViewModel matchViewModel)
    {
        MatchViewModel = matchViewModel;
    }

    public MatchViewModel MatchViewModel { get; }
}