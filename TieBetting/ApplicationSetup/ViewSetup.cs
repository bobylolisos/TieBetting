namespace TieBetting.ApplicationSetup;

public static class ViewSetup
{
    public static MauiAppBuilder SetupViews(this MauiAppBuilder builder)
    {
        // Pages
        builder.RegisterViewTransient<MainView>();
        builder.RegisterViewTransient<MatchDetailsView>();
        builder.RegisterViewTransient<TeamsView>();

        // Popups
        builder.RegisterViewTransient<EnterRateView>();

        return builder;
    }

    private static void RegisterViewTransient<T>(this MauiAppBuilder builder) where T : ContentPage
    {
        var viewType = typeof(T);
        var viewName = viewType.Name;
        var viewModelName = $"{viewName}Model";
        var fullViewModelName = $"TieBetting.ViewModels.{viewModelName}";

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