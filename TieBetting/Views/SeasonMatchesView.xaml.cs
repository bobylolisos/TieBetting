namespace TieBetting.Views;

public partial class SeasonMatchesView
{
    private readonly SeasonMatchesViewModel _viewModel;

    public SeasonMatchesView(INavigationService navigationService, SeasonMatchesViewModel viewModel) 
        : base(navigationService)
    {
        _viewModel = viewModel;
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        MatchesCollectionView.ScrollToToday(_viewModel.Matches, 2);
    }
}