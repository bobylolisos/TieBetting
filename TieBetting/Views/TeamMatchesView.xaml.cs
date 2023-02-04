namespace TieBetting.Views;

public partial class TeamMatchesView
{
    public TeamMatchesView(INavigationService navigationService, TeamMatchesViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
    }
}