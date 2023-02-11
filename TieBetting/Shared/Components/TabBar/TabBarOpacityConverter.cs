namespace TieBetting.Shared.Components.TabBar;

public class TabBarOpacityConverter : ValueConverterBase<AsyncRelayCommand>
{
    protected override object Convert(AsyncRelayCommand value, object parameter)
    {
        if (value != null && value.CanExecute(null) == false)
        {
            return 0.3;
        }

        return 1.0;
    }
}