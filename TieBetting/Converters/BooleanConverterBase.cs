namespace TieBetting.Converters;

public abstract class BooleanConverterBase<T> : ValueConverterBase<bool>
{
    protected BooleanConverterBase()
    { }

    protected BooleanConverterBase(T trueValue, T falseValue)
    {
        True = trueValue;
        False = falseValue;
    }

    public T True { get; set; }

    public T False { get; set; }

    protected override object Convert(bool value, object parameter)
    {
        return value ? True : False;
    }

    protected override bool ConvertBack(object value, object parameter)
    {
        return value is T genericValue && EqualityComparer<T>.Default.Equals(genericValue, True);
    }
}