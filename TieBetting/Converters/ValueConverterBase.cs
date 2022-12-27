namespace TieBetting.Converters;

public abstract class ValueConverterBase<TSource> : ValueConverterBase<TSource, object>
{

}

public abstract class ValueConverterBase<TSource, TParameter> : IValueConverter
{
    protected abstract object Convert(TSource value, TParameter parameter);

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isParameterValid = IsParameterValid(parameter);

        if (isParameterValid)
        {
            if (value is TSource sourceValue)
            {
                return Convert(sourceValue, (TParameter)parameter);
            }

            if (value == null && IsTypeNullable(typeof(TSource)))
            {
                return Convert(default(TSource), (TParameter)parameter);
            }
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return ConvertBack(value, parameter);
    }

    protected virtual TSource ConvertBack(object value, object parameter)
    {
        throw new NotImplementedException();
    }

    private bool IsParameterValid(object parameter)
    {
        if (parameter == null)
        {
            return IsTypeNullable(typeof(TParameter));
        }

        return parameter is TParameter;
    }

    private bool IsTypeNullable(Type type)
    {
        if (!type.IsValueType)
        {
            return true;
        }

        if (Nullable.GetUnderlyingType(type) != null)
        {
            return true;
        }

        return false;
    }
}