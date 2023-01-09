namespace TieBetting.Converters;

public class ProfitToColorConverter : ValueConverterBase<int>
{
    protected override object Convert(int value, object parameter)
    {
        if (value < 0)
        {
            return Colors.OrangeRed;
        }

        if (value > 0)
        {
            return Colors.Green;
        }

        return Colors.Black;
    }
}