using TieBetting.Services.Navigation;

namespace TieBetting;

public partial class App : Application
{
	public App(INavigationService navigationService)
	{
		InitializeComponent();

		MainPage = new AppShell();
        navigationService.NavigateToPageAsync<MainView>();
	}
}
