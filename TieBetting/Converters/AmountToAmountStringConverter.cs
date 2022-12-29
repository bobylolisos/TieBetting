namespace TieBetting.Converters;

public class AmountToAmountStringConverter : ValueConverterBase<int?>
{
    protected override object Convert(int? value, object parameter)
    {
        if (value.HasValue == false)
        {
            return string.Empty;
        }

        return $"  {value.Value} :-";
    }
}