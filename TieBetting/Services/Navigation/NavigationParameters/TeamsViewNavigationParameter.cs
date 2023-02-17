namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamsViewNavigationParameter : NavigationParameterBase
{
    public TeamsViewNavigationParameter(IReadOnlyCollection<TeamViewModel> teams)
    {
        Teams = teams;
    }

    public IReadOnlyCollection<TeamViewModel> Teams { get; }
}