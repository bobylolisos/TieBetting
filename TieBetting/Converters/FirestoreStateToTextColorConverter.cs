namespace TieBetting.Converters;

public class FirestoreStateToTextColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (FirestoreRepository.SandBox)
        {
            return Colors.Red;
        }

        return Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}