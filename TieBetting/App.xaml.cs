using TieBetting.Services.Navigation;

namespace TieBetting;

public partial class App : Application
{
    INavigationService _navigationService;

    public App(INavigationService navigationService)
	{
		InitializeComponent();
        _navigationService= navigationService;

		MainPage = new AppShell();
        //navigationService.NavigateToPageAsync<MainView>();

		
	}

    protected override async void OnStart()
    {
        base.OnStart();

        await _navigationService.NavigateToPageAsync<MainView>();
    }
}
