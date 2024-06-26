﻿namespace TieBetting.ViewModels.NavigationViewModels;

public class MatchBettingViewModel : ViewModelNavigationBase, IRecipient<MatchRateChangedMessage>
{
    private readonly IPopupService _popupService;
    private readonly IMessenger _messenger;
    private readonly IQueryService _queryService;
    private readonly IDialogService _dialogService;

    public MatchBettingViewModel(INavigationService navigationService, IPopupService popupService, IMessenger messenger, IQueryService queryService, IDialogService dialogService) 
        : base(navigationService)
    {
        _popupService = popupService;
        _messenger = messenger;
        _queryService = queryService;
        _dialogService = dialogService;

        EnterRateCommand = new AsyncRelayCommand(ExecuteEnterRateCommand, CanExecuteEnterRateCommand);
        SetStatusCommand = new AsyncRelayCommand<MatchStatus>(ExecuteSetStatusCommand);
        TabBarItem1Command = new AsyncRelayCommand(ExecuteShowSelectStatusPopupView, CanExecuteShowSelectStatusPopupView);
    }

    public MatchViewModel Match { get; set; }

    public AsyncRelayCommand EnterRateCommand { get; }

    public AsyncRelayCommand<MatchStatus> SetStatusCommand { get; }

    public bool IsTabBarVisible => Match != null ? Match.IsAnyActiveOrDone() || Match.MatchStatus == MatchStatus.Dormant : false;

    public bool HasPreviousActiveMatch => ResolveHasPreviousActiveMatch();

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchBettingViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
            OnPropertyChanged(nameof(IsTabBarVisible));
            OnPropertyChanged(nameof(HasPreviousActiveMatch));
            NotifyTabItemsCanExecuteChanged();
        }

        _messenger.RegisterAll(this);

        return base.OnNavigatingToAsync(navigationParameter);
    }

    public override Task OnNavigatedFromAsync(bool isForwardNavigation)
    {
        if (!isForwardNavigation)
        {
            _messenger.UnregisterAll(this);
        }
        return Task.CompletedTask;
    }

    public async void Receive(MatchRateChangedMessage message)
    {
        if (!Equals(message?.Rate, Match.Rate))
        {
            await Match.SetRate(message?.Rate);
            SetStatusCommand.NotifyCanExecuteChanged();
        }
    }

    private async Task ExecuteEnterRateCommand()
    {
        await _popupService.OpenPopupAsync<EnterRatePopupView>(new EnterRatePopupParameter(Match.Rate));
    }

    private bool CanExecuteEnterRateCommand()
    {
        if (Match.IsAnyActiveOrDone())
        {
            return false;
        }

        if (Match.MatchStatus == MatchStatus.Dormant || (Match.HomeTeam.IsDormant && Match.AwayTeam.IsDormant))
        {
            return false;
        }

        // Make sure we won't set rate on match before an active match reported status
        if (Match.HomeTeam.Matches.Any(x => x.IsActive(Match.HomeTeamName)))
        {
            return false;
        }

        if (Match.AwayTeam.Matches.Any(x => x.IsActive(Match.AwayTeamName)))
        {
            return false;
        }

        return true;
    }

    private async Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus == MatchStatus.Active)
        {
            var settings = await _queryService.GetSettingsAsync();
            var lostMatchesLimit = settings.WarnToBetWhenLostMatchesExceeds;
            if (Match.HomeTeam.LostMatchesInSession >= lostMatchesLimit ||
                Match.AwayTeam.LostMatchesInSession >= lostMatchesLimit)
            {
                var result = await _dialogService.ShowQuestion("Lost matches limit",
                    $"Lost limit is set to '{lostMatchesLimit}' matches.{Environment.NewLine}Do you wanna exceed that limit?");
                if (!result)
                {
                    return;
                }
            }

            var rateLimit = settings.WarnToBetWhenRateExceeds / 10d;
            if (Match.Rate > rateLimit)
            {
                var result = await _dialogService.ShowQuestion("Rate limit",
                    $"Rate limit is set to '{rateLimit}'.{Environment.NewLine}Do you wanna exceed that limit?");
                if (!result)
                {
                    return;
                }
            }
        }

        await Match.SetStatusAsync(matchStatus);

        await ExecuteNavigateBackCommand();

        OnPropertyChanged(nameof(IsTabBarVisible));
    }

    private async Task ExecuteShowSelectStatusPopupView()
    {
        await _popupService.OpenPopupAsync<SelectStatusPopupView>(new SelectStatusPopupParameter(Match));

        OnPropertyChanged(nameof(Match));
        NotifyTabItemsCanExecuteChanged();
    }

    private bool CanExecuteShowSelectStatusPopupView()
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

        return Match.IsAnyActiveOrDone() || Match.MatchStatus == MatchStatus.Dormant;
    }

    private bool ResolveHasPreviousActiveMatch()
    {
        if (Match == null)
        {
            return false;
        }

        var hasHomeTeamActiveMatch = Match.HomeTeam.Matches.Any(x => x.Day < Match.Day && x.IsActive(Match.HomeTeamName));
        if (hasHomeTeamActiveMatch)
        {
            return true;
        }

        var hasAwayTeamActiveMatch = Match.AwayTeam.Matches.Any(x => x.Day < Match.Day && x.IsActive(Match.AwayTeamName));
        return hasAwayTeamActiveMatch;
    }

}