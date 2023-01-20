namespace TieBetting.Converters;

public class AmountToAmountStringConverter : ValueConverterBase<int?>
{
    protected override object Convert(int? value, object parameter)
    {
        if (value.HasValue == false)
        {
            return string.Empty;
        }
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";

        var formatted = value.Value.ToString("#,0", nfi);

        return $"{formatted} :-";
    }
}