namespace TieBetting.Converters;

public class MatchStatusToBackgroundColorConverter : ValueConverterBase<MatchStatus>
{
    protected override object Convert(MatchStatus value, object parameter)
    {
        switch (value)
        {
            case MatchStatus.NotActive: 
                return Colors.Gray;
            case MatchStatus.Active:
                return Colors.Orange;
            case MatchStatus.Failed:
                return Colors.OrangeRed;
            case MatchStatus.Success:
                return Colors.LightGreen;
            default:
                throw new ArgumentNullException($"Uknown status on match: <{value}");
        }
    }
}