namespace TieBetting.Converters;

public class HasDoubleToVisibilityConverter : ValueConverterBase<double?>
{
    public HasDoubleToVisibilityConverter()
    {
        // Default values
        VisibilityWhenHasDecimal = true;
        VisibilityWhenNoDecimal = false;
    }

    public bool VisibilityWhenHasDecimal { get; set; }

    public bool VisibilityWhenNoDecimal { get; set; }

    protected override object Convert(double? value, object parameter)
    {
        if (value.HasValue)
        {
            return VisibilityWhenHasDecimal;
        }

        return VisibilityWhenNoDecimal;
    }
}