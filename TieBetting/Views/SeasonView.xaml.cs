namespace TieBetting.Views;

public partial class SeasonView : IRecipient<SelectedSeasonChangedMessage>
{
    private readonly SeasonViewModel _viewModel;
    private bool _firstNavigation = true;

    public SeasonView(INavigationService navigationService, IMessenger messenger, SeasonViewModel viewModel) 
        : base(navigationService)
    {
        _viewModel = viewModel;
        InitializeComponent();

        BindingContext = viewModel;

        messenger.RegisterAll(this);
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

    public void Receive(SelectedSeasonChangedMessage message)
    {
        if (_firstNavigation == false)
        {
            MatchesCollectionView.ScrollToToday(_viewModel.Matches, 2);
        }
    }
}