namespace TieBetting.Views.PopupViews;

public partial class SelectStatusPopupView
{
    public SelectStatusPopupView(IPopupService popupService, SelectStatusPopupViewModel viewModel)
        : base(popupService)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}