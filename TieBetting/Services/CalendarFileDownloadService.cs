namespace TieBetting.Services;

public class CalendarFileDownloadService : ICalendarFileDownloadService
{
    public async Task<IReadOnlyCollection<Match>> DownloadAsync(string href)
    {
        var uri = new Uri(href);
        var client = new HttpClient();

        string[] lines;
        using (var response = await client.GetAsync(uri))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            lines = content.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
        }

        var matches = new List<List<string>>();
        var match = new List<string>();
        var matchFound = false;
        var calenderName = "";
        foreach (var line in lines)
        {
            if (line.ToLower().Contains("x-wr-calname"))
            {
                calenderName = line.Split(":")[1];
                continue;
            }

            if (line.ToLower().Contains("begin:vevent"))
            {
                matchFound = true;
                continue;
            }

            if (line.ToLower().Contains("end:vevent"))
            {
                matches.Add(match);
                matchFound = false;
                match = new List<string>();
                continue;
            }

            if (matchFound)
            {
                match.Add(line);
            }
        }

        var allMatches = new List<Match>();

        foreach (var match1 in matches)
        {
            var date = DateTime.ParseExact(
                match1.Single(x => x.ToLower().Contains("dtstart")).Split(":").Last().Trim().Substring(0, 8),
                "yyyyMMdd", CultureInfo.CurrentCulture);
            var m = new Match
            {
                Season = calenderName,
                Id = match1.Single(x => x.ToLower().Contains("uid")).Split(":").Last().Trim(),
                HomeTeam = match1.Single(x => x.ToLower().Contains("summary")).Split(":").Last().Split("-")[0]
                    .ResolveTeamName(),
                AwayTeam = match1.Single(x => x.ToLower().Contains("summary")).Split(":").Last().Split("-")[1]
                    .ResolveTeamName(),
                Day = (date - new DateTime(2022, 01, 01)).Days
            };

            allMatches.Add(m);
        }

        return allMatches;
    }
}