namespace TieBetting.ApplicationSetup;

public static class ServiceSetup
{
    public static MauiAppBuilder SetupServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IPopupService, PopupService>();
        builder.Services.AddSingleton<ICalendarFileDownloadService, CalendarFileDownloadService>();
        builder.Services.AddSingleton<IRepository, Repository>();

        return builder;
    }
}