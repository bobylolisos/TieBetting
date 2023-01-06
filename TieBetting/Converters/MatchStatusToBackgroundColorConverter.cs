﻿namespace TieBetting.Converters;

public class MatchStatusToBackgroundColorConverter : ValueConverterBase<MatchStatus>
{
    protected override object Convert(MatchStatus value, object parameter)
    {
        switch (value)
        {
            case MatchStatus.NotActive: 
                return Colors.LightGray;
            case MatchStatus.Active:
                return Colors.Orange;
            case MatchStatus.Lost:
                return Colors.OrangeRed;
            case MatchStatus.Win:
                return Colors.LightGreen;
            default:
                throw new ArgumentNullException($"Uknown status on match: <{value}");
        }
    }
}