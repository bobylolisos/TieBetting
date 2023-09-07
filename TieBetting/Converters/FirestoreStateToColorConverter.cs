namespace TieBetting.Converters;

public class FirestoreStateToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (FirestoreRepository.SandBox)
        {
            return Colors.Red;
        }

        if (App.Current.Resources.TryGetValue("Primary", out var colorvalue))
        {
            return (Color)colorvalue;
        }

        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}