namespace TieBetting.Views;

public partial class TeamMaintenanceView
{
    public TeamMaintenanceView(INavigationService navigationService, TeamMaintenanceViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
    }
}