using System.ComponentModel;

namespace TieBetting.Shared.Components.BorderedEntry;

public partial class BorderedEntry : ContentView, INotifyPropertyChanged
{
    public BorderedEntry()
    {
        InitializeComponent();
    }

    public static BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(BorderedEntry), string.Empty, BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(BorderedEntry), string.Empty, BindingMode.OneTime);

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static BindableProperty LabelProperty = BindableProperty.Create(
        nameof(Label), typeof(string), typeof(BorderedEntry), string.Empty, BindingMode.OneTime);
    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static BindableProperty MaxCharactersProperty = BindableProperty.Create(
        nameof(MaxCharacters), typeof(int), typeof(BorderedEntry), int.MaxValue, BindingMode.OneTime);
    public int MaxCharacters
    {
        get => (int)GetValue(MaxCharactersProperty);
        set => SetValue(MaxCharactersProperty, value);
    }

    public static BindableProperty IsRequiredProperty = BindableProperty.Create(
        nameof(IsRequired), typeof(bool), typeof(BorderedEntry), false, BindingMode.OneTime);
    public bool IsRequired
    {
        get => (bool)GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }
}