namespace TieBetting.Services;

public interface INavigationService
{
    public Task<bool> NavigateToPageAsync<T>(NavigationParameterBase parameter = null) where T : Page;

    public Task<bool> NavigateBackAsync();
}