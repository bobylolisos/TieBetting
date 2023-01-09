namespace TieBetting.ViewModels;

public class TeamsViewModel : ViewModelNavigationBase
{
    public TeamsViewModel(INavigationService navigationService) 
        : base(navigationService)
    {
    }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    public int TotalBet => Teams.Sum(x => x.TotalBet);
    
    public int TotalWin => Teams.Sum(x => x.TotalWin);

    public int TotalProfit => Teams.Sum(x => x.Profit);

    public int BetsInSession => Teams.Sum(x => x.BetsInSession);

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is TeamsViewNavigationParameter teamsViewNavigationParameter)
        {
            foreach (var team in teamsViewNavigationParameter.Teams)
            {
                Teams.Add(new TeamViewModel(team));
            }
        }

        OnPropertyChanged(nameof(TotalBet));
        OnPropertyChanged(nameof(TotalWin));
        OnPropertyChanged(nameof(BetsInSession));
        OnPropertyChanged(nameof(TotalProfit));
        return base.OnNavigatingToAsync(navigationParameter);
    }
}