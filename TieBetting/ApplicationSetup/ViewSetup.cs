namespace TieBetting.ApplicationSetup;

public static class ViewSetup
{
    public static MauiAppBuilder SetupViews(this MauiAppBuilder builder)
    {
        // Pages
        builder.RegisterNavigationViewTransient<MainView>();
        builder.RegisterNavigationViewTransient<MatchDetailsView>();
        builder.RegisterNavigationViewTransient<TeamsView>();
        builder.RegisterNavigationViewTransient<StatisticsView>();
        builder.RegisterNavigationViewTransient<SettingsView>();
        builder.RegisterNavigationViewTransient<AllMatchesView>();
        builder.RegisterNavigationViewTransient<TeamMatchesView>();
        builder.RegisterNavigationViewTransient<MatchMaintenanceView>();

        // Popups
        builder.RegisterPopupViewTransient<EnterRatePopupView>();

        return builder;
    }

    private static void RegisterNavigationViewTransient<T>(this MauiAppBuilder builder)
        where T : ContentPage
    {
        builder.RegisterViewTransient<T>(true);
    }

    private static void RegisterPopupViewTransient<T>(this MauiAppBuilder builder)
        where T : ContentPage
    {
        builder.RegisterViewTransient<T>(false);
    }

    private static void RegisterViewTransient<T>(this MauiAppBuilder builder, bool navigationViewModel) where T : ContentPage
    {
        var viewType = typeof(T);
        var viewName = viewType.Name;
        var viewModelName = $"{viewName}Model";
        var viewModelPath = navigationViewModel ? "TieBetting.ViewModels.NavigationViewModels" : "TieBetting.ViewModels.PopupViewModels";
        var fullViewModelName = $"{viewModelPath}.{viewModelName}";

        var viewModelServiceType = Type.GetType(fullViewModelName);
        if (viewModelServiceType == null)
        {
            throw new Exception($"Unable to register viewmodel: {fullViewModelName}");
        }
        builder.Services.AddTransient(viewModelServiceType);

        builder.Services.AddTransient<T>();

        Routing.RegisterRoute(viewModelName, viewType);
    }
}