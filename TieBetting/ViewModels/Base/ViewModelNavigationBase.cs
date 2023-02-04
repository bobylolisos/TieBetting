namespace TieBetting.ViewModels.Base;

public abstract class ViewModelNavigationBase : ObservableObject
{
    protected readonly INavigationService NavigationService;

    protected ViewModelNavigationBase(INavigationService navigationService)
    {
        NavigationService = navigationService;

        NavigateBackCommand = new AsyncRelayCommand(ExecuteNavigateBackCommand);
    }

    public AsyncRelayCommand NavigateBackCommand { get; set; }

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

    protected virtual async Task ExecuteNavigateBackCommand()
    {
        await NavigationService.NavigateBackAsync();
    }
}