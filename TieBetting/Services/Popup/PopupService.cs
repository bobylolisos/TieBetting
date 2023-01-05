namespace TieBetting.Services.Popup;

public class PopupService : IPopupService
{
    private readonly IServiceProvider _services;

    public PopupService(IServiceProvider services)
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


    private IPopupViewModel GetPopupViewModel(Page p)
        => p?.BindingContext as IPopupViewModel;

    private async Task<T> ResolvePage<T>() where T : Page
    {
        try
        {
            return _services.GetService<T>();
        }
        catch (Exception e)
        {
            var message = e.InnerException?.Message ?? e.Message;
            await Application.Current.MainPage.DisplayAlert("PopupService Error", message, "Ok");

            Console.WriteLine(e);
            throw;
        }
    }

    public async Task OpenPopupAsync<T>(PopupParameterBase parameter = null) where T : BasePopupPage
    {
        var toPage = await ResolvePage<T>();

        if (toPage is not null)
        {
            var popupViewModel = GetPopupViewModel(toPage);
            if (popupViewModel is not null)
            {
                await popupViewModel.OnOpenPopupAsync(parameter);
            }
            else
            {
                // Something is wrong
                if (Debugger.IsAttached)
                    Debugger.Break();
                throw new Exception();
            }

            await Navigation.PushModalAsync(toPage, true);
        }
        else
            throw new InvalidOperationException($"Unable to resolve type {typeof(T).FullName}");
    }

    public async Task ClosePopupAsync()
    {
        var currentPage = Navigation.ModalStack.LastOrDefault();
        if (currentPage is not null)
        {
            var popupViewModel = GetPopupViewModel(currentPage);
            if (popupViewModel is not null)
            {
                await popupViewModel.OnClosePopupAsync();
            }
            else
            {
                // Something is wrong
                if (Debugger.IsAttached)
                    Debugger.Break();
                throw new Exception();
            }
        }

        // Todo: Change animated to TRUE. 2022-12-29 the value TRUE throws NullReferenceException
        await Navigation.PopModalAsync(false);
    }
}