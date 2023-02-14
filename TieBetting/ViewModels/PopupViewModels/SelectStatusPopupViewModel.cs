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

    public MatchStatus MatchStatus => _matchViewModel?.Status ?? MatchStatus.NotActive;

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

    private async Task ExecuteSetStatusCommand(MatchStatus matchStatus)
    {
        if (matchStatus == MatchStatus.NotActive)
        {
            await _matchViewModel.SetRate(null);
        }

        await _matchViewModel.SetStatus(matchStatus);

        await _popupService.ClosePopupAsync();
    }
}