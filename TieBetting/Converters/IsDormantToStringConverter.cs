namespace TieBetting.Converters;

public class IsDormantToStringConverter : ValueConverterBase<MatchViewModel, TeamType>
{
    protected override object Convert(MatchViewModel matchViewModel, TeamType teamType)
    {
        if (matchViewModel == null)
        {
            return null;
        }

        var teamIsActive = teamType == TeamType.HomeTeam ? matchViewModel.HomeTeam.IsActive : matchViewModel.AwayTeam.IsActive;

        if (matchViewModel.IsNotActive() || matchViewModel.IsActive())
        {
            return teamIsActive ? null : "IS DORMANT";
        }

        if (matchViewModel.IsActiveOrDone())
        {
            if (teamType == TeamType.HomeTeam)
            {
                if (matchViewModel.HomeTeamBet.HasValue && matchViewModel.HomeTeamBet.Value > 0)
                {
                    return null;
                }

                return "WAS DORMANT";
            }

            if (matchViewModel.AwayTeamBet.HasValue && matchViewModel.AwayTeamBet.Value > 0)
            {
                return null;
            }

            return "WAS DORMANT";
        }

        return null;
    }
}