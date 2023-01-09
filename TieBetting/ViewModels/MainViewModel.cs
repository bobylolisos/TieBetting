using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace TieBetting.ViewModels;

public class MainViewModel : ViewModelNavigationBase
{
    private readonly ICalendarFileDownloadService _calendarFileDownloadService;
    private readonly IRepository _repository;
    private readonly INavigationService _navigationService;
    private IReadOnlyCollection<Team> _teams;

    public MainViewModel(ICalendarFileDownloadService calendarFileDownloadService, IRepository repository, INavigationService navigationService)
    : base(navigationService)
    {
        _calendarFileDownloadService = calendarFileDownloadService;
        _repository = repository;
        _navigationService = navigationService;
        NavigateToMatchDetailsViewCommand = new AsyncRelayCommand<MatchViewModel>(ExecuteNavigateToMatchDetailsViewCommand);
        NavigateToTeamsViewCommand = new AsyncRelayCommand(ExecuteNavigateToTeamsViewCommand);
        MyCommand = new AsyncRelayCommand(ExecuteMyCommand);
        ImportCalendarToDatabaseCommand = new AsyncRelayCommand(ExecuteImportCalendarToDatabaseCommand);
    }

    public List<MatchGroupViewModel> Matches { get; set; } = new();

    public AsyncRelayCommand<MatchViewModel> NavigateToMatchDetailsViewCommand { get; set; }

    public AsyncRelayCommand NavigateToTeamsViewCommand { get; set; }

    public AsyncRelayCommand ImportCalendarToDatabaseCommand { get; set; }

    public AsyncRelayCommand MyCommand { get; set; }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        Matches.Clear();

        //var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        //var matches = await _calendarFileDownloadService.DownloadAsync(href);


        _teams = await _repository.GetTeamsAsync();

        var fetchedPreviousMatches = await _repository.GetPreviousOngoingMatchesAsync();

        var previousMatches = new List<MatchViewModel>();
        foreach (var previousMatch in fetchedPreviousMatches)
        {
            var homeTeam = _teams.SingleOrDefault(x => x.Name == previousMatch.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _repository.CreateTeamAsync(previousMatch.HomeTeam);
            }

            var awayTeam = _teams.SingleOrDefault(x => x.Name == previousMatch.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _repository.CreateTeamAsync(previousMatch.AwayTeam);
            }

            var matchViewModel = new MatchViewModel(_repository, previousMatch, homeTeam, awayTeam);

            previousMatches.Add(matchViewModel);
        }

        if (previousMatches.Any())
        {
            Matches.Add(new MatchGroupViewModel("Previous", previousMatches));
        }


        var fetchedUpcomingMatches = await _repository.GetNextMatchesAsync(20);

        var todayDay = (DateTime.Today - new DateTime(2022, 01, 01)).Days;
        Debug.WriteLine("");
        Debug.WriteLine($"Today day is: {todayDay}");
        Debug.WriteLine("");

        var todayMatches = new List<MatchViewModel>();
        var upcomingMatches = new List<MatchViewModel>();
        foreach (var match in fetchedUpcomingMatches)
        {
            var homeTeam = _teams.SingleOrDefault(x => x.Name == match.HomeTeam);

            if (homeTeam == null)
            {
                homeTeam = await _repository.CreateTeamAsync(match.HomeTeam);
            }

            var awayTeam = _teams.SingleOrDefault(x => x.Name == match.AwayTeam);
            if (awayTeam == null)
            {
                awayTeam = await _repository.CreateTeamAsync(match.AwayTeam);
            }

            var matchViewModel = new MatchViewModel(_repository, match, homeTeam, awayTeam);

            //if (homeTeam.Name == "Modo")
            //{
            //    matchViewModel.SetRate(4.35);
            //    matchViewModel.SetStatus(MatchStatus.Active);
            //}
            //if (homeTeam.Name == "Karlskoga")
            //{
            //    matchViewModel.SetRate(4.25);
            //    matchViewModel.SetStatus(MatchStatus.Active);
            //}
            //if (homeTeam.Name == "Västervik")
            //{
            //    matchViewModel.SetRate(4.25);
            //    matchViewModel.SetStatus(MatchStatus.Active);
            //}
            //if (homeTeam.Name == "Mora")
            //{
            //    matchViewModel.SetRate(4.90);
            //    matchViewModel.SetStatus(MatchStatus.Active);
            //}
            //if (homeTeam.Name == "Almtuna")
            //{
            //    matchViewModel.SetRate(4.35);
            //    matchViewModel.SetStatus(MatchStatus.Active);
            //}

            if (match.Day == todayDay)
            {
                todayMatches.Add(matchViewModel);
            }
            else
            {
                upcomingMatches.Add(matchViewModel);
            }
        }

        if (todayMatches.Any())
        {
            Matches.Add(new MatchGroupViewModel($"{DateTime.Today:yyyy-MM-dd} - Today", todayMatches));
        }
        else
        {
            Matches.Add(new MatchGroupViewModel($"{DateTime.Today:yyyy-MM-dd} - Today, no matches", new List<MatchViewModel>()));

        }
        if (upcomingMatches.Any())
        {
            var groupedUpcomingMatches = upcomingMatches.GroupBy(x => x.Date).Select(x => x.ToList());
            foreach (var groupedUpcomingMatch in groupedUpcomingMatches)
            {
                Matches.Add(new MatchGroupViewModel(groupedUpcomingMatch[0].Date, groupedUpcomingMatch));
            }
        }

        if (Matches.Any() == false)
        {
            Matches.Add(new MatchGroupViewModel("No matches", new List<MatchViewModel>()));
        }
    }

    private async Task ExecuteNavigateToMatchDetailsViewCommand(MatchViewModel viewModel)
    {
        await _navigationService.NavigateToPageAsync<MatchDetailsView>(new MatchDetailsViewNavigationParameter(viewModel));
    }

    private async Task ExecuteNavigateToTeamsViewCommand()
    {
        await _navigationService.NavigateToPageAsync<TeamsView>(new TeamsViewNavigationParameter(_teams));
    }

    private async Task ExecuteImportCalendarToDatabaseCommand()
    {
        var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        var matches = await _calendarFileDownloadService.DownloadAsync(href);

        var dateTime = new DateTime(2022, 12, 15);
        var latestMatches = matches.Where(x => x.Date > dateTime).ToList();
        //await _repository.AddMatchesAsync(latestMatches);
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