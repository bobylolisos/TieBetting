namespace TieBetting.Services;

public interface IRepository
{
    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);

    Task<IReadOnlyCollection<Match>> GetNextMatchesAsync(int? numberOfMatches = null);
}