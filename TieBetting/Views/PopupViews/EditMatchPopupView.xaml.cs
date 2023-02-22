namespace TieBetting.Views.PopupViews;

public partial class EditMatchPopupView
{
	public EditMatchPopupView(IPopupService popupService, EditMatchPopupViewModel viewModel)
        : base(popupService)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}