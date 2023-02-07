namespace TieBetting.Services.Navigation.NavigationParameters;

public class MatchDetailsViewNavigationParameter : NavigationParameterBase
{
    public MatchDetailsViewNavigationParameter(MatchBettingViewModel matchViewModel)
    {
        MatchViewModel = matchViewModel;
    }

    public MatchBettingViewModel MatchViewModel { get; }
}