namespace TieBetting.ViewModels;

public class TeamMatchesViewModel : ViewModelNavigationBase
{
    private string _headerText;
    private string _headerImage;

    public TeamMatchesViewModel(INavigationService navigationService) 
        : base(navigationService)
    {
    }

    public string HeaderImage
    {
        get => _headerImage;
        set => SetProperty(ref _headerImage, value);
    }

    public string HeaderText
    {
        get => _headerText;
        set => SetProperty(ref _headerText, value);
    }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamMatchesViewNavigationParameter parameter)
        {
            var team = parameter.TeamViewModel;
            HeaderImage = team.Image;
            HeaderText = team.Name;
        }

        return base.OnNavigatingToAsync(navigationParameter);

    }
}