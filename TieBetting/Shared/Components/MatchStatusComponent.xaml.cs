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

        if (teamType == TeamType.HomeTeam && matchViewModel.HasHomeTeamBet() != true)
        {
            return Colors.DarkGray;
        }

        if (teamType == TeamType.AwayTeam && matchViewModel.HasAwayTeamBet() != true)
        {
            return Colors.DarkGray;
        }

        switch (matchViewModel.MatchStatus)
        {
            case MatchStatus.NotActive:
                return Colors.DarkGray;
            case MatchStatus.Active:
                return Colors.Orange;
            case MatchStatus.Lost:
                return Colors.OrangeRed;
            case MatchStatus.Win:
                return Colors.Green;
            default:
                throw new ArgumentNullException($"Uknown status on match: <{matchViewModel.MatchStatus}");
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}