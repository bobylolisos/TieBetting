namespace TieBetting.Views;

public partial class MatchBettingView : ViewBase
{
    public MatchBettingView(MatchBettingViewModel viewModel, INavigationService navigationService)
    : base(navigationService)
	{
        InitializeComponent();

		BindingContext = viewModel;
	}
}