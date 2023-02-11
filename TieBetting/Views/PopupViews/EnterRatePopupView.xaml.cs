namespace TieBetting.Views.PopupViews;

public partial class EnterRatePopupView
{
    public EnterRatePopupView(IPopupService popupService, EnterRatePopupViewModel popupViewModel)
        : base(popupService)
    {
        InitializeComponent();

        BindingContext = popupViewModel;
    }
}