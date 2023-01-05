﻿namespace TieBetting.ViewModels;

public class MatchDetailsViewModel : ViewModelNavigationBase, IPubSub<MatchRateChangedMessage>
{
    private readonly IPopupService _popupService;

    public MatchDetailsViewModel(INavigationService navigationService, IPopupService popupService) 
        : base(navigationService)
    {
        _popupService = popupService;

        EnterRateCommand = new AsyncRelayCommand(ExecuteEnterRateCommand);
        SetStatusCommand = new AsyncRelayCommand<MatchStatus>(ExecuteSetStatusCommand);
    }

    public override Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        if (navigationParameter is MatchDetailsViewNavigationParameter parameter)
        {
            Match = parameter.MatchViewModel;
            OnPropertyChanged(nameof(Match));
        }

        return base.OnNavigatingToAsync(navigationParameter);
    }

    public MatchViewModel Match { get; set; }

    public AsyncRelayCommand EnterRateCommand { get; set; }

    public AsyncRelayCommand<MatchStatus> SetStatusCommand { get; set; }

    public void Receive(MatchRateChangedMessage message)
    {
        if (message?.Rate != Match.Rate && Match.SetRate(message?.Rate))
        {
            // Todo: Save to firebase
        }
    }

    public void RegisterMessages()
    {
        //StrongReferenceMessenger.Default.Register("rrr", new MessageHandler<object, MatchRateChangedMessage>((a, b) => Receive(b)));
        WeakReferenceMessenger.Default.RegisterAll(this);
        //_messengerService.Register<MatchRateChangedMessage>(Receive);
    }

    public void UnregisterMessages()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        //_messengerService.UnregisterAll(this);
        //_messengerService.Unregister<MatchRateChangedMessage>(Receive);
    }

    private async Task ExecuteEnterRateCommand()
    {
        await _popupService.OpenPopupAsync<EnterRateView>(new EnterRatePopupParameter(Match.Rate));
    }

    private Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus != Match.Status)
        {
            Match.SetStatus(matchStatus);

            // Todo: Save to firebase
        }

        return Task.CompletedTask;
    }
}