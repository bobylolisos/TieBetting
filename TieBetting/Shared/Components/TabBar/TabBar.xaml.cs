namespace TieBetting.Shared.Components.TabBar;

public partial class TabBar : ContentView
{
	public TabBar()
	{
		InitializeComponent();
	}

    public static BindableProperty TabBarItem1ImageSourceProperty = BindableProperty.Create(
        nameof(TabBarItem1ImageSource), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem1ImageSource
    {
        get => (string)GetValue(TabBarItem1ImageSourceProperty);
        set => SetValue(TabBarItem1ImageSourceProperty, value);
    }

    public static BindableProperty TabBarItem1LabelProperty = BindableProperty.Create(
        nameof(TabBarItem1Label), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem1Label
    {
        get => (string)GetValue(TabBarItem1LabelProperty);
        set => SetValue(TabBarItem1LabelProperty, value);
    }

    public static BindableProperty TabBarItem1CommandProperty = BindableProperty.Create(
        nameof(TabBarItem1Command), typeof(IAsyncRelayCommand), typeof(TabBar), null, BindingMode.OneWay);

    public IAsyncRelayCommand TabBarItem1Command
    {
        get => (IAsyncRelayCommand)GetValue(TabBarItem1CommandProperty);
        set => SetValue(TabBarItem1CommandProperty, value);
    }

    public static BindableProperty TabBarItem2ImageSourceProperty = BindableProperty.Create(
        nameof(TabBarItem2ImageSource), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem2ImageSource
    {
        get => (string)GetValue(TabBarItem2ImageSourceProperty);
        set => SetValue(TabBarItem2ImageSourceProperty, value);
    }

    public static BindableProperty TabBarItem2LabelProperty = BindableProperty.Create(
        nameof(TabBarItem2Label), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem2Label
    {
        get => (string)GetValue(TabBarItem2LabelProperty);
        set => SetValue(TabBarItem2LabelProperty, value);
    }

    public static BindableProperty TabBarItem2CommandProperty = BindableProperty.Create(
        nameof(TabBarItem2Command), typeof(IAsyncRelayCommand), typeof(TabBar), null, BindingMode.OneWay);

    public IAsyncRelayCommand TabBarItem2Command
    {
        get => (IAsyncRelayCommand)GetValue(TabBarItem2CommandProperty);
        set => SetValue(TabBarItem2CommandProperty, value);
    }

    public static BindableProperty TabBarItem3ImageSourceProperty = BindableProperty.Create(
        nameof(TabBarItem3ImageSource), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem3ImageSource
    {
        get => (string)GetValue(TabBarItem3ImageSourceProperty);
        set => SetValue(TabBarItem3ImageSourceProperty, value);
    }

    public static BindableProperty TabBarItem3LabelProperty = BindableProperty.Create(
        nameof(TabBarItem3Label), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem3Label
    {
        get => (string)GetValue(TabBarItem3LabelProperty);
        set => SetValue(TabBarItem3LabelProperty, value);
    }

    public static BindableProperty TabBarItem3CommandProperty = BindableProperty.Create(
        nameof(TabBarItem3Command), typeof(IAsyncRelayCommand), typeof(TabBar), null, BindingMode.OneWay);

    public IAsyncRelayCommand TabBarItem3Command
    {
        get => (IAsyncRelayCommand)GetValue(TabBarItem3CommandProperty);
        set => SetValue(TabBarItem3CommandProperty, value);
    }

    public static BindableProperty TabBarItem4ImageSourceProperty = BindableProperty.Create(
        nameof(TabBarItem4ImageSource), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem4ImageSource
    {
        get => (string)GetValue(TabBarItem4ImageSourceProperty);
        set => SetValue(TabBarItem4ImageSourceProperty, value);
    }

    public static BindableProperty TabBarItem4LabelProperty = BindableProperty.Create(
        nameof(TabBarItem4Label), typeof(string), typeof(TabBar), string.Empty, BindingMode.OneWay);

    public string TabBarItem4Label
    {
        get => (string)GetValue(TabBarItem4LabelProperty);
        set => SetValue(TabBarItem4LabelProperty, value);
    }

    public static BindableProperty TabBarItem4CommandProperty = BindableProperty.Create(
        nameof(TabBarItem4Command), typeof(IAsyncRelayCommand), typeof(TabBar), null, BindingMode.OneWay);

    public IAsyncRelayCommand TabBarItem4Command
    {
        get => (IAsyncRelayCommand)GetValue(TabBarItem4CommandProperty);
        set => SetValue(TabBarItem4CommandProperty, value);
    }
}