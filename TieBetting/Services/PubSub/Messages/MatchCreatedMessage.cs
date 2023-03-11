namespace TieBetting.Services.PubSub.Messages;

public class MatchCreatedMessage : MessageBase
{
    public MatchCreatedMessage(Match match)
    {
        Match = match;
    }

    public Match Match { get; }
}