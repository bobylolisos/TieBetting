namespace TieBetting.Converters;

public class HasContentToVisibilityConverter : ValueConverterBase<string>
{
    public HasContentToVisibilityConverter()
    {
        // Default values
        VisibilityWhenHasContent = true;
        VisibilityWhenNoContent = false;
    }

    public bool VisibilityWhenHasContent { get; set; }

    public bool VisibilityWhenNoContent { get; set; }

    protected override object Convert(string value, object parameter)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return VisibilityWhenNoContent;
        }

        return VisibilityWhenHasContent;
    }
}

public class HasDecimalToVisibilityConverter : ValueConverterBase<decimal?>
{
    public HasDecimalToVisibilityConverter()
    {
        // Default values
        VisibilityWhenHasDecimal = true;
        VisibilityWhenNoDecimal = false;
    }

    public bool VisibilityWhenHasDecimal { get; set; }

    public bool VisibilityWhenNoDecimal { get; set; }

    protected override object Convert(decimal? value, object parameter)
    {
        if (value.HasValue)
        {
            return VisibilityWhenHasDecimal;
        }

        return VisibilityWhenNoDecimal;
    }
}