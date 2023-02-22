namespace TieBetting.Services.PubSub.Messages;

public class MatchCreatedMessage : MessageBase
{
    public MatchCreatedMessage(MatchViewModel match)
    {
        Match = match;
    }

    public MatchViewModel Match { get; }
}