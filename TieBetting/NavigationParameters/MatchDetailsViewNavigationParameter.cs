namespace TieBetting.NavigationParameters;

public class MatchDetailsViewNavigationParameter : NavigationParameterBase
{
    public MatchDetailsViewNavigationParameter(MatchViewModel matchViewModel)
    {
        MatchViewModel = matchViewModel;
    }

    public MatchViewModel MatchViewModel { get; }
}