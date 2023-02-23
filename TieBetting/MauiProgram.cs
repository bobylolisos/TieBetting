using CommunityToolkit.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Logging;

namespace TieBetting;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseDevExpress()
            .SetupFonts()
            .SetupServices()
            .SetupViews();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
