namespace TieBetting.ApplicationSetup;

public static class ServiceSetup
{
    public static MauiAppBuilder SetupServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IPopupService, PopupService>();
        builder.Services.AddSingleton<ICalendarFileDownloadService, CalendarFileDownloadService>();
        builder.Services.AddSingleton<IFirestoreRepository, FirestoreRepository>();
        builder.Services.AddSingleton<IQueryService, QueryService>();
        builder.Services.AddSingleton<ISaverService, SaverService>();

        var messenger = WeakReferenceMessenger.Default;
        builder.Services.AddSingleton<IMessenger>(messenger);

        return builder;
    }
}