namespace TieBetting.ApplicationSetup;

public static class FontSetup
{
    public static MauiAppBuilder SetupFonts(this MauiAppBuilder builder)
    {
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

        return builder;
    }
}