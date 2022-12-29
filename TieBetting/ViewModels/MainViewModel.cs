namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IRepository _repository;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IRepository repository)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _repository = repository;
        MyCommand = new AsyncRelayCommand(ExecuteMyCommand);
        ImportCalendarToDatabaseCommand = new AsyncRelayCommand(ExecuteImportCalendarToDatabaseCommand);
    }

    public List<MatchViewModel> UpcomingMatches { get; set; } = new();

    public AsyncRelayCommand ImportCalendarToDatabaseCommand { get; set; }

    public AsyncRelayCommand MyCommand { get; set; }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        UpcomingMatches.Clear();

        //var matches = await _repository.GetNextMatchesAsync(20);

        var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        var matches = await _calendarFileDownloadService.DownloadAsync(href);


        var collection = matches.Take(12);

        var teams = await _repository.GetTeamsAsync();
        foreach (var match in collection)
        {
            var homeTeam = teams.SingleOrDefault(x => x.Name == match.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _repository.CreateTeamAsync(match.HomeTeam);
            }

            var awayTeam = teams.SingleOrDefault(x => x.Name == match.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _repository.CreateTeamAsync(match.AwayTeam);
            }

            var matchViewModel = new MatchViewModel(match, homeTeam, awayTeam);

            if (homeTeam.Name == "Björklöven")
            {
                matchViewModel.SetRate(4.80);
                matchViewModel.SetStatus(MatchStatus.Active);
            }

            UpcomingMatches.Add(matchViewModel);
        }
    }

    private async Task ExecuteImportCalendarToDatabaseCommand()
    {
        var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        var matches = await _calendarFileDownloadService.DownloadAsync(href);

        //await _repository.AddMatchesAsync(matches);
    }

    private async Task ExecuteMyCommand()
    {
        //var team = new Team
        //{
        //    Name = "Aik",
        //    TotalBet = 800,
        //    TotalWin = 850,
        //    PreviousBet = 40,
        //    Statuses = new List<bool> { true, false, true }
        //};

        //await _repository.AddTeamAsync(team);

        //await _repository.CreateTeamAsync("Modo");

        var teams = await _repository.GetTeamsAsync();
        await Task.Delay(1);
    }
}