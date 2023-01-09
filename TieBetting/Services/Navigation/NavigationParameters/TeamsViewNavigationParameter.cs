namespace TieBetting.Services.Navigation.NavigationParameters;

public class TeamsViewNavigationParameter : NavigationParameterBase
{
    public TeamsViewNavigationParameter(IReadOnlyCollection<Team> teams)
    {
        Teams = teams;
    }

    public IReadOnlyCollection<Team> Teams { get; }
}