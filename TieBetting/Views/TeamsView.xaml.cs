namespace TieBetting.Views;

public partial class TeamsView
{
	public TeamsView(TeamsViewModel viewModel, INavigationService navigationService) 
        : base(navigationService)
    {
		InitializeComponent();

		BindingContext = viewModel;
	}
}