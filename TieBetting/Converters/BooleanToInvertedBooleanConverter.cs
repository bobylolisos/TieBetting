namespace TieBetting.Converters;

public class BooleanToInvertedBooleanConverter : BooleanConverterBase<bool>
{
    public BooleanToInvertedBooleanConverter()
        : base(false, true)
    { }
}