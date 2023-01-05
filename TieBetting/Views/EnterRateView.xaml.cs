namespace TieBetting.Views;

public partial class EnterRateView
{
    public EnterRateView(IPopupService popupService, EnterRateViewModel viewModel)
        : base(popupService)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}