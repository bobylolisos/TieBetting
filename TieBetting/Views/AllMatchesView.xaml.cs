namespace TieBetting.Views;

public partial class AllMatchesView
{
    public AllMatchesView(INavigationService navigationService, AllMatchesViewModel viewModel) 
        : base(navigationService)
    {
		InitializeComponent();

        BindingContext = viewModel;
	}
}