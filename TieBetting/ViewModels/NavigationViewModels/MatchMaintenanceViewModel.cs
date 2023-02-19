﻿using TieBetting.Shared.Extensions;

namespace TieBetting.ViewModels.NavigationViewModels;

public class MatchMaintenanceViewModel : ViewModelNavigationBase
{
    private readonly IPopupService _popupService;

    public MatchMaintenanceViewModel(INavigationService navigationService, IPopupService popupService)
        : base(navigationService)
    {
        _popupService = popupService;
        TabBarItem1Command = new AsyncRelayCommand(ExecuteChangeStatusCommand, CanExecuteChangeStatusCommand);
        TabBarItem2Command = new AsyncRelayCommand(ExecuteChangeDateCommand, CanExecuteChangeDateCommand);
        TabBarItem3Command = new AsyncRelayCommand(ExecuteDeleteMatchCommand, CanExecuteDeleteMatchCommand);
    }

    public MatchViewModel Match { get; set; }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchMaintenanceViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
            NotifyTabItemsCanExecuteChanged();
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

    private async Task ExecuteChangeStatusCommand()
    {
        await _popupService.OpenPopupAsync<SelectStatusPopupView>(new SelectStatusPopupParameter(Match));

        OnPropertyChanged(nameof(Match));
        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteChangeStatusCommand()
    {
        if (Match == null)
        {
            return false;
        }

        var homeTeamHasLaterActiveMatches = Match.HomeTeam.Matches.Any(x => x.Day > Match.Day && x.IsActiveOrDone(Match.HomeTeamName));
        if (homeTeamHasLaterActiveMatches)
        {
            return false;
        }

        var awayTeamHasLaterActiveMatches = Match.AwayTeam.Matches.Any(x => x.Day > Match.Day && x.IsActiveOrDone(Match.AwayTeamName));
        if (awayTeamHasLaterActiveMatches)
        {
            return false;
        }

        return Match.IsAnyActiveOrDone();
    }

    private async Task ExecuteChangeDateCommand()
    {
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");

        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteChangeDateCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.IsNotActive(TeamType.HomeTeam) && Match.IsNotActive(TeamType.AwayTeam);
    }

    private async Task ExecuteDeleteMatchCommand()
    {
        await Application.Current.MainPage.DisplayAlert("Not implemented", "Not implemented!", "Ok");

        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteDeleteMatchCommand()
    {
        if (Match == null)
        {
            return false;
        }

        return Match.IsNotActive(TeamType.HomeTeam) && Match.IsNotActive(TeamType.AwayTeam);
    }
}