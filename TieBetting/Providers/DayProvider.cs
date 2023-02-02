namespace TieBetting.Providers;

public class DayProvider
{
    public static int TodayDay => (DateTime.Today - new DateTime(2022, 01, 01)).Days;

    public static int GetDay(DateTime dateTime)
    {
        return (dateTime - new DateTime(2022, 01, 01)).Days;
    }

    public static DateTime GetDate(int day)
    {
        return new DateTime(2022, 01, 01).AddDays(day);
    }
}