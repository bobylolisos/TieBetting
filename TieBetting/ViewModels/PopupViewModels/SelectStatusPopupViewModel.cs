namespace TieBetting.ViewModels.PopupViewModels;

public class SelectStatusPopupViewModel : ViewModelBase, IPopupViewModel
{
    private readonly IPopupService _popupService;
    private MatchViewModel _matchViewModel;

    public SelectStatusPopupViewModel(IPopupService popupService)
    {
        _popupService = popupService;
        SetStatusCommand = new AsyncRelayCommand<MatchStatus>(ExecuteSetStatusCommand);
    }

    public AsyncRelayCommand<MatchStatus> SetStatusCommand { get; set; }

    public MatchStatus MatchStatus => ResolveMatchStatus();

    public Task OnOpenPopupAsync(PopupParameterBase parameter = null)
    {
        if (parameter is SelectStatusPopupParameter selectStatusPopupParameter)
        {
            _matchViewModel = selectStatusPopupParameter.Match;
            OnPropertyChanged(nameof(MatchStatus));
        }

        return Task.CompletedTask;
    }

    public Task<bool> OnClosePopupAsync()
    {
        return Task.FromResult(true);

    }

    private MatchStatus ResolveMatchStatus()
    {
        if (_matchViewModel == null)
        {
            return MatchStatus.NotActive;
        }

        if (_matchViewModel.IsActiveOrDone(TeamType.HomeTeam) && _matchViewModel.IsAbandon(TeamType.HomeTeam) == false)
        {
            return _matchViewModel.HomeTeamMatchStatus;
        }

        if (_matchViewModel.IsActiveOrDone(TeamType.AwayTeam) && _matchViewModel.IsAbandon(TeamType.AwayTeam) == false)
        {
            return _matchViewModel.AwayTeamMatchStatus;
        }

        if (_matchViewModel.IsDormant(TeamType.HomeTeam) || _matchViewModel.IsDormant(TeamType.AwayTeam))
        {
            return MatchStatus.Dormant;
        }

        if (_matchViewModel.IsAbandon(TeamType.HomeTeam) || _matchViewModel.IsAbandon(TeamType.AwayTeam))
        {
            return MatchStatus.Abandon;
        }

        return MatchStatus.NotActive;
    }

    private async Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus == MatchStatus.NotActive)
        {
            await _matchViewModel.SetRate(null);
        }

        await _matchViewModel.SetStatusAsync(matchStatus);

        await _popupService.ClosePopupAsync();
    }
}