namespace TieBetting.ApplicationSetup;

public static class ServiceSetup
{
    public static MauiAppBuilder SetupServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        return builder;
    }
}