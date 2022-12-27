namespace TieBetting.Shared.Components.TabBar;

public class TabBarSeparatorVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
        {
            throw new Exception("TabBarSeparatorVisibilityConverter requires 2 string values");
        }

        var stringValue1 = values[0] as string;
        var stringValue2 = values[1] as string;

        if (stringValue1.HasContent() || stringValue2.HasContent())
        {
            return true;
        }

        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}