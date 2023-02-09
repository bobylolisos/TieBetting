namespace TieBetting.Views;

public partial class MatchMaintenanceView
{
    public MatchMaintenanceView(INavigationService navigationService, MatchMaintenanceViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
	}
}