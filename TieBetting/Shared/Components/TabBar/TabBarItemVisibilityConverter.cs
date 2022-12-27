namespace TieBetting.Shared.Components.TabBar;

public class TabBarItemVisibilityConverter : ValueConverterBase<string>
{
    protected override object Convert(string value, object parameter)
    {
        return value.HasContent();
    }
}