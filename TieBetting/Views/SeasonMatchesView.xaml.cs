namespace TieBetting.Views;

public partial class SeasonMatchesView
{
    public SeasonMatchesView(INavigationService navigationService, SeasonMatchesViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
	}
}