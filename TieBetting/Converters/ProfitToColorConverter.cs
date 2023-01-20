namespace TieBetting.Converters;

public class ProfitToColorConverter : ValueConverterBase<int>
{
    public ProfitToColorConverter()
    {
        NegativeColor = Colors.OrangeRed;
        ZeroColor = Colors.Black;
        PositiveColor = Colors.Green;
    }

    public Color NegativeColor { get; set; }
    
    public Color ZeroColor { get; set; }
    
    public Color PositiveColor { get; set; }

    protected override object Convert(int value, object parameter)
    {
        if (value < 0)
        {
            return NegativeColor;
        }

        if (value > 0)
        {
            return PositiveColor;
        }

        return ZeroColor;
    }
}