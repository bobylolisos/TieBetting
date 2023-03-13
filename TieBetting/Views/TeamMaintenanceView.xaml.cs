namespace TieBetting.Views;

public partial class TeamMaintenanceView
{
    private readonly TeamMaintenanceViewModel _viewModel;
    private bool _firstNavigation = true;

    public TeamMaintenanceView(INavigationService navigationService, TeamMaintenanceViewModel viewModel) 
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
            MatchesCollectionView.ScrollToToday(_viewModel.Matches, 5);

            _firstNavigation = false;
        }
    }
}