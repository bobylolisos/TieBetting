namespace TieBetting.Shared.Extensions;

public static class StringExtensions
{
    public static bool HasContent(this string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }

    public static bool IsEmpty(this string str)
    {
        return string.IsNullOrWhiteSpace(str);
    }

    public static string ResolveTeamName(this string stringValue)
    {
        var teamName = stringValue.ToLower().ToLower();

        switch (teamName)
        {
            // Skellefteå needs to be before aik
            case { } str when str.Contains("skellefteå"):
                return "Skellefteå";
            case { } str when str.Contains("aik"):
                return "Aik";

            case { } str when str.Contains("almtuna"):
                return "Almtuna";
            case { } str when str.Contains("björklöven"):
                return "Björklöven";
            case { } str when str.Contains("djurgården"):
                return "Djurgården";
            case { } str when str.Contains("karlskoga"):
                return "Karlskoga";
            case { } str when str.Contains("kristianstad"):
                return "Kristianstad";
            case { } str when str.Contains("modo"):
                return "Modo";
            case { } str when str.Contains("mora"):
                return "Mora";
            case { } str when str.Contains("södertälje"):
                return "Södertälje";
            case { } str when str.Contains("tingsryd"):
                return "Tingsryd";
            case { } str when str.Contains("hästen"):
                return "Vita hästen";
            case { } str when str.Contains("västervik"):
                return "Västervik";
            case { } str when str.Contains("västerås"):
                return "Västerås";
            case { } str when str.Contains("östersund"):
                return "Östersund";

            case { } str when str.Contains("oskarshamn"):
                return "Oskarshamn";
            case { } str when str.Contains("växjö"):
                return "Växjö";
            case { } str when str.Contains("rögle"):
                return "Rögle";
            case { } str when str.Contains("brynäs"):
                return "Brynäs";
            case { } str when str.Contains("frölunda"):
                return "Frölunda";
            case { } str when str.Contains("timrå"):
                return "Timrå";
            case { } str when str.Contains("luleå"):
                return "Luleå";
            case { } str when str.Contains("hv71"):
                return "HV71";
            case { } str when str.Contains("malmö"):
                return "Malmö";
            case { } str when str.Contains("färjestad"):
                return "Färjestad";
            case { } str when str.Contains("leksands"):
                return "Leksand";
            case { } str when str.Contains("örebro"):
                return "Örebro";
            case { } str when str.Contains("linköping"):
                return "Linköping";

            default: return null;
        }

    }
}