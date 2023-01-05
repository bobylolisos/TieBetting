namespace TieBetting.Views;

public partial class MatchDetailsView : ViewBase
{
    public MatchDetailsView(MatchDetailsViewModel viewModel, INavigationService navigationService)
    : base(navigationService)
	{
        InitializeComponent();

		BindingContext = viewModel;
	}
}