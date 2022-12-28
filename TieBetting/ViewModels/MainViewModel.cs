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

    public List<Match> UpcomingMatches { get; set; } = new();

    public AsyncRelayCommand ImportCalendarToDatabaseCommand { get; set; }

    public AsyncRelayCommand MyCommand { get; set; }

    public override async Task OnNavigatingToAsync(NavigationParameterBase navigationParameter)
    {
        UpcomingMatches.Clear();

        var matches = await _repository.GetNextMatchesAsync(20);
        UpcomingMatches.AddRange(matches);
    }

    private async Task ExecuteImportCalendarToDatabaseCommand()
    {
        var href = "https://calendar.ramses.nu/calendar/778/show/hockeyallsvenskan-2022-23.ics";
        var matches = await _calendarFileDownloadService.DownloadAsync(href);

        //await _repository.AddMatchesAsync(matches);
    }

    private async Task ExecuteMyCommand()
    {
        await Task.Delay(1);
    }
}