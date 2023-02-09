using System.Resources;

namespace TieBetting.Converters;

public class LeagueToImageConverter : ValueConverterBase<string>
{
    protected override object Convert(string value, object parameter)
    {
        if (value == null)
        {
            return null;
        }

        if (value.ToLower().Contains("hockeyallsvenskan"))
        {
            return "ha.png";
        }

        if (value.ToLower().Contains("shl"))
        {
            return "shl.png";
        }

        return null;
    }
}