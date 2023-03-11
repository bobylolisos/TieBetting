namespace TieBetting.ViewModels.PopupViewModels;

public class EditMatchPopupViewModel : ViewModelBase, IPopupViewModel
{
    private readonly IQueryService _queryService;
    private readonly ISaverService _saverService;
    private readonly IPopupService _popupService;
    private string _selectedSeason;
    private TeamViewModel _selectedHomeTeam;
    private TeamViewModel _selectedAwayTeam;
    private string _homeTeamName;
    private string _homeTeamImage;
    private string _awayTeamImage;
    private string _awayTeamName;
    private DateTime _selectedDate;
    private MatchViewModel _matchViewModel;
    private string _errorMessage;

    public EditMatchPopupViewModel(IQueryService queryService, ISaverService saverService, IPopupService popupService)
    {
        _queryService = queryService;
        _saverService = saverService;
        _popupService = popupService;

        SaveChangesCommand = new AsyncRelayCommand(ExecuteSaveChangesCommand, CanExecuteSaveChangesCommand);
    }

    public AsyncRelayCommand SaveChangesCommand { get; }

    public ObservableCollection<TeamViewModel> Teams { get; } = new();

    public string SelectedSeason
    {
        get => _selectedSeason;
        set => SetProperty(ref _selectedSeason, value);
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetProperty(ref _selectedDate, value);
            SaveChangesCommand.NotifyCanExecuteChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public TeamViewModel SelectedHomeTeam
    {
        get => _selectedHomeTeam;
        set
        {
            if (SetProperty(ref _selectedHomeTeam, value))
            {
                HomeTeamName = _selectedHomeTeam?.Name ?? "Home";
                HomeTeamImage = SelectedHomeTeam?.Image ?? "shirt_black.svg";
                SaveChangesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public bool IsTeamSelectable => _matchViewModel == null;

    public string HomeTeamImage
    {
        get => _homeTeamImage;
        set => SetProperty(ref _homeTeamImage, value);
    }

    public string HomeTeamName
    {
        get => _homeTeamName;
        set => SetProperty(ref _homeTeamName, value);
    }

    public TeamViewModel SelectedAwayTeam
    {
        get => _selectedAwayTeam;
        set
        {
            if (SetProperty(ref _selectedAwayTeam, value))
            {
                AwayTeamName = _selectedAwayTeam?.Name ?? "Away";
                AwayTeamImage = _selectedAwayTeam?.Image ?? "shirt_black.svg";
                SaveChangesCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string AwayTeamImage
    {
        get => _awayTeamImage;
        set => SetProperty(ref _awayTeamImage, value);
    }

    public string AwayTeamName
    {
        get => _awayTeamName;
        set => SetProperty(ref _awayTeamName, value);
    }

    public async Task OnOpenPopupAsync(PopupParameterBase parameter = null)
    {
        Teams.Clear();

        if (parameter is AddMatchPopupParameter addMatchPopupParameter)
        {
            SelectedSeason = addMatchPopupParameter.Season;

            var teams = await _queryService.GetTeamsAsync();
            var seasonTeams = teams.Where(x => x.Matches.Any(y => y.Season == SelectedSeason));
            foreach (var team in seasonTeams)
            {
                Teams.Add(team);
            }

            // Strange, OnPropertyChanged(nameof(...)) doesnt work, i have to assign selected and then set to null for ui to update to default values
            SelectedHomeTeam = Teams.First();
            SelectedAwayTeam = Teams.First();
            SelectedHomeTeam = null;
            SelectedAwayTeam = null;
            SelectedDate = DateTime.Now;
        }

        if (parameter is EditMatchPopupParameter editMatchPopupParameter)
        {
            _matchViewModel = editMatchPopupParameter.Match;
            SelectedSeason = _matchViewModel.Season;
            SelectedHomeTeam = _matchViewModel.HomeTeam;
            SelectedAwayTeam = _matchViewModel.AwayTeam;
            SelectedDate = DateTime.Parse(_matchViewModel.Date);
        }

        OnPropertyChanged(nameof(IsTeamSelectable));
        SaveChangesCommand.NotifyCanExecuteChanged();
    }

    public Task<bool> OnClosePopupAsync()
    {
        return Task.FromResult(true);
    }

    private async Task ExecuteSaveChangesCommand()
    {
        if (_matchViewModel != null)
        {
            await _matchViewModel.SetDateAsync(SelectedDate);
        }
        else
        {
            await _saverService.CreateMatchAsync(SelectedSeason, _selectedHomeTeam, _selectedAwayTeam, SelectedDate);
        }

        await _popupService.ClosePopupAsync();
    }

    private bool CanExecuteSaveChangesCommand()
    {
        if (_selectedHomeTeam == null || _selectedAwayTeam == null)
        {
            ErrorMessage = null;
            return false;
        }

        if (_selectedHomeTeam == _selectedAwayTeam)
        {
            ErrorMessage = "Same teams!";
            return false;
        }

        if (_matchViewModel != null && DateTime.Parse(_matchViewModel.Date) == _selectedDate)
        {
            ErrorMessage = "Date is same as original!";
            return false;
        }

        var futureHomeTeamMatches = _selectedHomeTeam.Matches.Where(x => DateTime.Parse(x.Date) > _selectedDate);
        if (futureHomeTeamMatches.Any(x => x.IsAnyActiveOrDone()))
        {
            ErrorMessage = "Date is before ongoing matches!";
            return false;
        }

        var futureAwayTeamMatches = _selectedAwayTeam.Matches.Where(x => DateTime.Parse(x.Date) > _selectedDate);
        if (futureAwayTeamMatches.Any(x => x.IsAnyActiveOrDone()))
        {
            ErrorMessage = "Date is before ongoing matches!";
            return false;
        }

        if (_selectedHomeTeam.Matches.Any(x => DateTime.Parse(x.Date) == _selectedDate && x.Date != _matchViewModel?.Date))
        {
            ErrorMessage = "Match on date already exists!";
            return false;
        }

        if (_selectedAwayTeam.Matches.Any(x => DateTime.Parse(x.Date) == _selectedDate && x.Date != _matchViewModel?.Date))
        {
            ErrorMessage = "Match on date already exists!";
            return false;
        }

        ErrorMessage = null;
        return true;
    }
}