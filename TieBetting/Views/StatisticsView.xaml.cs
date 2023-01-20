namespace TieBetting.Views;

public partial class StatisticsView : ViewBase
{
    public StatisticsView(INavigationService navigationService, StatisticsViewModel viewModel) 
        : base(navigationService)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}