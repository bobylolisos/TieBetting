namespace TieBetting.ViewModels.Base;

public abstract class ViewModelNavigationBase : ObservableObject
{
    protected readonly INavigationService NavigationService;
    private string _tabBarItem1Label;
    private string _tabBarItem2Label;
    private string _tabBarItem3Label;
    private string _tabBarItem4Label;
    private string _tabBarItem1Image;
    private string _tabBarItem2Image;
    private string _tabBarItem3Image;
    private string _tabBarItem4Image;

    protected ViewModelNavigationBase(INavigationService navigationService)
    {
        NavigationService = navigationService;

        NavigateBackCommand = new AsyncRelayCommand(ExecuteNavigateBackCommand);
    }

    public AsyncRelayCommand NavigateBackCommand { get; set; }

    public AsyncRelayCommand TabBarItem1Command { get; protected set; }

    public AsyncRelayCommand TabBarItem2Command { get; protected set; }

    public AsyncRelayCommand TabBarItem3Command { get; protected set; }

    public AsyncRelayCommand TabBarItem4Command { get; protected set; }

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

    public virtual Task OnNavigatedBackAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual async Task ExecuteNavigateBackCommand()
    {
        await NavigationService.NavigateBackAsync();
    }

    public bool IsTabBarItem1CommandEnabled => TabBarItem1Command?.CanExecute(null) ?? true;

    public bool IsTabBarItem2CommandEnabled => TabBarItem2Command?.CanExecute(null) ?? true;

    public bool IsTabBarItem3CommandEnabled => TabBarItem3Command?.CanExecute(null) ?? true;

    public bool IsTabBarItem4CommandEnabled => TabBarItem4Command?.CanExecute(null) ?? true;

    public string TabBarItem1Label
    {
        get => _tabBarItem1Label;
        set => SetProperty(ref _tabBarItem1Label, value);
    }

    public string TabBarItem2Label
    {
        get => _tabBarItem2Label;
        set => SetProperty(ref _tabBarItem2Label, value);
    }

    public string TabBarItem3Label
    {
        get => _tabBarItem3Label;
        set => SetProperty(ref _tabBarItem3Label, value);
    }

    public string TabBarItem4Label
    {
        get => _tabBarItem4Label;
        set => SetProperty(ref _tabBarItem4Label, value);
    }

    public string TabBarItem1Image
    {
        get => _tabBarItem1Image;
        set => SetProperty(ref _tabBarItem1Image, value);
    }

    public string TabBarItem2Image
    {
        get => _tabBarItem2Image;
        set => SetProperty(ref _tabBarItem2Image, value);
    }

    public string TabBarItem3Image
    {
        get => _tabBarItem3Image;
        set => SetProperty(ref _tabBarItem3Image, value);
    }

    public string TabBarItem4Image
    {
        get => _tabBarItem4Image;
        set => SetProperty(ref _tabBarItem4Image, value);
    }

    protected void NotifyTabItemsCanExecuteChanged()
    {
        Task.Run(async () =>
        {
            await Task.Delay(100);
            OnPropertyChanged(nameof(TabBarItem1Command));
            OnPropertyChanged(nameof(IsTabBarItem1CommandEnabled));
            OnPropertyChanged(nameof(TabBarItem2Command));
            OnPropertyChanged(nameof(IsTabBarItem2CommandEnabled));
            OnPropertyChanged(nameof(TabBarItem3Command));
            OnPropertyChanged(nameof(IsTabBarItem3CommandEnabled));
            OnPropertyChanged(nameof(TabBarItem4Command));
            OnPropertyChanged(nameof(IsTabBarItem4CommandEnabled));
        });
    }
}