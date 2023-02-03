namespace TieBetting.Views;

public partial class SettingsView : ViewBase
{
    public SettingsView(INavigationService navigationService, SettingsViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
	}
}