namespace TieBetting.Views.Base;

public abstract class ViewBase : ContentPage
{
    private readonly INavigationService _navigationService;

    protected ViewBase(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override bool OnBackButtonPressed()
    {
        _navigationService.NavigateBackAsync();

        return true;
    }
}