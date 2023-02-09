namespace TieBetting.Services.Navigation;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    private INavigation Navigation
    {
        get
        {
            var navigation = Shell.Current.Navigation;
            if (navigation is not null)
                return navigation;

            // Something is wrong
            if (Debugger.IsAttached)
                Debugger.Break();
            throw new Exception();
        }
    }

    public async Task<bool> NavigateToPageAsync<T>(NavigationParameterBase parameter = null) where T : Page
    {
        var currentPage = Navigation.NavigationStack.LastOrDefault();

        if (currentPage is not null && currentPage.GetType() == typeof(T))
        {
            // Navigating to same page, double click ???
            return false;
        }

        if (currentPage is not null)
        {
            var currentViewModel = GetPageViewModelBase(currentPage);
            if (currentViewModel is not null)
            {
                var result = await currentViewModel.CanNavigateFromAsync();
                if (result == false)
                {
                    return false;
                }
            }
        }
        var toPage = await ResolvePage<T>();

        if (toPage is not null)
        {
            toPage.NavigatedTo += Page_NavigatedTo;

            var toViewModel = GetPageViewModelBase(toPage);
            if (toViewModel is not null)
            {
                await toViewModel.OnNavigatingToAsync(parameter);

                if (toViewModel is IPubSub pubSub)
                {
                    pubSub.RegisterMessages();
                }
            }

            await Navigation.PushAsync(toPage, true);

            toPage.NavigatedFrom += Page_NavigatedFrom;
        }
        else
            throw new InvalidOperationException($"Unable to resolve type {typeof(T).FullName}");

        return true;
    }

    public async Task<bool> NavigateBackAsync()
    {
        var currentPage = Navigation.NavigationStack.LastOrDefault();
        if (currentPage is not null)
        {
            var currentViewModel = GetPageViewModelBase(currentPage);
            if (currentViewModel is not null)
            {
                var result = await currentViewModel.CanNavigateFromAsync();
                if (result == false)
                {
                    return false;
                }
            }
        }

        // Todo: Change animated to TRUE. 2022-12-29 the value TRUE throws NullReferenceException
        await Navigation.PopAsync(false);
        return true;
    }

    private async void Page_NavigatedFrom(object sender, NavigatedFromEventArgs e)
    {
        if (Navigation.ModalStack.Any())
        {
            // Modal dialog open, do not navigate from current view
            return;
        }

        //To determine forward navigation, we look at the 2nd to last item on the NavigationStack
        //If that entry equals the sender, it means we navigated forward from the sender to another page
        bool isForwardNavigation = Navigation.NavigationStack.Count > 1
                                   && Navigation.NavigationStack[^2] == sender;
        if (sender is Page thisPage)
        {
            if (!isForwardNavigation)
            {
                thisPage.NavigatedTo -= Page_NavigatedTo;
                thisPage.NavigatedFrom -= Page_NavigatedFrom;
            }
            await CallNavigatedFrom(thisPage, isForwardNavigation);
        }
    }
    private Task CallNavigatedFrom(Page p, bool isForward)
    {
        var fromViewModel = GetPageViewModelBase(p);
        if (fromViewModel is not null)
        {
            if (fromViewModel is IPubSub pubSub)
            {
                pubSub.UnregisterMessages();
            }

            return fromViewModel.OnNavigatedFromAsync(isForward);
        }

        return Task.CompletedTask;
    }

    private async void Page_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        await CallNavigatedTo(sender as Page);
    }

    private Task CallNavigatedTo(Page p)
    {
        var fromViewModel = GetPageViewModelBase(p);
        if (fromViewModel is not null)
        {
            return fromViewModel.OnNavigatedToAsync();
        }

        return Task.CompletedTask;
    }

    private ViewModelNavigationBase GetPageViewModelBase(Page p)
        => p?.BindingContext as ViewModelNavigationBase;

    private async Task<T> ResolvePage<T>() where T : Page
    {
        try
        {
            await Task.Delay(1);
            return _services.GetService<T>();
        }
        catch (Exception e)
        {
            var message = e.InnerException?.Message ?? e.Message;

            Console.WriteLine(e);
            throw;
        }
    }
}