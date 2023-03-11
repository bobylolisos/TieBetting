namespace TieBetting.Services.Navigation.NavigationParameters;

public class MatchDetailsViewNavigationParameter : NavigationParameterBase
{
    public MatchDetailsViewNavigationParameter(MatchViewModel matchViewModel)
    {
        MatchViewModel = matchViewModel;
    }

    public MatchViewModel MatchViewModel { get; }
}