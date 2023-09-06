namespace TieBetting.Converters;

public class MatchStatusToStatusButtonVisibilityConverter : ValueConverterBase<MatchStatus, MatchStatus>
{
    protected override object Convert(MatchStatus value, MatchStatus parameter)
    {
        if (value == MatchStatus.NotActive)
        {
            return parameter == MatchStatus.Active || parameter == MatchStatus.Dormant;
        }

        if (value == MatchStatus.Active)
        {
            return parameter == MatchStatus.Lost || parameter == MatchStatus.Win || parameter == MatchStatus.Dormant;
        }

        return false;
    }
}