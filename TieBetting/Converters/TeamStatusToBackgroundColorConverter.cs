namespace TieBetting.Converters;

public class TeamStatusToBackgroundColorConverter : ValueConverterBase<bool?>
{
    protected override object Convert(bool? value, object parameter)
    {
        if (value.HasValue == false)
        {
            return Colors.LightGray;
        }

        return value.Value ? Colors.LightGreen : Colors.OrangeRed;
    }
}