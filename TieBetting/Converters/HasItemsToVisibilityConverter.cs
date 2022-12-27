namespace TieBetting.Converters;

public class HasItemsToVisibilityConverter : ValueConverterBase<IEnumerable<object>>
{
    public HasItemsToVisibilityConverter()
    {
        // Default values
        VisibilityWhenHasItems = true;
        VisibilityWhenNoItems = false;
    }

    public bool VisibilityWhenHasItems { get; set; }

    public bool VisibilityWhenNoItems { get; set; }

    protected override object Convert(IEnumerable<object> value, object parameter)
    {
        if (value != null && value.Any())
        {
            return VisibilityWhenHasItems;
        }

        return VisibilityWhenNoItems;
    }
}