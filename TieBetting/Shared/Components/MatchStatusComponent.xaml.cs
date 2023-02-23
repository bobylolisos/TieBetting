namespace TieBetting.Shared.Components;

public partial class MatchStatusComponent : ContentView
{
	public MatchStatusComponent()
	{
		InitializeComponent();
	}
}

public class TeamMatchStatusToBackgroundColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3)
        {
            throw new Exception("TeamMatchStatusToBackgroundColorConverter requires 3 values");
        }

        // first value is only used so propertychanged will trigger this converter
        var matchViewModel = values[1] as MatchViewModel;
        var teamType = (TeamType)values[2];

        if (matchViewModel == null)
        {
            return Colors.DarkGray;
        }

        if (teamType == TeamType.HomeTeam)
        {
            return GetMatchStatusColor(matchViewModel.HomeTeamMatchStatus);
        }

        // Away team
        return GetMatchStatusColor(matchViewModel.AwayTeamMatchStatus);
    }

    private static object GetMatchStatusColor(MatchStatus matchStatus)
    {
        switch (matchStatus)
        {
            case MatchStatus.NotActive:
                return Colors.DarkGray;
            case MatchStatus.Active:
                return Colors.Orange;
            case MatchStatus.Lost:
                return Colors.OrangeRed;
            case MatchStatus.Win:
                return Colors.Green;
            case MatchStatus.Dormant:
                return Colors.Black;
            default:
                throw new ArgumentNullException($"Unknown status on match: <{matchStatus}");
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}