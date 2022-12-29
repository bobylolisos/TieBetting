namespace TieBetting.Views;

public partial class MatchDetailsView : ContentPage
{
	public MatchDetailsView(MatchDetailsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}