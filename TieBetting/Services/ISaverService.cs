namespace TieBetting.Services;

public interface ISaverService
{
    Task AddMatchesAsync(IReadOnlyCollection<Match> matches);
    
    Task<TeamViewModel> CreateTeamAsync(string teamName);

    Task<MatchViewModel> CreateMatchAsync(string season, TeamViewModel homeTeam, TeamViewModel awayTeam, DateTime date);

    Task UpdateMatchAsync(Match match, bool refreshRequired = false);

    Task UpdateTeamAsync(Team team);

    Task UpdateSettingsAsync(Settings settings);
}