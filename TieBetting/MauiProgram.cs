using CommunityToolkit.Maui;
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
            .SetupFonts()
            .SetupServices()
            .SetupViews();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
