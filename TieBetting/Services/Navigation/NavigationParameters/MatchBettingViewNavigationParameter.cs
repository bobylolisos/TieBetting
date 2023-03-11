namespace TieBetting.Services.Navigation.NavigationParameters;

public class MatchBettingViewNavigationParameter : NavigationParameterBase
{
    public MatchBettingViewNavigationParameter(MatchViewModel matchViewModel)
    {
        MatchViewModel = matchViewModel;
    }

    public MatchViewModel MatchViewModel { get; }
}