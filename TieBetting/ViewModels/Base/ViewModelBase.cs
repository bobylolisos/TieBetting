namespace TieBetting.ViewModels.Base;

public abstract class ViewModelBase : ObservableObject
{
    public virtual Task<bool> CanNavigateFromAsync()
    {
        return Task.FromResult(true);
    }

    public virtual Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnNavigatedFromAsync(bool isForwardNavigation)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnNavigatedToAsync()
    {
        return Task.CompletedTask;
    }
}