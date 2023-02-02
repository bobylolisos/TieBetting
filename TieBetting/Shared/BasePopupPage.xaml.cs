namespace TieBetting.Shared;

public partial class BasePopupPage : ContentPage
{
    private readonly IPopupService _popupService;

    public BasePopupPage(IPopupService popupService)
    {
        _popupService = popupService;
        InitializeComponent();
    }

    public static readonly BindableProperty PopupContentProperty = BindableProperty.Create(
        propertyName: nameof(PopupContent),
        returnType: typeof(View),
        declaringType: typeof(BasePopupPage),
        defaultValue: null,
        defaultBindingMode: BindingMode.OneWay,
        propertyChanged: PopupContentPropertyChanged);

    private static void PopupContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var controls = (BasePopupPage)bindable;
        if (newValue != null)
            controls.ContentView.Content = (View)newValue;
    }

    public View PopupContent
    {
        get => (View)GetValue(PopupContentProperty);
        set => SetValue(PopupContentProperty, value);
    }

    protected override bool OnBackButtonPressed()
    {
        var _ = ExecutePopModelCommand();
        return true;
    }

    public AsyncRelayCommand PopModelCommand => new (ExecutePopModelCommand);

    private async Task ExecutePopModelCommand()
    {
        await _popupService.ClosePopupAsync();
    }
}