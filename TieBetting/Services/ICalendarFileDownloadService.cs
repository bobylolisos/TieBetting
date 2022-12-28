namespace TieBetting.Services;

public interface ICalendarFileDownloadService
{
    Task<IReadOnlyCollection<Match>> DownloadAsync(string href);
}