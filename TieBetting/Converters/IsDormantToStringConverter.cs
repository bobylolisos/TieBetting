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

        if (matchViewModel.IsAnyNotActive() || matchViewModel.IsAnyActive())
        {
            return matchViewModel.IsDormant(teamType) || teamIsActive == false ? "IS DORMANT" : null;
        }

        if (matchViewModel.IsAnyDone())
        {
            return matchViewModel.IsDormant(teamType) ? "WAS DORMANT" : null;
        }

        return null;
    }
}