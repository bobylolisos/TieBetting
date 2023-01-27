namespace TieBetting.ViewModels.Base;

public abstract class ViewModelNavigationBase : ObservableObject
{
    private readonly INavigationService _navigationService;

    protected ViewModelNavigationBase(INavigationService navigationService)
    {
        _navigationService = navigationService;

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

    protected async Task ExecuteNavigateBackCommand()
    {
        await _navigationService.NavigateBackAsync();
    }
}