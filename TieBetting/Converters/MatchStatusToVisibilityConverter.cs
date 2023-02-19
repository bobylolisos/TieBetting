namespace TieBetting.Converters;

public class MatchStatusToVisibilityConverter : ValueConverterBase<MatchStatus>
{
    public bool NotActiveVisibility { get; set; } = false;
    public bool ActiveVisibility { get; set; } = false;
    public bool LostVisibility { get; set; } = false;
    public bool WinVisibility { get; set; } = false;
    public bool DormantVisibility { get; set; } = false;


    protected override object Convert(MatchStatus value, object parameter)
    {
        switch (value)
        {
            case MatchStatus.NotActive:
                return NotActiveVisibility;
            case MatchStatus.Active:
                return ActiveVisibility;
            case MatchStatus.Lost:
                return LostVisibility;
            case MatchStatus.Win:
                return WinVisibility;
            case MatchStatus.Dormant:
                return DormantVisibility;
            default:
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
        }
    }
}