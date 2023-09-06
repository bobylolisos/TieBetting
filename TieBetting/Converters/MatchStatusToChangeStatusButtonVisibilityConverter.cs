namespace TieBetting.Converters;

public class MatchStatusToChangeStatusButtonVisibilityConverter : ValueConverterBase<MatchStatus, MatchStatus>
{
    protected override object Convert(MatchStatus value, MatchStatus parameter)
    {
        if (value == MatchStatus.NotActive)
        {
            return false;
        }

        if (value == MatchStatus.Dormant)
        {
            return parameter == MatchStatus.NotActive;
        }

        if (value == MatchStatus.Active)
        {
            return parameter != MatchStatus.Active;
        }

        if (value == MatchStatus.Lost)
        {
            return parameter != MatchStatus.Lost;
        }

        if (value == MatchStatus.Win)
        {
            return parameter != MatchStatus.Win;
        }

        return true;
    }
}