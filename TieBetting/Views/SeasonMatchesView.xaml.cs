namespace TieBetting.Views;

public partial class SeasonMatchesView
{
    private readonly SeasonMatchesViewModel _viewModel;
    private bool _firstNavigation = true;

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

        if (_firstNavigation)
        {
            MatchesCollectionView.ScrollToToday(_viewModel.Matches, 2);

            _firstNavigation = false;
        }
    }
}