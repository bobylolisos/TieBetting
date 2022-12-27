namespace TieBetting.Converters;

public sealed class BooleanToVisibilityConverter : BooleanConverterBase<bool>
{
    public BooleanToVisibilityConverter()
        : base(true, false)
    { }
}